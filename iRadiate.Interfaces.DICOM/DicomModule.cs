using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;
using NLog;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;

namespace iRadiate.Interfaces.DICOM
{

    [PreferredView("iRadiate.Interfaces.DICOM.View.DicomModuleView", "iRadiate.Interfaces.DICOM")]
    public class DicomModule : Module
    {
        

        private InterfacePreferences _interfacePreferences;
        private AsyncObservableCollection<StoredDicomServer> _servers;
        private StoredDicomServer _selectedServer;
        private DicomConnector _dicomConnector;
        private AsyncObservableCollection<RoomBridge> _roomBridges;
        private RoomBridge _selectedRoomBridge;
        private ObservableCollection<LogEntry> _logEntries;
        private DateTime _logStartDate;
        private DateTime _logEndDate;

        public DicomModule() : base()
        {
            _logStartDate = DateTime.Today;
            _logEndDate = DateTime.Today.AddDays(1);
        }

        #region overrides
        public override string Name
        {
            get
            {
                return "Dicom Interface";
            }

            set
            {
                //base.Name = value;
            }
        }

        public override System.Windows.Controls.ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.XRaySolid;
                icon.Height = 18;
                icon.Width = 18;
                cc.Content = icon;
                return cc;
            }
        }

        public override string IconSource
        {
            get { return "/iRadiate.Interfaces.CentricityInterface;component/Images/HomeIcon.png"; }
        }

        public override void NonUIThreadInitialize()
        {
            base.NonUIThreadInitialize();
        }

        public override void GetData()
        {
            base.GetData();
            logger.Trace("GetData()...");
            InterfacePreferences = GetPreferences();
            var tmp = GetDicomServers();
            foreach(StoredDicomServer s in tmp)
            {
                Servers.Add(s);
            }
            _dicomConnector = new DicomConnector(tmp);
            var moo = GetRoomBridges();
            foreach(RoomBridge r in moo)
            {
                RoomBridges.Add(r);
            }

        }

        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            SavePreferencesCommand = new RelayCommand(savePreferences);
            AddNewServerCommand = new RelayCommand(addNewServer);
            SaveServersCommand = new RelayCommand(saveServers);
            //PingAllServersCommand = new RelayCommand(pingAllServers);
           
            AddNewSerialNumberCommand = new RelayCommand(addNewSerialNumber);
            SaveRoomBridgesCommand = new RelayCommand(saveRoomBridges);
            GetLogEntriesCommand = new RelayCommand(GetLogEntries);
        }
        #endregion

        #region publicProperties
        public RoomBridge SelectedRoomBridge
        {
            get { return _selectedRoomBridge; }
            set { _selectedRoomBridge = value;  RaisePropertyChanged("SelectedRoomBridge"); }
        }
        public StoredDicomServer SelectedServer
        {
            get { return _selectedServer; }
            set { _selectedServer = value; RaisePropertyChanged("SelectedServer"); }
        }
        public InterfacePreferences InterfacePreferences
        {
            get
            {
                return _interfacePreferences;
            }
            set { _interfacePreferences = value; RaisePropertyChanged("InterfacePreferences"); }
        }

        public AsyncObservableCollection<StoredDicomServer> Servers
        {
            get
            {
                if (_servers == null)
                    _servers = new AsyncObservableCollection<StoredDicomServer>();

                return _servers;
            }
            set
            {
                _servers = value;
                RaisePropertyChanged("Servers");
            }
        }

        public AsyncObservableCollection<RoomBridge> RoomBridges
        {
            get
            {
                if (_roomBridges == null)
                    _roomBridges = new AsyncObservableCollection<RoomBridge>();

                return _roomBridges;
            }
            set
            {
                _roomBridges = value;
                RaisePropertyChanged("Servers");
            }
        }
        public string ConnectionString
        {
            get { return Properties.Settings.Default.DatabaseConnString; }
            set { Properties.Settings.Default.DatabaseConnString = value; RaisePropertyChanged("ConnectionString"); }
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
        public void savePreferences()
        {
            SavePreferences(InterfacePreferences);
        }

        private void saveServers()
        {
            SaveServers(Servers.ToList());
        }

        private void saveRoomBridges()
        {
            SaveRoomBridges(RoomBridges.ToList());
        }
        private void addNewServer()
        {
            StoredDicomServer s = new StoredDicomServer();
            SelectedServer = s;
            Servers.Add(s);
        }
        private void addNewSerialNumber()
        {
            RoomBridge r = new RoomBridge();
            SelectedRoomBridge = r;
            RoomBridges.Add(r);
        }
       

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DesktopApplication.MainViewModel.Busy = false;
        }

        

        private void GetLogEntries()
        {
            LogEntries.Clear();
            if (LogEndDate <= LogStartDate)
            {
                DesktopApplication.ShowDialog("Error", "Start date must be earlier than end date");
                return;
            }
            using (SqlConnection tmpCon = new SqlConnection(Properties.Settings.Default.DatabaseConnString))
            {
                tmpCon.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM system_logging WHERE entered_date > '" + LogStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "' AND log_date < '" + LogEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'", tmpCon);
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

        #region commands
        public RelayCommand SavePreferencesCommand { get; set; }

        public RelayCommand SaveServersCommand { get; set; }

        public RelayCommand AddNewServerCommand { get; set; }

        public RelayCommand PingAllServersCommand { get; set; }

        public RelayCommand SaveRoomBridgesCommand { get; set; }

        public RelayCommand AddNewSerialNumberCommand { get; set; }

        public RelayCommand GetLogEntriesCommand { get; set; }
        #endregion

        #region staticMethods
        public static void SavePreferences(InterfacePreferences preferences)
        {
#if DEBUG
            return;
#endif
            XmlSerializer mySerializer = new XmlSerializer(typeof(InterfacePreferences));
            MemoryStream stream = new MemoryStream();
            Utf8StringWriter myWriter = new Utf8StringWriter();
            mySerializer.Serialize(myWriter, preferences);
            //StreamReader reader = new StreamReader(stream);

            string text = myWriter.ToString();
            logger.Info("text = " + text);
            myWriter.Close();

            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DatabaseConnString))
            {
                conn.Open();
                string cmdString = "UPDATE Settings SET SettingValue = @SettingValue WHERE ID = 1";
                SqlCommand updateCmd = new SqlCommand(cmdString, conn);
                updateCmd.Parameters.AddWithValue("@SettingValue", text);
                if (updateCmd.ExecuteNonQuery() > 0)
                {
                    //DesktopApplication.ShowDialog("Saved", "Interface updated.");
                    conn.Close();
                }
                else
                {
                    //DesktopApplication.ShowDialog("Error", "Interface failed to save.");
                    logger.Error("Dicom Interface failed to save user preferences: " + text);
                    conn.Close();
                }
            }
        }

        public static InterfacePreferences GetPreferences()
        {
            logger.Trace("GetPreferences()...");
            InterfacePreferences InterfacePreferences = null;
#if DEBUG
            InterfacePreferences = new InterfacePreferences();
            InterfacePreferences.AETitle = "DESKTOP-UR5U52N";
            InterfacePreferences.HostName = "DESKTOP-UR5U52N";
            InterfacePreferences.Port = 1040;
            return InterfacePreferences;
#endif
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DatabaseConnString))
            {
                conn.Open();
                string cmdText = "SELECT * FROM Settings WHERE ID = 1";
                string xmlString;
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        xmlString = reader["SettingValue"].ToString();
                        XmlSerializer serializer = new XmlSerializer(typeof(InterfacePreferences));
                        // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(xmlString);
                        //byte[] byteArray = Encoding.ASCII.GetBytes(contents);

                        MemoryStream stream = new MemoryStream(byteArray);
                        try
                        {
                            InterfacePreferences = (InterfacePreferences)serializer.Deserialize(stream);
                            
                        }
                        catch (Exception ex)
                        {
                            InterfacePreferences = new InterfacePreferences();
                            InterfacePreferences.AETitle = "Failed to deserialized " + ex.Message;
                        }
                        continue;
                    }
                }
                else
                {
                    InterfacePreferences = new InterfacePreferences();
                    InterfacePreferences.AETitle = "HELLO WORLD FROM NULL";
                }

            }
            return InterfacePreferences;
        }

        public static List<StoredDicomServer> GetDicomServers()
        {
            logger.Trace("GetDicomServers()...");
            List<StoredDicomServer> result = new List<StoredDicomServer>();
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DatabaseConnString))
            {
                conn.Open();
                string cmdText = "SELECT * FROM DicomServers";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    logger.Trace("Dicomservers found in database");
                    while (reader.Read())
                    {
                       for(int i = 0; i < reader.FieldCount;i++)
                        {
                            logger.Trace(reader.GetName(i) + ": " + reader[i].ToString());
                        }
                        StoredDicomServer server = new StoredDicomServer();
                        if(!reader.IsDBNull(reader.GetOrdinal("IPAddress")))
                            server.IPAddress = reader["IPAddress"].ToString();
                        if (!reader.IsDBNull(reader.GetOrdinal("Enabled")))
                            server.Enabled = reader.GetBoolean(reader.GetOrdinal("Enabled"));
                        if (!reader.IsDBNull(reader.GetOrdinal("AETitle")))
                            server.AETitle = reader["AETitle"].ToString();
                        if (!reader.IsDBNull(reader.GetOrdinal("Port")))
                            server.Port = reader.GetInt32(reader.GetOrdinal("Port"));
                        if (!reader.IsDBNull(reader.GetOrdinal("ID")))
                            server.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        if (!reader.IsDBNull(reader.GetOrdinal("ImageStore")))
                            server.ImageStore = reader.GetBoolean(reader.GetOrdinal("ImageStore"));
                        else
                            server.ImageStore = false;
                        if (!reader.IsDBNull(reader.GetOrdinal("Worklist")))
                            server.Worklist = reader.GetBoolean(reader.GetOrdinal("Worklist"));
                        else
                            server.Worklist = false;
                        server.Online = false;
                       
                        result.Add(server);
                        
                    }
                }
                else
                {
                    logger.Warn("No DicomServers found");
                }
            }
            return result;
        }

        public static void SaveServers(List<StoredDicomServer> servers)
        {
            foreach(StoredDicomServer s in servers)
            {
                if(s.ID != 0)
                {
                    using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DatabaseConnString))
                    {
                        conn.Open();
                        string cmdString = "UPDATE DicomServers SET IPAddress = @IPAddress, Port = @Port, AETitle = @AETitle, Enabled=@Enabled, ImageStore = @ImageStore, Worklist = @Worklist WHERE ID = " + s.ID;
                        SqlCommand updateCmd = new SqlCommand(cmdString, conn);
                        updateCmd.Parameters.AddWithValue("@IPAddress", s.IPAddress);
                        updateCmd.Parameters.AddWithValue("@Port", s.Port);
                        updateCmd.Parameters.AddWithValue("@AETitle", s.AETitle);
                        updateCmd.Parameters.AddWithValue("@Enabled", s.Enabled);
                        updateCmd.Parameters.AddWithValue("@ImageStore", s.ImageStore);
                        updateCmd.Parameters.AddWithValue("@Worklist", s.Worklist);
                        if (updateCmd.ExecuteNonQuery() > 0)
                        {
                            //DesktopApplication.ShowDialog("Saved", "Interface updated.");
                            conn.Close();
                        }
                        else
                        {
                            //DesktopApplication.ShowDialog("Error", "Interface failed to save.");
                            logger.Error("Dicom Interface failed to save user preferences: " + cmdString);
                            conn.Close();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DatabaseConnString))
                    {
                        conn.Open();
                        string cmdString = "INSERT INTO DicomServers (IPAddress, Port,AETitle,Enabled, ImageStore, Worklist) VALUES (@IPAddress,@Port,@AETitle,@Enabled,@ImageStore,@Worklist)";
                        SqlCommand updateCmd = new SqlCommand(cmdString, conn);
                        updateCmd.Parameters.AddWithValue("@IPAddress", s.IPAddress);
                        updateCmd.Parameters.AddWithValue("@Port", s.Port);
                        updateCmd.Parameters.AddWithValue("@AETitle", s.AETitle);
                        updateCmd.Parameters.AddWithValue("@Enabled", s.Enabled);
                        updateCmd.Parameters.AddWithValue("@ImageStore", s.ImageStore);
                        updateCmd.Parameters.AddWithValue("@Worklist", s.Worklist);
                        if (updateCmd.ExecuteNonQuery() > 0)
                        {
                            //DesktopApplication.ShowDialog("Saved", "Interface updated.");
                            conn.Close();
                        }
                        else
                        {
                            //DesktopApplication.ShowDialog("Error", "Interface failed to save.");
                            logger.Error("Dicom Interface failed to save user preferences: " + cmdString);
                            conn.Close();
                        }
                    }
                }
                
            }
            DesktopApplication.ShowDialog("Saved", "Interface updated.");
        }

        public static List<RoomBridge> GetRoomBridges()
        {
            List<RoomBridge> result = new List<RoomBridge>();
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DatabaseConnString))
            {
                conn.Open();
                string cmdText = "SELECT * FROM RoomBridges";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    logger.Trace("Dicomservers found in database");
                    while (reader.Read())
                    {
                       
                        RoomBridge bridge = new RoomBridge();
                        if (!reader.IsDBNull(reader.GetOrdinal("ID")))
                            bridge.ID = reader.GetInt32(reader.GetOrdinal("ID"));

                        if (!reader.IsDBNull(reader.GetOrdinal("RoomID")))
                            bridge.RoomID = reader.GetInt32(reader.GetOrdinal("RoomID"));

                        if (!reader.IsDBNull(reader.GetOrdinal("SerialNumber")))
                            bridge.SerialNumber = reader["SerialNumber"].ToString();

                        if (!reader.IsDBNull(reader.GetOrdinal("ModelName")))
                            bridge.ModelName = reader["ModelName"].ToString();

                        if (bridge.RoomID != 0)
                        {
                            
                            var room = Platform.Retriever.RetrieveItem(bridge.RoomID, typeof(Room));
                            if (room != null)
                                bridge.Room = room as Room;
                        }

                        result.Add(bridge);

                    }
                }
                else
                {
                    logger.Warn("No DicomServers found");
                }
            }
            return result;
        }

        public static void SaveRoomBridges(List<RoomBridge> roomBridges)
        {
            foreach(RoomBridge r in roomBridges)
            {
                if (r.ID != 0)
                {
                    using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DatabaseConnString))
                    {
                        conn.Open();
                        string cmdString = "UPDATE RoomBridges SET SerialNumber = @SerialNumber, RoomID = @RoomID, ModelName = @ModelName WHERE ID = " + r.ID;
                        SqlCommand updateCmd = new SqlCommand(cmdString, conn);
                        updateCmd.Parameters.AddWithValue("@SerialNumber", r.SerialNumber ?? string.Empty);
                        updateCmd.Parameters.AddWithValue("@RoomID", r.RoomID);
                        updateCmd.Parameters.AddWithValue("@ModelName", r.ModelName ?? string.Empty);
                        if (updateCmd.ExecuteNonQuery() > 0)
                        {
                            //DesktopApplication.ShowDialog("Saved", "Interface updated.");
                            conn.Close();
                        }
                        else
                        {
                            //DesktopApplication.ShowDialog("Error", "Interface failed to save.");
                            logger.Error("Dicom Interface failed to save user preferences: " + cmdString);
                            conn.Close();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DatabaseConnString))
                    {
                        conn.Open();
                        string cmdString = "INSERT INTO RoomBridges (SerialNumber, RoomID, ModelName) VALUES (@SerialNumber,@RoomID, @ModelName)";
                        SqlCommand updateCmd = new SqlCommand(cmdString, conn);
                        updateCmd.Parameters.AddWithValue("@SerialNumber", r.SerialNumber ?? string.Empty);
                        updateCmd.Parameters.AddWithValue("@RoomID", r.RoomID);
                        updateCmd.Parameters.AddWithValue("@ModelName", r.ModelName ?? string.Empty);
                        if (updateCmd.ExecuteNonQuery() > 0)
                        {
                            //DesktopApplication.ShowDialog("Saved", "Interface updated.");
                            conn.Close();
                        }
                        else
                        {
                            //DesktopApplication.ShowDialog("Error", "Interface failed to save.");
                            logger.Error("Dicom Interface failed to save user preferences: " + cmdString);
                            conn.Close();
                        }
                    }
                }
            }
        }

        public static bool IsScreencapStored(string seriesUID)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DatabaseConnString))
            {
                string cmdText = "SELECT * FROM Screencaps WHERE SeriesInstanceUID = '" + seriesUID + "'";
                conn.Open();
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    return true;
                }
                return false;
            }
        }

        public static bool SaveScreencapLink(string seriesUID, iRadiate.DataModel.Common.File f)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DatabaseConnString))
            {
                conn.Open();
                string cmdString = "INSERT INTO Screencaps (SeriesInstanceUID, FileID) VALUES (@SeriesUID, @FileID)";
                SqlCommand updateCmd = new SqlCommand(cmdString, conn);
                updateCmd.Parameters.AddWithValue("@SeriesUID", seriesUID);
                updateCmd.Parameters.AddWithValue("@FileID", f.ID);
                if (updateCmd.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
#endregion
    }
    public class Utf8StringWriter : StringWriter
    {
        // Use UTF8 encoding but write no BOM to the wire
        public override Encoding Encoding
        {
            get { return new UTF8Encoding(false); } // in real code I'll cache this encoding.
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

}
