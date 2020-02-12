using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{
    
    public class ScanTaskViewModel : BaseFiniteTaskViewModel
    {
        public ScanTaskViewModel()
            : base()
        {

        }

        public ScanTaskViewModel(DataStoreItem item)
            : base(item)
        {

        }

        #region publicProperties
        public override string Name
        {
            get
            {
                return ((ScanTask)Item).TaskName;
            }
        }


        public override StaffMemberRole Role
        {
            get
            {
                return ((BasicTask)Item).StaffMemberRole;
            }
            set
            {
                ((BasicTask)Item).StaffMemberRole = value;
               
                RaisePropertyChanged("Role");
                if (value != null)
                {
                    if (value.Room != null)
                    {
                        Room = value.Room;
                    }
                }
                
            }
        }

        
        #endregion
    }
}
