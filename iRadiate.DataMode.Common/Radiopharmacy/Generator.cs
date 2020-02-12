using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.Common;
using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Radiopharmacy
{
    public class Generator : BaseRadioactiveInventoryItem
    {
        private Isotope _parentRadionuclide;
        private List<Elution> _elutions;
        private Chemical _product;


        public override string InventoryName
        {
            get
            {
                if(ParentRadionuclide != null)
                {
                    return ParentRadionuclide.Name;
                }
                return base.InventoryName;
            }
        }

        [Queryable]
        public virtual Isotope ParentRadionuclide
        {
            get
            {
                return _parentRadionuclide;
            }
            set
            {
                _parentRadionuclide = value;                
            }
        }

        [Queryable]
        public virtual Isotope DaughterRadionuclide
        {
            get
            {
                if(Isotope != null)
                {
                    return ParentRadionuclide.Daugher;
                }
                return null;
            }
        }
        public override Type ConcreteType
        {
            get
            {
                return typeof(Generator);
            }
        }
        public override bool IsExpirable
        {
            get
            {
                return true;
            }
        }

        public override Chemical Radiopharmaceutical
        {
            get
            {
                return Product;
            }

            set
            {
                //base.Radiopharmaceutical = value;
            }
        }

        [Queryable]
        public virtual Chemical Product
        {
            get
            {
                return _product;
            }
            set
            {
                _product = value;
            }
        }
        public virtual List<Elution> Elutions
        {
            get
            {
                if(_elutions == null)
                {
                    _elutions = new List<Elution>();
                }
                return _elutions;
            }
            set
            {
                _elutions = value;
            }
        }

        public override double CurrentActivity
        {
            get
            {
                if (ParentRadionuclide == null)
                {
                    return 0;
                }
                return CalibrationActivity * Math.Exp(-(Math.Log(2) / (ParentRadionuclide.HalfLife / 86400)) * (DateTime.Now - CalibrationDate).TotalDays);
            }
        }

        public void Elute(double volume, double activity, double moly, DateTime ElutionDate, DateTime ExpiryDate, string batchNumber)
        {
            Elution result = new Elution();
            result.Creator = iRadiate.DataModel.Common.DataStoreItem.CurrentUser;
            result.Radiopharmaceutical = _product;
            result.Volume = volume;
            result.Breakthrough = moly;
            result.Generator = this;
            result.ManufacturerBatchNumber = batchNumber;
            result.Supplier = "In-house";
            result.Manufacturer = "In-house";
            result.CalibrationActivity = activity;
            result.CalibrationDate = ElutionDate;
            result.ExpiryDate = ExpiryDate;
            OnGeneratorEluting(new NewDataStoreItemEventArgs(result));
            Elutions.Add(result);
            OnGeneratorEluted(new NewDataStoreItemEventArgs(result));
            
        }

        #region events
        /// <summary>
        /// Fires when the generator is about to insert an elution into it's Elutions collection
        /// </summary>
        public event EventHandler<NewDataStoreItemEventArgs> GeneratorEluting;

        protected virtual void OnGeneratorEluting(NewDataStoreItemEventArgs e)
        {
            if(GeneratorEluting != null )
                GeneratorEluting(this, e);
        }

        /// <summary>
        /// Fires when the generator is eluted and a new object has been added to the Elutions collection
        /// </summary>
        public event EventHandler<NewDataStoreItemEventArgs> GeneratorEluted;

        protected virtual void OnGeneratorEluted(NewDataStoreItemEventArgs e)
        {
            if (GeneratorEluted != null)
            {
                GeneratorEluted(this, e);
            }
        }
        #endregion
    }

   public class Elution : BaseBulkDose, IElution
    {
        private Generator _generator;
        private double _breakthrough;

        public Elution() : base()
        {

        }
        public Generator Generator
        {
            get
            {
                return _generator;
            }
            set
            {
                _generator = value;
            }
        }

       
        public override Type ConcreteType
        {
            get
            {
                return typeof(Elution);
            }
        }

        /// <summary>
        /// Gets or sets the breakthrough of the elution
        /// </summary>
        /// <remarks>
        /// This is the breakthrough measured in %
        /// </remarks>
        public double Breakthrough
        {
            get
            {
                return _breakthrough;
            }
            set
            {
                _breakthrough = value;
            }
        }

       
    }
}
