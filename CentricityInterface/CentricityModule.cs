using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using NLog;

using iRadiate.Common;

using iRadiate.Common.IO;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Radiopharmacy;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace iRadiate.Interfaces.CentricityInterface
{
    [PreferredView("iRadiate.Interfaces.CentricityInterface.View.CentricityView", "iRadiate.Interfaces.CentricityInterface")]
    public class CentricityModule : Module
    {

        #region privateFields
        private AsyncObservableCollection<ProcedureTypeBridge> _studyTypeBridges;
        private AsyncObservableCollection<WardBridge> _wardBridges;
        private AsyncObservableCollection<DoctorBridge> _doctorBridges;
        private ProcedureTypeBridge _selectedStudyTypeBridge;
        private WardBridge _selectedWardBridge;
        static IDataRetriever retriever = new EFDataRetriever();
        private ObservableCollection<LogEntry> _logEntries;
        private DateTime _logStartDate, _logEndDate;
        private List<IDataStoreItem> _allChemicals;
        private List<IDataStoreItem> _allRooms;
        #endregion

        #region constructors
        public CentricityModule()
        {
            if(DesktopApplication.Librarian != null)
            {
                retriever = DesktopApplication.Librarian.DataRetriever;
            }
            GetLogEntriesCommand = new RelayCommand(GetLogEntries);
            LogStartDate = DateTime.Today;
            LogEndDate = LogStartDate.AddDays(1);
        }
        #endregion

        #region overrides
        public override void GetData()
        {
            logger.Trace("GetData()...");
            base.GetData();
            _allChemicals = retriever.RetrieveItems(typeof(Chemical), new List<RetrievalCriteria>());
            _allRooms = retriever.RetrieveItems(typeof(Room), new List<RetrievalCriteria>());
            logger.Trace("GetData()......Complete");
        }
        public override void UIThreadInitialize()
        {
            logger.Trace("UIThreadInitialize()...");
            base.UIThreadInitialize();
            StudyTypeBridges = new AsyncObservableCollection<ProcedureTypeBridge>();
            WardBridges = new AsyncObservableCollection<WardBridge>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = Properties.Settings.Default.InterfaceConnString;
                conn.Open();
                // Create the command
                SqlCommand command = new SqlCommand("SELECT * FROM dbo.StudyTypeBridges", conn);


                /* Get the rows and display on the screen! 
                 * This section of the code has the basic code
                 * that will display the content from the Database Table
                 * on the screen using an SqlDataReader. */

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        ProcedureTypeBridge stb = new ProcedureTypeBridge();
                        stb.ID = Convert.ToInt32(reader[0]);
                        stb.ForeignKey = reader[1].ToString();
                        stb.ForeignName = reader[2].ToString();
                        logger.Trace("ProcedureTypeBridge- ID: " + stb.ID + "; ForeignKey: " + stb.ForeignKey + "; ForeignName: " + stb.ForeignName);

                        if (reader[3] != System.DBNull.Value)
                        {
                            stb.LocalKey = Convert.ToInt32(reader[3]);
                            //ok now we have to get the studytype for this


                            stb.StudyType = (StudyType)retriever.RetrieveItem(stb.LocalKey, typeof(StudyType));
                            logger.Trace("Linked Study Type: " + stb.StudyType.Name);

                        }

                        stb.IsFollowUp = reader[4] as bool? ?? false;
                        if (reader[5] != System.DBNull.Value)
                        {
                            stb.Range = Convert.ToInt32(reader[5]);
                        }
                        if (!reader.IsDBNull(6))
                        {
                            stb.SetNumInjections(Convert.ToInt32(reader[6]));
                            logger.Trace("stb.NumInjections = " + stb.NumInjections.ToString());
                            for (int x = 1; x < stb.NumInjections + 1; x = x + 1)
                            {
                                InjectionDetail id = new InjectionDetail();
                                id.InjectionDelay = Convert.ToInt32(reader["Injection" + (x).ToString() + "Delay"]);
                                if (Convert.ToInt32(reader["Injection" + (x).ToString() + "RadiopharmaceuticalID"]) > 0)
                                    id.Radiopharmaceutical = retriever.RetrieveItem(Convert.ToInt32(reader["Injection" + (x).ToString() + "RadiopharmaceuticalID"]), typeof(Chemical));
                                id.InjectionActivity = Convert.ToInt32(reader["Injection" + (x).ToString() + "Activity"]);
                                var tmp1 = reader["Injection" + x.ToString() + "Route"] as int? ?? -1;
                               
                                if (tmp1 == -1)
                                    id.AdministrationRoute = null;
                                else
                                {
                                    id.AdministrationRoute = (AdministrationRoute)Enum.ToObject(typeof(AdministrationRoute), tmp1);
                                }

                                stb.InjectionDetails.Add(id);
                                logger.Trace("Delay: " + id.InjectionDelay.ToString() + "; Chemical: " + ((Chemical)id.Radiopharmaceutical).Name + "; Activity: " + id.InjectionActivity.ToString());
                            }
                            if (Convert.IsDBNull(reader["NumberOfScans"]))
                            {
                                stb.SetNumScans(0);
                                logger.Trace("Number of scans: " + stb.NumScans);
                            }
                            else
                            {
                                stb.SetNumScans(Convert.ToInt32(reader["NumberOfScans"]));
                                logger.Trace("Number of scans: " + stb.NumScans);
                                for (int y = 1; y < stb.NumScans + 1; y = y + 1)
                                {
                                    ScanDetail sd = new ScanDetail();
                                    sd.ScanDelay = Convert.ToInt32(reader["Scan" + (y).ToString() + "Delay"]);
                                    sd.ScanDuration = Convert.ToInt32(reader["Scan" + (y).ToString() + "Duration"]);
                                    if (Convert.ToInt32(reader["Scan" + (y).ToString() + "RoomID"]) > 0)
                                        sd.Room = retriever.RetrieveItem(Convert.ToInt32(reader["Scan" + (y).ToString() + "RoomID"]), typeof(Room));
                                    stb.ScanDetails.Add(sd);
                                    logger.Trace("Scan" + y.ToString() + " - Delay: " +sd.ScanDelay +"; ScanDuration: " + sd.ScanDuration + "; Room: " +(sd.Room as Room).Name);
                                }
                            }
                            
                        }

                        StudyTypeBridges.Add(stb);

                    }
                }

                command = new SqlCommand("SELECT * FROM dbo.WardBridges", conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        WardBridge stb = new WardBridge();
                        stb.ID = Convert.ToInt32(reader[0]);
                        stb.ForeignKey = reader[1].ToString();
                        stb.ForeignName = reader[2].ToString();
                        if (reader[3] != System.DBNull.Value)
                        {
                            stb.LocalID = Convert.ToInt32(reader[3]);
                            //ok now we have to get the studytype for this

                            stb.Ward = (Ward)retriever.RetrieveItem(stb.LocalID, typeof(Ward));

                        }

                        WardBridges.Add(stb);

                    }
                }

            }

        }
        #endregion

        #region publicProperties
        public List<IDataStoreItem> AllChemicals
        {
            get
            {
                if (_allChemicals == null)
                    _allChemicals = new List<IDataStoreItem>();

                return _allChemicals;
            }
            set
            {
                _allChemicals = value;
                RaisePropertyChanged("AllChemicals");
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
        public virtual ProcedureTypeBridge SelectedStudyTypeBridge
        {
            get
            {
                return _selectedStudyTypeBridge;
            }
            set
            {
                _selectedStudyTypeBridge = value;
                RaisePropertyChanged("SelectedStudyTypeBridge");
            }
        }

        public WardBridge SelectedWardBridge
        {
            get
            {
                return _selectedWardBridge;
            }
            set
            {
                _selectedWardBridge = value;
                RaisePropertyChanged("SelectedWardBridge");
            }
        }

        public virtual AsyncObservableCollection<ProcedureTypeBridge> StudyTypeBridges
        {
            get
            {
                return _studyTypeBridges;
            }
            set
            {
                _studyTypeBridges = value;
            }
        }
        public AsyncObservableCollection<WardBridge> WardBridges
        {
            get
            {
                return _wardBridges;
            }
            set
            {
                _wardBridges = value;
            }
        }

        public override string Name
        {
            get
            {
                return "Centricity";
            }
            set
            {

            }
        }

        public override string IconSource
        {
            get { return "/iRadiate.Interfaces.CentricityInterface;component/Images/GEIcon.png"; }
        }
        public override ContentControl IconContent
        {
            get
            {
                ResourceDictionary resourceDictionary = (ResourceDictionary)SharedResourceDictionary.SharedDictionary;
                Path p = (Path)resourceDictionary["GEIconAlternative"];
                p.Stroke = new SolidColorBrush(Colors.Black);
                p.Height = 24;
                p.Width = 24;
                p.StrokeThickness = 0.8;
                p.Stretch = Stretch.Uniform;
                p.Fill = new SolidColorBrush(Colors.Transparent);
                ContentControl cc = new ContentControl();
                cc.Content = p;
                return cc;
                //return base.ButtonContent;
            }
        }

        public ObservableCollection<LogEntry> LogEntries
        {
            get
            {
                if (_logEntries == null)
                {
                    _logEntries = new ObservableCollection<LogEntry>();
                }
                return _logEntries;
            }
            set
            {
                _logEntries = value;

            }
        }

        public DateTime LogStartDate
        {
            get
            {
                return _logStartDate;
            }
            set
            {
                _logStartDate = value;
                RaisePropertyChanged("LogStartDate");
            }
        }

        public DateTime LogEndDate
        {
            get { return _logEndDate; }
            set
            {
                _logEndDate = value;
                RaisePropertyChanged("LogEndDate");
            }
        }
        #endregion

        #region privateMethods
        private void GetLogEntries()
        {
            LogEntries.Clear();
            if(LogEndDate <= LogStartDate)
            {
                DesktopApplication.ShowDialog("Error", "Start date must be earlier than end date");
                return;
            }
            using (SqlConnection tmpCon = new SqlConnection(Properties.Settings.Default.InterfaceConnString))
            {
                tmpCon.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM system_logging WHERE entered_date > '" + LogStartDate.ToString("yyyy-MM-dd HH:mm:ss") +"' AND log_date < '" + LogEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'", tmpCon);
                //command.Parameters.AddWithValue("@StartDate", LogStartDate);
                //SqlParameter startDateParam = new SqlParameter("@StartDate", SqlDbType.DateTime);
                //startDateParam.Value = LogStartDate;
               // SqlParameter endDateParam = new SqlParameter("@EndDate", SqlDbType.DateTime);
                //endDateParam.Value = endDateParam;
                //command.Parameters.Add(startDateParam);
                //command.Parameters.Add(endDateParam);
                //command.Parameters.AddWithValue("@EndDate", LogEndDate);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //reader.Read();
                    LogEntry entry = new LogEntry();
                    entry.LogCallSite = reader["log_call_site"].ToString();
                    entry.LogDate = reader["log_date"].ToString();
                    //entry.LogDate= reader.GetDateTime(reader.GetOrdinal("log_date"));
                    entry.LogException = reader["log_exception"].ToString();
                    entry.LogLevel = reader["log_level"].ToString();
                    entry.LogLogger = reader["log_logger"].ToString();
                    entry.LogMessage = reader["log_message"].ToString();
                    entry.LogStackTrace = reader["log_stacktrace"].ToString();
                    LogEntries.Add(entry);
                }
            }

           
        }
        #endregion        

        #region Commands
        public RelayCommand GetLogEntriesCommand { get; set; }
        #endregion

    }


    [Export(typeof(IModuleLauncher))]
    public class CentricityModuleLauncher : ModuleLauncher
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override string Name
        {
            get
            {

                return "Centricity";
            }
        }

        public string IconSource
        {
            get { return "/iRadiate.Interfaces.CentricityInterface;component/Images/GEIconWhite.png"; }
        }

        public override void Launch()
        {
            //logger.Trace("SettingsLauncher.Launch()");
            DesktopApplication.MainViewModel.LaunchModule(typeof(CentricityModule));

        }
        public override ContentControl ButtonContent
        {
            get
            {
                ResourceDictionary resourceDictionary = (ResourceDictionary)SharedResourceDictionary.SharedDictionary;
                Path p = (Path)resourceDictionary["GEIcon"];
                p.Stroke = new SolidColorBrush(Colors.White);
                p.Height = 24;
                p.Width = 24;
                p.StrokeThickness = 0.8;
                p.Stretch = Stretch.Uniform;
                p.Fill = new SolidColorBrush(Colors.Transparent);
                ContentControl cc = new ContentControl();
                cc.Content = p;
                return cc;
                //return base.ButtonContent;
            }
        }
        public override int Order
        {
            get
            {
                return Properties.Settings.Default.CentricityModuleLauncherOrder;
            }
            set
            {
                Properties.Settings.Default.CentricityModuleLauncherOrder = value;
                RaisePropertyChanged("Order");
            }
        }

        public override bool Visible
        {
            get
            {
                return Properties.Settings.Default.CentricityModuleLauncherVisible;
            }

            set
            {
                Properties.Settings.Default.CentricityModuleLauncherVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override void Save()
        {
            Properties.Settings.Default.Save();
        }
    }

    public class LogEntry
    {
        public string LogDate { get; set; }
        public string LogLevel { get; set; }
        public string LogLogger { get; set; }
        public string LogMessage { get; set; }
        public string LogCallSite { get; set; }
        public string LogException { get; set; }
        public string LogStackTrace { get; set; }
    }

    internal static class SharedResourceDictionary
    {
        internal static ResourceDictionary SharedDictionary
        {
            get
            {
                if (_sharedDictionary == null)
                {
                    try
                    {
                        //System.Uri resourceLocater1 = new System.Uri(string.Format("/{0};component/YourResourceDictionary.xaml","YourProject"), System.UriKind.Relative);
                        System.Uri resourceLocater1 = new System.Uri("/iRadiate.Desktop.Common;component/Themes/MainTheme.xaml", UriKind.Relative);
                        ResourceDictionary resourceDictionary = new ResourceDictionary
                        {
                            Source = resourceLocater1
                        };
                        _sharedDictionary = resourceDictionary;
                    }
                    catch (Exception e)
                    {
                        
                    }
                }

                return _sharedDictionary;
            }
        }
        private static ResourceDictionary _sharedDictionary;
    }
}
