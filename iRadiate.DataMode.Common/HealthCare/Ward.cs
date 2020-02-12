using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.HealthCare
{
    /// <summary>
    /// Represents a ward where patients stay.
    /// </summary>
    public class Ward : DataStoreItem
    {
        private string _name;
        private string _abbreviation;
        private Hospital _hospital;

        public Ward() : base()
        {

        }

        /// <summary>
        /// The name of the Ward.
        /// </summary>
        [Auditable]
        [Queryable]
        public virtual string Name
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
        /// The short name of the ward.
        /// </summary>
        [Auditable]
        [Queryable]
        public virtual string Abbreviation
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
        /// The hospital to which the ward belongs.
        /// </summary>
        [Queryable]
        public virtual Hospital Hospital
        {
            get
            {
                return _hospital;
            }
            set
            {
                _hospital = value;
            }
        }

        /// <summary>
        /// Returns the name of the hsopital and the ward
        /// </summary>
        [Queryable]
        public string FullName
        {
            get { return Hospital.Name + " - " + Name; }
        }

        [Queryable]
        public string AbbreviatedFullName
        {
            get { return Hospital.Abbreviation + " - " + Abbreviation; }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(Ward);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
