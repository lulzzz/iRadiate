using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;

namespace iRadiate.Desktop.Common.ViewModel
{
    [PreferredView("iRadiate.Desktop.Search.View.PatientDetailsView","iRadiate.Desktop.Common")]
    public class PatientViewModel : DataStoreItemViewModel
    {

        #region privateFields

        private StudyViewModel _selectedStudy;
        private AsyncObservableCollection<StudyViewModel> _studies;
        #endregion

        #region constructors
        public PatientViewModel()
            : base()
        {
            
            
            
        }

        public PatientViewModel(IDataStoreItem item)
            : base(item)
        {
           
        }
        #endregion

        #region overrides
        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            AddStudyCommand = new RelayCommand(AddStudy);
            ViewReportCommand = new RelayCommand(ViewReport);
        }
        public override void NonUIThreadInitialize()
        {
            base.NonUIThreadInitialize();

            if(((Patient)Item).Studies == null)
            {
                DesktopApplication.ShowDialog("Error", "Studies are null??");
            }
            else
            {
                foreach (Study s in ((Patient)Item).Studies)
                {
                    StudyViewModel svm = new StudyViewModel(s);
                    Studies.Add(svm);
                }
            }
        }

        public override string DocumentTitle
        {
            get
            {
                return ((Patient)Item).GivenNames + " " + ((Patient)Item).Surname;
            }
        }

        public override string DocumentIcon
        {
            get
            {
                return "/iRadiate.Desktop.Common;component/Images/PatientIcon.png";
            }
        }
        #endregion

        #region publicProperties
        
        public AsyncObservableCollection<StudyViewModel> Studies
        {
            get {
                if (_studies == null)
                {
                    _studies = new AsyncObservableCollection<StudyViewModel>();
                }
                return _studies; }
            set { _studies = value; }
        }

        public bool IsStudySelected
        {
            get
            {
                if(SelectedStudy == null)
                {
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region privateMethods
        private void AddStudy()
        {
           
            Study s = new Study();
            s.Practice = DesktopApplication.CurrentPratice;
            ((Patient)Item).Studies.Add(s);
            StudyViewModel vm = new StudyViewModel(s);
            Studies.Add(vm);
            SelectedStudy = vm;
            
        }

        private void ViewReport()
        {
            if (SelectedStudy == null)
                return;
            if ((SelectedStudy.Item as Study).Report == null)
                return;
            StudyReportViewModel viewModel = new StudyReportViewModel((SelectedStudy.Item as Study).Report);
            DesktopApplication.MakeModalDocument(viewModel);
        }

         
      
        #endregion

        #region RelayCommands

        public RelayCommand AddStudyCommand { get; private set; }
        public RelayCommand StudyTypeSelectedCommand { get; private set; }
       
        public RelayCommand ViewReportCommand { get; private set; }
        
        #endregion


        public StudyViewModel SelectedStudy
        {
            get { return _selectedStudy; }
            set { _selectedStudy = value;
                RaisePropertyChanged("SelectedStudy"); RaisePropertyChanged("IsStudySelected"); }
        }
    }

  

  

   
}
