using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;


using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Desktop.Search.ViewModel;


using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NLog;

namespace iRadiate.Desktop.Search
{
    public interface IPatientListTool
    {
        void Execute();

        bool Available { get; }

        string IconSource { get; }

        string Name { get; }
        
        void SetPatientList(PatientListViewModel patientList);
    }

    public class BasePatientListTool : ViewModelBase, IPatientListTool
    {
        private bool _available;
        private PatientListViewModel _patientList;

        public BasePatientListTool()
        {
            ExecuteCommand = new RelayCommand(Execute);
        }

        public virtual void Execute()
        {
            throw new NotImplementedException();
        }

        public bool Available
        {
            get
            {
                return _available;
            }
            set
            {
                _available = value;
                RaisePropertyChanged("Available");
            }

        }

        public virtual string IconSource
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string Name
        {
            get
            {
                return "Base";
            }
        }
        
        public RelayCommand ExecuteCommand
        {
            get;
            set;
        }

        public virtual void SetPatientList(PatientListViewModel list)
        {
            _patientList = list;
            _patientList.SelectionChanged += _patientList_SelectionChanged;
        }

        public virtual void _patientList_SelectionChanged(object sender, EventArgs e)
        {
            if (_patientList.SelectedPatient != null)
            {
                Available = true;
            }
            else
            {
                Available = false;
            }
        }
        public PatientListViewModel PatientList
        {
            get
            {
                return _patientList;
            }
        }
    }

    [Export(typeof(IPatientListTool))]
    public class DetailsPatientLisTool : BasePatientListTool
    {
        public override string Name
        {
            get
            {
                return "View Patient Details";
            }
        }

        public override string IconSource
        {
            get
            {
                return "/iRadiate.Desktop.Common;component/Images/DetailsIcon.png";
            }
        }

        public override void Execute()
        {
            //Application.ShowDialog("Alert", "ToolClicked");
            //DataStoreItemViewModel vm = new DataStoreItemViewModel(PatientList.SelectedPatient);
            PatientViewModel vm = new PatientViewModel(PatientList.SelectedPatient);
            DesktopApplication.MakeModalDocument(vm, DesktopApplication.DocumentMode.Edit);
        }
    }
}
