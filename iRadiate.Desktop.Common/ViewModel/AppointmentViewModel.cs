using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;


using GalaSoft.MvvmLight.Command;
using NLog;
using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common.Diary;
using System.Windows.Media;

namespace iRadiate.Desktop.Common.ViewModel
{
    

   
    public class AppointmentViewModel : DataStoreItemViewModel
    {
        #region privateFields
        private new static Logger logger = LogManager.GetCurrentClassLogger();        
        private AsyncObservableCollection<IDataStoreItem> _tasks;        
        private DateTime _selectedTaskTime;         
        private List<WorkflowTemplate> _workflowTemplates;
        private WorkflowTemplate _selectedWorkFlowTemplate;
        private String _selectedTaskType;
        private IDataStoreItem _selectedTask;
        #endregion

        #region constructors
        public AppointmentViewModel():base()
        {
           
        }

        public AppointmentViewModel(IDataStoreItem item)
            : base(item)
        {
            foreach(BasicTask bt in ((Appointment)Item).Tasks)
            {
                Tasks.Add(bt);
            }
            TasksView.Filter = new Predicate<object>(FilterTask);
            TasksView.SortDescriptions.Add(new SortDescription("SchedulingTime", ListSortDirection.Ascending));
            
        }
        #endregion

        #region overrides
        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();            
            //CreateWorkflowCommand = new RelayCommand(ToggleWorkFlowNameOpen);           
            LoadTemplateCommand = new RelayCommand(LoadFromTemplate);
            AddTaskCommand = new RelayCommand(AddTask);
            ViewTaskCommand = new RelayCommand(viewTask);
            CancelTaskCommand = new RelayCommand(cancelTask);
        }
       
        void p_TaskDeleted(object sender, EventArgs e)
        {
            //refresh or something?
            //_diary.RefreshDiaryEvents();
        }
        public override void OnSomethingChanged(EventArgs e)
        {
            RaisePropertyChanged("AvailableActions");
            base.OnSomethingChanged(e);
        }

        public void AppointmentViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //OnSomethingChanged(e);
            RaiseAllPropertiesChanged();
            
        }

       

       
        #endregion

        #region publicProperties
        
        public DateTime DiaryStartTime
        {
            get
            {
                return (Item as Appointment).ScheduledArrivalTime.Date.AddHours(8.5);
            }
        }

        public DateTime DiaryEndTime
        {
            get
            {
                return DiaryStartTime.AddHours(8.5);
            }
        }

        public IDataStoreItem SelectedTask
        {
            get
            {
                return _selectedTask;

            }
            set
            {
                _selectedTask = value;
                RaisePropertyChanged("SelectedTask");
            }
        }

        public AsyncObservableCollection<IDataStoreItem> Tasks
        {
            get
            {
                if(_tasks == null)
                {
                    _tasks = new AsyncObservableCollection<IDataStoreItem>();
                }
                return _tasks;
            }
            set
            {
                _tasks = value;
            }
        }

        public DateTime SelectedTaskTime
        {
            get
            {
                if (_selectedTaskTime == null)
                {
                    _selectedTaskTime = ((Appointment)Item).ScheduledArrivalTime.AddMinutes(60);
                }
                return _selectedTaskTime;
            }
            set
            {
                _selectedTaskTime = value;
                RaisePropertyChanged("SelectedTaskTime");
            }
        }

        /// <summary>
        /// The workflows which can be applied to this appointment
        /// </summary>
        public List<WorkflowTemplate> WorkflowTemplates
        {
            get
            {

                return ((Appointment)Item).Study.StudyType.Workflows;
            }
            
        }

        public WorkflowTemplate SelectedWorkflowTemplate
        {
            get
            {
                return _selectedWorkFlowTemplate;
            }
            set
            {
                _selectedWorkFlowTemplate = value;
                RaisePropertyChanged("SelectedWorkflowTemplate");
            }
        }

        public List<String> TaskTypes
        {
            get
            {
                return new List<string>(){ "Arrival", "Injection", "Scan" };
            }
        }

        public String SelectedTaskType
        {
            get
            {
                return _selectedTaskType;
            }
            set
            {
                _selectedTaskType = value;
            }
        }

       public ICollectionView TasksView
        {
            get
            {
                return CollectionViewSource.GetDefaultView(Tasks);
            }
        }

        
        #endregion

        #region privateMethods
        
        

        //Should this be moved to the underlying DataStoreItem?
        //Also I want this method to be able to override old templates
        private void LoadFromTemplate()
        {
            /*
            if (Tasks.Where(x => x.Deleted == false).Any())
            {
                Application.ShowDialog("Error","You can't load a workflow into a non-empty study");
                return;
            }
            if (SelectedWorkflowTemplate == null)
            {
                return;
            }
            DateTime templateArrival = SelectedWorkflowTemplate.Appointment.ScheduledArrivalTime;

            foreach (BasicTask t in SelectedWorkflowTemplate.Appointment.Tasks.Where(x => x.Deleted == false))
            {
                BasicTask b = (BasicTask)Activator.CreateInstance(t.ConcreteType);
                b.ScheduledCompletionTime = ((Appointment)Item).ScheduledArrivalTime.AddMinutes((t.ScheduledCompletionTime-templateArrival).TotalMinutes);
                b.Room = t.Room;
                b.StaffMemberRole = t.StaffMemberRole;
                if (b is BasicFiniteTask)
                {
                    ((BasicFiniteTask)b).ScheduledCommencementTime = ((Appointment)Item).ScheduledArrivalTime.AddMinutes((((BasicFiniteTask)t).ScheduledCommencementTime - templateArrival).TotalMinutes);
                }
                if(b is StandardNonFiniteTask)
                {
                    ((StandardNonFiniteTask)b).TaskType = ((StandardNonFiniteTask)t).TaskType;
                }
                if (b is StandardFiniteTask)
                {
                    ((StandardFiniteTask)b).TaskType = ((StandardFiniteTask)t).TaskType;
                }
                if (b is DoseAdministrationTask)
                {
                    ((DoseAdministrationTask)b).PrescribedRadioPharmaceutical = ((DoseAdministrationTask)t).PrescribedRadioPharmaceutical;
                    ((DoseAdministrationTask)b).PrescribedMinimum = ((DoseAdministrationTask)t).PrescribedMinimum;
                    ((DoseAdministrationTask)b).PrescribedMaximum = ((DoseAdministrationTask)t).PrescribedMaximum;
                }
                b.SetAppointment((Appointment)Item);
                ((Appointment)Item).Tasks.Add((BasicTask)b);
                Type ty = b.GetType();

                //ViewModel
                //ObjectHandle handle = Activator.CreateInstance("iRadiate.Desktop.Common", "iRadiate.Desktop.Common.ViewModel." + ty.Name + "ViewModel");
                //object p = handle.Unwrap();
                //DataStoreItemViewModel p = Application.GetLibrarian().GetViewModel(b);
                DataStoreItemViewModel p = new DataStoreItemViewModel(b);
                //((DataStoreItemViewModel)p).SetItem(b);
                //Tasks.Add((DataStoreItemViewModel)p);
                p.SaveButtonVisible = true;
                p.EditButtonVisible = false;
                p.PropertyChanged += AppointmentViewModel_PropertyChanged;
                p.SaveItem();
               
                
            }
            int taskCounter = 0;
            //t = tasks from the template
            //at = task from this appointment
            foreach (BasicTask t in SelectedWorkflowTemplate.Appointment.Tasks.Where(x => x.Deleted == false))
            {
                BasicTask at = ((Appointment)Item).Tasks.Where(x => x.Deleted == false).ToList()[taskCounter];
                foreach (BaseConstraint tc in t.Constraints)
                {
                    BaseConstraint ac = (BaseConstraint)Activator.CreateInstance(tc.ConcreteType);
                    ac.Constrainee = ((Appointment)Item).Tasks.Where(x => x.Deleted == false).ToList()[taskCounter];
                    if(tc.Constrainor != null)
                    {
                        
                        int constrainorIndex = SelectedWorkflowTemplate.Appointment.Tasks.Where(x => x.Deleted == false).ToList().IndexOf(tc.Constrainor);
                        ac.Constrainor = ((Appointment)Item).Tasks.Where(x => x.Deleted == false).ToList()[constrainorIndex];
                    }
                    if (tc is DelayConstraint) 
                    {
                        ((DelayConstraint)ac).MinDelay = ((DelayConstraint)tc).MinDelay;
                        ((DelayConstraint)ac).MaxDelay = ((DelayConstraint)tc).MaxDelay; 
                    }
                    if (tc is AwaitCompletionToCommence)
                    {
                        ((AwaitCompletionToCommence)ac).MinDelay = ((AwaitCompletionToCommence)tc).MinDelay;
                        ((AwaitCompletionToCommence)ac).MaxDelay = ((AwaitCompletionToCommence)tc).MaxDelay;
                    }
                    at.Constraints.Add(ac);
                    DataStoreItemViewModel acvm = Application.GetLibrarian().GetViewModel(ac);
                    DataStoreItemViewModel atvm = Application.GetLibrarian().GetViewModel(at);
                    BaseTaskViewModel btvm = (BaseTaskViewModel)atvm;
                    ((BaseTaskViewModel)atvm).Constraints.Add((BaseConstraintViewModel)acvm);
                    btvm.RaisePropertyChanged("Actions");
                    btvm.RaisePropertyChanged("AvailableActions");
                    
                }
                DataStoreItemViewModel p = Application.GetLibrarian().GetViewModel(at);
                p.SaveItem();
                taskCounter++;

            }

            
            RaisePropertyChanged("Actions");
            RaisePropertyChanged("AvailableActions");
            ToggleWorkflowSelector();
            */
        }

     
        private void RescheduleTasks(TimeSpan ts)
        {
            
        }

        private void AddTask()
        {
            if (SelectedTaskType == null)
            {
                DesktopApplication.ShowDialog("Error", "Please select a type of task");
                return;
            }
            
            if(SelectedTaskType == "Arrival")
            {
                ArrivalTask a = new ArrivalTask();
                ((Appointment)Item).Tasks.Add(a);
                a.Appointment = ((Appointment)Item);
                a.ScheduledCompletionTime = ((Appointment)Item).ScheduledArrivalTime.Date.Add(SelectedTaskTime.TimeOfDay);
                Tasks.Add(a);
                return;
            }

            if(SelectedTaskType == "Injection")
            {
                DoseAdministrationTask d = new DoseAdministrationTask();
                ((Appointment)Item).Tasks.Add(d);
                d.Appointment = ((Appointment)Item);
                d.ScheduledCompletionTime = ((Appointment)Item).ScheduledArrivalTime.Date.Add(SelectedTaskTime.TimeOfDay);
                Tasks.Add(d);
                return;
            }
            if (SelectedTaskType == "Scan")
            {
                ScanTask d = new ScanTask();
                ((Appointment)Item).Tasks.Add(d);
                d.Appointment = ((Appointment)Item);
                d.ScheduledCommencementTime = ((Appointment)Item).ScheduledArrivalTime.Date.Add(SelectedTaskTime.TimeOfDay);
                d.ScheduledCompletionTime = d.ScheduledCommencementTime.AddMinutes(15);
                Tasks.Add(d);
                return;
            }
        }

        private void viewTask()
        {
            if(SelectedTask is DoseAdministrationTask)
            {
                DoseAdministrationTaskViewModel datvm = new DoseAdministrationTaskViewModel(SelectedTask as DataStoreItem);
                DesktopApplication.MakeModalDocument(datvm);
            }
            else if(SelectedTask is ScanTask)
            {
                ScanTaskViewModel svm = new ScanTaskViewModel(SelectedTask as DataStoreItem);
                DesktopApplication.MakeModalDocument(svm);
                
            }
            else if(SelectedTask is ArrivalTask)
            {
                ArrivalTaskViewModel avm = new ArrivalTaskViewModel(SelectedTask as DataStoreItem);
                DesktopApplication.MakeModalDocument(avm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.StandardNonFiniteTaskView");
            }
        }

        private async void cancelTask()
        {
            if(SelectedTask != null)
            {
                bool x = await DesktopApplication.ShowConfirmDialog("Warning", "Are you sure you want to cancel this task?");
                if(x)
                {
                    (SelectedTask as BasicTask).Cancelled = true;
                    DesktopApplication.Librarian.SaveItem(SelectedTask);
                    TasksView.Refresh();
                }
                
            }
        }

        private bool FilterTask(object task)
        {
            BasicTask t = (BasicTask)task;
            if (t.IsCancelled)
                return false;
            if (t.Deleted)
                return false;
            return true;
        }
        #endregion

        #region RelayCommands
        public RelayCommand AddTaskCommand
        {
            get;
            private set;
        }

        public RelayCommand CreateWorkflowCommand
        {
            get;
            private set;
        }

                
        public RelayCommand LoadTemplateCommand
        {
            get;
            private set;
        }

        public RelayCommand ViewTaskCommand
        {
            get;
            private set;
        }

        public RelayCommand CancelTaskCommand
        {
            get;
            private set;
        }
        #endregion  


    }
}
