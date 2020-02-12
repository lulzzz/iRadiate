using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NLog;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;


using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace iRadiate.Settings.Common.ViewModel
{
    public class UserListViewModel : Module
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private AsyncObservableCollection<IDataStoreItem> _users;
        private IDataStoreItem _selectedUser;

        public UserListViewModel():base()
        {
            
            AddUserCommand = new RelayCommand(AddUser);
           
        }        
        public AsyncObservableCollection<IDataStoreItem> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
            }
        }
        public override void GetData()
        {
            logger.Trace("GetData() ...");
           
            List<RetrievalCriteria> criteria = new List<RetrievalCriteria>();
            Users = DesktopApplication.Librarian.GetItems(typeof(User),criteria);
            
            
            logger.Trace("GetData() ...Done");

        }

        public IDataStoreItem SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set 
            {
                _selectedUser = value;
                RaisePropertyChanged("SelectedUser");
            }
        }

        public RelayCommand AddUserCommand
        {
            get;
            private set;
        }

        private void AddUser()
        {
            User u = new User();
            DataStoreItemViewModel uvm = new DataStoreItemViewModel(u);
            Users.Add(u);
            DesktopApplication.MakeModalDocument(uvm);
        }
    }
}
