using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitsNet;
using UnitsNet.Units;

namespace iRadiate.DataModel.DataDictionary
{
    /// <summary>
    /// Represents a QA Variable with a quantifiable value
    /// </summary>
    public class MeasureableDataDictionaryEntry : QuantifableDataDictionaryEntry
    {

       
        private QuantityType? _quantityType;
        private string _minimumValueUnit;
        private double _minimumValueNumber;
        private double _maximumValueNumber;
        private string _maximumValueUnit;

        public MeasureableDataDictionaryEntry() : base()
        {
            
            
        }


        public override void Initialize()
        {
            base.Initialize();
            IQuantity min;

            if (QuantityType.HasValue)
            {
                QuantityInfo qInfo = UnitsNet.Quantity.GetInfo(QuantityType.Value);
                if (Quantity.TryParse(qInfo.ValueType, MinimumValue, out min))
                {
                    MinimumValueNumber = min.Value;

                    var x = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(qInfo.UnitType, Convert.ToInt32(min.Unit));
                    MinimumValueNumber = min.Value;
                    MinimumValueUnit = x;
                }
                else
                {
                    MinimumValueNumber = 0;
                    MinimumValueUnit = null;
                }

                IQuantity max;
                if (Quantity.TryParse(qInfo.ValueType, MaximumValue, out max))
                {
                    MaximumValueNumber = max.Value;

                    var x = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(qInfo.UnitType, Convert.ToInt32(max.Unit));
                    MaximumValueNumber = max.Value;
                    MaximumValueUnit = x;
                }
                else
                {
                    MaximumValueNumber = 0;
                    MaximumValueUnit = null;
                }
            }
        }

        public QuantityType? QuantityType
        {
            get { return _quantityType; }
            set { _quantityType = value; }
        }

        public override string ToString()
        {
            return base.ToString() + " - " + QuantityType;
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(MeasureableDataDictionaryEntry);
            }
        }

        public string DefaultUnit
        {
            get
            {
                if (QuantityType.HasValue)
                    return "";
                QuantityInfo i = Quantity.GetInfo(QuantityType.Value);
                return i.BaseUnitInfo.Name;
                
            }
        }

        public string DefaultStringFormat
        {
            get
            {
                return "{}{0:G} " + DefaultUnit;
            }
        }

        public List<string> AvailableUnits
        {
            get
            {
                if (!QuantityType.HasValue)
                    return new List<String>();
                QuantityInfo i = Quantity.GetInfo(QuantityType.Value);
                var res = new List<String>();
                foreach(var u in i.UnitInfos)
                {
                    res.Add(UnitAbbreviationsCache.Default.GetDefaultAbbreviation(i.UnitType,Convert.ToInt32(u.Value)));
                }
                return res;
            }
        }

        

        [NotMapped]
        public override double MinimumValueNumber
        {
            get 
            {
                return _minimumValueNumber;
                
                            
            }
            set
            {
                _minimumValueNumber = value;
                MinimumValue = value.ToString() + " " + MinimumValueUnit;
            }
        }

        [NotMapped]
        public virtual string MinimumValueUnit
        {
            get
            {
                return _minimumValueUnit;
                
            }
            set
            {
                _minimumValueUnit = value;
                MinimumValue = MinimumValueNumber.ToString() + " " + value;
            }
        }

        [NotMapped]
        public double MaximumValueNumber
        {
            get
            {
                return _maximumValueNumber;


            }
            set
            {
                _maximumValueNumber = value;
                MaximumValue = value.ToString() + " " + MaximumValueUnit;
            }
        }

        [NotMapped]
        public virtual string MaximumValueUnit
        {
            get
            {
                return _maximumValueUnit;

            }
            set
            {
                _maximumValueUnit = value;
                MaximumValue = MaximumValueNumber.ToString() + " " + value;
            }
        }





    }
}
