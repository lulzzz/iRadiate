using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRadiate.DataModel.Common;

using GalaSoft.MvvmLight;

namespace Reporting
{
    public class StandardQueryableProperty : ViewModelBase, IQueryableProperty
    {
        protected string _name;
        protected QueryablePropertyType _propertyType;
        protected string _description;
        private string _filterOperator;
        private string _filterValue;
        private bool _isFiltering;
        private bool _isReturning;
        private bool _isSelected;
        private string _format = "";
        private int _columnOrder;
        private string _columnHeader;

        public StandardQueryableProperty()
        {
            
        }
        public StandardQueryableProperty(string name, QueryablePropertyType type, string description)
        {
            _name = name;
            _propertyType = type;
            _description = description;
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string FilterOperator
        {
            get
            {
                return _filterOperator;
            }

            set
            {
                _filterOperator = value;
                if(value == "")
                {
                    FilterValue = "";
                }
                else
                {
                    if (_propertyType == QueryablePropertyType.DateTime)
                    {
                        if (_filterOperator == "Between")
                            FilterValue = "dd/mm/yyyy AND dd/mm/yyyy";
                        else if (_filterOperator == "On" || _filterOperator == "Before" || _filterOperator == "After")
                            FilterValue = "dd/mm/yyyy";
                        else
                            FilterValue = "";
                    }
                }
                RaisePropertyChanged("FilterOperator");
                
                    
                
            }
        }

        public List<string> FilterOperators
        {
            get
            {
                var operators = new List<string>();
                operators.Add("");
                if (PropertyType == QueryablePropertyType.Text)
                {
                    operators.Add("Contains");
                    operators.Add("Equals");

                }
                else if (PropertyType == QueryablePropertyType.Number)
                {
                    operators.Add("<=");
                    operators.Add("=");
                    operators.Add(">=");
                    operators.Add(">");
                    operators.Add("<");
                }
                else if (PropertyType == QueryablePropertyType.DateTime)
                {
                    operators.Add("After");
                    operators.Add("Before");
                    operators.Add("On");
                    operators.Add("Between");
                    operators.Add("Today");
                    operators.Add("Yesterday");
                    operators.Add("This Week");
                    operators.Add("Last Week");
                    operators.Add("This Month");
                    operators.Add("Last Month");
                    operators.Add("This Year");
                    operators.Add("Last Year");
                }
                else if (PropertyType == QueryablePropertyType.Enumeration)
                {
                    operators.Add("=");
                }
                else
                    operators.Add("=");

                return operators;
            }

           
        }

        public string FilterValue
        {
            get
            {
                return _filterValue;
            }

            set
            {
                _filterValue = value;
                RaisePropertyChanged("FilterValue");
            }
        }

        public bool IsFiltering
        {
            get
            {
                return _isFiltering;
            }

            set
            {
                _isFiltering = value;
                if (value == false)
                {
                    FilterOperator = "";
                    FilterValue = "";
                }
                else
                {
                    if(PropertyType == QueryablePropertyType.DateTime)
                    {
                        FilterOperator = "Today";
                        FilterValue = "";
                    }
                    else if(PropertyType == QueryablePropertyType.Number)
                    {
                        FilterOperator = "=";
                        FilterValue = "";
                    }
                    else if(PropertyType == QueryablePropertyType.Enumeration)
                    {
                        FilterOperator = "=";
                        FilterValue = "";
                    }
                    else
                    {
                        FilterOperator = "=";
                        FilterValue = "True";
                    }
                }
                RaisePropertyChanged("IsFiltering");
            }
        }

        public bool IsReturning
        {
            get
            {
                return _isReturning;
            }

            set
            {
                _isReturning = value;
                RaisePropertyChanged("IsReturning");
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
                if (value)
                    IsReturning = true;
                OnIsSelectedChanged();
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public QueryablePropertyType PropertyType
        {
            get
            {
                return _propertyType;
            }
        }

        public string Format
        {
            get
            {
                return _format;
            }

            set
            {
                _format = value;
                RaisePropertyChanged("Format");
            }
        }

        public int ColumnOrder
        {
            get
            {
                return _columnOrder;
            }

            set
            {
                _columnOrder = value;
                RaisePropertyChanged("ColumnOrder");
            }
        }

        public string ColumnHeader
        {
            get
            {
                return _columnHeader;
            }

            set
            {
                _columnHeader = value;
                RaisePropertyChanged("ColumnHeader");
            }
        }

        public event EventHandler IsSelectedChanged;

        public bool FilterItem(IDataStoreItem item)
        {
            string val = GetPropertyValue((IDataStoreItem)item).ToString();

            if (PropertyType == QueryablePropertyType.Text)
            {
                if (FilterOperator == "Contains")
                {

                    if (val.ToLower().Contains(FilterValue.ToLower()))
                        return true;
                    else
                        return false;
                }
                else if (FilterOperator == "Equals")
                {
                    if (val.ToLower() == FilterValue.ToLower())
                        return true;
                    else
                        return false;
                }
                else
                    return true;

            }
            else if (PropertyType == QueryablePropertyType.Number)
            {
                double valNum = Convert.ToDouble(val);
                double filterValNum = 0;
                if (FilterValue == String.Empty)
                {

                }
                else
                {
                    filterValNum = Convert.ToDouble(FilterValue.Trim());
                }
               
                
                if(FilterOperator == "<")
                {
                    if (valNum < filterValNum)
                        return true;
                    else
                        return false;
                }
                else if (FilterOperator == ">")
                {
                    if (valNum > filterValNum)
                        return true;
                    else
                        return false;
                }
                else if (FilterOperator == "=")
                {
                    if (valNum == filterValNum)
                        return true;
                    else
                        return false;
                }
                else if (FilterOperator == "<=")
                {
                    if (valNum <= filterValNum)
                        return true;
                    else
                        return false;
                }
                else if (FilterOperator == ">=")
                {
                    if (valNum >= filterValNum)
                        return true;
                    else
                        return false;
                }
            }
            else if(PropertyType == QueryablePropertyType.DateTime)
            {
                DateTime valDate = Convert.ToDateTime(val).Date;
                if (FilterOperator == "Between")
                {
                    var s = FilterValue.Split(' ');
                    DateTime startDate = Convert.ToDateTime(s[0]);
                    DateTime endDate = Convert.ToDateTime(s[2]);
                    if (valDate >= startDate && valDate < endDate)
                        return true;
                    else
                        return false;
                }
                DateTime filterDate = new DateTime();
                
                if (DateTime.TryParse(FilterValue, out filterDate))
                {
                    
                }
                

                if (FilterOperator == "On")
                {
                    if (filterDate == valDate)
                        return true;
                    else
                        return false;
                }
                else if(FilterOperator == "After")
                {
                    if (valDate > filterDate)
                        return true;
                    else
                        return false;
                }
                else if (FilterOperator == "Before")
                {
                    if (valDate < filterDate)
                        return true;
                    else
                        return false;
                }
                else if(FilterOperator == "Today")
                {
                    if (valDate >= DateTime.Today && valDate < DateTime.Today.AddDays(1))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FilterOperator == "Yesterday")
                {
                    if (valDate >= DateTime.Today.AddDays(-1) && valDate < DateTime.Today)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FilterOperator == "This Week")
                {
                    int diff = (Convert.ToInt16(DateTime.Today.DayOfWeek) - 1)*-1;
                    if (valDate >= DateTime.Today.AddDays(diff) && valDate < DateTime.Today.AddDays(7 + diff))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FilterOperator == "Last Week")
                {
                    int diff = (Convert.ToInt16(DateTime.Today.DayOfWeek) - 1) * -1;
                    if (valDate >= DateTime.Today.AddDays(diff - 7) && valDate < DateTime.Today.AddDays(7 + diff - 7))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FilterOperator == "This Month")
                {
                    int month = DateTime.Today.Month;
                    int year = DateTime.Today.Year;
                    if (valDate >= new DateTime(year,month,1) && valDate < new DateTime(year, month + 1, 1))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FilterOperator == "Last Month")
                {
                    int month = DateTime.Today.Month;
                    int year = DateTime.Today.Year;
                    if (valDate >= new DateTime(year, month-1, 1) && valDate < new DateTime(year, month , 1))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FilterOperator == "This Year")
                {
                    int month = DateTime.Today.Month;
                    int year = DateTime.Today.Year;
                    if (valDate >= new DateTime(year, 1, 1) && valDate < new DateTime(year + 1, 1, 1))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FilterOperator == "Last Year")
                {
                    int month = DateTime.Today.Month;
                    int year = DateTime.Today.Year;
                    if (valDate >= new DateTime(year-1, 1, 1) && valDate < new DateTime(year, 1, 1))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            else if(PropertyType == QueryablePropertyType.Enumeration)
            {
                var s = FilterValue.ToString();
                if (val == s)
                    return true;
                else
                    return false;
            }
            else if(PropertyType == QueryablePropertyType.Boolean)
            {
                var s = Convert.ToBoolean(FilterValue);
                var b = Convert.ToBoolean(val);
                if (s == b)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public virtual object GetPropertyValue(IDataStoreItem item)
        {
            var val = getValue(item, Name);
            if(val == null)
            {
                switch (PropertyType)
                {
                    case (QueryablePropertyType.DateTime):
                        return new DateTime();
                    case (QueryablePropertyType.Number):
                        return 0;
                    case (QueryablePropertyType.Enumeration):
                        return 0;
                    default:
                        return null;

                }
            }else
            {
                return val;
            }
            
        }

        private void OnIsSelectedChanged()
        {
          
            if (IsSelectedChanged != null)
                IsSelectedChanged(this, new EventArgs());

        }

        private object getValue(IDataStoreItem item, string property)
        {
            //System.Diagnostics.Debug.WriteLine("finding property " + property + " on " + item.ConcreteType.Name);
            Type t = item.GetType();
            if (!property.Contains(">"))
            {

                var p = t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name == property).First();
                if (p.GetValue(item) == null)
                {
                    //System.Diagnostics.Debug.WriteLine("Returning string.empty");
                    return null;
                }

                //System.Diagnostics.Debug.WriteLine("Returning " + p.GetValue(item).ToString());
                return p.GetValue(item);
            }
            else
            {
                var s = property.Split('>');
                string typeName = s[0];
                string prName = property.Remove(0, typeName.Length + 1);
                //System.Diagnostics.Debug.WriteLine("typeName = " + typeName + ", prName = " + prName);
                var p = t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name == typeName).First();

                if (p.GetValue(item) == null)
                {
                    //System.Diagnostics.Debug.WriteLine("Returning string.empty");
                    return null;
                }
                    
                var obj = p.GetValue(item);
                //System.Diagnostics.Debug.WriteLine("obj is " + (obj as IDataStoreItem).ConcreteType.Name);
                //obj will be the IDataStoreItem that matches typeName

                if (s.Length > 2)
                {
                    //System.Diagnostics.Debug.WriteLine("s.Length > 2");
                    string newString = "";
                    for (int i =1; i < s.Length; i++)
                    {
                        newString = s[i] + ">";
                    }
                    newString = newString.Remove(newString.Length - 1);
                    //System.Diagnostics.Debug.WriteLine("newString = " + newString);
                    return getValue(obj as IDataStoreItem,prName);
                }
                else
                {

                    //System.Diagnostics.Debug.WriteLine("s.Length !> 2");
                    var y = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name == prName).First();
                    if (y.GetValue(obj) == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Returning string.empty");
                        return null;
                    }

                    //System.Diagnostics.Debug.WriteLine("Returning " + y.GetValue(obj).ToString());
                    return y.GetValue(obj);
                }

            }
        }
         
    }
}
