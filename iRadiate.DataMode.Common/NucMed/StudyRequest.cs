using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.DataModel.NucMed
{
    public class StudyRequest : DataStoreItem
    {
        private Doctor _referrer;
        private DateTime _referraldate;
        private string _patientName;
        private string _patientHistory;
        private string _requestRemark;
        private string _clinicalInfo;
        private string _requestedStudy;
        private List<Study> _studies;


        public virtual Doctor Referrer
        {
            get { return _referrer; }
            set { _referrer = value; }
        }

        public DateTime ReferralDate
        {
            get { return _referraldate; }
            set { _referraldate = value; }
        }

        public string PatientName
        {
            get { return _patientName; }
            set { _patientName = value; }
        }

        public string PatientHistory
        {
            get { return _patientHistory; }
            set { _patientHistory = value; }
        }

        public string RequestRemark
        {
            get { return _requestRemark; }
            set { _requestRemark = value; }
        }

        public string ClinicalInfo 
        {
            get { return _clinicalInfo; }
            set { _clinicalInfo = value; }
        }

        public string RequestedStudy
        {
            get { return _requestedStudy; }
            set { _requestedStudy = value; }
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
    }
}
