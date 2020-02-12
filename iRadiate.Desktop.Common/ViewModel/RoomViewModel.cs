using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{
    [Obsolete]
    public class RoomViewModel : DataStoreItemViewModel
    {
        public string Name
        {
            get
            {
                return ((Room)Item).Name;
            }
            set
            {
                ((Room)Item).Name = value;
                RaisePropertyChanged("Name");
            }
        }

        public int MaximumOccupancy
        {
            get
            {
                return ((Room)Item).MaximumOccupancy;
            }
            set
            {
                ((Room)Item).MaximumOccupancy = value;
                RaisePropertyChanged("MaximumOccupancy");
            }
        }

        public bool CameraRoom
        {
            get
            {
                return ((Room)Item).CameraRoom;
            }
            set
            {
                ((Room)Item).CameraRoom = value;
                RaisePropertyChanged("CameraRoom");
            }
        }
    }
}
