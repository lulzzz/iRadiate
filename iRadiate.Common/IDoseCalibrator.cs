using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Radiopharmacy;

namespace iRadiate.Common
{
    public interface IDoseCalibrator
    {
        /// <summary>
        /// Changes the dose calibrator channel to the specified isotope
        /// </summary>
        /// <param name="isotope">The isotope being measured</param>
        /// <returns>True if the isotope was changed correctly, false otherwise</returns>
        bool ChangeIsotope(Isotope isotope);

        /// <summary>
        /// Read the activity on the dose calibrtor for the current isotope
        /// </summary>
        /// <returns>The activity read by the calibrator in MBq</returns>
        /// 
        double ReadActivity(Isotope isotope);

        /// <summary>
        /// Gets the isotope the calibrator is currently set to
        /// </summary>
        /// <returns>Isotope</returns>
        Isotope GetCurrentIsotope();


        bool IsConnected { get; }

    }
}
