using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Windows.Data;

using GalaSoft.MvvmLight.Command;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel;
using iRadiate.DataModel.NucMed;

using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Common.RoomReservations
{
    [PreferredView("iRadiate.Desktop.Common.RoomReservations.RoomReservationListView", "iRadiate.Desktop.Common")]
    public class RoomReservationsModule : Module
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private AsyncObservableCollection<DataStoreItemViewModel> _reservations;
        private CollectionViewSource viewSource;
        private DataStoreItemViewModel _selectedRoomReservation;


        public RoomReservationViewModel SelectedReservation
        {
            get
            {
                return (RoomReservationViewModel)SelectedRoomReservation;
            }
        }
        public RoomReservationsModule ()
        {
           
            AddNewReservationCommand = new RelayCommand(AddNewReservation);
        }

        public DataStoreItemViewModel SelectedRoomReservation
        {
            get
            {
                return _selectedRoomReservation;
            }
            set
            {
                _selectedRoomReservation = value;
                RaisePropertyChanged("SelectedRoomReservation");
                RaisePropertyChanged("SelectedReservation");
            }
        }

        public AsyncObservableCollection<DataStoreItemViewModel> Reservations
        {
            get
            {
                if (_reservations == null)
                {
                    _reservations = new AsyncObservableCollection<DataStoreItemViewModel>();
                }
                return _reservations;
            }
            set
            {
                _reservations = value;
                RaisePropertyChanged("Reservations");
            }
        }

        public ICollectionView ReservationsView
        {
            get;
            set;
        }
        public override void GetData()
        {
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            RetrievalCriteria rcstart = new RetrievalCriteria("ReservationStart",CriteraType.GreaterThan,new DateTime(DateTime.Today.Year,1,1));
            RetrievalCriteria rcfinish = new RetrievalCriteria("ReservationFinish",CriteraType.LessThan,new DateTime(DateTime.Today.Year+2,1,1));
            rcList.Add(rcstart);
            rcList.Add(rcfinish);
            //Reservations = Application.GetLibrarian().GetViewModels(typeof(RoomReservation), rcList);
            //ReservationsView = Application.CreateCollectionView(Reservations);
            //ReservationsView.Filter = CustomerFilter;
            
            //viewSource = new CollectionViewSource();
            //viewSource.Source = Reservations;
            

        }

        private bool CustomerFilter(object item)
        {
            DataStoreItemViewModel customer = item as DataStoreItemViewModel;
            if (customer == null)
            {
                return true;
            }
            if (customer.Deleted)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void UIThreadInitialize()
        {
            base.UIThreadInitialize();
            ReservationsView.Refresh();
        }

        private void AddNewReservation()
        {
            RoomReservation r = new RoomReservation();
            r.Description = "Describe what this reservation is for";
            r.ReservationStart = DateTime.Now;
            r.ReservationFinish = DateTime.Now.AddHours(1);
            //DataStoreItemViewModel rvm = Application.GetLibrarian().GetViewModel(r);
            DataStoreItemViewModel rvm = new DataStoreItemViewModel(r);
            
            
            Reservations.Add(rvm);
        }

        public RelayCommand AddNewReservationCommand
        {
            get;
            set;
        }

        public override string Name
        {
            get
            {
                return "Reservations";
            }
            set
            {

            }
        }

        public override string IconSource
        {
            get { return "/iRadiate.Desktop.Common;component/Images/ReservationsIcon.png"; ; }
        }

    }
}
