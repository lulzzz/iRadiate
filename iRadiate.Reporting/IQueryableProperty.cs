using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using iRadiate.DataModel.Common;

namespace Reporting
{
    public interface IQueryableProperty 
    {
        event EventHandler IsSelectedChanged;

        string Name { get;  }

        QueryablePropertyType PropertyType { get;  }

        object GetPropertyValue(IDataStoreItem item);

        bool IsSelected { get; set; }

        bool IsReturning { get; set; }

        bool IsFiltering { get; set; }

        string FilterOperator { get; set; }

        List<string> FilterOperators { get;  }

        string FilterValue { get; set; }

        bool FilterItem(IDataStoreItem item);

        string Description { get; }

        string Format { get; set; }

        int ColumnOrder { get; set; }

        string ColumnHeader { get; set; }

       
        
    }

    public enum QueryablePropertyType
    {
        Text,Number,DateTime, Enumeration, Boolean
    }
}
