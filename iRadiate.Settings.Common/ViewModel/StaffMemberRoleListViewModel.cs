using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel;
using iRadiate.DataModel.NucMed;

using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace iRadiate.Settings.Common.ViewModel
{
    [PreferredView("iRadiate.Settings.Common.View.StaffMemberRoleListView","iRadiate.Settings.Common")]
    public class StaffMemberRoleListViewModel : ViewModelBase
    {
        private AsyncObservableCollection<IDataStoreItem> _staffMemberRoles;
      
        private IDataStoreItem _selectedStaffMemberRole;

        public StaffMemberRoleListViewModel()
        {
            
            StaffMemberRoles = DesktopApplication.Librarian.GetItems(typeof(StaffMemberRole), new List<RetrievalCriteria>());
            AddNewRoleCommand = new RelayCommand(AddNewRole);
        }

        

        public AsyncObservableCollection<IDataStoreItem> StaffMemberRoles
        {
            get
            {
               
                return _staffMemberRoles;
            }
            set
            {
                _staffMemberRoles = value;
                RaisePropertyChanged("StaffMemberRoles");
            }
        }

        public IDataStoreItem SelectedStaffMemberRole
        {
            get { return _selectedStaffMemberRole; }
            set 
            { 
                _selectedStaffMemberRole = value;
                RaisePropertyChanged("SelectedStaffMemberRole");
            }
        }

        public RelayCommand AddNewRoleCommand
        {
            get;
            private set;
        }

        private void AddNewRole()
        {
            StaffMemberRole r = new StaffMemberRole();
            DataStoreItemViewModel d = new DataStoreItemViewModel((StaffMemberRole)r);
            StaffMemberRoles.Add(r);
            DesktopApplication.MakeModalDocument(d,DesktopApplication.DocumentMode.New);
        }

       

       
        
    }
}
