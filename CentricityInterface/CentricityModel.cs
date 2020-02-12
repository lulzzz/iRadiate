using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.Interfaces.CentricityInterface
{
    public enum ProcedureStatus { Requested,Scheduled,Registered,Started, Examined, ReportDictated, ReportWritten, ReportVerified, Deleted };

    public enum PatientConditionCode { Escort, ICU, MobilePro, OwnBed, PerformIn, Trolley, Walk, Wheelchair};

    [AttributeUsage(AttributeTargets.All)]
    public class OracleColumn : System.Attribute
    {
        private string columnName;
        private bool _nullable;
        private bool _isDate;
        private bool _isDecimal;
        public string ColumnName
        {
            get { return columnName; }
        }

        public bool IsDecimal
        {
            get { return _isDecimal; }
        }

        public bool Nullable
        {
            get { return _nullable; }
        }

        public bool IsDate
        {
            get { return _isDate; }
        }

        public OracleColumn(string colName)
        {
            columnName = colName;
            _nullable = false;
            _isDecimal = false;
            if (colName.Contains("DATE"))
                _isDate = true;
        }

        public OracleColumn(string colName,bool nullable, bool isDate, bool isDecimal)
        {
            columnName = colName;
            _nullable = nullable;
            _isDate = IsDate;
            _isDecimal = isDecimal;
        }
    }
    
    public class CentricityPatient
    {
        [OracleColumn("PAT_BIRTH_DATE",true,true,false)]
        public DateTime? BirthDate { get; set; }

        [OracleColumn("PAT_TITLE")]
        public string Title { get; set; }

        [OracleColumn("PAT_FIRST_NAME")]
        public string FirstName { get; set; }

        [OracleColumn("PAT_LAST_NAME")]
        public string LastName { get; set; }
        public Gender Gender { get; set; }

        [OracleColumn("PAT_STREET")]
        public string StreetName { get; set; }

        [OracleColumn("PAT_TOWN")]
        public string Town { get; set; }

        public string Province { get; set; }
        public string MRN { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }
        public PatientConditionCode PatientConditionCode { get; set; }
        public PatientTransportType PatientTransportType { get; set; }
        
    }
       
    public class CentricityProcedure 
    {
        [OracleColumn("PATIENT_DOMAIN_ID")]
        public string PatientMRN { get; set; }

        public DateTime AppiontmentTime { get; set; }
        public string ProcedureKey { get; set; }
        public string ProcedureCode { get; set; }
        public string ProcedureName { get; set; }
        public string ClinicalInfo { get; set; }
        public string RequestRemark { get; set; }
        public string PatientHistory { get; set; }
        public string ReferralKey { get; set; }
        public string ReferrerShortName { get; set; }
        public DateTime? RegistrationArrival { get; set; }
        public ProcedureStatus ProcedureStatus { get; set; }
        public PatientConditionCode PatientConditionCode { get; set; }

        public DateTime? CompletionTime { get; set; }

        public string ProcedureRemark { get; set; }

        public string PatientRisks { get; set; }
    }

    public class CentricityLocation 
    {
        [OracleColumn("PAT_CURRENT_LOCATION_CODE")]
        public string Code { get; set; }

        [OracleColumn("PAT_CURRENT_LOCATION_NAME")]
        public string Name { get; set; }
    }

    public class CentricityReferral
    {
        [OracleColumn("REF_SOURCE_KEY",false,false,true)]
        public string ReferralSourceKey { get; set; }

        [OracleColumn("REF_SOURCE_CODE")]
        public string ReferralSourceCode { get;set;}

        [OracleColumn("REF_SOURCE_FOREIGN_ID")]
        public string ReferralSourceForeignID { get; set; }

        [OracleColumn("REF_SOURCE_NAME")]
        public string ReferralSourceName { get; set; }

        [OracleColumn("REF_SOURCE_POSTAL_ADDR")]
        public string ReferralSourcePostalAddress { get; set; }

        [OracleColumn("REF_SOURCE_POSTAL_CODE")]
        public string ReferralSourcePostCode { get; set; }

        [OracleColumn("REF_SOURCE_POSTAL_TOWN")]
        public string ReferralSourceTown { get; set; }

        [OracleColumn("REF_SOURCE_TEL")]
        public string ReferralSourceTelephoneNumber { get; set; }

        [OracleColumn("REF_SOURCE_FAX")]
        public string ReferralSourceFaxNumber { get; set; }

        public string RequestDetails { get; set; }

        [OracleColumn("REQUEST_DATE")]
        public DateTime? RequestDate { get; set; }

        [OracleColumn("REQUEST_EVENT_DATE")]
        public DateTime? RequestEventDate { get; set; }

        public bool? RequestApproved { get; set; }

        [OracleColumn("REQUEST_APPROVAL_DATE")]
        public DateTime? RequestApprovalDate { get; set; }

        [OracleColumn("RELEVANT_CLINICAL_INFO")]
        public string ClinicalInformation { get; set; }

        [OracleColumn("PROCEDURE_MED_QUESTION")]
        public string MedicalQuestion { get; set; }

        [OracleColumn("REF_SOURCE_SHORT_NAME")]
        public string RefSourceShortName { get; set; }

    }

    /// <summary>
    /// Represents an object from the A_US_REPORT_ALL table, 1 row is one report
    /// </summary>
    public class CentricityReport
    {

        private List<CentricityReportText> _reportTexts;

        /// <summary>
        /// j =  additional reports exist, n = otherwise
        /// </summary>
        [OracleColumn("ADDITIONAL_REPORTS_YN")]
        public string AdditionalReportsExistYN { get; set; }

        /// <summary>
        /// j = co-report exists; j = otherwise
        /// </summary>
        [OracleColumn("CO_REPORTED_YN")]
        public string CoReportedYN { get; set; }

        /// <summary>
        /// Code of the person dictating the report
        /// </summary>
        [OracleColumn("DICT_PERS_CODE")]
        public string DictPersCode { get; set; }

        /// <summary>
        /// Start date of the first completed dictation
        /// </summary>
        [OracleColumn("DICTATION_DATE",true,true,false)]
        public DateTime? DictationDate { get; set; }

        /// <summary>
        /// Key of the patient whom the report is about
        /// </summary>
        [OracleColumn("PATIENT_KEY",false,false,true)]
        public string PatientKey { get; set; }

        /// <summary>
        /// Key of the procedure
        /// </summary>
        [OracleColumn("PROCEDURE_KEY", false, false, true)]
        public string ProcedureKey { get; set; }

        /// <summary>
        /// Date of the report
        /// </summary>
        [OracleColumn("REPORT_DATE")]
        public DateTime? ReportDate { get; set; }

        /// <summary>
        /// Key of the report
        /// </summary>
        [OracleColumn("REPORT_KEY", false, false, true)]
        public string ReportKey { get; set; }

        /// <summary>
        /// Status of the report
        /// </summary>
        /// <remarks>
        /// d = report dictated
        /// s = report written
        /// g = co-read
        /// f = report verified
        /// </remarks>
        [OracleColumn("REPORT_STATUS")]
        public string ReportStatus { get; set;}

        /// <summary>
        /// Type of report
        /// </summary>
        /// <remarks>
        /// main = main report
        /// additional = additional report
        /// </remarks>
        [OracleColumn("REPORT_TYPE")]
        public string ReportType { get; set; }

        /// <summary>
        /// Date of the verification of the report
        /// </summary>
        [OracleColumn("REPORT_VERIFICATION_DATE")]
        public DateTime? VerificationDate { get; set; }

        /// <summary>
        /// Code of the reporting doctor/consultant
        /// </summary>
        [OracleColumn("REPORTING_DOCTOR")]
        public string ReportingDoctorCode { get; set; }

        /// <summary>
        /// Code of the verifier
        /// </summary>
        [OracleColumn("VERIFIER_CODE")]
        public string VerifierCode { get; set; }

        /// <summary>
        /// Workplace the procedure was performed at
        /// </summary>
        [OracleColumn("WORKPLACE_CODE")]
        public string WorkplaceCode { get; set; }

        /// <summary>
        /// Key of the workplace
        /// </summary>
        [OracleColumn("WORKPLACE_KEY")]
        public string WorkplaceKey { get; set; }

        /// <summary>
        /// The ReportTexts which belong to this 
        /// </summary>
        public List<CentricityReportText> ReportTexts
        {
            get
            {
                if (_reportTexts == null)
                {
                    _reportTexts = new List<CentricityReportText>();
                }

                return _reportTexts;
            }
            set
            {
                _reportTexts = value;
            }

        }

    }

    public class CentricityReportText
    {
        /// <summary>
        /// The key of the report to which this text belongs
        /// </summary>
        public string ReportKey { get; set; }

        /// <summary>
        /// Report text in RTF format
        /// </summary>
        public string Text { get; set;}

        /// <summary>
        /// The sequence number for this report
        /// </summary>
        public int Sequence { get; set; }
    }
   
    public class CentricityStaffMember
    {
        /// <summary>
        /// Date of birth of the staff member
        /// </summary>
        [OracleColumn("STAFF_MEMB_BIRTH_DATE")]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Code of the staff member
        /// </summary>
        [OracleColumn("STAFF_MEMB_CODE")]
        public string Code { get; set; }

        /// <summary>
        /// Indicates whether the staff member is a consultant
        /// </summary>
        /// <remarks>
        /// j = yes
        /// n = no
        /// </remarks>
        [OracleColumn("STAFF_MEMB_CONSULTANT_YN")]
        public string ConsultantYN { get; set; }

        /// <summary>
        /// Duty of the staff member
        /// </summary>
        [OracleColumn("STAFF_MEMB_DUTY")]
        public string Duty { get;set;}

        /// <summary>
        /// First name of the staff member
        /// </summary>
        [OracleColumn("STAFF_MEMB_FIRST_NAME")]
        public string FirstName { get; set; }

        /// <summary>
        /// Unequivocal key of the staff member
        /// </summary>
        [OracleColumn("STAFF_MEMB_KEY",false,false,true)]
        public string Key { get; set; }

        /// <summary>
        /// Last name of the staff membera
        /// </summary>
        [OracleColumn("STAFF_LAST_NAME")]
        public string LastName { get; set; }

        
    }
}
