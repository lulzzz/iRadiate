using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;

using System.Windows.Data;

using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{
    [Obsolete]
    public class StudyTypeViewModel : DataStoreItemViewModel
    {
        ObservableCollection<WorkflowTemplateViewModel> _workflows;
        private WorkflowTemplateViewModel _selectedWorkflow;
        private bool _workflowSelected = false;

        public bool WorkflowSelected
        {
            get
            {
                return _workflowSelected;
            }
            set
            {
                _workflowSelected = value;
                RaisePropertyChanged("WorkflowSelected");
            }
        }

        public WorkflowTemplateViewModel SelectedWorkflow
        {
            get
            {
                return _selectedWorkflow;
            }
            set
            {
                _selectedWorkflow = value;
                RaisePropertyChanged("SelectedWorkflow");
                WorkflowSelected = true;
            }
        }
        public StudyTypeViewModel()
            : base()
        {
            
        }

        protected override void SetRelayCommands()
        {
            base.SetRelayCommands(); 
           
        }

      
        public override void NonUIThreadInitialize()
        {
            base.NonUIThreadInitialize();
           
            foreach (WorkflowTemplate w in ((StudyType)Item).Workflows.Where(x=>x.Deleted == false))
            {
                DataStoreItemViewModel wvm = new DataStoreItemViewModel(w);
                Workflows.Add((WorkflowTemplateViewModel)wvm);
            }
        }
                      
        public string Name
        {
            get
            {
                return ((StudyType)Item).Name;
            }
            set
            {
                ((StudyType)Item).Name = value;
                RaisePropertyChanged("Name");
            }
        }

        public ObservableCollection<WorkflowTemplateViewModel> Workflows
        {
            get
            {
                if (_workflows == null)
                {
                    _workflows = new ObservableCollection<WorkflowTemplateViewModel>();
                }
                return _workflows;
            }
            set
            {
                _workflows = value;
                RaisePropertyChanged("Workflows");
            }
        }

        public RelayCommand RemoveWorkflowCommand
        {
            get;
            private set;
        }

        

        
       
    }
}
