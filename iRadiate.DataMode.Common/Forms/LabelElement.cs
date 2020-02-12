using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.Forms
{
    public class LabelFormElement : FormElement
    {
        private string _labelText;

        public LabelFormElement() : base()
        {

        }

        public string LabelText
        {
            get { return _labelText; }
            set { _labelText = value; }
        }
    }
}
