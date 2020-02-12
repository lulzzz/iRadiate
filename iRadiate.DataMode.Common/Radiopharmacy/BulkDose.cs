using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.DataModel.Radiopharmacy
{
    
    public abstract class BaseBulkDose : BaseRadioactiveInventoryItem, IBulkDose
    {
        private double _volume; // in mL
        private List<BaseUnitDose> _unitDoses;
        private Elution _elution;
        private Kit _coldkit;
        private Chemical _chemical;
        private RadiochemicalPurityAnalysis _qcAnalysis;
        private BaseBulkDose _bulkDose;

        public BaseBulkDose() : base()
        {
            
            
        }

        public event EventHandler<NewDataStoreItemEventArgs> UnitDoseDrawn;

        protected virtual void OnUnitDosedrawn(NewDataStoreItemEventArgs e)
        {
            if (UnitDoseDrawn != null)
                UnitDoseDrawn(this, e);
        }
        /// <summary>
        /// Gets or sets the volume of the BulkDose in mLs
        /// </summary>
        [Queryable]
        public double Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the collection of UnitDoses that have been made from this BulkDose
        /// </summary>
        public virtual List<BaseUnitDose> UnitDoses
        {
            get
            {
                if(_unitDoses == null)
                {
                    _unitDoses = new List<BaseUnitDose>();
                }
                return _unitDoses;
            }
            set
            {
                _unitDoses = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(BaseBulkDose);
            }
        }
       
        /// <summary>
        /// Draws a unit dose from this bulk dose
        /// </summary>
        /// <param name="activity">The activity of the unit dose</param>
        /// <param name="volume">The volume of the unit dose</param>
        /// <param name="calibrationDate">The calibration date of the unit dose</param>
        /// <param name="expiryDate">The expiry date of the unit dose</param>
        /// <param name="batchNumber">The batch number of the unit dose</param>
        /// <returns>The unt dose which has just been drawn</returns>
        public SyringeUnitDose DrawDose(double activity, double volume, DateTime calibrationDate, DateTime expiryDate, string batchNumber, double TotalVolume)
        {
            
            SyringeUnitDose u = new SyringeUnitDose();
            u.Manufacturer = "In-house";
            u.ManufacturerBatchNumber = batchNumber;
            u.Supplier = "In-house";
            u.Radiopharmaceutical = this.Radiopharmaceutical;
            u.CalibrationActivity = activity;
            u.CalibrationDate = calibrationDate;
            u.ExpiryDate = ExpiryDate;
            u.Volume = volume;
            u.BulkDose = this;
            UnitDoses.Add(u);
            //this.Volume = this.Volume - volume;
            //this.CalibrationActivity = this.CurrentActivity - activity;
            //this.CalibrationDate = calibrationDate;
            OnUnitDosedrawn(new NewDataStoreItemEventArgs(u));
            return u;
           
        }

        [Queryable]
       public virtual Kit ColdKit
        {
            get
            {
                return _coldkit;
            }
            set
            {
                _coldkit = value;
            }
        }

        /// <summary>
        /// Gets or sets the elution with which this BulkDose was reconstituted
        /// </summary>
        /// <remarks>
        /// This should be in ReconstitutedColdKit class!
        /// </remarks>
        public Elution Elution
        {
            get
            {
                return _elution;
            }
            set
            {
                _elution = value;
            }
        }

        public override string InventoryName
        {
            get
            {
                if(Radiopharmaceutical != null)
                {
                    return Radiopharmaceutical.Name;
                }
                return base.InventoryName;
            }
        }

        public virtual RadiochemicalPurityAnalysis QCAnalysis
        {
            get { return _qcAnalysis; }
            set { _qcAnalysis = value; }
        }

       

    }

   
    public class ReconstitutedColdKit : BaseBulkDose
    {
        public ReconstitutedColdKit() : base()
        {

        }



        public override string InventoryName
        {
            get
            {
                return Radiopharmaceutical.Name;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(ReconstitutedColdKit);
            }
        }

        public override double CurrentActivity
        {
            get
            {
                var retVal = base.CurrentActivity;
                foreach(var u in UnitDoses)
                {
                    //decay to calibration date
                    retVal = retVal - u.CurrentActivity;
                }
                return retVal;
            }
        }
    }
}
