using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.HealthCare
{
    /// <summary>
    /// Represents a hospital (private or public or anything)
    /// </summary>
    public class Hospital : DataStoreItem
    {
        private string _name;
        private string _abbreviation;
        private List<Ward> _wards;

        public Hospital() : base()
        {

        }

        /// <summary>
        /// The full name for the hospital.
        /// </summary>
        [Auditable]
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

        /// <summary>
        /// A short name for the hospital.
        /// </summary>
        [Auditable]
        [Queryable]
        public string Abbreviation
        {
            get
            {
                return _abbreviation;
            }
            set
            {
                _abbreviation = value;
            }
        }

        /// <summary>
        /// All the wards at this hospital
        /// </summary>
        public virtual List<Ward> Wards
        {
            get
            {
                if (_wards == null)
                {
                    _wards = new List<Ward>();
                }
                return _wards;
            }
            set
            {
                _wards = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(Hospital);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
