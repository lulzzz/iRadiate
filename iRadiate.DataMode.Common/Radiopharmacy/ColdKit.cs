using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Radiopharmacy
{
    public class Kit : BaseInventoryItem, IKit
    {
        private Chemical _radiopharmaceutical;
        private int _totalVials;
        private int _remainingVials;
        private List<IBulkDose> _bulkDoses;
        private Chemical _ingredient;
        private KitDefinition _kitDefinition;

        public Kit() : base()
        {
            ExpiryDate = DateTime.Now.AddDays(1);
        }
        
        /// <summary>
        /// The radiopharmaceutical which is produced by this kit
        /// </summary>
        [Queryable]
        public virtual Chemical Radiopharmaceutical
        {
            get
            {
                if (KitDefinition != null)
                {
                    return KitDefinition.Product;
                }

                return null;
            }
            
        }

        /// <summary>
        /// Gets or set the total number of vials that came with this kit
        /// </summary>
        [Queryable]
        public int TotalVials
        {
            get
            {
                return _totalVials;
            }
            set
            {
                _totalVials = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of vials remaining
        /// </summary>
        [Queryable]
        public int RemainingVials
        {
            get
            {
                return _remainingVials;
            }
            set
            {
                _remainingVials = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(Kit);
            }
        }

        public override bool IsExpirable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the list of BulkDoses which have been reconstituted from this kit
        /// </summary>
        public virtual List<IBulkDose> BulkDoses
        {
            get
            {
                if(_bulkDoses == null)
                {
                    _bulkDoses = new List<IBulkDose>();
                }
                return _bulkDoses;
            }
            set
            {
                _bulkDoses = value;
            }
        }

        /// <summary>
        /// The ingredient chemical that is needed to make the final product
        /// </summary>
        /// <remarks>
        /// Example HDP cold kit required TcO4 ingredient to produce Tc99m-HDP. In this model, 
        /// each cold kit uses one ingredient to produce its Radiopharmaceutical
        /// </remarks>
        public virtual Chemical RadioactiveIngredient
        {
            get
            {
                if (KitDefinition != null)
                {
                    return KitDefinition.RadioactiveIngredient;
                }

                return null;
            }
            
        }


        public override string InventoryName
        {
            get
            {
                if(KitDefinition != null)
                {
                    return KitDefinition.Name;
                }
                return base.InventoryName;
            }
        }

      

        public List<IUnitDose> UnitDoses
        {
            get
            {
                throw new NotImplementedException();
            }

           
        }
        [Queryable]
        public virtual KitDefinition KitDefinition
        {
            get
            {
                return _kitDefinition;
            }

            set
            {
                _kitDefinition = value;
            }
        }

       

        /// <summary>
        /// Fires when the cold kit has been reconstituted and a new bulkdose add to the ColdKit's BulkDoses property
        /// </summary>
        public event EventHandler<NewDataStoreItemEventArgs> ColdKitReconstituted;

        protected virtual void OnColdKitReconstituted(NewDataStoreItemEventArgs e)
        {
            if(ColdKitReconstituted != null)
            {
                ColdKitReconstituted(this, e);
            }
        }
        public ReconstitutedColdKit ReconstituteColdKit(double activity, DateTime calibrationDate, DateTime expiryDate, string batchNumber, double volume, BaseBulkDose ingredient, double totalVolume)
        {
            ReconstitutedColdKit bd = new ReconstitutedColdKit();
            bd.CalibrationActivity = activity;
            bd.CalibrationDate = calibrationDate;
            bd.ExpiryDate = expiryDate;
            bd.Manufacturer = "In-house";
            bd.ManufacturerBatchNumber = batchNumber;
            bd.Supplier = "In-house";
            bd.Radiopharmaceutical = Radiopharmaceutical;
            bd.Volume = totalVolume;
            bd.ColdKit = this;
            RemainingVials--;

            ///Uncomment all of this!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (ingredient.Volume < volume)
            {
                ingredient.Volume = 0;
            }
            else
            {
                ingredient.Volume = ingredient.Volume - volume;
            }

            if (ingredient.CurrentActivity < activity)
            {
                ingredient.CalibrationActivity = 0;
                ingredient.CalibrationDate = calibrationDate;
            }
            else
            {
                ingredient.CalibrationActivity = ingredient.CurrentActivity - activity;
                ingredient.CalibrationDate = calibrationDate;
            }


            BulkDoses.Add(bd);
            OnColdKitReconstituted(new NewDataStoreItemEventArgs(bd));
            return bd;
        }

        public override string QuantityString
        {
            get
            {
                if (RemainingVials == 1)
                    return "1 vial left";
                return RemainingVials.ToString() + " vials left";
            }
        }

    }

    public class KitDefinition:DataStoreItem, IKitDefinition
    {
        private bool _coldAdministerable;
        private string _name;
        private Chemical _product;
        private Chemical _radioactiveIngredient;

        public KitDefinition() : base()
        {

        }

        public bool ColdAdministerable
        {
            get
            {
                return _coldAdministerable;
            }

            set
            {
                _coldAdministerable = value;
            }
        }

        [Queryable]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        [Queryable]
        public Chemical Product
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

        [Queryable]
        public Chemical RadioactiveIngredient
        {
            get
            {
                return _radioactiveIngredient;
            }

            set
            {
                _radioactiveIngredient = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(KitDefinition);
            }
        }
    }
}
