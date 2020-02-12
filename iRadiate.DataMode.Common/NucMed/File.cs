using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.NucMed;

namespace iRadiate.DataModel.Common
{
    /// <summary>
    /// Represents a binary file (.pdf etc)
    /// </summary>
    public class File : DataStoreItem
    {
        private byte[] _data;
        private string _description;
        private string _extension;
        private int _studyID;
        private Study _study;

        public File() :base()
        {

        }
        public virtual byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        [Auditable]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        [Auditable]
        public string Extension
        {
            get
            {
                return _extension;
            }
            set
            {
                _extension = value;
            }
        }
 

        public virtual Study Study
        {
            get
            {
                return _study;
            }
            set
            {
                _study = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(File);
            }
        }
    }
}
