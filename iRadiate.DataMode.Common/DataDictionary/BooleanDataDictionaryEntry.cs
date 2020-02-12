using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.DataDictionary
{
    /// <summary>
    /// Represents a QA variable where the data is either yes/no, pass/fail
    /// </summary>
    public class BooleanDataDictionaryEntry : DataDictionaryEntry
    {
        private BooleanRepresentation _booleanRepresentation;

        public BooleanDataDictionaryEntry() : base()
        {


        }

        public BooleanRepresentation BooleanRepresentation
        {
            get { return _booleanRepresentation; }
            set { _booleanRepresentation = value; }
        }
        public override Type ConcreteType
        {
            get
            {
                return typeof(BooleanDataDictionaryEntry);
            }
        }

    }



    public enum BooleanRepresentation
    {
        [Description("Yes/No")]
        YesNo,
        [Description("Pass/Fail")]
        PassFail,
        [Description("True/False")]
        TrueFalse,
        [Description("Complete/Incomplete")]
        CompleteIncomplete,
        [Description("On/Off")]
        OnOff,
        [Description("Up/Down")]
        UpDown
    }

}
