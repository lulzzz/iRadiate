using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.RegularExpressions;

using NLog; 

namespace iRadiate.DataModel.Common
{
    public interface IDataStoreItem
    {
       /// <summary>
       /// ID number of the DatastoreItem
       /// </summary>
       /// <remarks>
       /// This would normally be automatically created by the
       /// </remarks>
        int ID { get; set; }

        /// <summary>
        /// The date on which the datastoreitem was created
        /// </summary>
        DateTime CreationDate { get; set; }

        /// <summary>
        /// The user who created the datastoreitem
        /// </summary>
        User Creator { get; set; }

        /// <summary>
        /// A list of alterations made to the DataStoreItem
        /// </summary>
        /// <remarks>
        /// Due to some entity framework complications this has to be instantiated manually at runtime
        /// </remarks>
        List<DataStoreItemAlteration> Alterations { get; set; } 

        [Obsolete("The librarian will call save onthe whole database")]
        List<DataStoreItem> LinkedItems { get; set; }

        /// <summary>
        /// Gets or sets whether this item has been deleted
        /// </summary>       
        bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date on which the item was deleted
        /// </summary>  
        Nullable<DateTime> DeletionDate { get; set; }

        /// <summary>
        /// Gets the type of this item
        /// </summary>
        /// <remarks>
        /// This property is used because EFDataRetriever creates proxy types that wrap the object.
        /// To make the ViewModel-View work we need the name of the original type
        /// </remarks>
        Type ConcreteType { get; }

        /// <summary>
        /// Clones the item
        /// </summary>
        /// <returns>
        /// A Memberwise clone of the item
        /// </returns>
        IDataStoreItem Clone();

        /// <summary>
        /// Gets or sets the date/time when the obect was last edited
        /// </summary>
        DateTime LastEditDate { get; set; }

        /// <summary>
        /// Gets a string which shows the concrete type in a readable format
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Returns true if the object is the same as this datastore item
        /// </summary>
        /// <param name="obj">The datastore item to be compared</param>
        /// <returns>true if they match</returns>
        bool ItemsMatch(object obj);
       
        /// <summary>
        /// Fires when a property in the object has been changed.
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires when the item has been deleted
        /// </summary>
        event EventHandler ItemDeleted;

        /// <summary>
        /// Fires when the item has been saved
        /// </summary>
        event EventHandler ItemSaved;

        /// <summary>
        /// Fires when the item is about to save
        /// </summary>
        event EventHandler ItemSaving;

        /// <summary>
        /// Prints a summary of all properties to the debugger
        /// </summary>
        void Debug();

        void FireSavedEvent();

        void FireSavingEvent();
    }

    public class PropertyChangedEventArgs : EventArgs
    {
        private string _propertyName;
        private DateTime _changeTime;

        public PropertyChangedEventArgs(string propertyName,DateTime changeTime)
        {
            _propertyName = propertyName;
            _changeTime = changeTime;
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
        } 

        public DateTime ChangeTime
        {
            get
            {
                return _changeTime;
            }
        }
    }

    /// <summary>
    /// An event args used for events where a new IDatastoreItem has been created
    /// </summary>
    public class NewDataStoreItemEventArgs : EventArgs
    {
        /// <summary>
        /// The new IDataStoreItem that was created in this event
        /// </summary>
        public IDataStoreItem NewItem { get; set; }

        public NewDataStoreItemEventArgs(IDataStoreItem newItem)
        {
            NewItem = newItem;
        }
    }

    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    /// <summary>
    /// Base class to implement IDataStoreItem
    /// </summary>
    public abstract class DataStoreItem: IDataStoreItem
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static User CurrentUser { get; set; }
        public static Workstation CurrentWorkstation { get; set; }
        
        #region privateFields
        private int _id;
        private DateTime _creationDate;
        private User _creator;
        //private List<BaseAlteration> _alterations;
        private bool _deleted;
        private Nullable<DateTime> _deletionDate;
       
        private DateTime _lastEditDate;
        private List<DataStoreItemAlteration> _alterations;
        private List<DataStoreItem> _linkedItems;
        #endregion

        #region Constructor
        public DataStoreItem()
        {
            _creationDate = DateTime.Now;
            //_creator = CurrentUser;
            _lastEditDate = _creationDate;
        }
        #endregion

        #region events
        /// <summary>
        /// Fires when a property of the object has been changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires when the item's Deleted Property is changed to true
        /// </summary>
        public event EventHandler ItemDeleted;

        /// <summary>
        /// Fires after the item has  been saved by the iDataLibrarian
        /// </summary>
        public event EventHandler ItemSaved;

        /// <summary>
        /// Fires just before the item is saved;
        /// </summary>
        public event EventHandler ItemSaving;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        protected void OnItemDeleted(EventArgs e)
        {
            if (ItemDeleted != null)
                ItemDeleted(this, e);
        }

        protected void OnItemSaved(EventArgs e)
        {
            if (ItemSaved != null)
                ItemSaved(this, e);
        }


        protected void OnItemSaving(EventArgs e)
        {
            if (ItemSaving != null)
                ItemSaving(this, e);
        }
        public void FireSavedEvent()
        {
            OnItemSaved(new EventArgs());
        }

        public void FireSavingEvent()
        {
            OnItemSaving(new EventArgs());
        }
        #endregion

        /// <summary>
        /// Clones the item
        /// </summary>
        /// <remarks>
        /// I don't think I have implemented this properly
        /// </remarks>
        /// <returns>
        /// A clone of the item where the primitive values are cloned but the references are not.
        /// </returns>
        [Obsolete("I don't think this is implemented correctly")]
        public IDataStoreItem Clone()
        {
            return (IDataStoreItem)this.MemberwiseClone();
        }
       

        #region publicProperties
        

        /// <summary>
        /// Used for concurrency checking
        /// </summary> 
        public DateTime LastEditDate
        {
            get
            {
                return _lastEditDate;
            }
            set
            {
                _lastEditDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique ID for this item.
        /// </summary>
        /// <remarks>
        /// This should be generated by the datastore, be it SQL server or something else.
        /// </remarks>
        [Queryable]
        public virtual int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Gets or sets the date on which this item was created.
        /// </summary>
        public DateTime CreationDate
        {
            get
            {
                return _creationDate;
            }
            set
            {
                _creationDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the iDataCreator that created this item.
        /// </summary>
       
        public virtual User Creator
        {
            get
            {
                return _creator;
            }
            set
            {
                _creator = value;
            }
        }

        [Queryable]
        public string CreatorName
        {
            get
            {
                if (Creator != null)
                    return Creator.FullName;
                return "";
            }
        }

        /// <summary>
        /// Gets or sets a boolean indicating if this item has been deleted.
        /// </summary>
        [Auditable]
        [Queryable]
        public virtual bool Deleted
        {
            get
            {
                return _deleted;
            }
            set
            {
                _deleted = value;
                if (value)
                {
                    OnItemDeleted(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Gets or sets the date-time on which this item was deleted.
        /// </summary>
        [Auditable]
        [Queryable]
        public Nullable<DateTime> DeletionDate
        {
            get
            {
                return _deletionDate;
            }
            set
            {
                _deletionDate = value;
                
                OnPropertyChanged(new PropertyChangedEventArgs("DeletionDate", DateTime.Now));
            }

        }

        /// <summary>
        /// Since the entity framework wraps the object in an inherited type 
        /// we need this to get a reference to the real type we want.
        /// </summary>
        [Queryable]
        public virtual Type ConcreteType
        {
            get
            {
                return typeof(DataStoreItem);
            }
        }

        /// <summary>
        /// A list of alterations made since the last save
        /// </summary>
        [NotMapped]
        public virtual List<DataStoreItemAlteration> Alterations
        {
            get
            {
                if (_alterations == null)
                {
                    _alterations = new List<DataStoreItemAlteration>();
                }
                return _alterations;
            }

            set
            {
                _alterations = value;
            }
        }

        /// <summary>
        /// These items are linked to the datstoreitem for the purposes of savinh
        /// </summary>
        /// <remarks>
        /// When the item is saved we must save also these other items in the process
        /// </remarks>
        [NotMapped]
        public List<DataStoreItem> LinkedItems
        {
            get
            {
                if(_linkedItems == null)
                {
                    _linkedItems = new List<DataStoreItem>();
                }
                return _linkedItems;
            }

            set
            {
                _linkedItems = value;   
            }
        }

        [Queryable]
        public string TypeName
        {
            get
            {
                return Regex.Replace(ConcreteType.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
            }
        }
        #endregion

        /// <summary>
        /// Returns true if the two objects are of the same concrete type and ID number
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ItemsMatch(object obj) 
        {
            if(obj == null)
            {
                return false;
            }
            if (!(obj is DataStoreItem))
            {
                return false;
            }
            if (this.ConcreteType.Name != ((DataStoreItem)obj).ConcreteType.Name)
            {
                return false;
            }
            return (((DataStoreItem)obj).ID == this.ID); 
        }

        public override string ToString()
        {
            return "DataStoreItem";
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Debug()
        {
            logger.Debug("******* DataStoreItem.Debug() *******");
            //Use reflection
            PropertyInfo[] properties = this.ConcreteType.GetProperties();
            foreach(PropertyInfo pi in properties)
            {
                try
                {
                    object propertyValue = pi.GetValue(this, null);
                    logger.Debug(pi.Name + ": " + propertyValue);
                }
                catch(Exception ex)
                {
                    logger.Error(ex,"Exception debugging object");
                }
            }

        }
    }

}
