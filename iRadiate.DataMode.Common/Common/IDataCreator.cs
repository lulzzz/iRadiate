using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.Common
{
    [Obsolete]
    public interface IDataCreator
    {
        //Since data can be created by a user or by some other process
        //lets create a generic interface
                
        String Description { get; set; }
    }
}
