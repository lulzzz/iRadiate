using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iRadiate.DataModel.Common
{
    /// <summary>
    /// Represents a computer at which a User is working
    /// </summary>
    public class Workstation : DataStoreItem
    {
        private string _name;

        public Workstation() : base()
        {

        }

        /// <summary>
        /// The name of the workstation
        /// </summary>
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

        public override string ToString()
        {
            return Name;
        }
    }
}
