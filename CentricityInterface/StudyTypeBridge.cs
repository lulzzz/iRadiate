using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using iRadiate.Desktop.Common;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Radiopharmacy;

namespace iRadiate.Interfaces.CentricityInterface
{
    public class ProcedureTypeBridge : ViewModelBase
    {
        #region privateFields
        private string _foreignKey;
        private string _foreignName;
        private int _localKey;
        private StudyType _studyType;
        private string _sequence;
        private int _ID;
        private bool _isFollowUp;
        private StudyType _leadStudyType;
        private int _range;
        private int _numInjections;
        private int _numScans;
        private ObservableCollection<InjectionDetail> _injectionDetails;
        private ObservableCollection<ScanDetail> _scanDetails;
        private AdministrationRoute? _administrationRoute;
        #endregion

        public ProcedureTypeBridge()
        {
            SaveCommand = new RelayCommand(saveBridge);
            
        }

        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
                RaisePropertyChanged("ID");
            }
        }

        public string ForeignKey
        {
            get
            {
                return _foreignKey;
            }
            set
            {
                _foreignKey = value;
                RaisePropertyChanged("ForeignKey");
            }
        }

        public string ForeignName
        {
            get
            {
                return _foreignName;
            }
            set
            {
                _foreignName = value;
                RaisePropertyChanged("ForeignName");
            }
        }

        public int LocalKey
        {
            get
            {
                return _localKey;
            }
            set
            {
                _localKey = value;
                RaisePropertyChanged("LocalKey");
            }
        }

        public virtual StudyType StudyType
        {
            get
            {
                return _studyType;
            }
            set
            {
                _studyType = value;
                if(value != null)
                {
                    LocalKey = value.ID;
                }
                
                RaisePropertyChanged("StudyType");
            }
        }

        public string Sequence
        {
            get
            {
                return _sequence;
            }
            set
            {
                _sequence = value;
                RaisePropertyChanged("Sequence");
            }
        }

        private void saveBridge()
        {
            if (ID != 0)
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = Properties.Settings.Default.InterfaceConnString;
                    conn.Open();
                    string cmdString =
                        "UPDATE StudyTypeBridges SET ForeignKey = @ForeignKey, " +
                        "ForeignName = @ForeignName, LocalKey = @LocalKey, IsFollowUp = @IsFollowUp, " +
                        "FollowUpRange = @FollowUpRange, NumberOfInjections = @NumberOfInjections, NumberOfScans = @NumberOfScans, " +
                        "Injection1Delay = @Injection1Delay, Injection2Delay = @Injection2Delay, Injection1RadiopharmaceuticalID = @Injection1ID, " +
                        "Injection2RadiopharmaceuticalID = @Injection2ID, Injection1Activity = @Injection1Activity, Injection2Activity = @Injection2Activity, " +
                        "Scan1Delay = @Scan1Delay, Scan1RoomID = @Scan1RoomID, Scan1Duration = @Scan1Duration, " +
                        "Scan2Delay = @Scan2Delay, Scan2RoomID = @Scan2RoomID, Scan2Duration = @Scan2Duration, " +
                        "Injection1Route = @Injection1Route, Injection2Route = @Injection2Route " +
                         "WHERE ID = @ID";
                    SqlCommand insertCommand = new SqlCommand(cmdString, conn);
                    insertCommand.Parameters.AddWithValue("@ForeignKey", ForeignKey);
                    insertCommand.Parameters.AddWithValue("@ForeignName", ForeignName);
                    insertCommand.Parameters.AddWithValue("@LocalKey", LocalKey);
                    insertCommand.Parameters.AddWithValue("@IsFollowUp", IsFollowUp);
                    insertCommand.Parameters.AddWithValue("@FollowupRange", Range);
                    insertCommand.Parameters.AddWithValue("@ID", ID);
                    insertCommand.Parameters.AddWithValue("@NumberOfInjections", NumInjections);
                    insertCommand.Parameters.AddWithValue("@NumberOfScans", NumScans);
                    if (NumInjections == 1)
                    {
                        insertCommand.Parameters.AddWithValue("@Injection1Delay", InjectionDetails[0].InjectionDelay);
                        //DesktopApplication.ShowDialog("Info", InjectionDetails[0].Radiopharmaceutical.Name);
                        insertCommand.Parameters.AddWithValue("@Injection1ID", InjectionDetails[0].Radiopharmaceutical.ID);
                        //insertCommand.Parameters.AddWithValue("@Injection1ID", 0);
                        insertCommand.Parameters.AddWithValue("@Injection1Activity", InjectionDetails[0].InjectionActivity);
                        if (InjectionDetails[0].AdministrationRoute.HasValue)
                        {
                            insertCommand.Parameters.AddWithValue("@Injection1Route", (int)InjectionDetails[0].AdministrationRoute);
                        }
                        else
                        {
                            insertCommand.Parameters.AddWithValue("@Injection1Route", DBNull.Value);
                        }
                            
                       
                        
                        insertCommand.Parameters.AddWithValue("@Injection2Delay", 0);
                        insertCommand.Parameters.AddWithValue("@Injection2ID", 0);
                        insertCommand.Parameters.AddWithValue("@Injection2Activity", 0);
                        insertCommand.Parameters.AddWithValue("@Injection2Route", DBNull.Value);
                    }
                    else if(NumInjections == 2)
                    {
                        insertCommand.Parameters.AddWithValue("@Injection1Delay", InjectionDetails[0].InjectionDelay);
                        insertCommand.Parameters.AddWithValue("@Injection1ID", InjectionDetails[0].Radiopharmaceutical.ID);
                        insertCommand.Parameters.AddWithValue("@Injection1Activity", InjectionDetails[0].InjectionActivity);
                        if (InjectionDetails[0].AdministrationRoute.HasValue)
                        {
                            insertCommand.Parameters.AddWithValue("@Injection1Route", (int)InjectionDetails[0].AdministrationRoute);
                        }
                        else
                        {
                            insertCommand.Parameters.AddWithValue("@Injection1Route", DBNull.Value);
                        }



                        insertCommand.Parameters.AddWithValue("@Injection2Delay", InjectionDetails[1].InjectionDelay);
                        insertCommand.Parameters.AddWithValue("@Injection2ID", InjectionDetails[1].Radiopharmaceutical.ID);
                        insertCommand.Parameters.AddWithValue("@Injection2Activity", InjectionDetails[1].InjectionActivity);


                        if (InjectionDetails[1].AdministrationRoute.HasValue)
                        {
                            insertCommand.Parameters.AddWithValue("@Injection2Route", (int)InjectionDetails[1].AdministrationRoute);
                        }
                        else
                        {
                            insertCommand.Parameters.AddWithValue("@Injection2Route", DBNull.Value);
                        }

                    }
                    else
                    {
                        insertCommand.Parameters.AddWithValue("@Injection1Delay", 0);
                        insertCommand.Parameters.AddWithValue("@Injection1ID", 0);
                        insertCommand.Parameters.AddWithValue("@Injection1Activity", 0);
                        insertCommand.Parameters.AddWithValue("@Injection1Route", DBNull.Value);

                        insertCommand.Parameters.AddWithValue("@Injection2Delay", 0);
                        insertCommand.Parameters.AddWithValue("@Injection2ID", 0);
                        insertCommand.Parameters.AddWithValue("@Injection2Activity", 0);
                        insertCommand.Parameters.AddWithValue("@Injection2Route", DBNull.Value);
                    }
                    if(NumScans == 1)
                    {
                        insertCommand.Parameters.AddWithValue("@Scan1Delay", ScanDetails[0].ScanDelay);
                        insertCommand.Parameters.AddWithValue("@Scan1RoomID", ScanDetails[0].Room.ID);
                        insertCommand.Parameters.AddWithValue("@Scan1Duration", ScanDetails[0].ScanDuration);
                        insertCommand.Parameters.AddWithValue("@Scan2Delay", 0);
                        insertCommand.Parameters.AddWithValue("@Scan2RoomID", 0);
                        insertCommand.Parameters.AddWithValue("@Scan2Duration", 0);
                    }
                    else if(NumScans == 2)
                    {
                        insertCommand.Parameters.AddWithValue("@Scan1Delay", ScanDetails[0].ScanDelay);
                        insertCommand.Parameters.AddWithValue("@Scan1RoomID", ScanDetails[0].Room.ID);
                        insertCommand.Parameters.AddWithValue("@Scan1Duration", ScanDetails[0].ScanDuration);
                        insertCommand.Parameters.AddWithValue("@Scan2Delay", ScanDetails[1].ScanDelay);
                        insertCommand.Parameters.AddWithValue("@Scan2RoomID", ScanDetails[1].Room.ID);
                        insertCommand.Parameters.AddWithValue("@Scan2Duration", ScanDetails[1].ScanDuration);
                    }
                    else
                    {
                        insertCommand.Parameters.AddWithValue("@Scan1Delay", 0);
                        insertCommand.Parameters.AddWithValue("@Scan1RoomID", 0);
                        insertCommand.Parameters.AddWithValue("@Scan1Duration", 0);
                        insertCommand.Parameters.AddWithValue("@Scan2Delay", 0);
                        insertCommand.Parameters.AddWithValue("@Scan2RoomID", 0);
                        insertCommand.Parameters.AddWithValue("@Scan2Duration", 0);
                    }
                    if (insertCommand.ExecuteNonQuery() > 0)
                    {
                        DesktopApplication.ShowDialog("Saved", "Interface updated.");
                        conn.Close();
                    }
                    else
                    {
                        DesktopApplication.ShowDialog("Error", "Interface failed to save.");
                        conn.Close();
                    }
                }
                

            }
        }

        public List<StudyType> AvailableStudyTypes
        {
            get
            {
                return DesktopApplication.CurrentPratice.StudyTypes.OrderBy(x=>x.Name).ToList();
            }
        }

        public RelayCommand SaveCommand
        {
            get;
            set;

        }

        public bool IsFollowUp
        {
            get
            {
                return _isFollowUp;
            }
            set
            {
                _isFollowUp = value;
                RaisePropertyChanged("IsFollowUp");
            }
        }

        public StudyType LeadStudyType
        {
            get
            {
                return _leadStudyType;
            }
            set
            {
                _leadStudyType = value;
                RaisePropertyChanged("LeadStudyType");
            }
        }

        public int Range
        {
            get
            {
                return _range;
            }
            set
            {
                _range = value;
                RaisePropertyChanged("Range");
            }
        }
        
        public void SetNumInjections(int x)
        {
            _numInjections = x;
        }

        public void SetNumScans(int x)
        {
            _numScans = x;
        }
        public int NumInjections
        {
            get
            {
                return _numInjections;
            }
            set
            {
                if (value > _numInjections)
                {
                    InjectionDetails.Add(new InjectionDetail());
                }
                if(value < _numInjections)
                {
                    InjectionDetails.RemoveAt(value);
                }
                _numInjections = value;
                RaisePropertyChanged("NumInjections");
                
            }
        }

        public int NumScans
        {
            get
            {
                return _numScans;
            }
            set
            {
                if(value > _numScans)
                {
                    ScanDetails.Add(new ScanDetail());
                }
                if(value < _numScans)
                {
                    ScanDetails.RemoveAt(value);
                }
                _numScans = value;
                RaisePropertyChanged("NumScans");
            }
        }

      

        public ObservableCollection<InjectionDetail> InjectionDetails
        {
            get
            {
                if (_injectionDetails == null)
                    _injectionDetails = new ObservableCollection<InjectionDetail>();

                return _injectionDetails;
            }
            set
            {
                _injectionDetails = value;
                RaisePropertyChanged("InjectionDetails");
            }
        }

        public ObservableCollection<ScanDetail> ScanDetails
        {
            get
            {
                if (_scanDetails == null)
                    _scanDetails = new ObservableCollection<ScanDetail>();

                return _scanDetails;
            }
            set
            {
                _scanDetails = value;
                RaisePropertyChanged("ScanDetails");
            }
        }
        

    }

    public class InjectionDetail
    {
        public int InjectionDelay { get; set;}
        public virtual IDataStoreItem Radiopharmaceutical { get; set; }
        public int InjectionActivity { get; set; }

        public AdministrationRoute? AdministrationRoute { get; set; }

        public string InjectionSite { get; set; }
    }

    public class ScanDetail
    {
        public int ScanDelay { get; set; }
        public virtual IDataStoreItem Room { get; set; }
        public int ScanDuration { get; set; }

       
    }
}
