using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
namespace iRadiate.Desktop.Common.ViewModel
{
    [Obsolete]
    public class WorkflowTemplateViewModel : DataStoreItemViewModel
    {
        public WorkflowTemplateViewModel()
            : base()
        {

        }
        public WorkflowTemplateViewModel(DataStoreItem item)
            : base(item)
        {

        }

        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            ViewAppointmentCommand = new RelayCommand(ViewAppointment);
        }

        public AppointmentViewModel Appointment
        {
            get
            {
                return (AppointmentViewModel)new DataStoreItemViewModel(((WorkflowTemplate)Item).Appointment);
            }
        }

        public string Name
        {
            get
            {
                return ((WorkflowTemplate)Item).Name;
            }
            set
            {
                ((WorkflowTemplate)Item).Name = value;
                RaisePropertyChanged("Name");

            }
        }

        public RelayCommand ViewAppointmentCommand
        {
            get;
            private set;
        }

        private void ViewAppointment()
        {
            DesktopApplication.MakeDocument(Appointment);
        }
        
    }
}
