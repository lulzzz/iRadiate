using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.DataDictionary
{
     public class QuantifableDataDictionaryEntry : DataDictionaryEntry
    {
        private string _maximumValue;
        private string _minimumValue;
        public QuantifableDataDictionaryEntry() : base()
        {
            //MinimumValue = 0;
            //MaximumValue = 1000000;
            System.Diagnostics.Debug.WriteLine("MinimumValue = " + MinimumValue);
        }

        public virtual string MaximumValue
        {
            get { return _maximumValue; }
            set { _maximumValue = value; }
        }

        public virtual string MinimumValue
        {
            get { return _minimumValue; }
            set { _minimumValue = value; }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(QuantifableDataDictionaryEntry);
            }
        }

        [NotMapped]
        public virtual double MinimumValueNumber
        {
            get
            {
                double val;
                if (double.TryParse(MinimumValue, out val))
                    return val;
                else
                    return 0;
            }
            set
            {
                MinimumValue = value.ToString();
            }
        }

        [NotMapped]
        public virtual double MaxNumber
        {
            get
            {
                double val;
                if (double.TryParse(MaximumValue, out val))
                    return val;
                else
                    return 0;
            }
            set
            {
                MaximumValue = value.ToString();
            }
        }

        
    }
}
