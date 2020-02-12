using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.Common.Misc
{
    public enum UnitType
    {
        distance,
        volume,
        area,
        angle,
        mass,      
        density,
        time,
        speed,
        acceleration,
        force,
        frequency,
        temperature,
        energy,
        activity,
        absorbed_dose,
        dose_rate,
        exposure,
        unitless,
        percentage
    };

    public class Unit
    {
        private UnitType _unitType;
        private string _fullName;
        private string _abbreviation;

        public UnitType UnitType
        {
            get { return _unitType; }
            set { _unitType = value; }
        }

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        public string Abbreviation
        {
            get { return _abbreviation; }
            set { _abbreviation = value; }
        }
    }

    
}
