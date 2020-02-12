using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{
    [Obsolete]
    public class WorkflowViewModel : DataStoreItemViewModel
    {
        public WorkflowViewModel()
            : base()
        {

        }

        public WorkflowViewModel(DataStoreItem Item)
            : base(Item)
        {

        }

        public string Name
        {
            get
            {
                return ((Workflow)Item).Name;
            }
            set
            {
                ((Workflow)Item).Name = value;
                RaisePropertyChanged("Name");
            }
        }
    }
}
