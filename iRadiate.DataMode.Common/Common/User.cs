using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.Common
{
    /// <summary>
    /// Represents a human user of the system.
    /// </summary>
    public partial class User : Person, IDataCreator
    {
        private string _pinNumber;
        private string _loginName;
        private string _password;
        private string _settingsString;
        
        private ICollection<DataStoreItemAlteration> _alterationsByUser;
        
        public User() : base()
        {

        }
        
        /// <summary>
        /// Gets or sets the user's login name
        /// </summary>
        [Auditable]
        [Queryable]
        public virtual string LoginName
        {
            get { return _loginName; }
            set { _loginName = value; }
        }

        /// <summary>
        /// Gets or sets the user's password
        /// </summary>
        public virtual string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Gets the description of this person
        /// </summary>
        public virtual string Description
        {
            get
            {
                return FullName;
            }
            set
            {
                
            }
        }

        /// <summary>
        /// Gets or sets the users pinNumber
        /// </summary>
        /// <remarks>
        /// Must be a string of numbers
        /// </remarks>
        public virtual string PinNumber
        {
            get
            {
                return _pinNumber;
            }
            set
            {
                int i = 0;
                if (int.TryParse(value,out i))
                {
                    _pinNumber = value.ToString();
                }
            }
        }

        /// <summary>
        /// The alterations made by this user to items in the datastore.
        /// Not to be cofused with Alterations, the alterations to the user as DataStoreItem
        /// </summary>
        public virtual ICollection<DataStoreItemAlteration> AlterationsByUser 
        {
            get 
            {
                if (_alterationsByUser == null)
                {
                    _alterationsByUser = new List<DataStoreItemAlteration>();
                }
                return _alterationsByUser; 
            }
            set
            {
                _alterationsByUser = value;

            }
        }

        /// <summary>
        /// The settings for the user
        /// </summary>
        /// <remarks>
        /// This is intended to be an XML document which stores things like security tokens
        /// </remarks>
        public virtual string SettingsString
        {
            get
            {
                return _settingsString;
            }
            set
            {
                _settingsString = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(User);
            }
        }

        

    }
}
