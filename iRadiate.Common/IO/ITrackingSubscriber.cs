using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.Common.IO
{
    public interface ITrackingSubscriber
    {
        event EventHandler Closing;
    }
}
