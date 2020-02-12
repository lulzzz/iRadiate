using System;
using System.Collections.Generic;
using System.Linq;
using iRadiate.Common;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.DataModel.NucMed
{
    [Obsolete("Use StandardTaskType instead")]
    public class BasicTaskType : DataStoreItem
    {
        private string _name;
        private string _completedName;
        private bool _completionOnly;
        private string _incompletedName;

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual string IncompletedName
        {
            get { return _incompletedName; }
            set { _incompletedName = value; }
        }

        public virtual string CompletedName
        {
            get { return _completedName; }
            set { _completedName = value; }
        }

        public virtual bool CompletionOnly
        {
            get { return _completionOnly; }
            set { _completionOnly = value; }
        }
    }
    
    /// <summary>
    /// This is the basic task that has a tasktype which could be either finite or completion only
    /// </summary>
    /// <remarks>
    /// All othe tasks will derive from this. I intend to derive three classes directly from here
    /// 1) A GenericNamedTask which has only a completion time where the user can create new task
    /// types at runtime by choosing a new name
    /// 2) A FiniteDurationTask which will be the base task for all finite duration tasks
    /// 3) Any specific nonFinite task with its own data such as injection, blood sample collection, etc.
    /// </remarks>
    public abstract class BasicTask : DataStoreItem, ITask, IRoomReservation
    {
        #region privateDateTimeVariables


        private DateTime _scheduledCompletionTime;
        private DateTime _completionTime;
        private DateTime _cancellationTime;
        #endregion

        #region privateBoolVariables
        
        private bool _completed = false;
        private bool _optional;
        private bool _cancelled;
        #endregion

        #region privateOtherVariables
        private StaffMemberRole _primaryRole;
        private User _primaryUser;
        private Room _completionRoom;
        
        private Appointment _appointment;
        private BasicTaskType _taskType;
        private List<BaseConstraint> _constraints;
        
        private int _sequenceNumber;
        private bool? _requestFormCorrect;
        private bool? _timeoutPerformed;
        private bool? _imageCorrect;
        #endregion

        #region events

        public event EventHandler TaskCompleted;

        public event EventHandler TaskRescheduled;

        public event EventHandler TaskRoomMoved;

        public event EventHandler TaskReassigned;
        protected virtual void OnTaskCompleted(EventArgs e)
        {
            if (TaskCompleted != null)
                TaskCompleted(this, e);
        }

        protected virtual void OnTaskRescheduled(EventArgs e)
        {
           
            if (TaskRescheduled != null)
                TaskRescheduled(this, e);
        }

        protected virtual void OnTaskRoomMoved(EventArgs e)
        {
            if (TaskRoomMoved != null)
                TaskRoomMoved(this, e);
        }

        protected virtual void OnTaskReassigned(EventArgs e)
        {
            if (TaskReassigned != null)
                TaskReassigned(this, e);
        }
        #endregion


        #region constructor
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public BasicTask():base()
        {

        }
        

        /// <summary>
        /// Constructor that assigns to appointment
        /// </summary>
        /// <param name="a"></param>
        public BasicTask(Appointment a):base()
        {
            _appointment = a;
        }
        #endregion

        #region publicMethods
        /// <summary>
        /// Returns true if tasks match
        /// </summary>
        /// <param name="otherTask"></param>
        /// <returns></returns>
        public virtual bool TaskTypesMatch(BasicTask otherTask)
        {
            return false;
        }

        /// <summary>
        /// Sets the appointment
        /// </summary>
        /// <param name="a">The appointentment which this task will belong to</param>
        public void SetAppointment(Appointment a)
        {
            _appointment = a;
        }

        /// <summary>
        /// Returns TaskName
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Appointment != null)
                return Appointment.ToString() + " " + TaskName;

            return TaskName;
        }

        /// <summary>
        /// Indicates whether this task can be completed.
        /// </summary>
        /// <returns>True if there are no constraints blocking this from completing</returns>
        public virtual bool CanComplete()
        {
            return CanComplete(DateTime.Now);
        }

        /// <summary>
        /// Indicates whether this task can be completed at a propsed time.
        /// </summary>
        /// <returns>True if there are no constraints blocking this from completing</returns>
        public virtual bool CanComplete(DateTime proposedTime)
        {
            //first do some logic checks here so we can prevent someone from completing the task
            //if the task has not been supplied with enough information
            foreach (IConstraint ic in Constraints)
            {
                if (ic.CanComplete(proposedTime) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Marks the task as completed and performs all business logic
        /// </summary>
        /// <returns>True if the business logic went through ok</returns>
        public virtual bool CompleteTask()
        {
            try
            {
                Completed = true;
                CompletionTime = DateTime.Now;
                Assignee = CurrentUser;
                return true;
            }
            catch (Exception ex)
            {
                //todo: log error here
                return false;
            }
        }
        #endregion

        #region publicProperties
        /// <summary>
        /// Gets the appointment to which this task applies
        /// </summary>
        [Queryable]
        public virtual Appointment Appointment
        {
            get { return _appointment; }
            set { _appointment = value; }
          
        }

        /// <summary>
        /// gets or sets the time at which the task was actually completed.
        /// </summary>
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
        /// Gets or sets whether the task was actually completed.
        /// </summary>
        [Auditable]
        [Queryable]
        public virtual bool Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean indicating whether this task was cancelled.
        /// </summary>
        [Auditable]
        [Queryable]
        public virtual bool Cancelled
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

        /// <summary>
        /// Gets or sets the time at which the task was scheduled for completion.
        /// </summary>
        [Auditable]
        [Queryable]
        public virtual DateTime ScheduledCompletionTime
        {
            get
            {
                return _scheduledCompletionTime;
            }
            set
            {
                _scheduledCompletionTime = value;
                OnTaskRescheduled(new EventArgs());
            }
        }

        /// <summary>
        /// Gets or sets the primary user to whom this task has assigned.
        /// </summary>
        /// <remarks>
        /// If the task is not complete, the property will get the user based on which user is currently in that role.
        /// At completion the primary users is recorded.
        /// </remarks>       
        [Queryable]
        public virtual User Assignee
        {
            get
            {
                return _primaryUser;
            }
            set
            {
                _primaryUser = value;
            }
        }

        /// <summary>
        /// Gets or sets the primary staff member role to which the task is assigned.
        /// </summary>
        public virtual StaffMemberRole StaffMemberRole
        {
            get
            {
                return _primaryRole;
            }
            set
            {
                _primaryRole = value;
            }
        }        

        /// <summary>
        /// The room in which this task is peformed
        /// </summary>
        [Queryable]
        public virtual Room Room
        {
            get { return _completionRoom; }
            set 
            {
                 _completionRoom = value;
                 
            }
                
        }
        

        /// <summary>
        /// The list of constraints on this task
        /// </summary>
        /// <remarks>
        /// This means the constraints where this task is the constrainee
        /// </remarks>
        public virtual List<BaseConstraint> Constraints
        {
            get
            {
                if (_constraints == null)
                {
                    _constraints = new List<BaseConstraint>();
                }
                return _constraints;
            }
            set
            {
                _constraints = value;
            }
        }

        #region readOnly
        /// <summary>
        /// The sequence number of this task
        /// </summary>
        /// <remarks>
        /// The sequence number is used for differentiating one scan from another.
        /// The first scan task will be have a sequence number of 1, the second 2 etc.
        /// This property works for all task types
        /// </remarks>
        [Queryable]
        public int SequenceNumber
        {
            get
            {
                int matching = 0;
                if (_appointment != null)
                {

                    foreach (BasicTask t in _appointment.Tasks.Where(y => y.Deleted == false && y.Cancelled == false).OrderBy(x => x.SequenceTime))
                    {
                        //Console.WriteLine("t.Deleted.ToString() = " + t.Deleted.ToString());
                        if (TaskTypesMatch((BasicTask)t))
                        {
                            matching++;
                            if (t.SchedulingTime == SchedulingTime)
                            {
                                if (NumMatching > 0)
                                {
                                    return matching;
                                }
                                matching--;

                                break;
                            }

                        }
                    }
                }

                return matching;


            }

        }

        /// <summary>
        /// the number of tasks in the appointment where TaskTypesMatch = true.
        /// </summary>
        public int NumMatching
        {
            get
            {
                int matching = 0;
                if (_appointment != null)
                {
                    foreach (ITask t in _appointment.Tasks.Where(y => y.Deleted == false).OrderBy(x => x.ScheduledCompletionTime).ToList())
                    {

                        if (TaskTypesMatch((BasicTask)t))
                        {
                            matching++;


                        }
                    }
                }

                return matching - 1;
            }
        }
        /// <summary>
        /// Returns either the completion time or the scheduled completion time
        /// </summary>
        public virtual DateTime ValidCompletionTime
        {
            get
            {
                if (Completed)
                {
                    return CompletionTime;
                }
                else
                {
                    return ScheduledCompletionTime;
                }
            }
        }
        public virtual string Description
        {
            get
            {
                return "desciption of basic task";
            }
        }

        /// <summary>
        /// The human readable summary of where the task is up to
        /// </summary>
        public virtual string Status
        {
            get
            {
                return "BasicTask.Status";
            }
        }
        /// <summary>
        /// The last time at which the user interacted with this task
        /// </summary>
        /// <remarks>
        /// Implementation of this will be determined within the concrete class.
        /// </remarks>
        public virtual DateTime? LastInteraction
        {
            get
            {
                if (Completed)
                {
                    return CompletionTime;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Gets or sets the name of this task.
        /// </summary>
        [Queryable]
        public virtual string TaskName
        {
            get
            {
                return "BasicTask.TaskName";
            }

        }
        /// <summary>
        /// The patient who is the subject of this task
        /// </summary>
        [Queryable]
        public virtual Patient Patient
        {
            get
            {
                if (Appointment == null)
                    return null;
                return Appointment.Patient;
            }
        }

        /// <summary>
        /// The Study which this task is part of
        /// </summary>
        [Queryable]
        public virtual Study Study
        {
            get { return _appointment.Study; }
        }
        
        [Queryable]
        public bool IsCancelled
        {
            get
            {
                if (Appointment == null)
                    return true;
                if (Appointment.Cancelled)
                {
                    return true;
                }
                if (Cancelled)
                    return true;

                return false;
            }
        }

        public virtual DateTime SequenceTime
        {
            get
            {
                if (ScheduledCompletionTime == new DateTime())
                {
                    return ValidCompletionTime;
                }
                else
                {
                    return ScheduledCompletionTime;
                }
            }
        }

        /// <summary>
        /// Gets the ValidCompletionTime less 10 minutes
        /// </summary>
        public virtual DateTime ValidCommencementTime
        {
            get
            {
                return ValidCompletionTime.AddMinutes(-10);
            }
        }
        public virtual string DiaryName
        {
            get
            {

                return Patient.FullName + " - " + TaskName;
            }
        }
        /// <summary>
        /// The name of the patient concat with the scheduling time
        /// </summary>
        public virtual string QualifiedName
        {
            get
            {
                if (SequenceNumber > 1)
                {
                    return Patient.FullName + " (" + SequenceNumber + ") - " + SchedulingTime.ToShortTimeString();
                }
                return Patient.FullName + " - " + SchedulingTime.ToShortTimeString();
            }
        }

        /// <summary>
        /// Returns an enum Deleted, Completed or Pending
        /// </summary>
        public virtual TaskStatus TaskStatus
        {
            get
            {
                if (Deleted)
                {
                    return NucMed.TaskStatus.Deleted;
                }
                if (Completed)
                {
                    return NucMed.TaskStatus.Completed;
                }
                return NucMed.TaskStatus.Pending;
            }
        }

        /// <summary>
        /// Gets or sets a scheduling time
        /// </summary>
        /// <remarks>
        /// This property provides type agnostic method for rescheduling. For a BasicTask it returns valid completion time.
        /// For a finite task it returns ValidCommencementTime
        /// </remarks>        
        public virtual DateTime SchedulingTime
        {
            get
            {
                return ValidCompletionTime;
            }
        }

        /// <summary>
        /// confirmation that the Patients and procedure match on request form
        /// </summary>
        /// <remarks>
        /// This corresponds to the "I confirm that the Patient ID, Procedure
        /// and Site are correct on the request form"
        /// </remarks>
        [Queryable]
        public bool? RequestFormCorrect
        {
            get { return _requestFormCorrect; }
            set { _requestFormCorrect = value; }
        }

        /// <summary>
        /// Confirmation that the timeout procedure has been performed        
        /// </summary>
        /// <remarks>
        /// This corresponds to "I confirm that the Patient Timeout Protocol (CCC) has been performed at the time of the procedure"
        /// </remarks>
        [Queryable]
        public bool? TimeoutPerformed
        {
            get { return _timeoutPerformed; }
            set { _timeoutPerformed = value; }
        }

        /// <summary>
        /// Confirmation that the correct patient/procedure details are in the image
        /// </summary>
        /// <remarks>
        /// This corresponds to "I confirm that the Patient Name/ID & side of examination with markers are correct on the images
        /// </remarks>
        [Queryable]
        public bool? ImageCorrect
        {
            get { return _imageCorrect; }
            set { _imageCorrect = value; }
        }

        /// <summary>
        /// Returns true when the task is scheduled for a different day to the appintment
        /// </summary>
        public bool IsDisconnectedFromAppointment
        {
            get
            {
                if (Appointment == null)
                    return false;
                if (Appointment.ScheduledArrivalTime.Date != ScheduledCompletionTime.Date)
                    return true;

                return false;
            }
        }
        #endregion

        #endregion

        #region IRoomReservation
        public DateTime ReservationStartDate
        {
            get
            {
                return ValidCommencementTime;
            }
        }

        public DateTime ReservationEndDate
        {
            get
            {
                return ValidCompletionTime;
            }
        }

        public User User
        {
            get
            {
                return Creator;
            }
        }
        #endregion
    }


    public enum TaskStatus
    {
        Pending, Commenced, Completed, Deleted
    }



}
