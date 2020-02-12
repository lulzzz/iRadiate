using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.Common;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace iRadiate.Desktop.Common.ViewModel
{
    [Obsolete]
    public class HospitalViewModel : DataStoreItemViewModel
    {
        private AsyncObservableCollection<DataStoreItemViewModel> _wards;

        public HospitalViewModel():base()
        {
           
            AddNewWardCommand = new RelayCommand(AddWard);
        }

        public HospitalViewModel(DataStoreItem item)
            : base(item)
        {
            
            foreach (Ward w in ((Hospital)Item).Wards)
            {
                Wards.Add(new DataStoreItemViewModel(w));
            }
            AddNewWardCommand = new RelayCommand(AddWard);
        }

        public string Name
        {
            get
            {
                return ((Hospital)Item).Name;
            }
            set
            {
                ((Hospital)Item).Name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string Abbreviation
        {
            get
            {
                return ((Hospital)Item).Abbreviation;
            }
            set
            {
                ((Hospital)Item).Abbreviation = value;
                RaisePropertyChanged("Abbreviation");
            }
        }

        public AsyncObservableCollection<DataStoreItemViewModel> Wards
        {
            get
            {
                if (_wards == null)
                {
                    _wards = new AsyncObservableCollection<DataStoreItemViewModel>();
                }
                return _wards;
            }
            set
            {
                _wards = value;
                RaisePropertyChanged("Wards");
            }
        }

        public override void SetItem(IDataStoreItem item) 
        {
            _item = item;
            Wards = DesktopApplication.CreateCollection();
            foreach (Ward w in ((Hospital)Item).Wards)
            {
                //Wards.Add(Application.GetLibrarian().GetViewModel(w));
            }
            RaisePropertyChanged("Item");
        }

        private void AddWard()
        {
            Ward w = new Ward();
            w.Name = "Name...";
            w.Hospital = (Hospital)Item;
            //Wards.Add(Application.GetLibrarian().GetViewModel(w));
        }

        public RelayCommand AddNewWardCommand
        {
            get;
            private set;
        }
    }
}
