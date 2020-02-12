using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.NucMed
{
    public interface IRoomReservation
    {
        DateTime ReservationStartDate { get; }

        DateTime ReservationEndDate { get; }

        Room Room { get; }

        User User { get; }

        string Description { get; }
    }
}
