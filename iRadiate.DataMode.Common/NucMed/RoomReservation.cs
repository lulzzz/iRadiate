using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.NucMed
{
    public class RoomReservation : DataStoreItem
    {
        private DateTime _reservationStart, _reservationFinish;
        private Room _room;
        private string _description;

        public RoomReservation():base()
        {

        }

        public DateTime ReservationStart
        {
            get
            {
                return _reservationStart;
            }
            set
            {
                _reservationStart = value;
            }
        }

        public DateTime ReservationFinish
        {
            get
            {
                return _reservationFinish;
            }
            set
            {
                _reservationFinish = value;
            }
        }

        public Room Room
        {
            get
            {
                return _room;
                
            }
            set
            {
                _room = value;
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
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(RoomReservation);
            }
        }
    }
}
