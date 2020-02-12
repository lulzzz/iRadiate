using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;

namespace iRadiate.Common.IO
{
    public class ItemTrackingRegistration
    {
        private IDataStoreItem _item;
        private List<ITrackingSubscriber> _subscribers;
        
        public ItemTrackingRegistration(IDataStoreItem item)
        {
            _item = item;
        }

        /// <summary>
        /// The IDataStoreItem being tracked
        /// </summary>
        public IDataStoreItem Item
        {
            get { return _item; }
        }

        /// <summary>
        /// A List of all the subscribers to the IDataStoreItem being tracked;
        /// </summary>
        public List<ITrackingSubscriber> Subscribers
        {
            get
            {
                if(_subscribers == null)
                {
                    _subscribers = new List<ITrackingSubscriber>();
                }

                return _subscribers;
            }
            set
            {
                _subscribers = value;
            }
        }

        /// <summary>
        /// The number of IModules that are tracking this item
        /// </summary>
        public int NumSubscribers
        {
            get { return Subscribers.Count; }
        }
        
        /// <summary>
        /// Gets the ConcreteType of the Item
        /// </summary>
        public Type ItemType
        {
            get { return Item.ConcreteType; }
        }

        /// <summary>
        /// Gets the ID number of the Item
        /// </summary>
        public Int32 IDNumber { get { return Item.ID; } }

        
    }
}
