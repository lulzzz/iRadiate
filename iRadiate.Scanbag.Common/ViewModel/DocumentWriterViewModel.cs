using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NLog;

namespace iRadiate.Scanbag.Common.ViewModel
{
    [PreferredView("iRadiate.Scanbag.Common.View.DocumentWriterView", "iRadiate.Scanbag.Common")]
    public class DocumentWriterViewModel : Module
    {
        private Study _study;
        private FlowDocument _document;
        ObservableCollection<IStudyDataProvider> _dataProviders;
        private IStudyDataProvider _currentProvider;
        private DataProviderParameter _selectedParameter;
        private string _description;

        #region overrides
        public override string Name
        {
            get
            {
                return Patient.FullName;
            }
            set
            {

            }
        }
        public override string IconSource
        {
            get { return "/iRadiate.ScanBag.Common;component/Images/DocumentWriterIcon.png"; }
        }
        #endregion

        public DocumentWriterViewModel()
        {
            Init();
        }
        public override void GetData()
        {
           
        }
        public DocumentWriterViewModel(Study s)
        {
            SetStudy(s);
            Init();
        }

        private void Init()
        {
            SaveCommand = new RelayCommand(SaveDocument);
            _document = new FlowDocument();
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
           
            foreach (IStudyDataProvider p in DataProviders)
            {
                p.SetStudy(_study);
            }
        }
        public void SetStudy(Study s)
        {
            _study = s;
        }

        public FlowDocument Document
        {
            get
            {
                return _document;
            }
            set
            {
                _document = value;
                RaisePropertyChanged("Document");
            }
        }

        [ImportMany(typeof(IStudyDataProvider))]
        public ObservableCollection<IStudyDataProvider> DataProviders
        {
            get
            {
                if (_dataProviders == null)
                {
                    _dataProviders = new ObservableCollection<IStudyDataProvider>();
                }
                return _dataProviders;
            }
            set
            {
                _dataProviders = value;
            }
        }

        private void SaveDocument()
        {
           
          
        }

        public Study Study
        {
            get
            {
                return _study;
            }
        }
        public Patient Patient
        {
            get
            {
                return _study.Patient;
            }
        }
        public RelayCommand SaveCommand
        {
            get;
            private set;
        }

        public IStudyDataProvider CurrentProvider
        {
            get
            {
                return _currentProvider;
            }
            set
            {
                _currentProvider = value;
                RaisePropertyChanged("CurrentProvider");
                RaisePropertyChanged("CurrentParameters");
            }
        }

        public List<DataProviderParameter> CurrentParameters
        {
            get
            {
                if (_currentProvider != null)
                {
                    return _currentProvider.Paramaters;
                }
                return null;
            }
        }

        public DataProviderParameter SelectedParameter
        {
            get
            {
                return _currentProvider.Parameter;
            }
            set
            {
                _currentProvider.Parameter = value;
                RaisePropertyChanged("SelectedParameter");
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged("Descrition");
            }
        }
    }
}
