using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Runtime.Remoting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using GalaSoft.MvvmLight;
using NLog;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Xceed.Wpf.AvalonDock.Layout;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Core;
using ToastNotifications.Position;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common.View;
using iRadiate.Desktop.Common.ViewModel;




namespace iRadiate.Desktop.Common
{
    /// <summary>
    /// Class containing static references the view model and the view.
    /// </summary>
    /// <remarks>
    /// Fundamentally this class provides the bits and pieces necessary to link the modules
    /// and the view and the viewmodel. The MainViewModel is the underlying guts of the program
    /// but this class glues a lot of it together by means of static properties
    /// </remarks>
    public class DesktopApplication
    {
        #region privateFields
        private static IDataLibrarian _dataLibrarian;
        private static MainViewModel _mainViewModel;
        private static MainWindow _mainWindow;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static NucMedPractice _currentPractice;
        private static List<TaskTypeWrapper> _taskTypes;
        private static ChildWindow _activeWindow;
        protected static Notifier notifier;
        #endregion

        #region enums
        public enum DocumentMode { View, Edit, New};

        /// <summary>
        /// A position relative to the Mainwindow where toast nofications will appear
        /// </summary>
        /// <remarks>
        /// This enum is used so that other projects do not need to add a reference ot the toastNotification package
        /// </remarks>
        public enum NotificationPosition { BottomLeft, BottomCentre, BottomRight, TopLeft,TopCenter};
        #endregion

        #region obsoletes
        [Obsolete]
        public static AsyncObservableCollection<DataStoreItemViewModel> CreateCollection()
        {
            AsyncObservableCollection<DataStoreItemViewModel> res = new AsyncObservableCollection<DataStoreItemViewModel>();

            res = (AsyncObservableCollection<DataStoreItemViewModel>)Dispatcher.Invoke(new Func<AsyncObservableCollection<DataStoreItemViewModel>>(() =>
            {
                AsyncObservableCollection<DataStoreItemViewModel> result;
                logger.Trace("Invoke...");
                result = new AsyncObservableCollection<DataStoreItemViewModel>();
                logger.Trace("Created Collection - type = " + result.GetType());
                logger.Trace("Invoke...End");
                return result;

            }));

            return res;
        }

        [Obsolete]
        public static UserControl GetViewVMB(ViewModelBase module)
        {
            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(module.GetType());
            foreach (System.Attribute attr in attrs)
            {
                if (attr is PreferredViewAttribute)
                {
                    PreferredViewAttribute a = (PreferredViewAttribute)attr;
                    ObjectHandle handle = Activator.CreateInstance(a.AssemblyName, a.ViewName);
                    object p = handle.Unwrap();
                    return (UserControl)p;
                }
            }
            return null;

        }

        /// <summary>
        /// Returns the one and only DataLibrarian
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use Application.Librarian instead")]
        public static IDataLibrarian GetLibrarian()
        {
            
            return MainViewModel.Librarian;
        }
        #endregion

        #region launchers
        /// <summary>
        /// Opens a module as a new document in the main window.
        /// </summary>
        /// <param name="_module">The module to be opened.</param>
        public static void MakeDocument(IModule _module)
        {
            logger.Trace("MakeDocument(" + _module.Name + ") called");
            UserControl control = GetView(_module);
            logger.Trace("UserControl received of type " + control.GetType().ToString());
            control.Margin = new Thickness(0);
            LayoutDocument lc = new LayoutDocument();
            
            lc.Title = _module.Name;
            lc.CanClose = true;
            lc.CanFloat = true;
            lc.Description = _module.Name;
            lc.ToolTip = _module.Name;
            Image img = new Image();
            BitmapImage image = new BitmapImage(new Uri(_module.IconSource, UriKind.Relative));
            lc.IconSource = image;
            
            control.DataContext = _module;

           

            lc.Content = control;
            
            MainWindow.MyLayoutDocumentPaneGroup.Children.Add(lc);
            lc.IsActive = true;
            
        }

        
        /// <summary>
        /// Opens a DataStoreItemViewModel in a new document
        /// </summary>
        /// <param name="_itemViewModel">The DataStoreItemViewModel to be opened.</param>
        public static void MakeDocument(DataStoreItemViewModel _itemViewModel)
        {
            UserControl control = GetView(_itemViewModel);
            control.DataContext = _itemViewModel;

            LayoutDocument lc = new LayoutDocument();
            lc.Title = _itemViewModel.DocumentTitle;
            lc.CanClose = true;
            lc.CanFloat = true;
            lc.Description = _itemViewModel.DocumentTitle;
            lc.ToolTip = _itemViewModel.DocumentTitle;
            Image img = new Image();
            BitmapImage image = new BitmapImage(new Uri(_itemViewModel.DocumentIcon, UriKind.Relative));
            lc.IconSource = image;
            lc.Content = control;
            MainWindow.MyLayoutDocumentPaneGroup.Children.Add(lc);
            lc.IsActive = true;
            
        }

        [Obsolete]
        public async static void MakeModalDocument(IModule _module)
        {

            ChildWindow w = new ChildWindow();
            w.OverlayBrush = null;
            w.CloseByEscape = true;
            w.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            w.EnableDropShadow = true;
            w.OverlayBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.5 };
            w.AllowMove = true;
            w.ShowTitleBar = true;
            w.ShowCloseButton = true;

            UserControl control = GetView(_module);
            control.Margin = new Thickness(2.0);
            control.DataContext = _module;
            w.Content = control;
            ActiveWindow = w;
            _module.Closing += _itemViewModel_ViewModelClosing;
            w.Title = _module.Name;
            MainWindow.ShowDialogsOverTitleBar = true;

            await MainWindow.ShowChildWindowAsync(w, ChildWindowManager.OverlayFillBehavior.FullWindow);
        }
       
       /// <summary>
       /// Opens a DataStoreItemViewModel in a modal pop-up document
       /// </summary>
       /// <param name="_itemViewModel">The DataStoreItemViewModel to be opened.</param>
        public async static void MakeModalDocument(DataStoreItemViewModel _itemViewModel, DocumentMode mode)
        {
            //ActiveWindow.Closing -= w_Closing;
              
            ChildWindow w = new ChildWindow();
            w.OverlayBrush = null;
            w.CloseByEscape = true;
            w.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            w.EnableDropShadow = true;
            w.OverlayBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.5 };
            w.AllowMove = true;
            w.ShowTitleBar = true;
            w.ShowCloseButton = true;

            UserControl control = GetView(_itemViewModel);
            control.Margin = new Thickness(2.0);
           
            control.DataContext = _itemViewModel;
            
            w.Content = control;
            ActiveWindow = w;
            _itemViewModel.Closing += _itemViewModel_ViewModelClosing;

            
            string t = Regex.Replace(_itemViewModel.Item.ConcreteType.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
            w.Title = t;
            MainWindow.ShowDialogsOverTitleBar = true;
            
            await MainWindow.ShowChildWindowAsync(w,ChildWindowManager.OverlayFillBehavior.FullWindow);

            
            
        }

        public async static void MakeModalDocument(DataStoreItemViewModel _itemViewModel)
        {
            MakeModalDocument(_itemViewModel, DocumentMode.View);
        }

        
        public async static void MakeModalDocument(DataStoreItemViewModel _itemViewModel, string AssemblyName, string ViewName)
        {
            ChildWindow w = new ChildWindow();
            w.OverlayBrush = null;
            w.CloseByEscape = true;
            w.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            w.EnableDropShadow = true;
            w.OverlayBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.5 };
            w.AllowMove = true;
            w.ShowTitleBar = true;
            w.ShowCloseButton = true;
            UserControl control = GetView(AssemblyName, ViewName);
            control.Margin = new Thickness(2.0);

            control.DataContext = _itemViewModel;

            w.Content = control;
            ActiveWindow = w;
            _itemViewModel.Closing += _itemViewModel_ViewModelClosing;

           
            string t = Regex.Replace(_itemViewModel.Item.ConcreteType.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
            w.Title = t;
            MainWindow.ShowDialogsOverTitleBar = true;

            await MainWindow.ShowChildWindowAsync(w, ChildWindowManager.OverlayFillBehavior.FullWindow);
           
            
            
        }

        
        public async static void MakeModalDocument(GenericViewModel _itemViewModel, string AssemblyName, string ViewName)
        {
            ChildWindow w = new ChildWindow();

            w.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            w.ShowTitleBar = true;
            w.ShowCloseButton = true;
            UserControl control = GetView(AssemblyName, ViewName);
            control.Margin = new Thickness(2.0);

            control.DataContext = _itemViewModel;
            w.Content = control;
            ActiveWindow = w;
            
            //w.Closing += w_Closing;
            //w.WindowStyle = WindowStyle.ToolWindow;

            //w.ShowInTaskbar = false;


            //w.Height = control.Height + 40;
            //w.Width = control.Width + 20;
            //ActiveWindow = w;
            await MainWindow.ShowChildWindowAsync(w);
            //ActiveWindow.Closing -= w_Closing;

        }

        
        public async static void MakeModalDocument(GenericViewModel _itemViewModel)
        {
            ChildWindow w = new ChildWindow();
            w.OverlayBrush = null;
            w.CloseByEscape = true;
            w.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            w.EnableDropShadow = true;
            w.OverlayBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.5 };
            w.AllowMove = true;
            w.ShowTitleBar = true;
            w.ShowCloseButton = true;
            UserControl control = GetViewVMB(_itemViewModel);
            control.Margin = new Thickness(2.0);

            control.DataContext = _itemViewModel;

            w.Content = control;
            ActiveWindow = w;
            MainWindow.ShowDialogsOverTitleBar = true;
            _itemViewModel.ViewModelClosing += _itemViewModel_ViewModelClosing;
            await MainWindow.ShowChildWindowAsync(w, ChildWindowManager.OverlayFillBehavior.FullWindow);

        }

        #endregion

        #region privateMethods

        private static void CloseAction(NotificationBase obj)
        {
            var opts = obj.DisplayPart.GetOptions();

        }
        private static void _itemViewModel_ViewModelClosing(object sender, EventArgs e)
        {
            if (ActiveWindow != null)
            {
                ActiveWindow.Close();

            }

        }

        private static void w_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            //ActiveWindow.Closing += w_Closing;
        }

        private static void OnApplicationIdle()
        {
            ApplicationIdle?.Invoke(MainViewModel, new EventArgs());
            
        }
        #endregion

        #region publicStaticMethods      
        /// <summary>
        /// Creates an ICollectionView of an IEnumerable using a dispatcher
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ICollectionView CreateCollectionView(IEnumerable<IDataStoreItem> source)
        {
            logger.Trace("CreateCollectionView...");
            if (source == null)
            {
                logger.Trace("source == null");
            }
            else
            {
                logger.Trace("source != null");
            }
            ICollectionView res;

            res = (ICollectionView)Dispatcher.Invoke(new Func<ICollectionView>(() =>
            {
                ICollectionView result;
                logger.Trace("Invoke...");
                result = CollectionViewSource.GetDefaultView(source);
                logger.Trace("Created CollectionView - type = " + result.GetType());
                if (result == null)
                {
                    logger.Trace("CollectionView == null");
                }
                else
                {
                    logger.Trace("CollectionView != null");
                }
                //result = new CollectionView(source);
                logger.Trace("Invoke...End");
                return result;

            }));

            return res;
        }

        /// <summary>
        /// Retrieves the view for the given module
        /// </summary>
        /// <param name="module">The module being opened</param>
        /// <returns>Returns a user control which is preffered for that module.</returns>
        public static UserControl GetView(IModule module)
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(module.GetType());
            foreach (System.Attribute attr in attrs)
            {
                if (attr is PreferredViewAttribute)
                {
                    PreferredViewAttribute a = (PreferredViewAttribute)attr;
                    ObjectHandle handle = Activator.CreateInstance(a.AssemblyName, a.ViewName);
                    object p = handle.Unwrap();
                    return (UserControl)p;
                }
            }
            return null;
            
        }

        /// <summary>
        /// Gets a view for a given DataStoreItemViewmodel
        /// </summary>
        /// <param name="viewModel">The DataStoreItemViewModel that will be tied to the view</param>
        /// <returns>If there DataStoreItemViewModel has a preferred view attribute it will instantiate and 
        /// return that. If not the method will try to instantiate and return a view based on the
        /// underlying DataStoreItem</returns>
        private static UserControl GetView(DataStoreItemViewModel viewModel)
        {
            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(viewModel.GetType());
            foreach (System.Attribute attr in attrs)
            {
                if (attr is PreferredViewAttribute)
                {
                    PreferredViewAttribute a = (PreferredViewAttribute)attr;
                    ObjectHandle handle = Activator.CreateInstance(a.AssemblyName, a.ViewName);
                    object p = handle.Unwrap();
                    return (UserControl)p;
                }
            }
            ObjectHandle handler = Activator.CreateInstance("iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View." + viewModel.Item.ConcreteType.Name + "View");
            object pr = handler.Unwrap();
            return (UserControl)pr;

        }

        /// <summary>
        /// Closes the active window
        /// </summary>
        public static void CloseActiveWindow()
        {
            ActiveWindow.Close();
        }

        /// <summary>
        /// Gets an explcit user control 
        /// </summary>
        /// <param name="AssemblyName">The assembly in whcih the view is stored</param>
        /// <param name="ViewName">the name of the view</param>
        /// <returns></returns>
        private static UserControl GetView(string AssemblyName, string ViewName)
        {
            ObjectHandle handle = Activator.CreateInstance(AssemblyName, ViewName);
            object p = handle.Unwrap();
            return (UserControl)p;
        }

        public static void FireIdleEvent()
        {
            
            OnApplicationIdle();
        }
        #endregion

        #region dialogsAndNotifications

        public static void ShowToastInformation(string message, NotificationPosition position)
        {

            Corner c = Corner.BottomLeft;
            switch (position)
            {
                case NotificationPosition.BottomCentre:
                    c = Corner.BottomCenter;
                    break;
                case NotificationPosition.BottomLeft:
                    c = Corner.BottomLeft;
                    break;
                case NotificationPosition.BottomRight:
                    c = Corner.BottomRight;
                    break;
                case NotificationPosition.TopLeft:
                    c = Corner.TopLeft;
                    break;
                case NotificationPosition.TopCenter:
                    c = Corner.TopRight;
                    break;

            }

            ShowToastInformation(message, true, c);


        }

        private static void ShowToastInformation(string message, bool autoClear, Corner position)
        {
            if (notifier == null)
            {
                notifier = new Notifier(cfg =>
                {
                    cfg.PositionProvider = new WindowPositionProvider(
                        parentWindow: DesktopApplication.MainWindow,

                        corner: position,
                        offsetX: 75,
                        offsetY: 100);
                    if (autoClear)
                    {
                        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                         notificationLifetime: TimeSpan.FromSeconds(3),
                         maximumNotificationCount: MaximumNotificationCount.FromCount(6));
                    }
                    else
                    {
                        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                         notificationLifetime: TimeSpan.FromHours(1),
                         maximumNotificationCount: MaximumNotificationCount.FromCount(6));
                    }

                    cfg.DisplayOptions.TopMost = true;
                    cfg.DisplayOptions.Width = 400;
                    cfg.Dispatcher = DesktopApplication.Dispatcher;
                });


            }


            var options = new MessageOptions
            {
                CloseClickAction = CloseAction,
                FontSize = 14, // set notification font size               
                FreezeOnMouseEnter = true,
                UnfreezeOnMouseLeave = true,
                ShowCloseButton = true



            };


            notifier.ShowInformation(message, options);

        }
        public async static void ShowDialog(string title,string text)
        {
            MetroDialogSettings s = new MetroDialogSettings();
            
            await MainWindow.ShowMessageAsync(title, text);
        }

        public async static Task<bool> ShowConfirmDialog(string title, string text)
        {
            MetroDialogSettings s = new MetroDialogSettings();
          
            MessageDialogResult res = await MainWindow.ShowMessageAsync(title, text,MessageDialogStyle.AffirmativeAndNegative);
            if (res == MessageDialogResult.Affirmative)
                return true;
            else
                return false;
        }
        #endregion

        #region staticProperties
        public static ChildWindow ActiveWindow
        {
            get
            {
                return _activeWindow;

            }
            set
            {
                _activeWindow = value;
            }
        }

        public static Dispatcher Dispatcher
        {
            get
            {
                return MainWindow.Dispatcher;
            }
        }
        public static string LogFileName
        {
            get { return Properties.Settings.Default.Logfile; }
        }

        /// <summary>
        /// Gets the setting whether the log file should show trace level information
        /// </summary>
        public static bool TraceDebug
        {
            get { return Properties.Settings.Default.TraceDebug; }
        }
        /// <summary>
        /// Gets the accent colour of the application
        /// </summary>
        public static string ThemeAccent
        {
            get
            {
                return Properties.Settings.Default.ColorTheme;
            }
        }

        /// <summary>
        /// Returns the MainViewModel
        /// </summary>
        public static MainViewModel MainViewModel
        {
            get { return _mainViewModel; }
            set { _mainViewModel = value; }
        }

        /// <summary>
        /// Gets or sets the main window in the application
        /// </summary>
        public static MainWindow MainWindow
        {
            get { return _mainWindow; }
            set { _mainWindow = value; }
        }

        /// <summary>
        /// Gets or sets the current user of the application
        /// </summary>
        public static User CurrentUser
        {
            get
            {
                return iRadiate.Common.Platform.CurrentUser;
            }
            set
            {
                iRadiate.Common.Platform.CurrentUser = value;
            }
        }

        /// <summary>
        /// Gets or sets the current practice
        /// </summary>
        public static NucMedPractice CurrentPratice
        {
            get
            {
                if (_currentPractice == null)
                {
                    _currentPractice = (NucMedPractice)GetLibrarian().GetItems(typeof(NucMedPractice), new List<RetrievalCriteria>()).First();
                }
                return _currentPractice;
            }
            set
            {
                _currentPractice = value;
            }
        }

        /// <summary>
        /// Retrieves the list of users in the system
        /// </summary>
        public static List<IDataStoreItem> Users
        {
            get
            {
                return Librarian.GetItems(typeof(User), new List<RetrievalCriteria>()).ToList();
            }
        }

        /// <summary>
        /// Returns the SynchroniationContext maintainted by the MainViewModel
        /// </summary>
        public static SynchronizationContext SynchronizationContext
        {
            get
            {
                return MainViewModel.SynchronizationContext;
            }
        }

        /// <summary>
        /// Gets the IDataLibrarian
        /// </summary>
        public static IDataLibrarian Librarian
        {
            get
            {
                return MainViewModel.Librarian;
            }
        }

        public static double IconWidth
        {
            get { return 24; }
        }

        public static double IconHeight
        {
            get { return 24; }
        }
        #endregion

        #region events
        public static event EventHandler ApplicationIdle;
        #endregion
    }
}
