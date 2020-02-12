using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.DataModel.NucMed
{
    public class Study : DataStoreItem, IPatientClinicalDetails
    {
        #region privateFields
        private Patient _patient;
        private List<Appointment> _appointments;
        private bool _completed = false;
        private Study _baseStudy;
        private string _name;
        private StudyRequest _request;
        private StudyType _studyType;
        private NucMedPractice _practice;
        private StudyReport _report;
        private List<File> _files;
       
        private int _reportID;
        private int _studyTypeID;
        #endregion

        #region publicProperties
        /// <summary>
        /// Tha name of the study
        /// </summary>
        /// <remarks>
        /// This might be something like "Bone Scan" or "White Blood Cell"
        /// Once study types are implemented we can just return the name of the study type.
        /// </remarks>
        [NotMapped]
        public virtual string Name
        {
            get               
            {
                if (StudyType == null)
                {
                    return null;
                }
                return StudyType.Name;                
            }
            
        }

        [NotMapped]
        public string ShortName
        {
            get
            {
                if (StudyType == null)
                {
                    return null;
                }
                return StudyType.ShortName;     
            }
            
        }

        /// <summary>
        /// The date of the first appointment in the study
        /// </summary>
        [Queryable]
        public DateTime Date
        {
            get
            {
                if (!Appointments.Any())
                {
                    return new DateTime(1,1,1);
                }
                return Appointments.OrderBy(x => x.ScheduledArrivalTime).First().ScheduledArrivalTime;
            }
            
        }

        /// <summary>
        /// The patient having the study
        /// </summary>
        public virtual Patient Patient
        {
            get { return _patient; }
            set { _patient = value; }
        }

        /// <summary>
        /// The apppointments which form part of this study
        /// </summary>
        public virtual List<Appointment> Appointments
        {
            get
            {
                if (_appointments == null)
                {
                    _appointments =  new List<Appointment>();
                }
                
                
                    return _appointments;
                
            }
            set
            {
                _appointments = value;
            }
        }

        /// <summary>
        /// Each study has a request. This is a separarete obejct to accomodate a situation where requests are received separately before
        /// a study is booked in
        /// </summary>
        public virtual StudyRequest Request
        {
            get
            {
                
                return _request;
            }
            set
            {
                _request = value;
            }
        }

        /// <summary>
        /// Gets or sets the report for this study
        /// </summary>
        public virtual StudyReport Report
        {
            get
            {
                return _report;
            }
            set
            {
                _report = value;
            }
        }

        
        

        /// <summary>
        /// The type of stiudy this is.
        /// </summary>
        public virtual StudyType StudyType
        {
            get { return _studyType; }
            set { _studyType = value; }
        }

        
       

        public NucMedPractice Practice
        {
            get{return _practice;}
            set{_practice = value;}
        }
        
        #endregion

        #region iPatientClinicalDetailsFields
      
        private Ward _currentWard;
        
        private PregnancyStatus _pregnancyStatus;

        private SmokingStatus _smokingStatus;
        private DateTime? _lastMenstrualDate;
        private PatientTransportType _transportType;
        private double? _patientHeight;
        private double? _patientWeight;
        private string _medicalAlerts;
        private string _allergies;
        private string _patientState;
        private string _referenceNumber;
        private bool _cancelled;

        #endregion

        #region constructors
        public Study()
        {

        }

        public Study(Study StandardStudy)
        {
            _baseStudy = StandardStudy;
        }
        #endregion

        #region iPatientClinicalDetailsProperties

        

        public bool Completed
        {
            get { return _completed; }
            set { _completed = value; }
        }

        
        public virtual Ward Ward
        {
            get
            {

                if (Appointments.Any())
                {
                    return Appointments.OrderBy(x => x.ScheduledArrivalTime).First().Ward;
                }
                return null;
               
            }
            set
            {
                
                    _currentWard = value;
                
            }
        }

        public bool InPatient
        {
            get
            {
                if(Ward == null)
                {
                    return false;
                }
                else
                {
                    return true;

                }
                    
                
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

      

        public bool OutPatient
        {
            get 
            {
                    return !InPatient;
            }
        }

        public PregnancyStatus PregnancyStatus
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
        #endregion

        public override Type ConcreteType
        {
            get
            {
                return typeof(Study);
            }
        }

        /// <summary>
        /// The attachments associated with this study
        /// </summary>
        public virtual List<File> Files
        {
            get
            {
                if (_files == null)
                {
                    _files = new List<File>();
                }
                return _files;
            }
            set
            {
                _files = value;
            }
        }

        /// <summary>
        /// Gets or set an arbitrary reference number for this study
        /// </summary>
        /// <remarks>
        /// Depending on the context this might be the accession number or some other number
        /// </remarks>
        public string ReferenceNumber
        {
            get { return _referenceNumber; }
            set { _referenceNumber = value; }
        }

        /// <summary>
        /// Gets or sets whether the study has been cancelled
        /// </summary>
        public bool Cancelled
        {
            get { return _cancelled; }
            set { _cancelled = value; }
        }

        /// <summary>
        /// Gets the status of the study.
        /// </summary>
        public string Status
        {
            get
            {
                if (!Appointments.Where(x => x.Cancelled == false).Any())
                    return "No appointments booked";
                if (!Appointments.Where(x => x.Cancelled == false && x.ScheduledArrivalTime < DateTime.Now).Any())
                    return "Scheduled";
                if (Appointments.Where(x => x.Cancelled == false && !x.Completed).Any())
                    return "In Progress";
                return "Complete";
            }
        }

        public bool IsCancelled
        {
            get
            {
                if (Cancelled)
                    return true;
                if (Appointments.Where(x => x.Deleted == false && x.Cancelled == false).Any())
                    return false;

                return true;
            }
        }
        
    }










}
