using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.DataModel.NucMed
{
    public class Room : DataStoreItem
    {
        private string _name;
        private int _maximumOccupancy;
        private List<Patient> _patients;
        private bool _cameraRoom;
        private List<RoomReservation> _roomReservations;
        private NucMedPractice _nucMedPractice;

        public Room() : base()
        {

        }
        public virtual List<RoomReservation> RoomReservations
        {
            get
            {
                if (_roomReservations == null)
                {
                    _roomReservations = new List<RoomReservation>();
                }
                return _roomReservations;

            }
            set
            {
                _roomReservations = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int MaximumOccupancy
        {
            get { return _maximumOccupancy; }
            set { _maximumOccupancy = value; }
        }


        public bool CameraRoom
        {
            get
            {
                return _cameraRoom;
            }
            set
            {
                _cameraRoom = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(Room);
            }
        }

        public NucMedPractice NucMedPractice
        {
            get
            {
                return _nucMedPractice;
            }
            set
            {
                _nucMedPractice = value;
            }
        }

       
    }


}
