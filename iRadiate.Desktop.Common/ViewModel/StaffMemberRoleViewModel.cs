using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{
    [Obsolete]
    public class StaffMemberRoleViewModel : DataStoreItemViewModel
    {
        private AsyncObservableCollection<StaffMemberRoleViewModel> _childRoles;
        public string Name
        {
            get
            {
                return ((StaffMemberRole)Item).Name;

            }
            set
            {
                ((StaffMemberRole)Item).Name = value;
                RaisePropertyChanged("Name");
            }
        }

        public StaffMemberRoleViewModel()
            : base()
        {

        }

        public StaffMemberRoleViewModel(DataStoreItem item)
            : base(item)
        {
            
        }
        public override void SetItem(IDataStoreItem Item)
        {
            base.SetItem(Item);
            
        }

        public override void NonUIThreadInitialize()
        {
            base.NonUIThreadInitialize();
            foreach (StaffMemberRole r in ((StaffMemberRole)Item).ChildRoles)
            {
                ChildRoles.Add(new StaffMemberRoleViewModel(r));
            }
        }
        public StaffMemberRole ParentRole
        {
            get
            {
                return ((StaffMemberRole)Item).ParentRole;
            }
            set
            {
                ((StaffMemberRole)Item).ParentRole = value;
                RaisePropertyChanged("ParentRole");
            }
        }

        public AsyncObservableCollection<StaffMemberRoleViewModel> ChildRoles
        {
            get
            {
                if (_childRoles == null)
                {
                    _childRoles = new AsyncObservableCollection<StaffMemberRoleViewModel>();
                    _childRoles._synchronizationContext = DesktopApplication.SynchronizationContext;
                }
                return _childRoles;
            }
            set
            {
                _childRoles = value;
                RaisePropertyChanged("ChildRoles");
            }
        }

    }
}
