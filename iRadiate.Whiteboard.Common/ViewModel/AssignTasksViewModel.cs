using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Whiteboard.Common.ViewModel
{
    public class AssignTasksViewModel : ViewModelBase
    {
        private bool _assignAllChecked;
        private AppointmentViewModel _appointment;
        private ICollectionView _assignableTasksView;
        private List<TaskAssignSelector> _taskSelectors;
        private StaffMemberRole _selectedRole;

        public StaffMemberRole SelectedRole
        {
            get
            {
                return _selectedRole;
            }
            set
            {
                _selectedRole = value;
                RaisePropertyChanged("SelectedRole");
            }
        }

        public AssignTasksViewModel()
            : base()
        {
            ReAssignCommand = new RelayCommand(ReAssign);
        }

        public bool AssignAllChecked
        {
            get
            {
                return _assignAllChecked;
            }
            set
            {
                _assignAllChecked = value;
                foreach (TaskAssignSelector tas in _taskSelectors)
                {
                    tas.selected = value;
                }
                AssignableTasksView.Refresh();
                RaisePropertyChanged("AssignAllChecked");
            }
        }

        public AppointmentViewModel Appointment
        {
            get
            {
                return _appointment;
            }
            set
            {
                _appointment = value;
                _taskSelectors = new List<TaskAssignSelector>();
                /*foreach (BaseTaskViewModel bt in value.Tasks)
                {
                    TaskAssignSelector tas = new TaskAssignSelector();
                    tas.task = bt;
                    tas.selected = false;
                    _taskSelectors.Add(tas);
                }*/
                AssignableTasksView = CollectionViewSource.GetDefaultView(_taskSelectors);
                AssignableTasksView.Filter = delegate(object item)
                {
                    TaskAssignSelector tas = item as TaskAssignSelector;
                    if (tas.task.Completed)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                };

                RaisePropertyChanged("Appointment");
            }
        }

        public ICollectionView AssignableTasksView
        {
            get
            {
                return _assignableTasksView;
            }
            set
            {
                _assignableTasksView = value;
                RaisePropertyChanged("AssignableTasksView");
            }
        }

        public virtual List<StaffMemberRole> Roles
        {
            get
            {
                return DesktopApplication.CurrentPratice.Roles.OrderBy(x => ((StaffMemberRole)x).Name).ToList();
            }
        }

        private void ReAssign()
        {
            foreach (TaskAssignSelector tas in _taskSelectors)
            {
                if (tas.task.Completed == false && tas.selected)
                {
                    tas.task.Role = SelectedRole;
                    //tas.task.SaveItem();
                }
            }
            
        }
        public RelayCommand ReAssignCommand
        {
            get;
            set;
        }

        private class TaskAssignSelector : ViewModelBase
        {
            private bool _selected;
            public BaseTaskViewModel task
            {
                get;
                set;
            }
            public string Name
            {
                get
                {
                    return task.Name;
                }
            }
            public DateTime SchedulingTime
            {
                get
                {
                    return task.SchedulingTime;
                }
            }
            public StaffMemberRole Role
            {
                get
                {
                    return task.Role;
                }
            }
            public bool selected
            {
                get
                {
                    return _selected;
                }
                set
                {
                    _selected = value;
                    RaisePropertyChanged("selected");
                }
            }
        }
    }
}
