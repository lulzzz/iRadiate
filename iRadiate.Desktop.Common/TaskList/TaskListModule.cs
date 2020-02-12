using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight.Command;

using iRadiate.DataModel.NucMed;
using iRadiate.Common;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Common.TaskList
{
    [PreferredView("iRadiate.Desktop.Common.TaskList.TaskListView", "iRadiate.Desktop.Common")]
    public class TaskListModule : Module
    {
        private AsyncObservableCollection<IDataStoreItem> _tasks;
        private bool _excludeCompleted;
        private ICollectionView _tasksView;
        private StaffMemberRole _selectedStaffMemberRole;

        public TaskListModule()
        {
            RefreshCommand = new RelayCommand(Refresh);
        }
        private void Refresh()
        {
            TasksView.Refresh();
        }
        public RelayCommand RefreshCommand { get; set; }
        public List<StaffMemberRole> StaffMemberRoles
        {
            get
            {
                return DesktopApplication.CurrentPratice.Roles.OrderBy(x=>x.Name).ToList();
            }
        }
        public StaffMemberRole SelectedStaffMemberRole
        {
            get
            {
                return _selectedStaffMemberRole;
            }
            set
            {
                _selectedStaffMemberRole = value;
                RaisePropertyChanged("SelectedStaffMemberRole");
                TasksView.Refresh();
                Properties.Settings.Default.TaskListSelectedRoleID = value.ID;
                Properties.Settings.Default.Save();
            }
        }
        public bool ExcludeCompleted
        {
            get
            {
                return _excludeCompleted;
            }
            set
            {
                _excludeCompleted = value;
                RaisePropertyChanged("ExcludeCompleted");
                Refresh();
                Properties.Settings.Default.TaskListExcludeCompleted = value;
                Properties.Settings.Default.Save();
            }
        }
        public AsyncObservableCollection<IDataStoreItem> Tasks
        {
            get
            {
                return _tasks;
            }
            set
            {
                _tasks = value;
                RaisePropertyChanged("Tasks");
            }
        }

        public ICollectionView TasksView
        {
            get
            {
                return _tasksView;
            }
            set
            {
                _tasksView = value;
                RaisePropertyChanged("TasksView");
            }
        } 
        public override void GetData()
        {
            base.GetData();
            _tasks  = DesktopApplication.GetLibrarian().GetTasks(DateTime.Today);
            _tasksView = DesktopApplication.CreateCollectionView(_tasks);
            _tasksView.SortDescriptions.Add(new SortDescription("SchedulingTime", ListSortDirection.Ascending));
            _tasksView.Filter = FilterTask;
            RaisePropertyChanged("TasksView");
            SelectedStaffMemberRole = StaffMemberRoles.Where(x => x.ID == Properties.Settings.Default.TaskListSelectedRoleID).First();
            ExcludeCompleted = Properties.Settings.Default.TaskListExcludeCompleted;
        }

        private bool FilterTask(object item)
        {
            BaseTaskViewModel task = item as BaseTaskViewModel;
            if (task.Deleted)
            {
                return false;
            }
            if (ExcludeCompleted && task.Completed)
            {
                return false;
            }
            if (SelectedStaffMemberRole == null)
            {
                return false;
            }
            if (SelectedStaffMemberRole != null)
            {
                if (task.Role != null)
                {
                    if (task.Role.ID == SelectedStaffMemberRole.ID)
                    {
                        return true;
                    }
                }
                
            }
            
            return false;
        }

        public override string Name
        {
            get
            {
                return "Task List";
            }
            set
            {

            }
        }

        public override void UIThreadInitialize()
        {
            base.UIThreadInitialize();
            TasksView.Refresh();
        }

        public override string IconSource
        {
            get { return "/iRadiate.Desktop.Common;component/Images/TaskListIcon.png"; }
        }
    }

    
    public class TaskListModuleLauncher : ModuleLauncher
    {
        public override int Order
        {
            get
            {
                return 40;
            }
        }
        public override string Name
        {
            get
            {

                return "Task List";
            }
        }

        public string IconSource
        {
            get { return "/iRadiate.Desktop.Common;component/Images/TaskListIconWhite.png"; }
        }

        public override void Launch()
        {
            //logger.Trace("SettingsLauncher.Launch()");
            DesktopApplication.MainViewModel.LaunchModule(typeof(TaskListModule));

        }
    }
}
