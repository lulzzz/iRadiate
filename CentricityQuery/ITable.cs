using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentricityQuery
{
    public interface ICentricityTable
    {
        string Name { get; set; }

        List<ICentricityColumn> Columns { get; set; }

        
    }

    public class CentricityTable : ICentricityTable
    {
        private List<ICentricityColumn> _columns;
      
        private string _name;

        public List<ICentricityColumn> Columns
        {
            get
            {
                if(_columns == null)
                {
                    _columns = new List<ICentricityColumn>();
                }
                return _columns;
            }

            set
            {
                _columns = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }
    }
}
