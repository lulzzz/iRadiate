using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

namespace iRadiate.Interfaces.DICOM
{
    public class StoredDicomServer : ViewModelBase
    {
        private bool _online, _enabled;
        private int _id, _port;
        private string _ipAddress, _aeTitle;
        private bool _imageStore;
        private bool _worklist;

        public StoredDicomServer()
        {

        }

        public int ID
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged("ID"); }
        }

        public string IPAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value;  RaisePropertyChanged("IPAddress"); }
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

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; RaisePropertyChanged("Enabled"); }
        }

        public bool Online
        {
            get { return _online; }
            set { _online = value; RaisePropertyChanged("Online"); }
        }

        public bool ImageStore
        {
            get { return _imageStore; }
            set { _imageStore = value; }
        }

        public bool Worklist
        {
            get { return _worklist; }
            set { _worklist = value; }
        }
    }

}
