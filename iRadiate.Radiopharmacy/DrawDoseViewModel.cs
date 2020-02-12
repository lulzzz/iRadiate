using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Radiopharmacy;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Radiopharmacy
{
    public class DrawDoseViewModel : BulkDoseViewModel
    {
        private List<IDataStoreItem> _potentialTasks;
        private IDataStoreItem _selectedTask;

        public DrawDoseViewModel() : base()
        {

        }

        public DrawDoseViewModel(DataStoreItem item) : base(item)
        {
            PreCalTime = DateTime.Now;
        }

        public IDataStoreItem SelectedTask
        {
            get { return _selectedTask; }
            set { _selectedTask = value; RaisePropertyChanged("SelectedTask"); }
        }

        protected override void MakeUnitDose()
        {
           
            BaseUnitDose u = (Item as BaseBulkDose).DrawDose(DrawnUpActivity, DrawnUpVolume, DateTime.Now, DateTime.Now.AddHours(12), "", DrawnUpVolume);
            UnitDoses.Add(u);
            if(SelectedTask != null)
            {
                u.DoseAdministrationTask = SelectedTask as DoseAdministrationTask;
            }
            
            DesktopApplication.Librarian.SaveItem(u);
            var vm = new BaseUnitDoseViewModel(u);
            vm.PreCalTime = PreCalTime;
            Close();
            DesktopApplication.MakeModalDocument(vm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.UnitDosePrintView");
            

        }

        protected override void ReadActivity()
        {
            IDoseCalibrator doseCalibrator = DesktopApplication.MainViewModel.DoseCalibrator;
            if (doseCalibrator.IsConnected)
            {
                
                DrawnUpActivity = doseCalibrator.ReadActivity(Radiopharmaceutical.Isotope);
                
            }
            else
            {
                DesktopApplication.ShowDialog("Error", "Dose calibrator is not connected");
            }
        }
        public List<IDataStoreItem> PotentialTasks
        {
            get
            {
                if (_potentialTasks == null)
                {
                    fillPotentialTasks();
                }
                return _potentialTasks;
            }
            set
            {
                _potentialTasks = value;
                RaisePropertyChanged("PotentialTasks");
            }
        }

        public override DateTime PreCalTime
        {
            get
            {
                return base.PreCalTime;
            }

            set
            {
                base.PreCalTime = value;
                RaisePropertyChanged("VolumeRange");
                RaisePropertyChanged("PrescribedActivityRange");
            }
        }
        public string PrescribedActivityRange
        {
            get
            {
                if (SelectedTask == null)
                    return "";
                else
                {
                    var diff = PreCalTime - DateTime.Now;
                    var halfLife = (Item as BaseBulkDose).Isotope.HalfLife;
                    var decayFactor = Math.Exp((-1 * Math.Log(2) / halfLife) * diff.TotalSeconds);
                    var min = (SelectedTask as DoseAdministrationTask).PrescribedMinimum / decayFactor;
                    var max = (SelectedTask as DoseAdministrationTask).PrescribedMaximum / decayFactor;
                    return min.ToString("F0") + " - " + max.ToString("F0");
                }
            }
        }

        public string VolumeRange
        {
            get
            {
                if (SelectedTask == null)
                    return "";
                else
                {
                    if (Item == null)
                        return "";
                    else
                    {
                        var v = (Item as BaseBulkDose).Volume;
                        var a = iRadiate.Common.Misc.DecayCorrecter.Decay((Item as BaseBulkDose).CalibrationDate, PreCalTime, (Item as BaseBulkDose).Isotope.HalfLife/3600,(Item as BaseBulkDose).CalibrationActivity);
                        var c = a / v;
                        return ((SelectedTask as DoseAdministrationTask).PrescribedMinimum / c).ToString("F1") + " - " + ((SelectedTask as DoseAdministrationTask).PrescribedMaximum / c).ToString("F1");
                    }
                }
            }
        }

        public double PreCalTarget
        {
            get
            {
                if (SelectedTask == null)
                    return 0;
                else
                {
                    return ((SelectedTask as DoseAdministrationTask).PrescribedMaximum + (SelectedTask as DoseAdministrationTask).PrescribedMinimum)/2.0;
                }
            }
            set
            {
                (SelectedTask as DoseAdministrationTask).PrescribedMaximum = Convert.ToInt16(value * 1.1);
                (SelectedTask as DoseAdministrationTask).PrescribedMinimum = Convert.ToInt16(value * 0.9);
                RaisePropertyChanged("VolumeRange");
                RaisePropertyChanged("PrescribedActivityRange");

            }
        }
        private void fillPotentialTasks()
        {
            if (Item != null)
            {
                //Need to find all DoseAdministrationTasks where
                //completed == false
                //RadiopharmaceuticalID = item.RadiopharmaceuticalID
                //scheduledCompletionDate == Today
                if (true == true)
                {
                    RetrievalCriteria rc1 = new RetrievalCriteria("ScheduledCompletionTime", CriteraType.GreaterThanOrEqual, DateTime.Today);
                    RetrievalCriteria rc2 = new RetrievalCriteria("Completed", CriteraType.Equals, false);
                    RetrievalCriteria rc3 = new RetrievalCriteria("ScheduledCompletionTime", CriteraType.LessThan, DateTime.Today.AddDays(1));
                    List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
                    rcList.Add(rc1);
                    rcList.Add(rc2);
                    rcList.Add(rc3);
                    _potentialTasks = DesktopApplication.Librarian.GetItems(typeof(DoseAdministrationTask), rcList).ToList();
                    _potentialTasks = _potentialTasks.Where(y => (y as DoseAdministrationTask).PrescribedRadioPharmaceutical != null && (y as DoseAdministrationTask).UnitDose == null).Where(x => (x as DoseAdministrationTask).PrescribedRadioPharmaceutical.ID == (Item as BaseBulkDose).Radiopharmaceutical.ID).ToList();
                }
                

            }
            else
            {
                _potentialTasks = DesktopApplication.Librarian.GetItems(typeof(DoseAdministrationTask), new List<RetrievalCriteria>()).ToList();
            }

            
        }
    }
}
