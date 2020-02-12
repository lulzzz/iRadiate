using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Equipment
{
    public class EquipmentItem : DataStoreItem
    {
        private EquipmentItem _parent;
        private string _name;
        private DateTime _purchaseDate;        
        private string _serialNumber;
        private List<EquipmentItem> _subEquipmentItems;
        private DateTime? _disposalDate;
        private bool _disposed;
        private bool _functional;
        private DateTime? _lastCalibrationDate;
        private DateTime? _lastServiceDate;
        private EquipmentItemType _itemType;

        public EquipmentItem() : base()
        {
            
        }

        public EquipmentItemType ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }
        public virtual EquipmentItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual DateTime PurchaseDate
        {
            get { return _purchaseDate; }
            set { _purchaseDate = value; }
        }

        public virtual string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; }
        }

        public virtual List<EquipmentItem> SubEquipmentItems
        {
            get {
                if (_subEquipmentItems == null)
                    _subEquipmentItems = new List<EquipmentItem>();
                return _subEquipmentItems; }
            set { _subEquipmentItems = value; }
        }

        public DateTime? DisposalDate
        {
            get { return _disposalDate; }
            set { _disposalDate = value; }
        }

        public bool Disposed
        {
            get { return _disposed; }
            set { _disposed = value; }
        }

        /// <summary>
        /// Gets or sets whether the equipment is currently functional or not
        /// </summary>
        public bool Functional
        {
            get { return _functional; }
            set { _functional = value; }
        }
        
        public string FullName
        {
            get
            {
                if (Parent != null)
                    return Parent.FullName + "." + Name;
                return Name; 
            }
        }

       
    }

    public class EquipmentItemType : DataStoreItem
    {
        private string _name;

        public EquipmentItemType() : base()
        {

        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
