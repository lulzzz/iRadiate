using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;



namespace iRadiate.DataModel.HealthCare
{
    /// <summary>
    /// Represents a nuclear medicine patient
    /// </summary>
    public partial class Patient : Person, IPatientClinicalDetails
    {
        #region privateFields

      
        private Ward _currentWard;
       
        private string _patientID;
        private Address _residentialAddress;
        private PostalAddress _postalAddress;
        private string _homePhone;
        private string _workPhone;
        private string _mobilePhone;
        private string _emailAddress;


        private List<Study> _studies;
        private string _comments;
        #endregion

        #region iPatientClinicalDetailsFields
        private bool _inPatient;
        private Ward _ward;

        private PregnancyStatus _pregnancyStatus;

        private SmokingStatus _smokingStatus;

        private PatientTransportType _transportType;

        private DateTime? _lastMenstrualDate;
        private string _allergies;
        private string _medicalAlerts;
        private double? _patientHeight;
        private string _patientState;
        private double? _patientWeight;
        #endregion

        #region constructors
        public Patient() :base()
        {

        }
        #endregion


        //I had to put the x on the end because the PatientID is a convention in entity framework that prevents ID from being an identity column.
        [Auditable]
        [Queryable]
        public string MRN
        {
            get { return _patientID; }
            set { _patientID = value; }
        }

        [Auditable]
        [Queryable]
        public string HomePhone
        {
            get { return _homePhone; }
            set { _homePhone = value; }
        }

        [Auditable]
        [Queryable]
        public string WorkPhone
        {
            get { return _workPhone; }
            set { _workPhone = value; }
        }

        [Auditable]
        [Queryable]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            set { _mobilePhone = value; }
        }

        [Auditable]
        [Queryable]
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
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
                return typeof(Patient);
            }
        }

        [Auditable]
        public string StreetNumber { get; set; }

        [Auditable]
        public string StreetName { get; set; }

        [Auditable]
        public string TownName { get; set; }

        [Auditable]
        public string ProvinceName { get; set; }

        [Auditable]
        public string Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                _comments = value;
            }
        }

        #region IPatientClinicalDetailsProperties
        public string Allergies
        {
            get
            {
                return _allergies;
            }

            set
            {
                _allergies = value;
            }
        }

        public bool InPatient
        {
            get
            {
                if (Ward == null)
                    return false;
                else
                    return true;
            }
        }

        public DateTime? LastMenstrualDate
        {
            get
            {
                return _lastMenstrualDate;
            }

            set
            {
                _lastMenstrualDate = value;
            }
        }

        public string MedicalAlerts
        {
            get
            {
                return _medicalAlerts;
            }

            set
            {
                _medicalAlerts = value;
            }
        }

        public bool OutPatient
        {
            get
            {
                if (Ward != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets or sets the patient's heigh in metres
        /// </summary>
        public double? PatientHeight
        {
            get
            {
                return _patientHeight;
            }

            set
            {
                _patientHeight = value;
            }
        }

        public string PatientState
        {
            get
            {
                return _patientState;
            }

            set
            {
                _patientState = value;
            }
        }

        public double? PatientWeight
        {
            get
            {
                return _patientWeight;
            }

            set
            {
                _patientWeight = value;
            }
        }

        public PregnancyStatus PregnancyStatus
        {
            get
            {
                if (Gender == Gender.Male)
                    return PregnancyStatus.NotPregnant;
                if (Age < 55 && Age > 12)
                    return PregnancyStatus.PossiblyPregnant;
                return PregnancyStatus.NotPregnant;
            }
            set
            {
                _pregnancyStatus = value;
            }
        }

        public SmokingStatus SmokingStatus
        {
            get
            {
                return _smokingStatus;
            }

            set
            {
                _smokingStatus = value;
            }
        }

        public PatientTransportType TransportType
        {
            get
            {
                return _transportType;
            }

            set
            {
                _transportType = value;
            }
        }

        public Ward Ward
        {
            get
            {
                return _ward;
            }

            set
            {
                _ward = value;
            }
        }

        #endregion

    }


}
