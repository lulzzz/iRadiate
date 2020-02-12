using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CentricityQuery
{
    public class CentricityQueryViewModel : ViewModelBase
    {
        private ICentricityTable _primaryTable;
        private string _sqlString;
        #region constructor
        public CentricityQueryViewModel()
        {

        }
        #endregion

        #region publicProperties
        public ICentricityTable PrimaryTable
        {
            get { return _primaryTable; }
            set
            {
                _primaryTable = value;
                RaisePropertyChanged("PrimaryTable");
            }
        }

        public string SQLString
        {
            get { return _sqlString; }
            set { _sqlString = value; RaisePropertyChanged("SQLString"); }
        }
        #endregion

        #region privateMethods
        private void buildSQL()
        {
            SQLString = "";
            SQLString = "SELECT";
            foreach(ICentricityColumn c in PrimaryTable.Columns)
            {
                if (c.IsSelected)
                {
                    SQLString = SQLString + " " + PrimaryTable.Name + "." + c.Name + ",";
                }
            }
            SQLString.Remove(SQLString.Length - 1);
            SQLString = SQLString + " FROM " + PrimaryTable.Name;
        }
        #endregion
    }
}
