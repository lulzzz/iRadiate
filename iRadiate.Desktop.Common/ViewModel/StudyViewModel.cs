using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight.Command;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{
   
    public class StudyViewModel : DataStoreItemViewModel
    {

        private AsyncObservableCollection<AppointmentViewModel> _appointments;
        private AppointmentViewModel _selectedAppointment;
   

        public StudyViewModel()
            : base()
        {
            
          
        }

        public StudyViewModel(DataStoreItem item)
            : base(item)
        {
            
        }

        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            AddAppointmentCommand = new RelayCommand(AddAppointment);
        }

       

        public override void NonUIThreadInitialize()
        {
            base.NonUIThreadInitialize();          

            #region appointments
            foreach (Appointment a in ((Study)Item).Appointments)
            {
                AppointmentViewModel avm = new AppointmentViewModel(a);
                //DataStoreItemViewModel avm = new DataStoreItemViewModel(a);
               
                Appointments.Add(avm);
            }
            #endregion

           
        }

        public string PatientHistory
        {
            get
            {
                if(((Study)Item).Request != null)
                    return ((Study)Item).Request.PatientHistory;
                return "";
            }
        }

        public string RisRemarks
        {
            get
            {
                if (((Study)Item).Request != null)
                    return ((Study)Item).Request.RequestRemark;
                return "";
            }
        }

        public string ClinicalInfo
        {
            get
            {
                if (((Study)Item).Request != null)
                    return ((Study)Item).Request.ClinicalInfo;
                return "";
            }
        }
        public AsyncObservableCollection<AppointmentViewModel> Appointments
        {
            get
            {
                if (_appointments == null)
                {
                    _appointments = new AsyncObservableCollection<AppointmentViewModel>();
                    _appointments._synchronizationContext = Platform.SynchronizationContext;
                }
                return _appointments;
            }
            set
            {
                _appointments = value;
                RaisePropertyChanged("Appointments");
            }
        }

        public AppointmentViewModel SelectedAppointment
        {
            get { return _selectedAppointment; }
            set 
            { 
                _selectedAppointment = value;
                RaisePropertyChanged("SelectedAppointment");
                RaisePropertyChanged("AppointmentSelected");
            }
        }

        public bool AppointmentSelected
        {
            get
            {
                if (SelectedAppointment != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Doctor Referrer
        {
            get
            {
                Study s = Item as Study;
                if (s.Request == null)
                    return null;
                if (s.Request.Referrer == null)
                    return null;
                return (Item as Study).Request.Referrer;
            }
        }
       
        

        #region relayCommands
        
        public RelayCommand AddAppointmentCommand
        {
            get;
            private set;
        }

        public RelayCommand NewAppointmentDateTimeSelectedCommand
        {
            get;
            private set;
        }

        public RelayCommand ViewReportCommand
        {
            get;
            private set;
        }
        #endregion
       
        

        

        #region privateMethods
       
        private void AddAppointment()
        {
            if (((Study)Item).Appointments.Any())
            {

            }
            else
            {
                Appointment a = new Appointment();
                a.Study = (Study)Item;
                a.ScheduledArrivalTime = DateTime.Today.AddDays(1).AddHours(9);
                ((Study)Item).Appointments.Add(a);
                AppointmentViewModel vm = new AppointmentViewModel(a);
                Appointments.Add(vm);
                SelectedAppointment = vm;
            }
        }
        

       
        #endregion
    }
}
