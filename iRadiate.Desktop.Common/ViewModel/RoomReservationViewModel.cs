using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{
    [Obsolete]
    public class RoomReservationViewModel : DataStoreItemViewModel
    {
        private RoomReservation _reservation;

        public RoomReservationViewModel()
            : base()
        {
           
        }
        public RoomReservationViewModel(DataStoreItem item)
            : base(item)
        {

        }

        public Room Room
        {
            get
            {
                return ((RoomReservation)Item).Room;
            }
            set
            {
                ((RoomReservation)Item).Room = value;
            }
        }

        public List<Room> Rooms
        {
            get
            {
                return DesktopApplication.CurrentPratice.Rooms;
            }
        }

        public DateTime ReservationStart
        {
            get
            {
                return ((RoomReservation)Item).ReservationStart;
            }
            set
            {
                ((RoomReservation)Item).ReservationStart = value;
                RaisePropertyChanged("ReservationStart");
            }
        }

        public DateTime ReservationFinish
        {
            get
            {
                return ((RoomReservation)Item).ReservationFinish;
            }
            set
            {
                ((RoomReservation)Item).ReservationFinish = value;
                RaisePropertyChanged("ReservationFinish");
            }
        }

        public string Description
        {
            get
            {
                return ((RoomReservation)Item).Description;
            }
            set
            {
                ((RoomReservation)Item).Description = value;
                RaisePropertyChanged("Description");
            }
        }
    }
}
