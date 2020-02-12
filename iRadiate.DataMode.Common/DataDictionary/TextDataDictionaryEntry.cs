using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace iRadiate.DataModel.DataDictionary
{
    public class TextDataDictionaryEntry :DataDictionaryEntry
    {
        private int _characterLimit;

        public TextDataDictionaryEntry() : base()
        {

        }

        public int CharacterLimit
        {
            get { return _characterLimit; }
            set { _characterLimit = value; }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(TextDataDictionaryEntry);
            }
        }
    }

    
}
