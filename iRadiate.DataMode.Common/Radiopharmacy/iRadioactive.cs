using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iRadiate.DataModel.Radiopharmacy
{
    public interface IRadioactive
    {
        Isotope Isotope
        {
            get;
           
        }

        Double CalibrationActivity
        {
            get;
            set;
        }

        DateTime CalibrationDate
        {
            get;
            set;
        }

        Double CurrentActivity
        {
            get;
         
        }
    }
}
