using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.DataDictionary;

namespace iRadiate.DataModel.Forms
{
    public class NumericFormElement : DataFormElement
    {
        private double _number;
        private int _numberOfDecimals;
        private string _format;
        private string _unit;
        

        private double _upperWarningLevel;
        private double _lowerWarningLevel;

        public NumericFormElement() : base()
        {

        }
       
        public int NumberOfDecimals
        {
            get { return _numberOfDecimals; }
            set { _numberOfDecimals = value; }
        }

        /// <summary>
        /// The abbreviation of the unit used for displaying
        /// </summary>
        /// <remarks>
        /// This will be something like "cm", or "mV". The DataFormElement
        /// has a reference to a DataDictionaryEntry from which it will become obivous
        /// what the abbreviation means
        /// </remarks>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        /// <summary>
        /// The maximum value that the controll will allow, a higher number would be meaningless in the context
        /// </summary>
        public double Maximum
        {
            get { return (DataDictionaryEntry as QuantifableDataDictionaryEntry).MaxNumber;  }
          
        }

        /// <summary>
        /// The minimum value that the control will allow, a lower number would be meaningless in the context.
        /// </summary>  
        public double Minimum
        {
            get { return (DataDictionaryEntry as QuantifableDataDictionaryEntry).MinimumValueNumber; }
           
        }

        /// <summary>
        /// If the user enters a number at this level or higher, some visual warning will show
        /// </summary>
        public double UpperWarningLevel
        {
            get { return _upperWarningLevel; }
            set { _upperWarningLevel = value; }
        }

        /// <summary>
        /// If the user enteres a number at this level or lower, some visual warning will show
        /// </summary>
        public double LowerWarningLevel
        {
            get { return _lowerWarningLevel; }
            set { _lowerWarningLevel = value; }
        }
    }

    
}
