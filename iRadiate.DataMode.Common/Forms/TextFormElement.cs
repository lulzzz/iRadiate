using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.Forms
{
    public class TextFormElement : DataFormElement
    {
        private string _text;

        public TextFormElement() : base()
        {

        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }

        }
    }
}
