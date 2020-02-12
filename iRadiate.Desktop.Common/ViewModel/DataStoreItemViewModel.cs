using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using NLog;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;

namespace iRadiate.Desktop.Common.ViewModel
{

    /// <summary>
    /// A class that can wrap around in DataStoreItem
    /// </summary>
    /// <remarks>
    /// This class exposes the Item property and the view knows the underlying properties
    /// of the Item which it binds to. This class provides the generic relay commands
    /// so that the view can manipulate the Item in an abstracted way.
    /// </remarks>
    public class DataStoreItemViewModel : Module, IDataStoreItemViewModel
    {
        
        #region  privateFields
        protected IDataStoreItem _item;
        protected readonly object locker = new object();
        private bool _readOnly = true;
        protected new static Logger logger = LogManager.GetCurrentClassLogger();

        private bool _editButtonVisible = true;
        private bool _saveButtonVisible = false;
        private bool _detailsButtonVisible;
        private bool _reloadButtonVisible;
        private bool _deleteButtonVisible;

        private bool _busy;
        #endregion

        #region events
      
        public event EventHandler ItemDeleted;
        public event EventHandler SomethingChanged;

     
        protected virtual void OnItemDeleted()
        {
            //Console.WriteLine("OnItemDeleted()");
            EventHandler handler = ItemDeleted;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
        public virtual void OnSomethingChanged(EventArgs e)
        {

            EventHandler handler = SomethingChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnSomethingChanged(e);

        }
        public virtual void RaiseAllPropertiesChanged()
        {
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                RaisePropertyChanged(pi.Name);
            }
        }
        #endregion

        #region constructors
        public DataStoreItemViewModel():base()
        {
            NonUIThreadInitialize();
        }

        public DataStoreItemViewModel(IDataStoreItem item):base()
        {

           
            SetItem(item);
            item.PropertyChanged += Item_PropertyChanged;
            NonUIThreadInitialize();
        }

        
        #endregion

        #region virtuals
       

        public virtual IDataStoreItem Item
        {
            get 
            {
                lock (locker)
                {
                    return _item; 
                }
                
            }
        }

        public virtual void SetItem(IDataStoreItem Item)
        {
            _item = Item;
            RaisePropertyChanged("Item");
           
            Item.PropertyChanged += Item_PropertyChanged;
        }

        public virtual void SaveItem()
        {
            try
            {
                DesktopApplication.Librarian.SaveItem(Item);
                //EditButtonVisible = true;
                //SaveButtonVisible = false;
                Close();
            }
            

            catch (Exception ex)
            {
                if(ex.InnerException == null)
                {
                    DesktopApplication.ShowDialog("Error caught", ex.Message);
                }
                else
                {
                    DesktopApplication.ShowDialog("Error caught", ex.Message + "; inner exception: " + ex.InnerException.Message);
                }
            }
        }

        public virtual void SaveItemWithoutClosing()
        {
            try
            {
                DesktopApplication.Librarian.SaveItem(Item);
                //EditButtonVisible = true;
                //SaveButtonVisible = false;
                
            }


            catch (Exception ex)
            {
                DesktopApplication.ShowDialog("Error caught", ex.Message);
            }
        }

        public virtual void ReloadItem()
        {
            DesktopApplication.Librarian.ReloadItem(Item);
            
        }

        public virtual void Edit()
        {
            _editButtonVisible = false;
            _saveButtonVisible = true;
            RaisePropertyChanged("EditButtonVisible");
            RaisePropertyChanged("SaveButtonVisible");
        }

        public virtual void Delete()
        {
            //Console.WriteLine("Delete()");
            if (Deleted)
            {
                DesktopApplication.GetLibrarian().UndeleteItem(Item);
            }
            else
            {
                DesktopApplication.GetLibrarian().DeleteItem(Item);
            }
            
            RaisePropertyChanged("Deleted");
            OnItemDeleted();
            Close();
        }

        [Obsolete("Use Title")]
        public virtual string DocumentTitle
        {
            get
            {
                return Item.ConcreteType.Name;
            }
        }

        [Obsolete("We use Content Controls now")]
        public virtual string DocumentIcon
        {
            get
            {
                return "/iRadiate.Desktop.Common;component/Images/DetailsIcon.png";
            }
        }

        public virtual bool Deleted
        {
            get
            {
                return Item.Deleted;
            }
        }

  
        public override void NonUIThreadInitialize()
        {
            base.NonUIThreadInitialize();
        }
        public override void UIThreadInitialize()
        {
            base.UIThreadInitialize();
        }

        #endregion

       

        #region relayCommands
        public RelayCommand SaveCommand { get; private set; }

        public RelayCommand ReloadCommand { get; private set; }

        public RelayCommand EditCommand { get; private set; }

        public RelayCommand DeleteCommand { get; private set; }

        public RelayCommand ViewDetailsCommand { get; private set; }

        public RelayCommand ViewDetailsDocumentCommand { get; private set; }

        public RelayCommand ViewMetaDataCommand { get; private set; }
        #endregion

        public bool IsPropertyACollection(PropertyInfo property)
        {
            if (property.PropertyType == typeof(string))
            {
                return false;
            }
            if(property.PropertyType == typeof(Byte[]))
            {
                return false;
            }
            return property.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) != null;
        } 

        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            SaveCommand = new RelayCommand(SaveItem);
            ReloadCommand = new RelayCommand(ReloadItem);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
            ViewMetaDataCommand = new RelayCommand(viewMetaData);
        }

        private void viewMetaData()
        {
            System.Diagnostics.Debug.Print("viewMetaData()...");
            DesktopApplication.Librarian.GetAlterations(Item);
            DesktopApplication.MakeModalDocument(this, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.MetaDataView");
            System.Diagnostics.Debug.Print("viewMetaData()...Done");
        }

    }
}
