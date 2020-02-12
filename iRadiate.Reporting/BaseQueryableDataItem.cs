using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
//using System.Threading.Task;
using iRadiate.Common;
using iRadiate.Common.Misc;
using System.Text.RegularExpressions;
using iRadiate.DataModel.Common;

namespace Reporting
{
    public abstract class BaseQueryableDataItem :IQueryableDataItem
    {
        private ICollection<IQueryableProperty> _queryableProperties;

        public event EventHandler SelectedPropertiesChanged;

        public BaseQueryableDataItem()
        {
            generateStandardProperties();
            foreach(var s in QueryableProperties)
            {
                s.IsSelectedChanged += S_IsSelectedChanged;
            }
        }

        protected void S_IsSelectedChanged(object sender, EventArgs e)
        {
           
            if (SelectedPropertiesChanged != null)
                SelectedPropertiesChanged(this, new EventArgs());
        }

        public virtual string Name
        {
            get
            {
                return "Base Queryable Data Item";
            }

           
        }

        public virtual Type DataStoreItemType
        {
            get
            {
                return typeof(DataStoreItem);
            }

            
        }

        public ICollection<IQueryableProperty> QueryableProperties
        {
            get
            {
                if (_queryableProperties == null)
                    _queryableProperties = new AsyncObservableCollection<IQueryableProperty>();
                return _queryableProperties;
            }

            set
            {
                _queryableProperties = value;
            }
        }

       

        protected void generateStandardProperties()
        {
            
            var properties = GetQueryablePropertiesRecursive(DataStoreItemType, "");
            foreach(var p in properties)
            {
                QueryableProperties.Add(p);
            }
        }

        private List<IQueryableProperty> GetQueryablePropertiesRecursive(Type itemType, string parentName)
        {
            
            string prefix = "";
            if (parentName == prefix)
            {
                prefix = "";
            }
            else
            {
                prefix = parentName  + itemType.Name + ">";

            }
           
            var result = new List<IQueryableProperty>();
            foreach (PropertyInfo pi in itemType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(IDataStoreItem).IsAssignableFrom(x.PropertyType) == false).OrderBy(y=>y.Name))
            {
                var description = Regex.Replace(pi.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
                
                if (Attribute.IsDefined(pi, typeof(iRadiate.DataModel.QueryableAttribute), false))
                {
                    var queryableAtt = (iRadiate.DataModel.QueryableAttribute)Attribute.GetCustomAttribute(pi, typeof(iRadiate.DataModel.QueryableAttribute));
                    if(queryableAtt.Description == null || queryableAtt.Description == string.Empty)
                    {

                    }
                    else
                    {
                        description = queryableAtt.Description;
                    }
                    if (pi.PropertyType.IsEnum)
                    {
                        result.Add(new StandardQueryableProperty(parentName + pi.Name, QueryablePropertyType.Enumeration, description));
                    }
                    else if (Type.GetTypeCode(pi.PropertyType) == TypeCode.String)
                    {
                        result.Add(new StandardQueryableProperty(parentName + pi.Name, QueryablePropertyType.Text, description));
                    }
                    else if (TypeChecker.IsNumericType(pi.PropertyType))
                    {
                        result.Add(new StandardQueryableProperty(parentName + pi.Name, QueryablePropertyType.Number, description));
                    }
                    else if (TypeChecker.IsDateTime(pi.PropertyType))
                    {
                        result.Add(new StandardQueryableProperty(parentName + pi.Name, QueryablePropertyType.DateTime, description));
                    }
                    else
                    {
                        result.Add(new StandardQueryableProperty(parentName + pi.Name, QueryablePropertyType.Boolean, description));
                    }
                    //else if (typeof(IDataStoreItem).IsAssignableFrom(pi.PropertyType))
                    //{
                    //    var blah = GetQueryablePropertiesRecursive(pi.PropertyType, pi.PropertyType.Name + ">");
                        
                    //    result.AddRange(blah);
                    //}
                }
            }
            foreach (PropertyInfo pi in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => typeof(IDataStoreItem).IsAssignableFrom(x.PropertyType)).OrderBy(y => y.Name))
            {
                var description = Regex.Replace(pi.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

                if (Attribute.IsDefined(pi, typeof(iRadiate.DataModel.QueryableAttribute), false))
                {
                    if (pi.PropertyType.IsEnum)
                    {
                        result.Add(new StandardQueryableProperty(parentName + pi.Name, QueryablePropertyType.Enumeration, description));
                    }
                    else if (Type.GetTypeCode(pi.PropertyType) == TypeCode.String)
                    {
                        result.Add(new StandardQueryableProperty(parentName + pi.Name, QueryablePropertyType.Text, description));
                    }
                    else if (TypeChecker.IsNumericType(pi.PropertyType))
                    {
                        result.Add(new StandardQueryableProperty(parentName + pi.Name, QueryablePropertyType.Number, description));
                    }
                    else if (TypeChecker.IsDateTime(pi.PropertyType))
                    {
                        result.Add(new StandardQueryableProperty(parentName + pi.Name, QueryablePropertyType.DateTime, description));
                    }
                    else if (typeof(IDataStoreItem).IsAssignableFrom(pi.PropertyType))
                    {
                        var blah = GetQueryablePropertiesRecursive(pi.PropertyType, parentName + pi.Name + ">");

                        result.AddRange(blah);
                    }
                }
            }
            return result;
        }
    }
}
