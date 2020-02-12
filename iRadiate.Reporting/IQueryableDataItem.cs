using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporting
{
    public interface IQueryableDataItem
    {
        event EventHandler SelectedPropertiesChanged;

        string Name { get;  }

        Type DataStoreItemType { get;  }

        ICollection<IQueryableProperty> QueryableProperties { get; set; }
    }
}
