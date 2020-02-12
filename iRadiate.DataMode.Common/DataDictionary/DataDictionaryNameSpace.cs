using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.DataDictionary
{
    public class DataDictionaryNamespace : DataStoreItem
    {
        private DataDictionaryNamespace _parentNameSpace;
        private string _name;
        private List<DataDictionaryEntry> _variables;
        private List<DataDictionaryNamespace> _nameSpaces;

        public DataDictionaryNamespace() : base()
        {

        }

        public virtual DataDictionaryNamespace ParentNamespace
        {
            get { return _parentNameSpace; }
            set { _parentNameSpace = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string FullName
        {
            get
            {
                if (ParentNamespace == null)
                    return Name;
                return ParentNamespace.FullName + "." + Name;
            }
        }

        public virtual List<DataDictionaryEntry> Entries
        {
            get
            {
                if (_variables == null)
                {
                    _variables = new List<DataDictionaryEntry>();
                }
                return _variables;
            }
            set
            {
                _variables = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(DataDictionaryNamespace);
            }
        }

        public List<DataDictionaryNamespace> Namespaces
        {
            get
            {
                if (_nameSpaces == null)
                {
                    _nameSpaces = new List<DataDictionaryNamespace>();
                }
                return _nameSpaces;
            }
            set
            {
                _nameSpaces = value;
            }

        }
        public IList Elements
        {
            get
            {
                return new CompositeCollection()
                {
                    new CollectionContainer() {Collection = Namespaces },
                    new CollectionContainer() {Collection = Entries }
                };
            }
        }

    }
}
