using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using GalaSoft.MvvmLight.Command;

namespace iRadiate.Desktop.Common.ViewModel
{
    [Obsolete]
    public class WardViewModel : DataStoreItemViewModel
    {

        public WardViewModel():base()
        {

        }

        public WardViewModel(DataStoreItem item):base(item)
        {

        }
        public virtual string Name
        {
            get
            {
                return ((Ward)Item).Name;
            }
            set
            {
                ((Ward)Item).Name = value;
                
                RaisePropertyChanged("Name");
            }
        }

        public virtual string Abbreviation
        {
            get
            {
                return ((Ward)Item).Abbreviation;
            }
            set
            {
                ((Ward)Item).Abbreviation = value;
                RaisePropertyChanged("Abbreviation");
            }
        }

        
    }
}
