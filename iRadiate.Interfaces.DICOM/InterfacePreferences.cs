using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
namespace iRadiate.Interfaces.DICOM
{
    
    public class InterfacePreferences :ViewModelBase
    {
        private bool _enabled;
        private string _aeTitle;
        private int _port;
        private int _timeoutDelay;
        private string _ipAddress;
        private string _hostName;
        public InterfacePreferences()
        {
            

        }
        
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }

        public string AETitle
        {
            get { return _aeTitle; }
            set { _aeTitle = value; RaisePropertyChanged("AETitle"); }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; RaisePropertyChanged("Port"); }
        }

        public int TimeoutDelay
        {
            get { return _timeoutDelay; }
            set { _timeoutDelay = value; RaisePropertyChanged("TimeoutDelay"); }
        }

        public string IPAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; RaisePropertyChanged("IPAddress"); }
        }

        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; RaisePropertyChanged("HostName"); }
        }
       
        
       
    }
}
