using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.NucMed;

namespace Reporting
{
    [Export(typeof(IQueryableDataItem))]
    public class DoseAdministrationTaskQueryable : BaseQueryableDataItem
    {
        public DoseAdministrationTaskQueryable() : base()
        {

        }

        public override Type DataStoreItemType
        {
            get
            {
                return typeof(DoseAdministrationTask);
            }
        }

        public override string Name
        {
            get
            {
                return "DoseAdministrationTask";
            }
        }
    }
}
