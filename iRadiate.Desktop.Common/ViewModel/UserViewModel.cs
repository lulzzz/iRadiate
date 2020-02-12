using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.Desktop.Common.ViewModel
{
    [Obsolete]
    public class UserViewModel : DataStoreItemViewModel
    {
        

        public UserViewModel()
        {

        }

        public UserViewModel(User u):base(u)
        {
            SetItem((DataStoreItem)u);
        }

        public string Password
        {
            get
            {
                return ((User)Item).Password;
            }
            set
            {
                ((User)Item).Password = value;
                RaisePropertyChanged("Password");
            }
        }
        public string Surname
        {
            get { return ((User)Item).Surname; }
            set
            {
                lock (locker)
                {
                    ((User)Item).Surname = value;
                    RaisePropertyChanged("Surname");
                }
                
            }
        }

        public string GivenNames
        {
            get { return ((User)Item).GivenNames; }
            set 
            {
                lock (locker)
                {
                    ((User)Item).GivenNames = value;

                    RaisePropertyChanged("GivenNames");
                }
                
            }
        }

        public DateTime DateOfBirth
        {
            get
            {
                return ((User)Item).DateOfBirth;
            }
            set
            {
                lock (locker)
                {
                    ((User)Item).DateOfBirth = value;
                    RaisePropertyChanged("DateOfBirth");
                }
            }
        }

        public string LoginName
        {
            get
            {
                return ((User)Item).LoginName;
            }
            set
            {
                lock (locker)
                {
                    ((User)Item).LoginName = value;
                    RaisePropertyChanged("LoginName");
                }
            }

        }

        public string FullName
        {
            get
            {
                return ((User)Item).FullName;
            }
        }

        public string PinNumber
        {
            get
            {
                return ((User)Item).PinNumber;
            }
            set
            {
                lock (locker)
                {
                    ((User)Item).PinNumber = value;
                    RaisePropertyChanged("PinNumber");
                }
            }
        }

        public bool Active
        {
            get
            {
                return !((User)Item).Deleted;
            }
            set
            {
                lock (locker)
                {
                    if (value)
                    {
                        ((User)Item).Deleted = false;
                    }
                    else
                    {
                        ((User)Item).Deleted = true;
                    }
                    
                    RaisePropertyChanged("Active");
                }
            }
        }
    }
}
