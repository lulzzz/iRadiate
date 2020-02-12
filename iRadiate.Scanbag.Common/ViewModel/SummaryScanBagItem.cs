using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Whiteboard.Common;
using iRadiate.Scanbag.Common.View;

namespace iRadiate.Scanbag.Common.ViewModel
{
    [PreferredView("iRadiate.Scanbag.Common.View.SummaryScanBagItemView","iRadiate.Scanbag.Common")]
    public class SummaryScanBagItem : ScanBagItem
    {
        private Study _study;
        private List<AppointmentSummary> _appointmentSummaries;
        private PatientImage _selectedPatientImage;
        public SummaryScanBagItem(Study s)
        {
            _study = s;
            LoadSelectedPatientImageCommand = new RelayCommand(loadSelectedPatientImage);
            foreach (Appointment a in _study.Appointments.Where(x=>x.Deleted == false).OrderBy(z=>z.ScheduledArrivalTime))
            {
                AppointmentSummary aSum = new AppointmentSummary();
                aSum.Comments = a.Comments;
                aSum.AppointmentDate = a.ScheduledArrivalTime;
                aSum.DayNumber = a.DayNumber;
                foreach (BasicTask b in a.Tasks.Where(y => y.Deleted == false && y.IsCancelled==false).OrderBy(z => z.SchedulingTime))
                {
                    if (b is ScanTask)
                    {
                        ProcedureEvent p = new ProcedureEvent();
                        if(b.Assignee != null)
                            p.StaffMember = b.Assignee.FullName;
                        p.Completed = b.Completed;
                        p.Summary = new ScanSummaryControl();
                        p.Summary.DataContext = new ScanTaskSummaryViewModel(b);
                        if(b.Room != null)
                        {
                            p.Description = "Scan on " + b.Room.Name;
                        }
                        p.ProcedureDate = ((ScanTask)b).ValidCommencementTime;
                        aSum.ProcedureEvents.Add(p);
                        
                    }
                    if (b is DoseAdministrationTask)
                    {
                        ProcedureEvent p = new ProcedureEvent();
                        if (!b.Completed)
                            continue;
                        if (b.Assignee != null)
                            p.StaffMember = b.Assignee.FullName;
                        p.Summary = new DoseSummaryControl();
                        p.Summary.DataContext = b;
                        p.Completed = b.Completed;
                        p.Description = "Dose administered";
                        p.ProcedureDate = (b as DoseAdministrationTask).UnitDose.AdministrationDate;
                        aSum.ProcedureEvents.Add(p);
                    }
                    if (b is ArrivalTask)
                    {
                        if (!b.Completed)
                            continue;
                        ProcedureEvent p = new ProcedureEvent();
                        if (b.Assignee != null)
                            p.StaffMember = b.Assignee.FullName;
                        p.Completed = b.Completed;
                        p.Description = "Patient arrived";
                        p.ProcedureDate = b.ValidCompletionTime;
                        aSum.ProcedureEvents.Add(p);
                    }
                }
                if (a.Completed)
                {
                    ProcedureEvent p = new ProcedureEvent();
                    p.Completed = true;
                    p.Description = "Appointment completed";
                    p.ProcedureDate = a.CompletionTime;
                    aSum.ProcedureEvents.Add(p);
                }
                AppointmentSummaries.Add(aSum);
            }

        }

        public List<AppointmentSummary> AppointmentSummaries
        {
            get
            {
                if (_appointmentSummaries == null)
                {
                    _appointmentSummaries = new List<AppointmentSummary>();
                }
                return _appointmentSummaries;
            }
            set
            {
                _appointmentSummaries = value;
            }
        }
        public override string Description
        {
            get
            {
                
                
                return "Summary";
            }
            set
            {
                
            }
        }

        public Study Study
        {
            get
            {
                return _study;
            }
        }

        public Patient Patient
        {
            get
            {
                return _study.Patient;
            }
        }

        public IEnumerable<DataStoreItem> ScanTasks
        {
            get
            {
                return _study.Appointments.SelectMany(x => x.Tasks).Where(y => y is ScanTask && y.Deleted == false).OrderBy(x=>((ScanTask)x).ValidCommencementTime);
            }
        }
        public IEnumerable<DataStoreItem> InjectionTasks
        {
            get
            {
                return _study.Appointments.SelectMany(x => x.Tasks).Where(y => y is DoseAdministrationTask).OrderBy(x=>x.ValidCompletionTime);
            }
        }

        public IEnumerable<IDataStoreItem> PatientImages
        {
            get
            {
                return _study.Appointments.SelectMany(x => x.Tasks).Where(y => y is ScanTask && y.Deleted == false && y.Cancelled == false).SelectMany(y=>(y as ScanTask).PatientImages).OrderBy(x => ((PatientImage)x).SeriesDateTime);
            }
        }

        public PatientImage SelectedPatientImage
        {
            get
            {
                return _selectedPatientImage;
            }
            set
            {
                _selectedPatientImage = value;
            }
        }

        private void loadSelectedPatientImage()
        {
            if(SelectedPatientImage != null)
            {
                PatientImageViewModel vm = new PatientImageViewModel(SelectedPatientImage);
                DesktopApplication.MakeModalDocument(vm);
            }
        }

        public RelayCommand LoadSelectedPatientImageCommand { get; set; }

       
    }

    public class ProcedureEvent
    {
        public string Description { get; set; }
        public DateTime ProcedureDate { get; set; }
        public bool Completed { get; set; }

        public ContentControl Summary
        {
            get;set;
        }

        public string StaffMember { get; set; }
    }

    public class AppointmentSummary
    {
        private List<ProcedureEvent> _procedureEvents;

        public List<ProcedureEvent> ProcedureEvents
        {
            get
            {
                if (_procedureEvents == null)
                {
                    _procedureEvents = new List<ProcedureEvent>();
                }
                return _procedureEvents;
            }
            set
            {
                _procedureEvents = value;
            }
        }

        public DateTime AppointmentDate { get; set; }

        public int DayNumber { get; set; }

        public string Comments { get; set; }
    }

    public class ScanTaskSummaryViewModel : ViewModelBase
    {
        private ScanTask _scanTask;
        private PatientImage _selectedImage;

        public ScanTaskSummaryViewModel(BasicTask scanTask)
        { 
            if(scanTask is ScanTask)
            _scanTask = (ScanTask)scanTask;
        }

        public List<PatientImage> PatientImages
        {
            get
            {
                return _scanTask.PatientImages;
            }
        }

        public PatientImage SelectedImage
        {
            get { return _selectedImage; }
            set { _selectedImage = value; RaisePropertyChanged("SelectedImage"); RaisePropertyChanged("SelectedImageDetail"); }
        }

        public ContentControl SelectedImageDetail
        {
            get
            {
                if(SelectedImage != null)
                {
                    if(SelectedImage.Modality == Modality.NM)
                     {
                        var ret = new NMImageDetails();
                        ret.DataContext = SelectedImage;
                        return ret;
                    }
                    if (SelectedImage.Modality == Modality.CT)
                    {
                        var ret = new CTImageDetails();
                        ret.DataContext = SelectedImage;
                        return ret;
                    }
                    return null;
                }
                return null;
            }
        }
        
    }
}
