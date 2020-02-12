using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NLog;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.Desktop.Common;

using iRadiate.DataModel.NucMed;

using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Search.ViewModel
{
    [PreferredView("iRadiate.Desktop.Search.View.PatientListView","iRadiate.Desktop.Common")]
    public class PatientListViewModel : Module
    {
        #region
        private iRadiate.DataModel.HealthCare.Patient _selectedPatient;
        private string _surnameSearch;
        private string _givenNamesSearch;
        private string _mrnSearch;
        private bool _patientSelectd = false;
        private ObservableCollection<IPatientListTool> _patientTools;
        private AsyncObservableCollection<IDataStoreItem> _patients;
        #endregion

        #region constructor
        public PatientListViewModel():base()
        {
            //Should move this to GetData() so it gets clled in a background thread
           
            
        }
        #endregion


        public AsyncObservableCollection<IDataStoreItem> Patients
        {
            get
            {
                return _patients;
            }
            set
            {
                _patients = value;
                RaisePropertyChanged("Patients");
            }
        }
        private void ViewPatient()
        {

            PatientViewModel vm = new PatientViewModel(SelectedPatient);
            DesktopApplication.MakeModalDocument(vm, DesktopApplication.DocumentMode.Edit);
            //SelectedPatient = null;
        }

        [ImportMany(typeof(IPatientListTool))]
        public ObservableCollection<IPatientListTool> PatientTools
        {
            get
            {
                if (_patientTools == null)
                {
                    _patientTools = new ObservableCollection<IPatientListTool>();
                }
                return _patientTools;
            }
            set
            {
                _patientTools = value;
                RaisePropertyChanged("PatientTools");
            }
        }

        public override void GetData()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            AddPatientCommand = new RelayCommand(AddPatient);
            ViewPatientCommand = new RelayCommand(ViewPatient);
            SearchCommand = new RelayCommand(search);
            foreach (IPatientListTool p in PatientTools)
            {
                p.SetPatientList(this);
            }

        }

        public iRadiate.DataModel.HealthCare.Patient SelectedPatient
        {
            get
            {
                return _selectedPatient;
            }
            set
            {
                _selectedPatient = value;
                RaisePropertyChanged("SelectedPatient");
                RaisePropertyChanged("PatientSelected");
                OnSelectionChanged(EventArgs.Empty);
                
            }
        }

        public string SurnameSearch
        {
            get { return _surnameSearch; }
            set { _surnameSearch = value; }
        }

        public string GivenNamesSearch
        {
            get
            {
                return _givenNamesSearch;
            }
            set
            {
                _givenNamesSearch = value;
                RaisePropertyChanged("GivenNamesSearch");
            }
        }

        public string MrnSearch
        {
            get
            {
                return _mrnSearch;
            }
            set
            {
                _mrnSearch = value;
                RaisePropertyChanged("MrnSearch");
            }
        }

        public override string IconSource
        {
            get { return "/iRadiate.Desktop.Common;component/Search/Images/PatientListIcon.png"; }
        }

        public override string Name
        {
            get
            {
                return "Patient List";
            }
            set
            {

            }
        }

        private bool SearchCriteriaEntered()
        {
            if (SurnameSearch != null && SurnameSearch.Trim() != "")
            {
                return true;
            }
            if (GivenNamesSearch != null && GivenNamesSearch.Trim() != "")
            {
                return true;
            }
            if (MrnSearch != null && MrnSearch.Trim() != "")
            {
                return true;
            }
            return false;
        }

        public bool PatientSelected
        {
            get
            {
                if (SelectedPatient != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            
        }

        private void AddPatient()
        {
           
            iRadiate.DataModel.HealthCare.Patient p = new DataModel.HealthCare.Patient();
            DataStoreItemViewModel vm = new DataStoreItemViewModel(p);
            
            DesktopApplication.MakeModalDocument(vm,DesktopApplication.DocumentMode.New);
            
        }

        private void search()
        {
            if (SearchCriteriaEntered())
            {
                object p = new object();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    List<RetrievalCriteria> criter = new List<RetrievalCriteria>();
                    if (SurnameSearch != null && SurnameSearch.Trim() != "")
                    {
                        RetrievalCriteria rc = new RetrievalCriteria("Surname", CriteraType.TextMatch, SurnameSearch);
                        criter.Add(rc);
                    }
                    if (GivenNamesSearch != null && GivenNamesSearch.Trim() != "")
                    {
                        RetrievalCriteria rc = new RetrievalCriteria("GivenNames", CriteraType.TextMatch, GivenNamesSearch);
                        criter.Add(rc);
                    }
                    if (MrnSearch != null && MrnSearch.Trim() != "")
                    {
                        RetrievalCriteria rc = new RetrievalCriteria("PatientID", CriteraType.TextMatch, MrnSearch);
                        criter.Add(rc);
                    }


                    _patients = DesktopApplication.Librarian.GetItems(typeof(Patient), criter);
                    RaisePropertyChanged("Patients");
                    //ViewModelCollection = Application.GetLibrarian().GetViewModels(typeof(iRadiate.DataModel.HealthCare.Patient), criter);


                };
                worker.RunWorkerCompleted += (o, ea) =>
                {

                    Busy = false;
                };
                Busy = true;
                worker.RunWorkerAsync();
            }
        }

        #region commands
        public RelayCommand AddPatientCommand
        {
            get;
            private set;
        }

        public RelayCommand ViewPatientCommand
        {
            get;
            private set;
        }

        public RelayCommand SearchCommand { get; set; }
        #endregion

        #region events
        public event SelectionChangedHandler SelectionChanged;

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        public delegate void SelectionChangedHandler(object sender, EventArgs e);
        #endregion
    }

    
}
