using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Linq;
using System.Text;
using System.Windows.Data;

using GalaSoft.MvvmLight.Command;
using NLog;

using iRadiate.Common;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{
    
    public abstract class BaseTaskViewModel : DataStoreItemViewModel
    {
        #region privateFields
        protected new static Logger logger = LogManager.GetCurrentClassLogger();     
        private bool _addConstraintOpen = false;
        private AppointmentViewModel _appointmentVM;
        private string _selectedConstraintType;
      
        private ListCollectionView _constraintsView;
        #endregion        

        #region constructors
        public BaseTaskViewModel():base()
        {
            
        }

        public BaseTaskViewModel(IDataStoreItem item):base(item)
        {
            
        }
        #endregion

        #region overrides
        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            RescheduleCommand = new RelayCommand(Reschedule);
        }
        
        public override string Name
        {
            get
            {
                return ((BasicTask)Item).TaskName;
            }
        }
        #endregion

        #region virtuals
        public virtual DateTime ScheduledCompletionTime
        {
            get
            {
                return ((BasicTask)Item).ScheduledCompletionTime;
            }
            set
            {

                ((BasicTask)Item).ScheduledCompletionTime = ((BasicTask)Item).Appointment.ScheduledArrivalTime.Date + value.TimeOfDay; ;
                RaisePropertyChanged("ScheduledCompletionTime");
                RaisePropertyChanged("SchedulingTime");
                RaisePropertyChanged("Duration");
            }
        }

        public virtual DateTime CompletionTime
        {
            get
            {
                return ((BasicTask)Item).CompletionTime;
            }
            set
            {
                
                ((BasicTask)Item).CompletionTime = value;
                RaisePropertyChanged("CompletionTime");
            }
        }

        public virtual StaffMemberRole Role
        {
            get
            {
                return ((BasicTask)Item).StaffMemberRole;
            }
            set
            {
                OldRole = ((BasicTask)Item).StaffMemberRole;
                ((BasicTask)Item).StaffMemberRole = value;
                RaisePropertyChanged("Role");
                SaveItemWithoutClosing();
            }
        }

        public virtual User User
        {
            get
            {
                return ((BasicTask)Item).Assignee;
            }
            set
            {
                ((BasicTask)Item).Assignee = value;
                RaisePropertyChanged("User");
            }
        }

        public virtual Room Room
        {
            get
            {
                return ((BasicTask)Item).Room;
            }
            set
            {
                OldRoom = ((BasicTask)Item).Room; 
                ((BasicTask)Item).Room = value;
                //This is ugle ugl code
                if (value != null)
                {    
                    if (RoomedRoles.Where(y => y.Room.ID == value.ID).Any())
                    {
                        ((BasicTask)Item).StaffMemberRole = RoomedRoles.Where(j => j.Room != null).Where(y => y.Room.ID == value.ID).First();
                    }
                }
                
                RaisePropertyChanged("Room");
                SaveItemWithoutClosing();
            }
        }

        public virtual Room OldRoom
        {
            get;
            set;
        }

        public virtual StaffMemberRole OldRole
        {
            get;
            set;
        }

        public virtual Patient Patient
        {
            get
            {
                return ((BasicTask)Item).Appointment.Patient;
            }
        }

        public virtual Appointment Appointment
        {
            get
            {
                return ((BasicTask)Item).Appointment;
            }
        }

        public virtual bool Completed
        {
            get
            {
                return ((BasicTask)Item).Completed;
            }
            set
            {
                ((BasicTask)Item).Completed = value;
                User = Platform.CurrentUser;
                //RaisePropertyChanged("User");
                RaisePropertyChanged("Completed");
                RaisePropertyChanged("WorkflowStatus");
            }
        }

        public virtual bool UnCompleted
        {
            get
            {
                return !Completed;
            }
        }

        public virtual DateTime ValidCompletionTime
        {
            get
            {
                if (Completed)
                {
                    return CompletionTime;
                }
                else
                {
                    return ScheduledCompletionTime;
                }
            }
            set
            {
                if (Completed)
                {
                    CompletionTime = value;
                }
                else
                {
                    ScheduledCompletionTime = value;
                }
            }
        }

        public virtual List<StaffMemberRole> Roles
        {
            get
            {
                return DesktopApplication.CurrentPratice.Roles.OrderBy(x=>((StaffMemberRole)x).Name).ToList();
            }
        }

        public virtual IEnumerable<StaffMemberRole> RoomedRoles
        {
            get
            {
                return Roles.Where(x => x.Room != null);
            }
        }

        public virtual List<IDataStoreItem> Users
        {
            get
            {
                return DesktopApplication.Users.OrderBy(x => ((User)x).FullName).ToList();
            }
        }

        public virtual List<Room> Rooms
        {
            get
            {
                return DesktopApplication.CurrentPratice.Rooms.OrderBy(x => x.Name).ToList();
            }
        }

        

        public virtual DateTime SchedulingTime
        {
            get
            {
                return ValidCompletionTime;
            }
            set
            {
                ValidCompletionTime = value;
                RaisePropertyChanged("SchedulingTime");
            }
        }

        public virtual WorkflowStatus WorkflowStatus
        {
            get
            {
                if (Deleted)
                {
                    return ViewModel.WorkflowStatus.Deleted;
                }
                if (Completed)
                {
                    return ViewModel.WorkflowStatus.Completed;
                }
                return ViewModel.WorkflowStatus.Pending;
            }
        }

        private void Reschedule()
        {
            
            DesktopApplication.MakeModalDocument(this, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.RescheduleTaskView");
        }

        public virtual bool TimeoutPerformed
        {
            get
            {
                if (!(Item as BasicTask).TimeoutPerformed.HasValue)
                {
                    return false;
                }
                return (Item as BasicTask).TimeoutPerformed.Value;
            }
            set
            {
                (Item as BasicTask).TimeoutPerformed = value;
                RaisePropertyChanged("TimeoutPerformed");
                RaisePropertyChanged("ChecksPerformed");
            }
        }

        public virtual bool RequestFormCorrect
        {
            get
            {
                if (!(Item as BasicTask).RequestFormCorrect.HasValue)
                {
                    return false;
                }
                return (Item as BasicTask).RequestFormCorrect.Value;
            }
            set
            {
                (Item as BasicTask).RequestFormCorrect = value;
                RaisePropertyChanged("RequestFormCorrect");
                RaisePropertyChanged("ChecksPerformed");
            }
        }

        public virtual bool ImageCorrect
        {
            get
            {
                if (!(Item as BasicTask).ImageCorrect.HasValue)
                {
                    return false;
                }
                return (Item as BasicTask).ImageCorrect.Value;
            }
            set
            {
                (Item as BasicTask).ImageCorrect = value;
                RaisePropertyChanged("ImageCorrect");
                RaisePropertyChanged("ChecksPerformed");
            }
        }
        public virtual bool ChecksPerformed
        {
            get
            {
                if (!(Item as BasicTask).TimeoutPerformed.HasValue)
                {
                    return false;
                }
                if (!(Item as BasicTask).RequestFormCorrect.HasValue)
                {
                    return false;
                }
                if (!(Item as BasicTask).ImageCorrect.HasValue)
                {
                    return false;
                }
                if ((Item as BasicTask).TimeoutPerformed.Value && (Item as BasicTask).RequestFormCorrect.Value && (Item as BasicTask).ImageCorrect.Value)
                {
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region relaycommands      
        public RelayCommand RescheduleCommand
        {
            get;
            set;
        }
        #endregion

        #region publicProperties

        #endregion

        #region events

        #endregion

        

    }

    public enum WorkflowStatus
    {
        Pending, Commenced, Completed, Deleted
    }
}
