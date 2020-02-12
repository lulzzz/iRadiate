using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel;
using iRadiate.DataModel.Common;

namespace iRadiate.Desktop.Common.ViewModel
{
    [PreferredView("iRadiate.Desktop.Common.View.KitDefinitionView", "iRadiate.Desktop.Common")]
    public class KitDefinitionViewModel : DataStoreItemViewModel
    {
        
        public KitDefinitionViewModel() : base()
        {

        }

        public KitDefinitionViewModel(DataStoreItem item) : base(item)
        {

        }
    }
}
