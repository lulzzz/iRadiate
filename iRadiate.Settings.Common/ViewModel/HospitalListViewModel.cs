using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.Common;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel;
using iRadiate.Common.IO;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;

using GalaSoft.MvvmLight.Command;

namespace iRadiate.Settings.Common.ViewModel
{
    [PreferredView("iRadiate.Settings.Common.View.HospitalListView", "iRadiate.Settings.Common")]
    public class HospitalListViewModel : Module
    {
        private Hospital _selectedHospital;
        private AsyncObservableCollection<IDataStoreItem> _hospitals;

        public HospitalListViewModel()
        {
            AddHospitalCommand = new RelayCommand(AddNew);
        }

        public Hospital SelectedHospital
        {
            get
            {
                return _selectedHospital;
            }
            set
            {
                _selectedHospital = value;
                RaisePropertyChanged("SelectedHospital");
            }
        }

        public AsyncObservableCollection<IDataStoreItem> Hospitals
        {
            get
            {
                return _hospitals;
            }
            set
            {
                _hospitals = value;
            }
        }
        public override void GetData()
        {
            List<RetrievalCriteria> criteria = new List<RetrievalCriteria>();
            Hospitals = DesktopApplication.Librarian.GetItems(typeof(Hospital), criteria);
            
        }
        
        public override void AddNew()
        {
            Hospital newHospital = new Hospital();
            newHospital.Name = "Enter name";


            DataStoreItemViewModel stvm = new DataStoreItemViewModel(newHospital);
            Hospitals.Add(newHospital);
            
            DesktopApplication.MakeModalDocument(stvm);
        }

        public RelayCommand AddHospitalCommand { get; private set; }
    }
}
