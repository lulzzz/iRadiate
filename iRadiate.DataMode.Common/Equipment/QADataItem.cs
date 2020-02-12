using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Forms;

using iRadiate.DataModel.DataDictionary;

namespace iRadiate.DataModel.Equipment
{
    public class QADataItem : BaseDataItem
    {
        private EquipmentItem _equipmentItem;
        private QAFormInstance _qaFormInstance;

        public QADataItem() : base()
        {

        }

        public virtual EquipmentItem EquipmentItem
        {
            get { return _equipmentItem; }
            set { _equipmentItem = value; }
        } 

        public virtual QAFormInstance QAFormInstance
        {
            get
            {
                return _qaFormInstance;
            }
            set
            {
                _qaFormInstance = value;
            }
        }

    }

    /// <summary>
    /// A recording of a data item for a quantifiable entry in the data dictionary
    /// </summary>
    /// <remarks>
    /// This class provides upper and lower limits on the QA data
    /// </remarks>
    public class QuantifiableQADataItem : QADataItem
    {
        private double _upperLimitLevel;
        private double _lowerLimitLevel;

        public QuantifiableQADataItem() : base()
        {

        }

        public double UpperLimitLevel
        {
            get { return _upperLimitLevel; }
            set { _upperLimitLevel = value; }
        }

        public double LowerLimitLevel
        {
            get { return _lowerLimitLevel; }
            set { _lowerLimitLevel = value; }
        }

        public double Value
        {
            get { return Convert.ToDouble(DataValue); }
            set { DataValue = value.ToString(); }
        }
    }
}
