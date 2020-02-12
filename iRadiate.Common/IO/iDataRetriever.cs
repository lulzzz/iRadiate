using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;

namespace iRadiate.Common.IO
{
    public interface IDataRetriever
    {
        /// <summary>
        /// Retrieve an iDataStoreItem from the store using a guid
        /// </summary>
        /// <param name="id">The identifier of the item</param>
        /// <param name="type">The type of item being retrieved</param>
        /// <returns></returns>
        IDataStoreItem RetrieveItem(int id, Type type);

        /// <summary>
        /// Retrieves a single iDataStoreItem from the store
        /// </summary>
        /// <param name="item">The item to be retrieved</param>
        /// <returns></returns>
        IDataStoreItem RetrieveItem(IDataStoreItem item);

        /// <summary>
        /// The total number of items retrieved since instantiation
        /// </summary>
        int NumberOfItemsRetrieved { get; }

        /// <summary>
        /// The total number of IDataStoreItem save since instantiation
        /// </summary>
        int NumberOfItemsSaved { get; }

        /// <summary>
        /// Saves an iDataStoreItem to the database, inserting if it is not in there.
        /// </summary>
        /// <param name="item">The iDataStoreItem to be retrieved</param>
        /// <returns></returns>
        bool SaveItem(IDataStoreItem item);

        /// <summary>
        /// Saves the list of items to the database as a transaction with a rollback if one fails. 
        /// Inserts into the database as needed.
        /// </summary>
        /// <param name="items">The List of iDataStoreItem that are being saved</param>
        /// <returns></returns>
        bool SaveItems(ICollection<IDataStoreItem> items);

        /// <summary>
        /// Gets the first IDataStoreItem that matches 
        /// </summary>
        /// <param name="type">The type of iDataStoreItem to be retrieved</param>
        /// <param name="criteria">The list of criteria that must be followed</param>
        /// <returns>The first item that matches the criteria</returns>
        IDataStoreItem RetrieveItem(Type type, ICollection<RetrievalCriteria> criteria);

        /// <summary>
        /// Retrieves a List of IDataStoreItem that match the given criteria
        /// </summary>
        /// <param name="type">The type of item to retrieve</param>
        /// <param name="criteria">The criteria by which tye filter</param>
        /// <returns>A List of IdataStoreItem that match the critiera</returns>
        /// <remarks>
        /// Returning as a list already calls the SQL select and does the round trip
        /// </remarks>
        List<IDataStoreItem> RetrieveItems(Type type, ICollection<RetrievalCriteria> criteria);

        /// <summary>
        /// Retrieves an IEnumerable of the given type
        /// </summary>
        /// <param name="type">The type retrieved</param>
        /// <returns></returns>
        IEnumerable<IDataStoreItem> RetrieveItems(Type type);

        

        void UpdateItem(IDataStoreItem item);

        void DeleteItem(IDataStoreItem Item);

        void UnDeleteItem(IDataStoreItem item);

        void ReloadAll();

        void SwitchOnAutoDetect();

        void SwitchOffAutoDetect();

        /// <summary>
        /// Firest when the item is about to be saved
        /// </summary>
        event EventHandler ItemSaving;

        /// <summary>
        /// Fires when the item has just been saved
        /// </summary>
        event EventHandler ItemSaved;

        /// <summary>
        /// Fires when an item has been retrieved
        /// </summary>
        event EventHandler ItemRetrieved;

        int TotalItemsTracked { get; }

        int NumberOfModifiedItems { get; }

        List<ModifiedDataStoreItem> ModifiedDataStoreItems { get; }
        //RetrieveAppointments(DateTime date);
    }

    public class RetrievalCriteria
    {
        private string _propertyName;
        private CriteraType _criteriaType;
        private object _filterValue;

        public RetrievalCriteria(string propertyName, CriteraType criteriaType, object filterValue)
        {
            _propertyName = propertyName;
            _criteriaType = criteriaType;
            _filterValue = filterValue;
        }

        public string PropertyName 
        {
            get { return _propertyName; }
            
        }

        public CriteraType CriteriaType
        {
            get { return _criteriaType; }
        }

        public object FilterValue
        {
            get { return _filterValue; }
        }
    }

    public enum CriteraType {
        TextMatch, ExactTextMatch, Equals, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, IsNull, IsNotNull};

    public class DataStoreItemProperty
    {
        public DataStoreItemProperty()
        {

        }
        public bool Modified
        {
            get { if (OriginalValue != CurrentValue) return true; return false; }
        }
        public string PropertyName { get; set; }

        public string OriginalValue { get; set; }

        public string CurrentValue { get; set; }
    }

    public class ModifiedDataStoreItem
    {
        private List<DataStoreItemProperty> _properties;

        public ModifiedDataStoreItem()
        {

        }

        public string ItemType { get; set; }

        public string IDNumber { get; set; }

        public string Name { get; set; }
        public List<DataStoreItemProperty> Properties
        {
            get
            {
                if (_properties == null)
                    _properties = new List<DataStoreItemProperty>();

                return _properties;
            }
            set { _properties = value; }
        }
    }
   
}
