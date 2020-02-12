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
    public class AppointmentQueryableItem:BaseQueryableDataItem
    {
        public AppointmentQueryableItem() : base()
        {

        }

        public override string Name
        {
            get
            {
                return "Appointment";
            }
        }

        public override Type DataStoreItemType
        {
            get
            {
                return typeof(Appointment);
            }
        }
    }
}
