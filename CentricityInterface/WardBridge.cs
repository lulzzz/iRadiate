using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;

namespace iRadiate.Interfaces.CentricityInterface
{
    public class WardBridge : ViewModelBase
    {
        private string _foreignKey;
        private string _foreignName;
        private int _localID;
        private Ward _ward;
        private int _ID;

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

        public WardBridge()
        {
            SaveCommand = new RelayCommand(saveBridge);
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

        public int LocalID
        {
            get
            {
                return _localID;
            }
            set
            {
                _localID = value;
                RaisePropertyChanged("LocalID");
            }
        }

        public Ward Ward
        {
            get
            {
                return _ward;
            }
            set
            {
                _ward = value;
                RaisePropertyChanged("Ward");
                LocalID = value.ID;
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
                    SqlCommand insertCommand = new SqlCommand("UPDATE dbo.WardBridges SET ForeignKey = @ForeignKey, ForeignName = @ForeignName, LocalKey = @LocalKey WHERE ID = @ID", conn);
                    insertCommand.Parameters.AddWithValue("@ForeignKey", ForeignKey);
                    insertCommand.Parameters.AddWithValue("@ForeignName", ForeignName);
                    insertCommand.Parameters.AddWithValue("@LocalKey", LocalID);                    
                    insertCommand.Parameters.AddWithValue("@ID", ID);

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

        public RelayCommand SaveCommand
        {
            get;
            set;

        }

        public List<IDataStoreItem> AvailableWards
        {
            get
            {
                return DesktopApplication.Librarian.GetItems(typeof(Ward), new List<RetrievalCriteria>()).OrderBy(x=>(x as Ward).FullName).ToList();
                //return null;
            }
        }
    }
}
