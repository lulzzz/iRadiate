using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Radiopharmacy
{
    public class RadiochemicalPurityAnalysis:DataStoreItem
    {
        private List<RadiochemicalPurityMeasurement> _measurements;
        private bool _pass;
        private BaseBulkDose _bulkDose;

        public RadiochemicalPurityAnalysis() : base()
        {

        }

        public virtual List<RadiochemicalPurityMeasurement> Measurements
        {
            get
            {
                if (_measurements == null)
                    _measurements = new List<RadiochemicalPurityMeasurement>();
                return _measurements;
            }
            set { _measurements = value; }
        }

        public bool Pass
        {
            get { return _pass; }
            set { _pass = value; }
        }

        public virtual BaseBulkDose BulkDose
        {
            get { return _bulkDose; }
            set { _bulkDose = value; }
        }
        
    }

    public class RadiochemicalPurityMeasurement : DataStoreItem
    {
        private RadiochemicalPurityAnalysis _analysis;
        private string _impurity;
        private double _impurityFraction;

        public RadiochemicalPurityMeasurement() : base()
        { 

        }

        public virtual RadiochemicalPurityAnalysis Analysis
        {
            get { return _analysis; }
            set
            {
                _analysis = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the impurity being measured e.g. colloid, free pertechnetate etc
        /// </summary>
        public string Impurity
        {
            get { return _impurity; }
            set { _impurity = value; }
        }

        /// <summary>
        /// Gets or sets the fraction of the sample which is impure
        /// </summary>
        public double ImpurityFraction
        {
            get { return _impurityFraction; }
            set { _impurityFraction = value; }
        }
    }
}
