using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Whiteboard.Common;

namespace iRadiate.Scanbag.Common.ViewModel
{
    [PreferredView("iRadiate.Desktop.Common.View.StudyReportView", "iRadiate.Desktop.Common")]
    public class ReportScanBagItem : ScanBagItem
    {
        private StudyReportViewModel rvm;

        public ReportScanBagItem(StudyReport rep)
        {
           
            rvm = new StudyReportViewModel(rep);
        }
        public override string Description
        {
            get
            {
                return "Report";
            }
            set
            {
                
            }
        }

        public StudyReportViewModel Report
        {
            get
            {
                return rvm;
            }
            set
            {
                rvm = value;
                RaisePropertyChanged("Report");
            }
        }
    }
}
