using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.Common
{
    /// <summary>
    /// Represents an alteration made to a DataStoreItem
    /// </summary>
    /// <remarks>
    /// Due to the entity framework problems this class does not contain a navigation property to the 
    /// datastoreitem that was altered. It does however have a navigation property to the User that
    /// made the alteration.
    /// </remarks>
    public class DataStoreItemAlteration : DataStoreItem, IAlteration
    {
        
        string _propertyName;
        string _oldValue;
        string _newValue;
        DateTime _alterationDate;
        byte[] _alteredRowVersion;
        private User _user;
        private Workstation _workstation;
        private string _datastoreItemName;
        private int _itemIDNumber;

        public DataStoreItemAlteration():base()
        {

        }

        /// <summary>
        /// Specifies the type of datastoreitem altered.
        /// </summary>
        /// <remarks>
        /// Could be Patient, Study, Appointment etc.
        /// </remarks>
        public virtual string DataStoreItemName
        {
            get
            {
                return _datastoreItemName;
            }
            set
            {
                _datastoreItemName = value;

            }
        }

        /// <summary>
        /// The name of the property that was changed.
        /// </summary>
        /// <remarks>
        /// With the property name i should be possible to reverse the altertions
        /// </remarks>
        public virtual string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        /// <summary>
        /// The value that the User changed the property from.
        /// </summary>
        public string OldValue
        {
            get
            {
                return _oldValue;
            }
            set
            {
                _oldValue = value;
            }
        }

        /// <summary>
        /// The value that the User changed the property to.
        /// </summary>
        public string NewValue
        {
            get
            {
                return _newValue;
            }
            set
            {
                _newValue = value;
            }
        }
        
              
        /// <summary>
        /// The alteration at which the workstation was made
        /// </summary>
        public virtual Workstation Workstation
        {
            get
            {
                return _workstation;
            }
            set
            {
                _workstation = value;
            }
        }

        /// <summary>
        /// The ID Number of the DataStoreItem which has been altered
        /// </summary>
        public int ItemIDNumber
        {
            get
            {
                return _itemIDNumber;
            }
            set
            {
                _itemIDNumber = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(DataStoreItemAlteration);
            }
        }

        public override string ToString()
        {
            return "DataStoreItemAlteration";
        }

    }
}
