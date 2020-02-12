using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.DataDictionary;
using iRadiate.DataModel.Equipment;

namespace iRadiate.DataModel.Forms
{
    public class DataFormElement : FormElement
    {
        private string _value;
        private DataDictionaryEntry _dataDictionaryEntry;
        private EquipmentItem _equipmentItem;
        private QADataItem _dataItem;

        public DataFormElement() : base()
        {
            
        }
       
        public virtual DataDictionaryEntry DataDictionaryEntry
        {
            get { return _dataDictionaryEntry; }
            set { _dataDictionaryEntry = value; }
        }

        public virtual EquipmentItem EquipmentItem
        {
            get { return _equipmentItem; }
            set { _equipmentItem = value; }
        }

        
    }
}
