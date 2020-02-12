using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Radiopharmacy
{
    public enum DisposalStatus { [Display(Name="Not disposed")]NotDisposed, Disposed, Returned}
    public interface IInventory
    {
        string Supplier { get; set; }
        string Manufacturer { get; set; }
        bool IsExpirable { get;  }
        DateTime ExpiryDate { get; set; }

        string ExpiryString { get; }
        DateTime DateAdded { get; set; }
        bool Expired { get;  }
        string SupplierBatchNumber { get; set; }
        string ManufacturerBatchNumber { get; set; }
        int InventoryIDNumber { get; }
        string InventoryName { get; }

        string QuantityString { get; }

        DisposalStatus Disposed { get; set; }

        Nullable<DateTime> DisposalDate { get; set; }

        User Disposer { get; set; }

        bool IsDisposed { get; }

        event EventHandler ItemDisposed;

        event EventHandler ItemUnDisposed;

    }

    public abstract class BaseInventoryItem : DataStoreItem, IInventory
    {
        private double _calibrationActivity;
        private DateTime _calibratioDate;
        private Chemical _radiopharmaceutical;
        private string _supplier;
        private string _manufacturer;
        private DateTime _expiryDate;
        private DateTime _dateAdded;
        private string _supplierBatchNumber;
        private string _manufacturerBatchNumber;
        private DisposalStatus _disposed;
        private Nullable<DateTime> _disposalDate;
        private User _disposer;
        public BaseInventoryItem():base()
        {
            DateAdded = DateTime.Now;
        }

        [Queryable]
        public string Supplier
        {
            get
            {
                return _supplier;
            }

            set
            {
                _supplier = value;
            }
        }

        [Queryable]
        public string Manufacturer
        {
            get
            {
                return _manufacturer;
            }

            set
            {
                _manufacturer = value;
            }
        }

        public virtual bool IsExpirable
        {
            get
            {
                return true;
            }


        }

        [Queryable]
        public DateTime ExpiryDate
        {
            get
            {
                return _expiryDate;
            }

            set
            {
                _expiryDate = value;
            }
        }

        public string ExpiryString
        {
            get
            {
                if (!IsExpirable)
                    return "Does not expire";
                if ((ExpiryDate - DateTime.Now).TotalHours < 0)
                    return "Expired";
                if ((ExpiryDate - DateTime.Now).TotalHours < 48)
                    return (ExpiryDate - DateTime.Now).TotalHours.ToString("N0") + " hours";
                return (ExpiryDate - DateTime.Now).TotalDays.ToString("N0") + " days";
            }
        }


        [Queryable]
        public DateTime DateAdded
        {
            get
            {
                return _dateAdded;
            }

            set
            {
                _dateAdded = value;

            }
        }

        public virtual bool Expired
        {
            get
            {
                if (!IsExpirable)
                    return false;
                if (ExpiryDate < DateTime.Now)
                    return true;
                else
                    return false;
            }


        }

        [Queryable]
        public string SupplierBatchNumber
        {
            get
            {
                return _supplierBatchNumber;
            }

            set
            {
                _supplierBatchNumber = value;
            }
        }

        [Queryable]
        public string ManufacturerBatchNumber
        {
            get
            {
                return _manufacturerBatchNumber;
            }

            set
            {
                _manufacturerBatchNumber = value;
            }
        }

        [Queryable]
        public int InventoryIDNumber
        {
            get
            {
                return ID;
            }
        }

        public virtual string InventoryName
        {
            get
            {
                return "Unkown";
            }

            
        }

        public virtual DisposalStatus Disposed
        {
            get
            {
                return _disposed;
            }

            set
            {
                _disposed = value;
                if(!(_disposed == DisposalStatus.NotDisposed))
                {
                    OnItemDisposed();
                }
                if(_disposed == DisposalStatus.NotDisposed)
                {
                    OnItemUnDisposed();
                }
               
            }
        }

        [Queryable]
        public DateTime? DisposalDate
        {
            get
            {
                return _disposalDate;
            }

            set
            {
                _disposalDate = value;
            }
        }

        [Queryable]
        public virtual User Disposer
        {
            get
            {
                return _disposer;
            }

            set
            {
                _disposer = value;
            }
        }

        public bool IsDisposed
        {
            get
            {
                if(Disposed != DisposalStatus.NotDisposed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// A string giving a readable indication of the quantity remaining
        /// </summary>
        /// <remarks>
        /// For a radioactive item it might be XXX MBQ where as for a vial it might be X vials (Y remaining)
        /// </remarks>
        public virtual string QuantityString
        {
            get
            {
                return "Quantity unknown";
            }
        }

        #region events
        public event EventHandler ItemDisposed;

        public event EventHandler ItemUnDisposed;

        protected virtual void OnItemDisposed()
        {
            ItemDisposed?.Invoke(this, new EventArgs());
        }

        protected virtual void OnItemUnDisposed()
        {
            ItemUnDisposed?.Invoke(this, new EventArgs());
        }
        #endregion
    }

    public abstract class BaseRadioactiveInventoryItem: BaseInventoryItem, IRadioactive
    {
        private double _calibrationActivity;
        private DateTime _calibratioDate;
        private Chemical _radiopharmaceutical;

        public BaseRadioactiveInventoryItem() : base()
        {

        }

        [Queryable]
        public double CalibrationActivity
        {
            get
            {
                return _calibrationActivity;
            }

            set
            {
                _calibrationActivity = value;
            }
        }

        [Queryable]
        public DateTime CalibrationDate
        {
            get
            {
                return _calibratioDate;
            }

            set
            {
                _calibratioDate = value;
            }
        }

        [Queryable]
        public virtual double CurrentActivity
        {
            get
            {
                if(Radiopharmaceutical == null)
                {
                    return 0;
                }
                return CalibrationActivity * Math.Exp(-(Math.Log(2) / (Radiopharmaceutical.Isotope.HalfLife / 86400))*(DateTime.Now-CalibrationDate).TotalDays);
            }
        }

        [Queryable]
        public virtual Chemical Radiopharmaceutical
        {
            get
            {
                return _radiopharmaceutical;
            }

            set
            {
                _radiopharmaceutical = value;
            }
        }

        [Queryable]
        public Isotope Isotope
        {
            get
            {
                if(Radiopharmaceutical != null)
                {
                    return Radiopharmaceutical.Isotope;
                }
                else
                {
                    return null;
                }
            }

            set
            {

            }
        }

        public override string QuantityString
        {
            get
            {
                return CurrentActivity.ToString("N0") + " MBq";
            }
        }
    }
}
