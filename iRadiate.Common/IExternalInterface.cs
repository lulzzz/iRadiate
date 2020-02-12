using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.Common
{
    public interface IExternalDataInterface
    {
        string Name { get; }

        void Execute(string command);

    }
}
