using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;
using NLog;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Desktop.Common.Diary;

using System.ComponentModel;

namespace iRadiate.Diary.Common.ViewModel
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
    }

    [PreferredView("iRadiate.Diary.Common.View.DiaryModuleView", "iRadiate.Diary.Common")]
    public class DiaryViewModel : Module
    {
        #region privateFields
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private AsyncObservableCollection<Diary> _diaries;
        private DateTime _selectedDate;
        private AsyncObservableCollection<DiaryEventWrapper> _scanTasks;
        private DiaryEventWrapper _selectedEvent;

        [ImportMany(typeof(IDiaryTool))]
        private List<IDiaryTool> _tools;
        #endregion

        #region constructor
        public DiaryViewModel() : base()
        {
            logger.Trace("DiaryViewmodel()...");
            _selectedDate = DateTime.Now.Date;
            _diaries = new AsyncObservableCollection<Diary>();
            var rooms = DesktopApplication.Librarian.GetItems(typeof(Room), new List<RetrievalCriteria>());
            foreach(IDataStoreItem i in rooms)
            {
                if((i as Room).CameraRoom)
                {
                    Diary d = new Diary(this, (i as Room));
                    Diaries.Add(d);
                }
            }
            logger.Trace(Diaries.Count + " Diaries created ");
           
        }
        #endregion

        #region overrides
        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            LoadCommand = new RelayCommand(Refresh);
        }
        public override void GetData()
        {
            logger.Trace("GetData()....");

           

            base.GetData();
            RetrievalCriteria rc1 = new RetrievalCriteria("ValidCompletionTime", CriteraType.GreaterThan, SelectedDate.Date);
            RetrievalCriteria rc2 = new RetrievalCriteria("ValidCompletionTime", CriteraType.LessThan, SelectedDate.Date.AddDays(1));
            RetrievalCriteria rc3 = new RetrievalCriteria("Cancelled", CriteraType.Equals, false);
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc1);
            rcList.Add(rc2);
            rcList.Add(rc3);
            var col = DesktopApplication.Librarian.GetItems(typeof(ScanTask), rcList);
            if (ScanTasks.Any())
            {
                logger.Trace("ScanTasks already found during GetData(), Clear()");
                ScanTasks.Clear();
            }
                
            foreach(IDataStoreItem i in col)
            {
                logger.Trace("Created new DiaryEventWrapper..");
                DiaryEventWrapper dew = new DiaryEventWrapper(i as ScanTask,this);
                dew.RoomChanged += Dew_RoomChanged;
                dew.Selected += Dew_Selected;
                ScanTasks.Add(dew);
            }
            RaisePropertyChanged("Diaries");
            
           
            logger.Trace("GetData()....Complete");
        }

        private void Dew_Selected(object sender, EventArgs e)
        {
            if(SelectedEvent!=null)
                SelectedEvent.IsSelected = false;
            SelectedEvent = sender as DiaryEventWrapper;
        }

        private void Dew_RoomChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Dairies");
            foreach (Diary d in Diaries)
            {
                d.Refresh();
            }
        }

        public override ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();

                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.CalendarAltRegular;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }
        public override string IconSource
        {
            get { return "/iRadiate.Whiteboard.Common;component/Images/WhiteboardIcon.png"; }
        }

        public override string Name
        {
            get
            {
                return "Diary";
            }

            set
            {
                
            }
        }

        public override void UIThreadInitialize()
        {
            base.UIThreadInitialize();
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            foreach(var t in Tools)
            {
                t.DiaryViewModel = this;
                t.NonUIThreadInitialise();
                t.UIThreadInitialise();
                
            }
            CollectionViewSource.GetDefaultView(Diaries).Refresh();
            OnDataLoaded();
        }

        #endregion

        #region publicProperties
        public DiaryEventWrapper SelectedEvent
        {
            get { return _selectedEvent; }
            set { _selectedEvent = value; RaisePropertyChanged("SelectedEvent"); OnSelectedItemChanged(); }
        }
        public AsyncObservableCollection<Diary> Diaries
        {
            get { return _diaries; }
            set { _diaries = value;  RaisePropertyChanged("Diaries"); }
        }
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                GetData();
                OnDateChanged();
                RaisePropertyChanged("SelectedDate");
                RaisePropertyChanged("DiaryStartTime");
                RaisePropertyChanged("DiaryEndTime");
            }
        }

        public DateTime DiaryStartTime
        {
            get { return SelectedDate.AddHours(8.5); }
        }

        public DateTime DiaryEndTime
        {
            get
            {
                return DiaryStartTime.AddHours(8.5);
            }
        }

        public AsyncObservableCollection<DiaryEventWrapper> ScanTasks
        {
            get
            {
                if (_scanTasks == null)
                    _scanTasks = new AsyncObservableCollection<DiaryEventWrapper>();

                return _scanTasks;
            }
            set
            {
                _scanTasks = value;
                RaisePropertyChanged("ScanTasks");
            }
        }

        public List<IDiaryTool> Tools
        {
            get
            {
                return _tools.OrderBy(x => x.DiaryPositionIndex).ToList();
            }
            set
            {
                _tools = value;
            }
        }
        #endregion

        #region privateMethods
        private void Refresh()
        {
            CollectionViewSource.GetDefaultView(Diaries).Refresh();
            OnDataLoaded();
        }

        private void OnDataLoaded()
        {
            if (DataLoaded != null)
                DataLoaded(this, new EventArgs());

        }

        private void OnDateChanged()
        {
            if (DateChanged != null)
                DateChanged(this, new EventArgs());
        }

        private void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, new EventArgs());
        }
        #endregion

        #region publicMethods
        public void RefreshDiaries()
        {
            Refresh();
        }
        #endregion

        #region commands
        public RelayCommand LoadCommand { get; set; }
        #endregion

        #region events
        public event EventHandler DataLoaded;
        public event EventHandler DateChanged;
        public event EventHandler SelectedItemChanged;
        #endregion
    }

    public enum DiaryType
    {
        Room,CameraRoom,Role,User
    };

    public class ObjectSelector
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public bool IsSelected { get; set; }
        public int Order { get; set; }
    }

    
    [Export(typeof(IModuleLauncher))]
    public class DiaryModuleLauncer : ModuleLauncher
    {
        public override bool Visible
        {
            get
            {
                return Properties.Settings.Default.DiaryModuleLauncherVisible;
            }

            set
            {
                Properties.Settings.Default.DiaryModuleLauncherVisible = value;
                RaisePropertyChanged("Visible");
            }
        }
        public override int Order
        {
            get
            {
                return Properties.Settings.Default.DiaryModuleLauncherOrder;
            }
            set
            {
                Properties.Settings.Default.DiaryModuleLauncherOrder = value;
                RaisePropertyChanged("Order");
            }
        }

        public override void Save()
        {
            Properties.Settings.Default.Save();
        }
        public override string Name
        {
            get
            {

                return "Diary";
            }
        }

        public string IconSource
        {
            get { return "/iRadiate.Diary.Common;component/Images/DiaryIconWhite.png"; }
        }

        public override void Launch()
        {
            //logger.Trace("SettingsLauncher.Launch()");
            DesktopApplication.MainViewModel.LaunchModule(typeof(DiaryViewModel));

        }

        public override ContentControl ButtonContent
        {
            get
            {
                ContentControl cc = new ContentControl();

                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.CalendarAltRegular;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }
    }

    /// <summary>
    /// I should have implemented this with an itnerface i nthe DataStore assembly
    /// </summary>
    public class DiaryEventWrapper : ViewModelBase
    {
        private IDataStoreItem _event;
        private List<IDataStoreItem> _allRooms;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private DiaryViewModel _diaryViewModel;
        
        public enum EventType { Scan, Other}
        private EventType _eventType;
        private bool _isSelected;

        public DiaryEventWrapper(ScanTask task, DiaryViewModel diaryViewModel)
        {
            _event = task;
            _eventType = EventType.Scan;
            _diaryViewModel = diaryViewModel;
            _allRooms = new List<IDataStoreItem>();
            _allRooms = DesktopApplication.Librarian.GetItems(typeof(Room), new List<RetrievalCriteria>()).ToList();
            ViewDetailsCommand = new RelayCommand(viewDetails);
        }

        public event EventHandler RoomChanged;

        private void OnRoomChanged()
        {
            logger.Trace("Onroomchanged()...");
            if (RoomChanged != null)
                RoomChanged(this, new EventArgs());
            else
                logger.Trace("No handler for RoomChanged");
        }
        public EventType TypeOfevent
        {
            get { return _eventType; }
        }

        public IDataStoreItem Event
        {
            get { return _event; }
        }
        public DateTime EventStart
        {
            get
            {
                if(_eventType == EventType.Scan)
                {
                    return (_event as ScanTask).ValidCommencementTime;
                }

                return new DateTime();
            }
            set
            {
                if (_eventType == EventType.Scan)
                {
                    ScanTask t = _event as ScanTask;
                    if (!t.Commenced)
                    {
                        int duration = t.Duration;
                        t.ScheduledCommencementTime = value;
                        t.ScheduledCompletionTime = t.ScheduledCommencementTime.AddMinutes(duration);
                        RaisePropertyChanged("Eventstart");
                        RaisePropertyChanged("EventFinish");
                    }
                }
            }
        }

        public string DiaryName
        {
            get
            {
                if(_eventType == EventType.Scan)
                {
                    return (_event as ScanTask).DiaryName;
                }
                return "Other";
            }
        }

        public DateTime EventFinish
        {
            get
            {
                if (_eventType == EventType.Scan)
                {
                    return (_event as ScanTask).ValidCompletionTime;
                }
                return new DateTime();
            }
            set
            {
                if (_eventType == EventType.Scan)
                {
                    ScanTask t = _event as ScanTask;
                    if (!t.Completed)
                    {
                        t.ScheduledCompletionTime = value;
                        RaisePropertyChanged("EventFinish");
                    }
                }
            }
        }

        public Room Room
        {
            get
            {
                if (_eventType == EventType.Scan)
                    return (_event as ScanTask).Room;

                return null;
            }
            set
            {
                if (_eventType == EventType.Scan)
                   (_event as ScanTask).Room = value;
                RaisePropertyChanged("Room");
                OnRoomChanged();
            }
        }

        public List<IDataStoreItem> AllRooms
        {
            get
            {
                if (_allRooms == null)
                    _allRooms = new List<IDataStoreItem>();
                return _allRooms;

            }
            set
            {
                _allRooms = value;
                RaisePropertyChanged("AllRooms");
            }
        }

        public List<PatientImage> PatientImages
        {
            get
            {
                if (_eventType == EventType.Scan)
                    return (_event as ScanTask).PatientImages.OrderBy(x => x.SeriesDateTime).ToList();

                return null;

            }
        }

        public bool ContainsImages
        {
            get
            {
                if (_eventType == EventType.Scan)
                {
                     if ((_event as ScanTask).PatientImages.Any())
                        return true;
                    else
                        return false;
                }

                return false;
            }
        }

        public bool Completed
        {
            get
            {
                if(_eventType == EventType.Scan)
                {
                    return (_event as ScanTask).Completed;
                }

                return false;
            }
        }

        public bool Commenced
        {
            get
            {
                if (_eventType == EventType.Scan)
                {
                    return (_event as ScanTask).Commenced;
                }

                return false;
            }
        }

        public bool Cancelled
        {
            get
            {
                if (_eventType == EventType.Scan)
                {
                    return (_event as ScanTask).IsCancelled;
                }

                return false;
            }
        }

        public bool Movable
        {
           get
            {
                if (Commenced || Completed)
                    return false;
                else
                    return true;
            }

        }

        public bool Reschedulable
        {
            get
            {
                if (Commenced || Completed)
                    return false;
                else
                    return true;
            }
        }

        public EventStatus Status
        {
            get
            {
                if (Cancelled)
                    return EventStatus.Cancelled;
                if (Completed)
                    return EventStatus.Completed;
                if (Commenced)
                    return EventStatus.Commenced;
                return EventStatus.Scheduled;

            }
        }

        private void viewDetails()
        {
            if(_eventType == EventType.Scan)
            {
                ScanTaskViewModel vm = new ScanTaskViewModel(_event as ScanTask);
                DesktopApplication.MakeModalDocument(vm);
            }
        }

        public bool IsSelected { get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
                if(_isSelected)
                    OnSelected();
            }
        }
        
        public RelayCommand ViewDetailsCommand { get; set; }

        public event EventHandler Selected;

        protected void OnSelected()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }
    }   

    public class Diary : ViewModelBase
    {
        private Room _room;
        private DiaryViewModel _module;
        private ICollectionView _events;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        public Diary(DiaryViewModel module, Room room)
        {
            _module = module;
            _room = room;
           
            logger.Trace("Created Diary for Room " + Room.Name + " " + Room.ID);
            module.DataLoaded += Module_DataLoaded;
            module.DateChanged += Module_DateChanged;
        }
       
        private void Module_DateChanged(object sender, EventArgs e)
        {
            logger.Trace("Module Datchanged to..." + _module.DiaryStartTime);
            Refresh();
            RaisePropertyChanged("Events");
            RaisePropertyChanged("DiaryStartTime");
            RaisePropertyChanged("DiaryEndtime");
        }

        private void Module_DataLoaded(object sender, EventArgs e)
        {
            logger.Trace("DataLoaded()...");
            RaisePropertyChanged("Events");
            RaisePropertyChanged("DiaryStartTime");
            RaisePropertyChanged("DiaryEndtime");
            Events = new CollectionViewSource { Source = _module.ScanTasks }.View;
            Events.Filter = new Predicate<object>(o=>FilterTask(o));
            Events.Refresh();
        }

        private bool FilterTask(object o)
        {
            logger.Trace("o type: " + o.GetType().ToString());
            if(o is DiaryEventWrapper)
            {
                DiaryEventWrapper w = o as DiaryEventWrapper;
                if(w.TypeOfevent == DiaryEventWrapper.EventType.Scan)
                {
                    logger.Trace("This is a scan event in Room " + (w.Event as ScanTask).Room.Name +" " + (w.Event as ScanTask).Room.ID);
                    if ((w.Event as ScanTask).Room.ID == Room.ID && (w.Event as ScanTask).IsDisconnectedFromAppointment == false)
                    {
                        logger.Trace("Room IDs are matching! ScanTaskRoom Name: " + (w.Event as ScanTask).Room.Name + "; ID: " + (w.Event as ScanTask).Room.ID + ". Room Name: " + Room.Name + "; ID: " + Room.ID);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                        

                }
                else
                {
                    logger.Trace("This event wrapper is not based on a scan");
                    return false;
                }
            }
            return false;
        }

        public Room Room
        {
            get { return _room; }
            set
            {
                _room = value;
                RaisePropertyChanged("Room");
            }
        }

        public ICollectionView Events
        {
            get
            {
                return _events;
            }
            set
            {
                _events = value;
                RaisePropertyChanged("ViewSource");
            }
        }

        public DateTime DiaryStartTime
        {
            get { return _module.DiaryStartTime; }
            
        }

        public DateTime DiaryEndTime
        {
            get { return _module.DiaryEndTime; }
        }

       
        public void Refresh()
        {
            logger.Trace("Refresh()....");
            Events.Refresh();
            
        }
    }

    public enum EventStatus
    {
        Scheduled,
        Commenced,
        Completed,
        Cancelled
    }
}
