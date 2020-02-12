using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using iRadiate.Common;

using iRadiate.Desktop.Common.ViewModel;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NLog;
using MahApps.Metro.IconPacks;

namespace iRadiate.Desktop.Common
{

    public interface IModule
    {
        string Name { get; set; }

        event EventHandler Opening;

        event EventHandler Opened;

        event EventHandler Closing;

        void GetData();

        void Close();

        void UIThreadInitialize();

        void NonUIThreadInitialize();        

        int Order { get; set; }

        ContentControl IconContent { get; }

        [Obsolete]
        string IconSource { get; set; }

        string Title { get; set; }

        
        
    }
    /// <summary>
    /// A module is a special viewmodel that represents a user accessible screen
    /// </summary>
    /// <remarks>
    /// Through some as-yet-undesigned system the application will locate the appropriate view,
    /// instantiate that view, set the module as its datacontext and add it to some visual container 
    /// for the user to interact with.
    /// </remarks>
    public abstract class Module : ViewModelBase, IModule
    {
        #region privateFields
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private bool _busy;

        private string _name;

        private bool _supportsMulti;

        [Obsolete]
        private AsyncObservableCollection<DataStoreItemViewModel> _viewModelCollection;
        #endregion

        #region events

        public event EventHandler Opening;
        public event EventHandler Opened;
        public event EventHandler Closing;

       
       
        protected virtual void OnClosing()
        {
            EventHandler handler = Closing;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
        protected virtual void OnOpening()
        {
            EventHandler handler = Opening;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected virtual void OnOpened()
        {
            EventHandler handler = Opened;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
        #endregion

        #region constructor
        public Module()
        {

            SetRelayCommands();
            
        }
        #endregion

        #region publicProperties
        /// <summary>
        /// Gets or sets the order in which modules are isted
        /// </summary>
        public virtual int Order
        {
            get;
            set;
        }

        public virtual ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.NintendoSwitch;
                icon.Height = 18;
                icon.Width = 18;
                cc.Content = icon;
                return cc;
            }
        }

        /// <summary>
        /// Gets or sets whether the module is busy
        /// </summary>
        /// <remarks>
        /// The UI can bind a busy indicator to this variable to make a smaller-scoped ui trigger.
        /// Alternatively the busy indicator in the main view model can be used.
        /// </remarks>
        public bool Busy
        {
            get { return _busy; }
            set
            {
                _busy = value;
                RaisePropertyChanged("Busy");
            }
        }

        public virtual string IconSource { get; set; }

        /// <summary>
        /// Gets or sets the name of the module which appears as a title
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets whether multiple copies of this module can be opened
        /// </summary>
        public bool SupportsMulti
        {
            get
            {
                return _supportsMulti;
            }
            set
            {
                _supportsMulti = value;
            }
        }
        #endregion

        [Obsolete]
        public AsyncObservableCollection<DataStoreItemViewModel> ViewModelCollection
        {
            get
            {
                if (_viewModelCollection == null)
                {
                    //_viewModelCollection = new AsyncObservableCollection<DataStoreItemViewModel>();
                }
                return _viewModelCollection;
            }
            set
            {
                _viewModelCollection = value;
                RaisePropertyChanged("ViewModelCollection");
            }
        }

        protected virtual void SetRelayCommands()
        {
            GetDataCommand = new RelayCommand(GetData);
            AddNewCommand = new RelayCommand(AddNew);
            CloseCommand = new RelayCommand(Close);
        }

        #region publicMethods
        public virtual void GetData()
        {

        }
        public virtual void AddNew()
        {

        }

        public virtual void Close()
        {
            OnClosing();
        }



      /// <summary>
      /// Initializes parts of the module that must be done on the UI thread
      /// </summary>
        public virtual void UIThreadInitialize()
        {

        }

        /// <summary>
        /// Initializes parts of the module that can be done on the non-UI thread
        /// </summary>
        public virtual void NonUIThreadInitialize()
        {
            
        }

        #endregion

        #region commands
        public RelayCommand GetDataCommand { get; private set; }

        public RelayCommand AddNewCommand { get; private set; }

        public RelayCommand CloseCommand { get; private set; }

        public string Title
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
