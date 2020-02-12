using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Radiopharmacy
{
    /// <summary>
    /// Represents an element of the periodic table
    /// </summary>
    public class Element : DataStoreItem
    {
        private int _atomicNumber;
        private string _name;
        private string _symbol;
        private List<Isotope> _isotopes;

        /// <summary>
        /// Gets or sets the name of the element
        /// </summary>
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

        /// <summary>
        /// Gets or sets the symbol for the element
        /// </summary>
        public string Symbol
        {
            get
            {
                return _symbol;
            }
            set
            {
                _symbol = value;
            }
        }

        /// <summary>
        /// Gets or sets the atomic number of the element
        /// </summary>
        public int AtomicNumber
        {
            get
            {
                return _atomicNumber;
            }
            set
            {
                _atomicNumber = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(Element);
            }
        }

        public List<Isotope> Isotopes
        {
            get
            {
                if (_isotopes == null)
                {
                    _isotopes = new List<Isotope>();
                }
                return _isotopes;
            }
            set
            {
                _isotopes = value;
            }
        }
    }
}
