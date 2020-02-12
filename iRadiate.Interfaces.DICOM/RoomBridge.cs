using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;

namespace iRadiate.Interfaces.DICOM
{
    public class RoomBridge :ViewModelBase
    {
        private int _ID, _roomID;
        private string _serialNumber;
        private Room _room;
        private string _modelName;
        public RoomBridge()
        {

        }
        public int ID
        {
            get { return _ID; }
            set { _ID = value; RaisePropertyChanged("ID"); }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; RaisePropertyChanged("SerialNumber"); }
        }
        public int RoomID
        {
            get { return _roomID; }
            set { _roomID = value;  }
        }
        public Room Room
        {
            get { return _room; }
            set { _room = value; RoomID = Room.ID; RaisePropertyChanged("Room"); }
        }

        public string ModelName
        {
            get { return _modelName; }
            set { _modelName = value; RaisePropertyChanged("ModelName"); }
        }
        public AsyncObservableCollection<IDataStoreItem> AllRooms
        {
            get
            {
                return DesktopApplication.Librarian.GetItems(typeof(Room), new List<RetrievalCriteria>());
            }
        }
    }
}
