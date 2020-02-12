using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.ComponentModel.Composition;


using iRadiate.Common;

namespace iRadiate.Desktop.Common
{
    [Export(typeof(AuthorityToken))]
    public class EditDataStoreItemToken : AuthorityToken
    {
        public EditDataStoreItemToken(string name, string group) : base(name, group)
        {
        }

        public override string Name
        {
            get
            {
                return "Edit";
            }
        }

        public override string ParentName
        {
            get
            {
                return "Patient.DataStoreItem";
            }
            
        }
    }

    [Export(typeof(AuthorityToken))]
    public class AddDataStoreItemToken : AuthorityToken
    {
        public AddDataStoreItemToken(string name, string group) : base(name, group)
        {
        }

        public override string Name
        {
            get
            {
                return "Add";
            }
        }

        public override string ParentName
        {
            get
            {
                return "Patient.DataStoreItem";
            }

        }
    }
}
