using System;
using System.Collections.Generic;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.HealthCare
{
    //DP stands for Demographic Profile
    public interface IPatientClinicalDetails
    {
       
        Ward Ward { get; set; }
       
        bool InPatient { get;  }
        DateTime? LastMenstrualDate { get; set; }
        /// <summary>
        /// Patient's heigh (in metres)
        /// </summary>
        double? PatientHeight { get; set; }
        /// <summary>
        /// Patient's weight (in kg)
        /// </summary>
        double? PatientWeight { get; set; }
        bool OutPatient { get; }
        PregnancyStatus PregnancyStatus { get; set; }
        SmokingStatus SmokingStatus { get; set; }
        PatientTransportType TransportType { get; set; }

        /// <summary>
        /// Conditions to which medical staff should be alerted (e.g., contagious condition, drug allergies, etc.)
        /// </summary>
        string MedicalAlerts { get; set;}

        /// <summary>
        /// Description of prior reaction to contrast agents, or other patient allergies or adverse reactions.
        /// </summary>
        string Allergies { get; set; }

        /// <summary>
        /// Description of patient state (comatose, disoriented, vision impaired, etc.)
        /// </summary>
        string PatientState { get; set; }
    }

    public abstract class BasePatientClinicalDetails : DataStoreItem,IPatientClinicalDetails
    {
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

        public BasePatientClinicalDetails() : base()
        {

        }

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

        [Queryable]
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

        [Queryable]
        public virtual PregnancyStatus PregnancyStatus
        {
            get
            {
                return _pregnancyStatus;
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

        [Queryable("Ambulatory, Wheelchair or Bed")]
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

        [Queryable("The ward where the patient is staying during the time of the study")]
        public virtual Ward Ward
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

        
    }
}
