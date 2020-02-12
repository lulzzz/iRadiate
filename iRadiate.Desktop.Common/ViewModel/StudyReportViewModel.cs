using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

using GalaSoft.MvvmLight.Command;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{

    [Obsolete]
    public class StudyReportViewModel : DataStoreItemViewModel
    {
        private FlowDocument _doc;

        public StudyReportViewModel():base()
        {
            
        }

        public StudyReportViewModel(DataStoreItem item)
            : base(item)
        {
            
            
        }
        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            
        }
      
        public Doctor ReportingDoctor
        {
            get
            {
                return ((StudyReport)Item).ReportingDoctor;
            }
        }

        public Doctor VerifyingDoctor
        {
            get
            {
                return ((StudyReport)Item).VerifyingDoctor;
            }
        }

        public Doctor DictatingDoctor
        {
            get
            {
                return ((StudyReport)Item).DictatingDoctor;
            }
        }

        public DateTime VerificationDate
        {
            get
            {
                return ((StudyReport)Item).VerificationDate;
            }
        }

        public string StudyName
        {
            get
            {
                return ((StudyReport)Item).Studies.First().Name;
            }
        }

        public DateTime StudyDate
        {
            get
            {
                return ((StudyReport)Item).Studies.Where(x => x.Deleted == false).OrderBy(y=>y.Date).First().Date;
            }
        }

        public Patient Patient
        {
            get
            {
                return ((StudyReport)Item).Studies.First().Patient;
            }
        }

        public FlowDocument ReportDocument
        {
            get
            {
                if(((StudyReport)Item).ReportDocument == null)
                {
                    return null;
                }
                if (_doc == null)
                {
                    MemoryStream fileStream = new MemoryStream(((StudyReport)Item).ReportDocument.Data);

                    _doc = new FlowDocument();

                    TextRange textRange = new TextRange(_doc.ContentStart, _doc.ContentEnd);

                    textRange.Load(fileStream, DataFormats.Rtf);
                }

                return _doc;
               
            }
        }
    }
}
