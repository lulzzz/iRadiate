using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.DataModel.NucMed
{
    public class StudyReport : DataStoreItem
    {
        private Doctor _dictatingDoctor;
        private Doctor _reportingDoctor;
        private Doctor _verifyingDoctor;
        private DateTime _dicationDate;
        private DateTime _verificationDate;
        private File _reportDocument;
        private List<Study> _studies;

        public virtual Doctor DictatingDoctor
        {
            get
            {
                return _dictatingDoctor;
            }
            set
            {
                _dictatingDoctor = value;
            }
        }

        public virtual Doctor ReportingDoctor
        {
            get
            {
                return _reportingDoctor;
            }
            set
            {
                _reportingDoctor = value;
            }
        }

        public virtual Doctor VerifyingDoctor
        {
            get
            {
                return _verifyingDoctor;
            }
            set
            {
                _verifyingDoctor = value;
            }
        }

        public DateTime DictationDate
        {
            get
            {
                return _dicationDate;
            }
            set
            {
                _dicationDate = value;
            }
        }

        public DateTime VerificationDate
        {
            get
            {
                return _verificationDate;
            }
            set
            {
                _verificationDate = value;
            }
        }

        public virtual File ReportDocument
        {
            get
            {
                return _reportDocument;
            }
            set
            {
                _reportDocument = value;
            }
        }

        public virtual List<Study> Studies
        {
            get
            {
                if (_studies == null)
                {
                    _studies = new List<Study>();
                }
                return _studies;
            }
            set
            {
                _studies = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(StudyReport);
            }
        }
    }
}
