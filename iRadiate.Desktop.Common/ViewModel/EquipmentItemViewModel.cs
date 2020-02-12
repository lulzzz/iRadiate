using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;

namespace iRadiate.Desktop.Common.ViewModel
{
    public class EquipmentItemViewModel : DataStoreItemViewModel
    {
        private List<IDataStoreItem> _equipmentItems;

        public List<IDataStoreItem> EquipmentItems
        {
            get
            {
                if (_equipmentItems == null)
                    _equipmentItems = new List<IDataStoreItem>();
                return _equipmentItems;
            }
            set
            {
                _equipmentItems = value;
            }
        }
    }
}
