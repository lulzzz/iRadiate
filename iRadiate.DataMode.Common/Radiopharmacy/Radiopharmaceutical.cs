using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Radiopharmacy
{
    /// <summary>
    /// A radiopharmaceutical
    /// </summary>
  
    public class Chemical : DataStoreItem
    {
        private Isotope _isotope;
        private string _ligand;
        private string _ligandAbbreviation;
        private TimeSpan _unitDoseExpiry;
        private bool _isReconstitutionIngredient, _isAdministerable;
        private bool? _isGaseous;

        [Queryable]
        public virtual Isotope Isotope
        {
            get { return _isotope; }
            set { _isotope = value; }
        }

        [Queryable]
        public virtual string Ligand
        {
            get { return _ligand; }
            set { _ligand = value; }
        }

        [Queryable]
        public virtual string LigandAbbreviation
        {
            get { return _ligandAbbreviation; }
            set { _ligandAbbreviation = value; }
        }

        [Queryable]
        public string Name
        {
            get
            {
                string returnVal = "";
                if(Isotope == null)
                {
                    return returnVal;
                }
                returnVal = Isotope.WeightString + "-" + Isotope.Element.Symbol + "-" + LigandAbbreviation;
                return returnVal;
            }
        }

        /// <summary>
        /// Gets the timespan before expiry when drawn into a unit dose
        /// </summary>
        public TimeSpan UnitDoseExpiry
        {
            get
            {
                return _unitDoseExpiry;
            }
            set
            {
                _unitDoseExpiry = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(Chemical);
            }
        }

        /// <summary>
        /// Gets or sets whether this chemical is used in reconstituting cold kits
        /// </summary>
        /// '<remarks>
        /// A cold kit has a product which is a chemical that is injectable and an
        /// ingredient which is a chemical that is a reconstitution ingredient
        /// </remarks>
        [Obsolete("The KitDefinition class covers this requirement")]
        public bool IsReconstitutionIngredient
        {
            get
            {
                return _isReconstitutionIngredient;
            }
            set
            {
                _isReconstitutionIngredient = value;
            }
        }

        /// <summary>
        /// Gets or sets whether this chemical can be administered to patients
        /// </summary>
        public bool IsAdministerable
        {
            get
            {
                return _isAdministerable;
            }
            set
            {
                _isAdministerable = value;
            }
        }

        /// <summary>
        /// Gets whether this chemical is radioactive
        /// </summary>
        public bool IsRadioactive
        {
            get
            {
                if(Isotope == null)
                {
                    return false;
                }

                return true;
            }
        }

       public bool? IsGaseous
        {
            get { return _isGaseous; }
            set { _isGaseous = value; }
        }
    }
}
