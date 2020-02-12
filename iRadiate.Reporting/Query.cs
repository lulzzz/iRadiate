using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporting
{
    [Serializable]
    public class Query
    {
        public Query()
        {
            QueryProperties = new List<QueryProperty>();
        }
        public void SetItem(IQueryableDataItem queryItem)
        {
            QueryProperties.Clear();
            foreach(IQueryableProperty q in queryItem.QueryableProperties.Where(x=>x.IsSelected))
            {
                QueryProperty qp = new QueryProperty();
                qp.FilterOperator = q.FilterOperator;
                qp.FilterValue = q.FilterValue;
                qp.IsFiltering = q.IsFiltering;
                qp.IsSelected = q.IsSelected;
                qp.IsReturning = q.IsReturning;
                qp.Format = q.Format;
                qp.ColumnOrder = q.ColumnOrder;
                qp.Name = q.Name;
                qp.ColumnHeader = q.ColumnHeader;
                
                QueryProperties.Add(qp);
            }
        }
        
        public string DataItem { get; set; }

        public string Code { get; set; }

        public List<QueryProperty> QueryProperties { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime SaveDate { get; set; }
    }

    public class QueryProperty
    {
        public string Name { get; set; }
        public bool IsFiltering { get; set; }
        public bool IsReturning { get; set; }
        public bool IsSelected { get; set; }
        public string FilterOperator { get; set; }
        public string FilterValue { get; set; }
        public int ColumnOrder { get; set; }
        public string Format { get; set; }
        public string ColumnHeader { get; set; }
    }
}
