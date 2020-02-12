using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using NLog;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.Radiopharmacy;

namespace iRadiate.DataModel.NucMed
{
    /// <summary>
    /// Represents an appointment by a patient as part of a study
    /// </summary>
    [PreferredView("iRadiate.Desktop.Common.View.AppointmentView", "iRadiate.Desktop.Common")]
    public class Appointment : BasePatientClinicalDetails, IPatientClinicalDetails
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region private 
        private Study _study;
        private List<BasicTask> _tasks;
        private bool _completed = false;
        private DateTime _scheduledArrivalTime;
        private DateTime _arrivalTime;
        //private List<RoomChange> _roomChanges;
        private bool _confirmed;
        private string _confirmationComment;
        private User _confirmingUser;
        private DateTime _confirmationDate;
        private string _comments;
        private DateTime _completionTime;
        private bool _patientRegistered;
        private DateTime _patientRegistrationDate;
        private string _referenceNumber;
        private bool _cancelled;
        private string _risks;
        #endregion

        #region events
        /// <summary>
        /// Fires when patient is arrived. Can happen multiple times
        /// </summary>
        public event EventHandler PatientArrived;

        /// <summary>
        /// Fires when Completed is set to True
        /// </summary>
        public event EventHandler AppointmentCompleted;

        
        protected virtual void OnPatientArrived(EventArgs e)
        {
            if (PatientArrived != null)
                PatientArrived(this, e);
        }

        protected virtual void OnAppointmentCompleted(EventArgs e)
        {
            if (AppointmentCompleted != null)
                AppointmentCompleted(this, e);
        }
        #endregion

        #region constructors
        /// <summary>
        /// Parameterless constructor - does nothing
        /// </summary>
        public Appointment():base()
        {

        }

        /// <summary>
        /// Constructor with parameter
        /// </summary>
        /// <param name="s">The study which this appointment belongs to</param>
        public Appointment(Study s):base()
        {
            Study = s;
            
        }
        #endregion

        #region publicProperties

        /// <summary>
        /// The Study which this appointment is part of
        /// </summary>        
        [Queryable]
        public virtual Study Study
        {
            get
            {
                return _study;
            }
            set
            {
                _study = value;
            }
        }

        /// <summary>
        /// The time at which the patient was scheduled to arrive
        /// </summary>
        /// <remarks>
        /// This is a property of the appointment and is not necessarily at the same time as the arrival task
        /// </remarks>
        [Auditable]
        [Queryable]
        public virtual DateTime ScheduledArrivalTime
        {
            get { return _scheduledArrivalTime; }
            set { _scheduledArrivalTime = value; }
        }

        /// <summary>
        /// The time at which this Appointment completed
        /// </summary>
        /// <remarks>
        /// In the most recent model this can be arbitrarily set and is not derived form the workflwo
        /// </remarks>
        [Auditable]
        [Queryable]
        public virtual DateTime CompletionTime
        {
        
            get
            {
                return _completionTime;
            }
            set
            {
                _completionTime = value;
            }
        }

        /// <summary>
        /// List of tasks for this appointment
        /// </summary>
        public virtual List<BasicTask> Tasks
        {
            get
            {
                if (_tasks == null)
                {
                    _tasks = new List<BasicTask>();
                    CollectionViewSource.GetDefaultView(_tasks).SortDescriptions.Add(new SortDescription("SchedulingTime", ListSortDirection.Ascending));
                }

                return _tasks;
            }
            set
            {
                _tasks = value;
            }
        }

        /// <summary>
        /// The patient who is having the appointment
        /// </summary>
        [Queryable]
        public virtual Patient Patient
        {
            get
            {
                return Study.Patient;
            }
           
        }

        /// <summary>
        /// All rooms used by tasks in this appointment
        /// </summary>
        /// <remarks>
        /// Iterates the not deleted tasks and adds all the rooms otherwise and empty list of rooms
        /// </remarks>
        public IEnumerable<Room> AppointmentRooms 
        {
            get
            {
                if (Tasks.Where(x=>x.Deleted == false).Where(j=>j.Room!=null).Select(y => y.Room).Any())
                {
                    return Tasks.Where(x => x.Deleted == false).Where(j => j.Room != null).Select(y => y.Room);
                }
                return new List<Room>();
            }
        }

        /// <summary>
        /// All the camera rooms used in the appointment
        /// </summary>
        public IEnumerable<Room> CameraRooms
        {
            get
            {
                if (AppointmentRooms == null)
                {
                    return new List<Room>();
                }
                if (AppointmentRooms.Where(x => x.CameraRoom).Any())
                {
                    return AppointmentRooms.Where(x => x.CameraRoom);
                }
                return new List<Room>();
                
            }
        }

     
        /// <summary>
        /// The task currently in progress returns null otherwise
        /// </summary>
        public BasicTask CurrentTask
        {
            get
            {
                if (Tasks.Where(x => x is BasicFiniteTask).Any() == false)
                {
                    return null;
                }

                if (Tasks.Where(y=>y is BasicFiniteTask).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == false).Any())
                {
                    return Tasks.OrderBy(y => y.CompletionTime).Where(y => y is BasicFiniteTask).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == false).First();
                }

                return null;
            }
        }

        /// <summary>
        /// The task most recently completed
        /// </summary>
        public BasicTask LastTask
        {
            get
            {
                if (Tasks.Where(x => x.Completed == true).Any())
                {
                    return Tasks.Where(x => x.Completed == true).OrderBy(y => y.CompletionTime).Last();
                }

                return null;
            }
        }

        /// <summary>
        /// The next task to be completed
        /// </summary>
        public BasicTask NextTask
        {
            get
            {
                if (Tasks.Where(x => x.Completed == false).Any())
                {
                    return Tasks.Where(x => x.Completed == false).OrderBy(y => y.ScheduledCompletionTime).First();
                }

                return null;
            }
        }

        /// <summary>
        /// The room in which the CurrenTask is taking place. Returns null otherwise
        /// </summary>
        public Room CurrentRoom
        {
            get
            {
                if (Tasks.Where(y=>y.Deleted==false).Where(x => x is BasicFiniteTask && x.Room != null).Any() == false)
                {
                    return null;
                }

                if (Tasks.Where(y => y.Deleted == false).Where(z => z is BasicFiniteTask).Where(y => y.Room != null).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == false).Any())
                {
                    return Tasks.Where(y => y.Deleted == false).Where(z => z is BasicFiniteTask).Where(y => y.Room != null).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == false).Last().Room;
                }

                return null;
            }
        }

        /// <summary>
        /// The room in which LastTask took place
        /// </summary>
        public Room LastRoom
        {
            get
            {
                if (Tasks.Where(x => x.Room != null && x.Completed == true).Any() == false)
                {
                    return null;
                }
                return Tasks.Where(x => x.Room != null && x.Completed == true).OrderBy(y => y.CompletionTime).Last().Room;
            }
        }

        /// <summary>
        /// The camera the patient is currently being scanned in
        /// </summary>
        public Room CurrentCamera
        {
            get
            {
                if (Tasks.Where(y=>y.Deleted == false).Where(x => x is ScanTask && x.Room != null).Any() == false)
                {
                    return null;
                }

                if (Tasks.Where(y => y.Deleted == false).Where(z => z is ScanTask).Where(y => y.Room != null).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == false).Any())
                {
                    return Tasks.Where(y => y.Deleted == false).Where(z => z is ScanTask).Where(y => y.Room != null).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == false).First().Room;
                }

                if (Tasks.Where(y => y.Deleted == false).Where(z => z is ScanTask).Where(y => y.Room != null).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == true).Any())
                {
                    return Tasks.Where(y => y.Deleted == false).Where(z => z is ScanTask).Where(y => y.Room != null).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == true).First().Room;
                }

                return null;
            }
        }

        /// <summary>
        /// The next camera is shceduled to be scanned on
        /// </summary>
        public Room NextCamera
        {
            get
            {
                if (Tasks.Where(x => x.Deleted == false).Where(y => y is ScanTask && y.Room != null).OrderBy(z => z.ScheduledCompletionTime).Where(j => (j as ScanTask).Commenced == false).Any() == false)
                {
                    return null;
                }
                else
                {
                    return Tasks.Where(x => x.Deleted == false).Where(y => y is ScanTask && y.Room != null).OrderBy(z => z.ScheduledCompletionTime).Where(j => (j as ScanTask).Commenced == false).First().Room;
                }
            }
        }

        /// <summary>
        /// Returns CurrentCamera or if null returns NextCamera or null otherwise
        /// </summary>
        public Room AssignedCamera
        {
            get
            {
                if(CurrentCamera != null)
                {
                    return CurrentCamera;
                }
                if(NextCamera != null)
                {
                    return NextCamera;
                }

                return null;
            }
        }

        [Obsolete]
        public StaffMemberRole CurrentAssignee
        {
            get
            {
                //Do we have any tasks that have been assigned to someone?
                if (Tasks.Where(y => y.Deleted == false).Where(x =>  x.StaffMemberRole != null).Any() == false)
                {
                    return null;
                }

                //Do we have any tasks that are currently in progress
                if (Tasks.Where(y => y.Deleted == false).Where(z => z is BasicFiniteTask).Where(y => y.StaffMemberRole != null).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == false).Any())
                {
                    return Tasks.Where(y => y.Deleted == false).Where(z => z is BasicFiniteTask).Where(y => y.StaffMemberRole != null).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == false).OrderBy(j => (j as BasicFiniteTask).CommencentTime).First().StaffMemberRole;
                }

                //Do we have any tasks that are finished?
                //if (Tasks.Where(y => y.Deleted == false).Where(z => z is ScanTask).Where(y => y.Room != null).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == true).Any())
                //{
                //    return Tasks.Where(y => y.Deleted == false).Where(z => z is BasicFiniteTask).Where(y => y.StaffMemberRole != null).Where(x => (x as BasicFiniteTask).Commenced == true && (x as BasicFiniteTask).Completed == true).OrderBy(j => (j as BasicFiniteTask).CompletionTime).First().StaffMemberRole;
                //}

                if (Tasks.Where(y => y.Deleted == false).Where(y => y.StaffMemberRole != null).Where(x => x.Completed == false).Any())
                {
                    return Tasks.Where(y => y.Deleted == false).Where(y => y.StaffMemberRole != null).Where(x => x.Completed == false).First().StaffMemberRole;
                }
                return null;
            }
        }

        /// <summary>
        /// The status of the task which was last interacted with by the user
        /// </summary>
        public string Status
        {
            get
            {
                if (Deleted)
                {
                    return "Cancelled";
                }
                if (Cancelled)
                {
                    return "Cancelled";
                }
                if (!Tasks.Where(x => x.Deleted == false).Any())
                {
                    return "No workflow assigned";
                }
                if (Completed)
                {
                    return " Complete";
                }
                if (!Commenced)
                {
                    return "Yet to commence";
                }
                if (Tasks.Where(y => y.Deleted == false && y.LastInteraction != null).Any())
                {
                    BasicTask lastTask = Tasks.Where(y => y.Deleted == false && y.LastInteraction != null).OrderBy(x => x.LastInteraction).Last();
                    return lastTask.Status;
                }
                else
                {
                    return "";
                }
                
                
                
            }
        }
        
        /// <summary>
        /// the name of the appointment
        /// </summary>
        /// <remarks>
        /// Will return the name of the Study and if it is a multi-day study it will append the Daynumber
        /// </remarks>
        [Queryable]
        public virtual string Name
        {
            get
            {
                if (Study != null)
                {
                    if (Study.Appointments.Count < 2)
                    {
                        return Study.Name;
                    }
                    else
                    {
                        DateTime d1 = Study.Appointments.OrderBy(x => x.ScheduledArrivalTime).First().ScheduledArrivalTime;
                        TimeSpan difference = ScheduledArrivalTime - d1;
                        var days = difference.TotalDays;
                        return Study.Name + " Day " + Convert.ToInt16(days).ToString();

                    }
                }
                else
                {
                    return null;
                }
                
            }
        }

        /// <summary>
        /// The number of this day within the study
        /// </summary>
        /// <remarks>
        /// the first day of the study is day 0 then day 1 etc.
        /// The numbering is always based on the number of days from the initial appointment
        /// rather than the number of appointments
        /// </remarks>
        [Queryable]
        public int DayNumber
        {
            get
            {
                DateTime d1 = Study.Appointments.OrderBy(x => x.ScheduledArrivalTime).First().ScheduledArrivalTime;
                TimeSpan difference = ScheduledArrivalTime - d1;
                int days = Convert.ToInt16(difference.TotalDays);
                return days;
            }
        }

        /// <summary>
        /// Gets whether this patient has arrived
        /// </summary>
        /// <remarks>
        /// REturns true if the tasks property containts an arrival task which has not been deleted and is completed
        /// </remarks>
        public bool HasPatientArrived
        {
            get
            {
                if(Tasks.Where(x=>x.Deleted == false && x is ArrivalTask && x.Completed == true).Any())
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets or sets an abritrary reference number for the appointment
        /// </summary>
        /// <remarks>
        /// Depending on the context this might be interpreted as the acccesion number or some other thing.
        /// </remarks>
        [Auditable]
        [Queryable]
        public string ReferenceNumber
        {
            get { return _referenceNumber; }
            set { _referenceNumber = value; }
        }

        #region Confirmation
        /// <summary>
        /// The date on which the appointment was confirmed
        /// </summary>
        [Queryable]
        public DateTime ConfirmationDate
        {
            get
            {
                return _confirmationDate;
            }
            set
            {
                _confirmationDate = value;
            }
        }

        /// <summary>
        /// Indicates whether the appointment has been confirmed with the Patient.
        /// </summary>
        [Queryable]
        public bool Confirmed
        {
            get { return _confirmed; }
            set { _confirmed = value; }
        }

        /// <summary>
        /// A comment made in relation to the confirmation of the booking
        /// </summary>
        [Queryable]
        public string ConfirmationComment
        {
            get { return _confirmationComment; }
            set { _confirmationComment = value; }
        }

        /// <summary>
        /// The User who confirmed the booking
        /// </summary>
        [Queryable]
        public virtual User ConfirmingUser
        {
            get { return _confirmingUser; }
            set { _confirmingUser = value; }
        }
        #endregion

        #region IPatientClinicalDetails

        [Queryable]
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

        [Queryable]
        public bool Completed
        {
            get 
            {
                return _completed;
            }
            set { _completed = value; }
        }
        
        public override Type ConcreteType
        {
            get
            {
                return typeof(Appointment);
            }
        }

        /// <summary>
        /// Whether the appointment has commenced or not
        /// </summary>
        /// <remarks>
        /// This value is dervied fro the workflow. Returns true if there are tasks which have had
        /// a non-null LastInteraction
        /// </remarks>
        public bool Commenced
        {
            get
            {
                if (Tasks.Where(y => y.Deleted == false && y.LastInteraction != null).Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The length of the appointment in minutes
        /// </summary>
        /// <remarks>
        /// This value is derived from the workflow. It takes the start time sa the llsat interaction of the first task.
        /// This will usually be the arrival but it may not be. Instead I should make property called ActualArrivalTime
        /// that is used to calculate this.
        /// </remarks>
        [Queryable("The Duration of the appointment in minutes")]
        public int? AppointmentDuration
        {
            get
            {
                try
               {
                    if (Tasks.Where(y => y.Deleted == false && y.LastInteraction != null).Any() == false)
                        return null;
                    BasicTask FirstTask = Tasks.Where(y => y.Deleted == false && y.LastInteraction != null).OrderBy(x => x.LastInteraction.Value).First();
                    
                    if (!Commenced)
                    {
                        return null;
                    }
                    if (Completed)
                    {
                        BasicTask LastTask = Tasks.Where(y => y.Deleted == false).OrderBy(x => x.LastInteraction).Last();
                        return Convert.ToInt32((CompletionTime - FirstTask.LastInteraction.Value).TotalMinutes);
                    }


                    return Convert.ToInt32((DateTime.Now - FirstTask.LastInteraction.Value).TotalMinutes);
                }
                catch
                {
                    
                    return null;
                }
            }
            
        }

        /// <summary>
        /// Returns the duration of the appointment in xx hours yy minutes
        /// </summary>
        public string AppointmentDurationText
        {
            get
            {
                if (AppointmentDuration == null)
                {
                    return "Not commenced";
                }
                if (AppointmentDuration.Value / 60 == 0)
                {
                    return AppointmentDuration.Value.ToString() + " minutes";
                }

                return (AppointmentDuration.Value/60).ToString() + " hours " + (AppointmentDuration.Value % 60).ToString()  + " minutes";
            }
        }

        /// <summary>
        /// The time of the nexy injectoin if there is one to come or the
        /// last injection or null
        /// </summary>
        /// <remarks>
        /// This property is used to sort appointments by injection time where there can be multiple injections
        /// </remarks>
        public DateTime? InjectionSortTime
        {
            get
            {
                if (Tasks.Where(x => x.Deleted == false && x is DoseAdministrationTask && x.Completed == false).Any())
                {
                    return Tasks.Where(x => x.Deleted == false && x is DoseAdministrationTask && x.Completed == false).OrderBy(y => y.ScheduledCompletionTime).First().ScheduledCompletionTime;
                }
                if (Tasks.Where(x => x.Deleted == false && x is DoseAdministrationTask).Any())
                {
                    return Tasks.Where(x => x.Deleted == false && x is DoseAdministrationTask).OrderBy(y => y.CompletionTime).Last().CompletionTime;
                }
                return null;
            }
        }

        /// <summary>
        /// The ScheduledCommenement of the next ScanTask or the lsat ScanTask if there
        /// are none to come
        /// </summary>
        /// <remarks>
        /// This property is used to sort appointments by scan time when appointments can have mutiple scans 
        /// </remarks>
        public DateTime? ScanSortTime
        {
            get
            {
                if (Tasks.Where(x => x.Deleted == false && x is ScanTask && ((ScanTask)x).Commenced == false).Any())
                {
                    return (Tasks.Where(x => x.Deleted == false && x is ScanTask && ((ScanTask)x).Commenced == false).OrderBy(y => ((ScanTask)y).ScheduledCommencementTime).First() as ScanTask).ScheduledCommencementTime;
                }
                if (Tasks.Where(x => x.Deleted == false && x is ScanTask).Any())
                {
                    return (Tasks.Where(x => x.Deleted == false && x is ScanTask).OrderBy(y => ((ScanTask)y).CommencentTime).Last() as ScanTask).CommencentTime;
                }
                return null;
            }
        }

       [Queryable]
        public override bool Deleted
        {
            get
            {
                //if (Study == null)
                //    return false;
                //try
                //{
                //    if (Study.Deleted)
                //    {
                //        return true;
                //    }
                //}
                //catch
                //{

                //}
                
                return base.Deleted;
            }
            set
            {
                base.Deleted = value;
               
            }
        }

        public bool PatientRegistered
        {
            get
            {
                return _patientRegistered;
            }
            set
            {
                _patientRegistered = value;
            }
        }

        [Queryable]
        public DateTime PatientRegistrationDate
        {
            get
            {
                return _patientRegistrationDate;
            }
            set
            {
                _patientRegistrationDate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Queryable]
        public bool Cancelled
        {
            get
            {
                return _cancelled;
            }
            set
            {
                _cancelled = value;
            }
        }


        #endregion

        /// <summary>
        /// A string describing infections risks from the patient (MRSA, VRE etc.)
        /// </summary>
        [Auditable]
        [Queryable]
        public string Risks
        {
            get { return _risks; }
            set { _risks = value; }
        }

        public IEnumerable<IDataStoreItem> UnitDoses
        {
            get
            {
                return Tasks.Where(x => x is DoseAdministrationTask && !x.IsCancelled & !x.Deleted).Where(y => (y as DoseAdministrationTask).UnitDose != null).ToList();
            }
        }

        public IEnumerable<IDataStoreItem> Scans
        {
            get
            {
                return Tasks.Where(x => x is ScanTask && !x.IsCancelled & !x.Deleted).ToList();
            }
        }
        #endregion

        #region publicMethods
        public void FireArrivalEvent()
        {
            OnPatientArrived(new EventArgs());
        }
        public void reschedule(DateTime aptTime)
        {
            TimeSpan diff = ScheduledArrivalTime - aptTime;
            foreach (BasicTask b in Tasks.Where(x => x.IsCancelled == false))
            {
                b.ScheduledCompletionTime = b.ScheduledCompletionTime.Add(diff);
                if(b is BasicFiniteTask)
                {
                    (b as BasicFiniteTask).ScheduledCommencementTime = (b as BasicFiniteTask).ScheduledCommencementTime.Add(diff);
                }
            }
            ScheduledArrivalTime = aptTime;
        }

        public void TransplantTasks()
        {
            logger.Debug("TransplantTasks()...");
            foreach (BasicTask b in Tasks)
            {
                if(b.ScheduledCompletionTime.Date != ScheduledArrivalTime.Date)
                {
                    
                    double days = (ScheduledArrivalTime.Date - b.ScheduledCompletionTime.Date).TotalDays;
                    logger.Debug("Task.Data != ScheduledArrivalTime, days = " + days);
                    b.ScheduledCompletionTime = b.ScheduledCompletionTime.AddDays(days);
                    logger.Debug("ScehduledCompletionTime adjusted");
                    if (b is BasicFiniteTask)
                        (b as BasicFiniteTask).ScheduledCommencementTime = (b as BasicFiniteTask).ScheduledCommencementTime.AddDays(days);
                    
                }   
            }
            logger.Debug("TransplantTasks()...Completed");
        }
        #endregion

        #region privateMethods

        #endregion

        public override string ToString()
        {
            if(Patient == null)
                return Name;

            return Patient.FullName + " " + Name;
        }

        /// <summary>
        /// returns true if the appointment is marked as cancelled or the Study is marked as cancelled
        /// </summary>
        [Queryable]
        public virtual bool IsCancelled
        {
            get
            {
                if (Cancelled)
                    return true;
                if (Study == null)
                    return true;
                if (Study.IsCancelled)
                    return true;

                return false;
            }
        }
    }

   

    
}
