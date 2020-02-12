using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;
using NLog;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

using iRadiate.Common;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using System.ComponentModel;

namespace iRadiate.Whiteboard.Common.ViewModel
{
    [PreferredView("iRadiate.Whiteboard.Common.View.WhiteboardView","iRadiate.Whiteboard.Common")]
    public class WhiteboardViewModel : Module
    {
        protected static Notifier notifier;
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        #region privateFields
        private DateTime _selectedDate;
        private AsyncObservableCollection<IDataStoreItem> _appointments;
      
        private IDataStoreItem _selectedAppointment;
        private bool _excludeCompleted;
        private bool _excludeCancelled;
        private CollectionViewSource _appointmentsView;
        private int _actionBarWidth = 0;
        private bool _readyForRefresh = false;
        private List<string> _whiteboards;
        private string _selectedWhiteboard;
        private List<string> _groupingOptions;
        private string _selectedGroupingOption;
        private bool _autoRefresh;
        
     
        [ImportMany(typeof(IWhiteboardTool))]
        private List<IWhiteboardTool> _tools;
        private bool _highlightArrived;
        #endregion

        #region constructors
        public WhiteboardViewModel():base()
        {
            logger.Trace("WhiteboardViewModel()...");
            //_appointmentsView = (CollectionView)CollectionViewSource.GetDefaultView(Appointments);
            SelectedDate = DateTime.Today;
            AutoRefresh = true;
            
            logger.Trace("WhiteboardViewModel()...Done");
            _whiteboards = new List<string>();
            _whiteboards.Add("All");
            _whiteboards.Add("Nukes-Roles");
            _whiteboards.Add("Nukes-Cameras");
            _whiteboards.Add("NukesDoctor");
            _whiteboards.Add("PET");
            

            
            _excludeCompleted = Properties.Settings.Default.ExcludeCompleted;
            _excludeCancelled = Properties.Settings.Default.ExcludeCancelled;
            _selectedWhiteboard = Properties.Settings.Default.ChosenWhiteboard;
            _highlightArrived = Properties.Settings.Default.HighlightArrived;
            RefreshWhiteboardCommand = new RelayCommand(RefreshWhiteboard);
            DownloadDataCommand = new RelayCommand(ReloadData);
            DesktopApplication.ApplicationIdle += DesktopApplication_ApplicationIdle;
        }

        private void DesktopApplication_ApplicationIdle(object sender, EventArgs e)
        {
            
            RefreshWhiteboard();
        }

        public bool FilterAppointment(Object item)
        {
            Appointment avm = (Appointment)item;
            if (avm.ScheduledArrivalTime > SelectedDate.Date.AddDays(1))
            {
                return false;
            }

            if (avm.ScheduledArrivalTime < SelectedDate.Date)
            {
                return false;
            }
            if (ExcludeCompleted)
            {
                if (avm.Completed)
                {
                    return false;
                }
                
            }
            if (ExcludeCancelled)
            {
                if (avm.Cancelled)
                {
                    return false;
                }
            }
            
            return true;
        }
        #endregion

        #region PublicProperties
        public RelayCommand RefreshWhiteboardCommand { get; private set; }

        public RelayCommand DownloadDataCommand { get; private set; }

        public AsyncObservableCollection<IDataStoreItem> Appointments
        {
            get
            {
                if (_appointments == null)
                {
                    _appointments = Platform.CreateCollection();
                    
                }
                return _appointments;
            }
            set
            {
                _appointments = value;
                RaisePropertyChanged("Appointments");
            }
        }

       

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                
                _selectedDate = value;
                
                RaisePropertyChanged("SelectedDate");
                if (_readyForRefresh)
                {
                    
                    
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += (o, ea) =>
                    {
                        DesktopApplication.MainViewModel.Busy = true;
                        GetData();
                        //System.Threading.Thread.Sleep(5000);
                    };
                    worker.RunWorkerCompleted += (o, ea) =>
                    {
                        if(ea.Error != null){
                            
                        }
                        UIThreadInitialize();
                        DesktopApplication.MainViewModel.Busy = false;
                        AppointmentsView.View.Refresh();
                        
                    };
                    worker.RunWorkerAsync();
                    
                    
                }
                else
                {
                    
                }
                
            }
        }

        public IDataStoreItem SelectedAppointment
        {
            get
            {
                return _selectedAppointment;
            }
            set
            {
                _selectedAppointment = value;
                RaisePropertyChanged("SelectedAppointment");
                OnSelectionChanged(EventArgs.Empty);
            }
        }

        public bool ExcludeCompleted
        {
            get { return _excludeCompleted; }
            set { _excludeCompleted = value; RaisePropertyChanged("ExcludeCompleted"); AppointmentsView.View.Refresh();
            Properties.Settings.Default.ExcludeCompleted = value;
            Properties.Settings.Default.Save();
            }
        }

        public bool ExcludeCancelled
        {
            get { return _excludeCancelled; }
            set { _excludeCancelled = value; RaisePropertyChanged("ExcludeCancelled"); AppointmentsView.View.Refresh();
            Properties.Settings.Default.ExcludeCancelled = value;
            Properties.Settings.Default.Save();
            }
        }

        public CollectionViewSource AppointmentsView
        {
            get
            {
                return _appointmentsView;
            }
            set
            {
                _appointmentsView = value;
                RaisePropertyChanged("AppointmentsView");
            }
        }

        public string SelectedGroupingOption
        {
            get
            {
                return _selectedGroupingOption;
            }
            set
            {
                _selectedGroupingOption = value;
                RaisePropertyChanged("SelectedGroupingOption");
                if (SelectedGroupingOption != "None")
                {
                    AppointmentsView.View.GroupDescriptions.Clear();
                    AppointmentsView.View.GroupDescriptions.Add(new PropertyGroupDescription(SelectedGroupingOption+"String"));
                    AppointmentsView.View.Refresh();
                }
                
            }
        }

        public List<string> GroupingOptions
        {
            get
            {
                return _groupingOptions;
            }
            set
            {
                _groupingOptions = value;
                RaisePropertyChanged("GroupingOptions");
            }
        }

        public List<IWhiteboardTool> Tools
        {
            get
            {
                if (_tools == null)
                    _tools = new List<IWhiteboardTool>();
                return _tools.OrderBy(x=>x.WhiteboardPositionIndex).ToList();
            }
            set
            {
                _tools = value;
            }
        }

        public delegate void SelectionChangedHandler(object sender, EventArgs e);

        public event SelectionChangedHandler SelectionChanged;

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        public bool AutoRefresh 
        {
            get
            {
                return _autoRefresh;
                ///Comment
            }
            set
            {
                _autoRefresh = value;
                RaisePropertyChanged("AutoRefresh");
                DesktopApplication.Librarian.AutoRefresh = value;
            }
        }

        public bool HighlightArrived
        {
            get
            {
                return _highlightArrived;
            }
            set
            {
                _highlightArrived = value; RaisePropertyChanged("HighlightArrived"); AppointmentsView.View.Refresh();
                Properties.Settings.Default.HighlightArrived = value;
                Properties.Settings.Default.Save();
            }
        }
        #endregion

        #region overrides
        public override void GetData()
        {

            logger.Trace("GetData() ...");
           

            Appointments = DesktopApplication.Librarian.GetAppointments(SelectedDate);
            foreach (IDataStoreItem d in Appointments)
            {
                Appointment a = d as Appointment;
                a.AppointmentCompleted += AppointmentCompleted;
                a.ItemSaving += AppointmentSaving;
                a.ItemSaved += AppointmentSaved;
                a.PatientArrived += AppointmentArrived;
                a.PropertyChanged += AppointmentPropertyChanged;
                
            }
            foreach(IWhiteboardTool t in Tools)
            {
                
            }
            _readyForRefresh = true;
            
            logger.Trace("GetData() ...Done");
        }

        public override ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();

                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.ChalkboardTeacherSolid;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }

        private void AppointmentPropertyChanged(object sender, DataModel.Common.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AppointmentArrived(object sender, EventArgs e)
        {
           
            ShowToastInformation((sender as Appointment).Patient.FullName + " - " + (sender as Appointment).Name + " has arrived");
            
        }

        private static void CloseAction(NotificationBase obj)
        {
            var opts = obj.DisplayPart.GetOptions();
            //_vm.ShowInformation($"Notification close clicked, Tag: {opts.Tag}");
        }

        private void AppointmentSaved(object sender, EventArgs e)
        {
            //Refresh maybe?
        }

        private void AppointmentSaving(object sender, EventArgs e)
        {
            //Nothing to do here.
        }

        private void AppointmentCompleted(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void a_SomethingChanged(object sender, EventArgs e)
        {
            if (AutoRefresh)
            {
               
                try
                {
                    //Application.SynchronizationContext.Send(delegate { AppointmentsView.View.Refresh(); },null);
                    ((DataStoreItemViewModel)sender).RaiseAllPropertiesChanged();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("WhiteboardRefresh error in a_SomethingChanged");
                }
                

            }
            
        }

        
        public override string Name
        {
            get
            {
                return "Whiteboard";
            }
            set
            {

            }
        }

        public override string IconSource
        {
            get { return "/iRadiate.Whiteboard.Common;component/Images/WhiteboardIcon.png"; }
        }

        public int ActionBarWidth
        {
            get
            {
                return _actionBarWidth;
            }
            set
            {
                _actionBarWidth = value;
                RaisePropertyChanged("ActionBarWidth");
            }
        }

        public override void UIThreadInitialize()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            AppointmentsView = new CollectionViewSource();
            AppointmentsView.Source = Appointments;
            AppointmentsView.View.Filter = FilterAppointment;
            RaisePropertyChanged("AppointmentsView");
            if (SelectedWhiteboard == "Nukes-Roles")
            {
                AppointmentsView.View.GroupDescriptions.Clear();
                AppointmentsView.View.GroupDescriptions.Add(new PropertyGroupDescription("Item.CurrentAssignee.Name"));

            }
            else if (SelectedWhiteboard == "Nukes-Cameras")
            {
                AppointmentsView.View.GroupDescriptions.Clear();
                AppointmentsView.View.GroupDescriptions.Add(new PropertyGroupDescription("Camera"));

            }
            else if (_selectedWhiteboard == "NukesDoctor")
            {
                AppointmentsView.View.GroupDescriptions.Clear();

            }
            else
            {
                AppointmentsView.View.GroupDescriptions.Clear();
                AppointmentsView.View.SortDescriptions.Clear();
            }

           

            System.Diagnostics.Debug.WriteLine("Now to iterate the IWhiteboardTools...");
            foreach (IWhiteboardTool t in Tools)
            {
                System.Diagnostics.Debug.WriteLine("IWhteboard tool .... " + t.Name);
                t.SetWhiteboard(this);
                t.NonUIThreadInitialise();
                t.UIThreadInitialise();
            }
        }

        public List<string> Whiteboards
        {
            get
            {
                return _whiteboards;
            }
        }

        public string SelectedWhiteboard
        {
            get
            {
                return _selectedWhiteboard;
            }
            set
            {
                _selectedWhiteboard = value;
                RaisePropertyChanged("SelectedWhiteboard");
                Properties.Settings.Default.ChosenWhiteboard = value;
                Properties.Settings.Default.Save();
                if (SelectedWhiteboard == "Nukes-Roles")
                {
                    AppointmentsView.View.GroupDescriptions.Clear();
                    AppointmentsView.View.GroupDescriptions.Add(new PropertyGroupDescription("Item.CurrentAssignee.Name"));
                    
                }
                else if (SelectedWhiteboard == "Nukes-Cameras")
                {
                    AppointmentsView.View.GroupDescriptions.Clear();
                    AppointmentsView.View.GroupDescriptions.Add(new PropertyGroupDescription("Camera"));

                }
                else if (SelectedWhiteboard == "NukesDoctor")
                {
                    AppointmentsView.View.GroupDescriptions.Clear();

                }
                else
                {
                    AppointmentsView.View.GroupDescriptions.Clear();
                    AppointmentsView.View.SortDescriptions.Clear();
                }
                AppointmentsView.View.Refresh();
            }
        }
        #endregion

        private static void ShowToastInformation(string message)
        {
            if (notifier == null)
            {
                notifier = new Notifier(cfg =>
                {
                    cfg.PositionProvider = new WindowPositionProvider(
                        parentWindow: DesktopApplication.MainWindow,

                        corner: Corner.TopRight,
                        offsetX: 75,
                        offsetY: 100);                    
                        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                         notificationLifetime: TimeSpan.FromHours(1),
                         maximumNotificationCount: MaximumNotificationCount.FromCount(6));
                    

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


            notifier.ShowSuccess(message, options);

        }

        //private static void CloseAction(NotificationBase obj)
        //{
        //    var opts = obj.DisplayPart.GetOptions();

        //}
        private void RefreshWhiteboard()
        {
            AppointmentsView.View.Refresh();
        }

        private void ReloadData()
        {
            BackgroundWorker worker = new BackgroundWorker();
           
            logger.Trace("Manually reload data from database");
            
            worker.DoWork += (o, ea) =>
            {


                DesktopApplication.Librarian.UpdateAppointments();

            };
            worker.RunWorkerCompleted += (o, ea) =>
            {

                AppointmentsView.View.Refresh();
                DesktopApplication.MainViewModel.Busy = false;
            };
            DesktopApplication.MainViewModel.Busy = true;
            worker.RunWorkerAsync();
        }

        public void SaveLayout()
        {
            Properties.Settings.Default.Save();
            DesktopApplication.ShowToastInformation("Whiteboard layout saved",DesktopApplication.NotificationPosition.BottomLeft);
        }

        public override void NonUIThreadInitialize()
        {
            
        }
    }

    [Export(typeof(IModuleLauncher))]
    public class WhiteboardModuleLauncher : ModuleLauncher
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override ContentControl ButtonContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                
                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.ChalkboardTeacherSolid;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }

        public override string Name
        {
            get
            {
                
                return "Whiteboard";
            }
        }

        public string IconSource
        {
            get { return "/iRadiate.Whiteboard.Common;component/Images/WhiteboardIconWhite.png"; }
        }

        public override void Launch()
        {
            //logger.Trace("SettingsLauncher.Launch()");
            DesktopApplication.MainViewModel.LaunchModule(typeof(WhiteboardViewModel));

        }

        public override int Order
        {
            get
            {
                return Properties.Settings.Default.WhiteboardLauncherOrder;
            }
            set
            {
                Properties.Settings.Default.WhiteboardLauncherOrder = value;
                RaisePropertyChanged("Order");
            }
        }

        public override bool Visible
        {
            get
            {
                return Properties.Settings.Default.WhiteboardLauncherVisible;
            }

            set
            {
                Properties.Settings.Default.WhiteboardLauncherVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
