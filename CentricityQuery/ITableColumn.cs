using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

namespace CentricityQuery
{
    public interface ICentricityColumn
    {
        ICentricityTable Table { get; set; }

        string Name { get; set; }

        ColumnType ColumnType { get; set; }

        bool IsSelected { get; set; }

        bool IsFiltered { get; set; }

        void Reset();
    }

    public enum ColumnType { Alphanumeric, Numeric, Date, Code, Key }

    public class CentricityColumn : ViewModelBase, ICentricityColumn
    {
        private ColumnType _columnType;
        private bool _isFiltered;
        private bool _isSelected;
        private string _name;
        private ICentricityTable _table;

        public ColumnType ColumnType
        {
            get
            {
                return _columnType;
            }

            set
            {
                _columnType = value;
            }
        }

        public bool IsFiltered
        {
            get
            {
                return _isFiltered;
            }

            set
            {
                _isFiltered = value;
                RaisePropertyChanged("IsFiltered");
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
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

        public ICentricityTable Table
        {
            get
            {
                return _table;
            }

            set
            {
                _table = value;
            }
        }

        public void Reset()
        {
            IsSelected = false;
            IsFiltered = false;
        }
    }

}
