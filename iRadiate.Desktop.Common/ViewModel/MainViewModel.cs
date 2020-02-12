using GalaSoft.MvvmLight;
using System.Runtime.Remoting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Radiopharmacy;
using NLog;
using MahApps.Metro;
using Xceed.Wpf.AvalonDock.Layout;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace iRadiate.Desktop.Common.ViewModel
{
    public class SimpleCommand : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; }
        public Action<object> ExecuteDelegate { get; set; }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
                return CanExecuteDelegate(parameter);
            return true; // if there is no can execute default to true
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
                ExecuteDelegate(parameter);
        }
    }

    public class AccentColorMenuData
    {
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        private ICommand changeAccentCommand;

        public ICommand ChangeAccentCommand
        {
            get { return this.changeAccentCommand ?? (changeAccentCommand = new SimpleCommand { CanExecuteDelegate = x => true, ExecuteDelegate = x => this.DoChangeTheme(x) }); }
        }

        protected virtual void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(DesktopApplication.MainWindow);
            if (theme == null)
            {
                Console.WriteLine("Theme == null");
            }
            var accent = ThemeManager.GetAccent(this.Name);
            if (accent == null)
            {
                Console.WriteLine("accent == null");
            }
            ThemeManager.ChangeAppStyle(DesktopApplication.MainWindow, accent, theme.Item1);
        }
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(DesktopApplication.MainWindow);
            if (theme == null)
            {
                Console.WriteLine("Theme == null");
            }
            var appTheme = ThemeManager.GetAppTheme(this.Name);
            if (appTheme == null)
            {
                Console.WriteLine("appTheme == null");
            }
            ThemeManager.ChangeAppStyle(DesktopApplication.MainWindow, theme.Item2, appTheme);
            
        }
    }

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region privateFields
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private SynchronizationContext _synchroinzationContext;
        private bool _busy;
        private ObservableCollection<IModuleLauncher> _moduleLauncers;
        private IDoseCalibrator _doseCalibrator;
        
        private Collection<IConstraint> _constraintsDefined;
        private IDataLibrarian _librarian;
        private User _currentUser;
        private NucMedPractice _currentPractice;
        private List<IDataStoreItem> _doctors;
        private List<IDataStoreItem> _towns;

       
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// <remarks>
        /// The main view model is fundamentally the program, after logging in ths gets instantiated.
        /// </remarks>
        public MainViewModel()
        {
            
            //_synchroinzationContext = SynchronizationContext.Current;
            
            _busy = false;
            
            _librarian = new StandardDataLibrarian();
            _librarian.LibraryRefreshing += _librarian_LibraryRefreshing;
           
            
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            
            
            CollectionViewSource.GetDefaultView(ModuleLaunchers).SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));
            
            this.AccentColors = ThemeManager.Accents
                                            .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                            .ToList();

            // create metro theme color menu items for the demo
            this.AppThemes = ThemeManager.AppThemes
                                           .Select(a => new AppThemeMenuData() { Name = a.Name, BorderColorBrush = a.Resources["BlackColorBrush"] as Brush, ColorBrush = a.Resources["WhiteColorBrush"] as Brush })
                                           .ToList();
            Platform.IsApplicationRunning = true;
            

        }

       
        #endregion

        #region publicMethods

        /// <summary>
        /// Sets the SynchronizationContext that will be used throughout the application
        /// </summary>
        /// <param name="_sync">The SynchronizationContext</param>
        /// <remarks>
        /// The login window sets this because the thread from the first window (GUI object)
        /// to be displayed is the main thread on which all UI activity must occur. 
        /// </remarks>
        public void SetSynchronizationContext(SynchronizationContext _sync)
        {
            logger.Trace("SetSynchronizationContext() ....");
            if (_sync == null)
            {
                logger.Trace("_sync == null");
            }
            else
            {
                logger.Trace("_sync != null");

            }

            Platform.SynchronizationContext = _sync;
        }

        /// <summary>
        /// Opens a module and calls the GetData() and UIThreadInitiliaze(). Will not open duplicat copies.
        /// </summary>
        /// <param name="moduleType">The module being loaded.</param>
        public void LaunchModule(Type moduleType)
        {
            logger.Trace("LaunchModule - moduleType = " + moduleType.Name + "...");

            object p = new object();
            BackgroundWorker worker = new BackgroundWorker();
            //AsyncObservableCollection<DataStoreItemViewModel> coll = new AsyncObservableCollection<DataStoreItemViewModel>();
            logger.Trace("LaunchModule - moduleType = " + moduleType.Name + "...DoWork()");
            ObjectHandle handle = Activator.CreateInstance(moduleType.Assembly.FullName, moduleType.FullName);
            p = handle.Unwrap();
            logger.Trace("Module instantiated");
            if (DesktopApplication.MainWindow.MyLayoutDocumentPaneGroup.Children.Where(x => x.Title == ((Module)p).Name).Any())
            {
                DesktopApplication.MainWindow.MyLayoutDocumentPaneGroup.Children.Where(x => x.Title == ((Module)p).Name).First().IsActive = true;
                logger.Info("This module has aleady been launched");
                Busy = false;
                return;
            }
            worker.DoWork += (o, ea) =>
            {


                ((Module)p).GetData();
                ((Module)p).NonUIThreadInitialize();
                logger.Trace("LaunchModule - moduleType = " + moduleType.Name + "...DoWork() ... done");

            };
            worker.RunWorkerCompleted += (o, ea) =>
            {

                logger.Trace("LaunchModule - moduleType = " + moduleType.Name + "...RunWorkerComplete() ...");
                MakeDocument((Module)p);
                Busy = false;
                ((Module)p).UIThreadInitialize();
                logger.Trace("LaunchModule - moduleType = " + moduleType.Name + "...RunWorkerComplete() ... done");
            };
            Busy = true;
            worker.RunWorkerAsync();



        }

        

        

        [Obsolete]
        public void MakeDocument(Module _module)
        {
            
            UserControl control = DesktopApplication.GetView(_module);
            

            LayoutDocument lc = new LayoutDocument();
            lc.Title = _module.Name;
            lc.CanClose = true;
            lc.CanFloat = true;
            lc.Description = _module.Name;
            lc.ToolTip = _module.Name;
            //Image img = new Image();
            //BitmapImage image = new BitmapImage(new Uri(_module.IconSource, UriKind.Relative));
            //lc.IconSource = image;
            
            control.DataContext = _module;



            lc.Content = control;

            DesktopApplication.MainWindow.MyLayoutDocumentPaneGroup.Children.Add(lc);
            lc.IsActive = true;

        }

        [Obsolete]
        public void LaunchModalModule(Type moduleType)
        {
            object p = new object();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                ObjectHandle handle = Activator.CreateInstance(moduleType.Assembly.FullName, moduleType.FullName);
                p = handle.Unwrap();
                logger.Trace("Module instantiated");

                ((Module)p).GetData();
                logger.Trace("((Module)p).GetData(); called");

            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                DesktopApplication.MakeModalDocument((Module)p);
                Busy = false;
            };
            Busy = true;
            worker.RunWorkerAsync();
        }

        [Obsolete]
        public void LaunchDataStoreItem(DataStoreItemViewModel ItemViewModel)
        {
            DesktopApplication.MakeModalDocument(ItemViewModel);
        }
        #endregion

        #region publicProperties
        /// <summary>
        /// Get or sets the IDataLibrarian used by the application
        /// </summary>
        public IDataLibrarian Librarian
        {
            get
            {
                return _librarian;
            }
        }

        /// <summary>
        /// The collection of ModuleLaunchers
        /// </summary>
        [ImportMany(typeof(IModuleLauncher))]
        public ObservableCollection<IModuleLauncher> ModuleLaunchers
        {
            get
            {
                if (_moduleLauncers == null)
                {
                    _moduleLauncers = new ObservableCollection<IModuleLauncher>();
                }
                return _moduleLauncers;
            }
            set
            {
                _moduleLauncers = value;
                RaisePropertyChanged("ModuleLauncers");
            }
        }

        [Obsolete]
        [ImportMany(typeof(IConstraint))]
        public Collection<IConstraint> ConstraintsDefined
        {
            get
            {
                if (_constraintsDefined == null)
                {
                    _constraintsDefined = new Collection<IConstraint>();
                }
                return _constraintsDefined;
            }
            set
            {
                _constraintsDefined = value;
                RaisePropertyChanged("ConstraintsDefined");
            }
        }

        [Import(typeof(IDoseCalibrator))]
        public IDoseCalibrator DoseCalibrator
        {
            get
            {
                return Platform.DoseCalibrator;
            }
            set
            {
                Platform.DoseCalibrator = value;
                RaisePropertyChanged("DoseCalibrator");
            }
        }

        /// <summary>
        /// Set Busy to true to keep the UI running during background tasks.
        /// </summary>
        public bool Busy
        {
            get { return _busy; }
            set
            {
                _busy = value;
                RaisePropertyChanged("Busy");
            }
        }

        /// <summary>
        /// Gets the synchronization context of the UI thread
        /// </summary>
        public SynchronizationContext SynchronizationContext
        {
            get
            {
                return Platform.SynchronizationContext;
            }
        }

        [Obsolete]
        public Thread MainThread
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the User currently logged in
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return iRadiate.Common.Platform.CurrentUser;
            }
            set
            {
                iRadiate.Common.Platform.CurrentUser = value;

                RaisePropertyChanged("CurrentUser");
            }
        }

        /// <summary>
        /// Gets or sets the workstation currently being used
        /// </summary>
        public Workstation CurrentWorkstation
        {
            get
            {
                return iRadiate.Common.Platform.CurrentWorkstation;
            }
            set
            {
                iRadiate.Common.Platform.CurrentWorkstation = value;

                RaisePropertyChanged("CurrentUser");
            }
        }

        /// <summary>
        /// Gets or sets the NucMedPractice at which the application is running
        /// </summary>
        public virtual NucMedPractice CurrentPractice
        {
            get
            {
                return iRadiate.Common.Platform.CurrentNucMedPractice;
            }
            set
            {
                iRadiate.Common.Platform.CurrentNucMedPractice = value;
            }
        }
        

        public List<AccentColorMenuData> AccentColors { get; set; }
        public List<AppThemeMenuData> AppThemes { get; set; }

       

        #endregion

        #region privateMethods
        private void _librarian_RefreshCompleted(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.ShowLibraryRefreshNotification)
                DesktopApplication.ShowToastInformation("... completed.", DesktopApplication.NotificationPosition.BottomLeft);
        }

        private void _librarian_LibraryRefreshing(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.ShowLibraryRefreshNotification)
                DesktopApplication.ShowToastInformation("Library reloading from database....", DesktopApplication.NotificationPosition.BottomLeft);
        }
        #endregion

        #region obsolete
        [Obsolete]
        public virtual List<IDataStoreItem> Doctors
        {
            get
            {
                if (_doctors == null)
                {
                    _doctors = Librarian.GetItems(typeof(Doctor), new List<RetrievalCriteria>()).ToList();
                }
                return _doctors;
            }
        }

        [Obsolete]
        public List<IDataStoreItem> ReferringDoctors
        {
            get
            {
                return Doctors.Where(x => ((Doctor)x).Referrer == true).ToList();
            }
        }

        [Obsolete]
        public List<IDataStoreItem> Towns
        {
            get
            {
                if (_towns == null)
                {
                    _towns = Librarian.GetItems(typeof(Town), new List<RetrievalCriteria>()).ToList();
                }
                return _towns;
            }
        }

        [Obsolete]
        public System.Windows.Application WindowsApplication { get; set; }
        #endregion

    }
}