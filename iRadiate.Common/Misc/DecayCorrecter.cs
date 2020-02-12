using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.Common.Misc
{
    public class DecayCorrecter
    {
        /// <summary>
        /// Decays an acticity to the current date
        /// </summary>
        /// <param name="CalibrationDate">The date-time of the radiaoctive item</param>
        /// <param name="HalfLife">The half-life, given in hours</param>
        /// <returns>The current activity of the source</returns>
        public static double Decay(DateTime CalibrationDate, Double HalfLife, Double CalibrationActivity)
        {
            double returnVal = 0;
            double hours = (DateTime.Now - CalibrationDate).TotalHours;
            returnVal = CalibrationActivity * Math.Exp((Math.Log(2) / HalfLife) * hours);
            return Decay(CalibrationDate, DateTime.Now, HalfLife, CalibrationActivity);
        }

        public static double Decay(DateTime CalibrationDate, DateTime decayDate, Double HalfLife, Double CalibrationActivity)
        {
            double returnVal = 0;
            double hours = (CalibrationDate - decayDate).TotalHours;
            returnVal = CalibrationActivity * Math.Exp((Math.Log(2) / HalfLife) * hours);
            return returnVal;
        }

        
    }
}
