using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NLog;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;

namespace iRadiate.Scanbag.Common.ViewModel
{
    public class ScanBagItem : Module
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string _description;
        private ScanBagSection _scanbagSection;

        public virtual string Description
        {
            get
            {
                return _description; 
            }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        public ScanBagSection ScanbagSection
        {
            get
            {
                return _scanbagSection;
            }
            set
            {
                _scanbagSection = value;
                RaisePropertyChanged("ScanbagSection");
            }
        }
    }
}
