using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.Forms
{
    public class BooleanFormElement : DataFormElement
    {
        private bool _boolValue;

        public BooleanFormElement() : base()
        {

        }

        [NotMapped]
        public bool BoolValue
        {
            get { return _boolValue; }
            set { _boolValue = value; }
        }
    }
}
