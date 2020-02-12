using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.DataDictionary
{
    /// <summary>
    /// An entry in the DataDictionary that gives a list of all defined types of data that be recorded in a form
    /// </summary>
    public  class DataDictionaryEntry : DataStoreItem, IDataDictionaryEntry
    {
        private string _name;
        private string _description;
        private string _code;
        private DataDictionaryNamespace _nameSpace;
        private bool _active;

        public DataDictionaryEntry() : base()
        {
            Active = true;
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public virtual DataDictionaryNamespace Namespace
        {
            get
            {
                
                return _nameSpace;
            }

            set
            {
                _nameSpace = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(DataDictionaryEntry);
            }
        }

        public string FullName
        {
            get
            {
                if(Namespace != null)
                {
                    return Namespace.FullName + "." + Name;
                }
                return Name;
            }
        }

        public bool Active
        {
            get
            {
                return _active;
            }

            set
            {
                _active = value;
            }
        }

        public override string ToString()
        {
            if (Active)
                return FullName + " (Active)";
            return FullName + "(Inactive)";
        }

        /// <summary>
        /// Because the properties of the instance are not available until after EF...
        /// we need this method
        /// </summary>
        public virtual void Initialize()
        {

        }

    }
}
