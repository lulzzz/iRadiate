using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;


namespace iRadiate.Desktop.Common.ViewModel
{
    [PreferredView("iRadiate.Desktop.Common.View.StandardNonFiniteTaskView", "iRadiate.Desktop.Common")]
    public class ArrivalTaskViewModel : BaseTaskViewModel
    {
        public ArrivalTaskViewModel()
            : base()
        {

        }

        public ArrivalTaskViewModel(DataStoreItem Item)
            : base(Item)
        {

        }

        public override string Name
        {
            get
            {
                return "Arrival";
            }
        }
        
        public override bool Completed
        {
            get
            {
                return base.Completed;
            }

            set
            {
                (Item as ArrivalTask).Completed = value;
                if(CompletionTime == new DateTime())
                {
                    CompletionTime = DateTime.Now;

                }
                if(User == null)
                {
                    User = Platform.CurrentUser;
                }
                RaisePropertyChanged("Completed");
            }
        }
    }
}
