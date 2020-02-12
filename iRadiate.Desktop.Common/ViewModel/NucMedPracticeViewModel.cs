using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

using GalaSoft.MvvmLight.Command;
using NLog;

namespace iRadiate.Desktop.Common.ViewModel
{
    [Obsolete]
    public class NucMedPracticeViewModel : DataStoreItemViewModel
    {
        private ObservableCollection<Room> _rooms;
        private ObservableCollection<StaffMemberRoleViewModel> _roles;
        private ObservableCollection<StudyTypeViewModel> _studyTypes;

        private StaffMemberRoleViewModel _selectedStaffMemberRole;
        private StudyTypeViewModel _selectedStudyType;

        public NucMedPracticeViewModel(DataStoreItem item)
            : base(item)
        {
            _rooms = new ObservableCollection<Room>();
            foreach (Room r in ((NucMedPractice)Item).Rooms)
            {
                _rooms.Add(r);
            }
            
            AddRoleCommand = new RelayCommand(AddRole);
            AddStudyTypeCommand = new RelayCommand(AddStudyType);
        }

        public NucMedPracticeViewModel():base()
        {
            AddRoleCommand = new RelayCommand(AddRole);
            AddStudyTypeCommand = new RelayCommand(AddStudyType);
        }

        public override void SetItem(IDataStoreItem item)
        {
            _item = item;
            _rooms = new ObservableCollection<Room>();
            _roles = new ObservableCollection<StaffMemberRoleViewModel>();
            foreach (Room r in ((NucMedPractice)Item).Rooms)
            {
                _rooms.Add(r);
            }
            foreach (StaffMemberRole m in ((NucMedPractice)Item).Roles.Where(x=>x.ParentRole == null).ToList())
            {
                _roles.Add(new StaffMemberRoleViewModel(m));
            }
            _studyTypes = new ObservableCollection<StudyTypeViewModel>();
            foreach (StudyType st in ((NucMedPractice)Item).StudyTypes)
            {
                StudyTypeViewModel stvm = new StudyTypeViewModel();
                stvm.SetItem(st);
                _studyTypes.Add(stvm);
            }
        }
        
        public string Name
        {
            get
            {
                return ((NucMedPractice)Item).Name;
            }
            set
            {
                ((NucMedPractice)Item).Name = value;
                RaisePropertyChanged("Name");
            }
        }

        public ObservableCollection<Room> Rooms
        {
            get
            {
                if (_rooms == null)
                {
                    _rooms = new ObservableCollection<Room>();
                }
                return _rooms;
            }
            set { _rooms = value; }
        }

        public ObservableCollection<StaffMemberRoleViewModel> Roles
        {
            get
            {
                if (_roles == null)
                {
                    _roles = new ObservableCollection<StaffMemberRoleViewModel>();
                }
                return _roles;
            }
            set
            {
                _roles = value;
                RaisePropertyChanged("Roles");
            }
        }

        public ObservableCollection<StudyTypeViewModel> StudyTypes
        {
            get
            {
                if (_studyTypes == null)
                {
                    _studyTypes = new ObservableCollection<StudyTypeViewModel>();
                }
                return _studyTypes;
            }
            set
            {
                _studyTypes = value;
                RaisePropertyChanged("StudyTypes");
            }
        }

        public StaffMemberRoleViewModel SelectedStaffMemberRole
        {
            get { return _selectedStaffMemberRole; }
            set 
            { 
                _selectedStaffMemberRole = value;
                RaisePropertyChanged("SelectedStaffMemberRole");
            }
        }

        public StudyTypeViewModel SelectedStudyType
        {
            get
            {
                return _selectedStudyType;
            }
            set
            {
                _selectedStudyType = value;
                RaisePropertyChanged("SelectedStudyType");
            }
        }



        private void AddRole()
        {
            StaffMemberRole r = new StaffMemberRole();
            r.Name = "Enter Name";
            StaffMemberRoleViewModel s = new StaffMemberRoleViewModel();
            s.SetItem(r);
            if (_selectedStaffMemberRole != null)
            {
                r.ParentRole = (StaffMemberRole)_selectedStaffMemberRole.Item;
                _selectedStaffMemberRole.ChildRoles.Add(s);
                logger.Trace("Parent Role set to " + r.ParentRole.Name);
            }
            r.Practice = (NucMedPractice)this.Item;
           
            SelectedStaffMemberRole = s;
            
            RaisePropertyChanged("Roles");
        }

        private void AddStudyType()
        {
            StudyType st = new StudyType();
            st.Name = "Enter Name";
            st.NucMedPractice = (NucMedPractice)this.Item;
            StudyTypeViewModel stvm = new StudyTypeViewModel();
            stvm.SetItem(st);
            SelectedStudyType = stvm;
            StudyTypes.Add(stvm);
        }

        public RelayCommand AddRoleCommand
        {
            get;
            private set;
        }

        public RelayCommand AddStudyTypeCommand
        {
            get;
            private set;
        }
    }
}
