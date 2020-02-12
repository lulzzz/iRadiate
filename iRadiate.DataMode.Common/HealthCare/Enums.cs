using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iRadiate.DataModel.HealthCare
{
    
        public enum PregnancyStatus
        {
            NotPregnant,
            PossiblyPregnant,
            DefinitelyPregnant,
            Unkown
        }

        public enum SmokingStatus
        {
            Yes, No, Unkown
        }

        public enum PatientTransportType
        {
            Ambulatory,
            Wheelchair,
            Bed,
            Unkown
        }
    
}
