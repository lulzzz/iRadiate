using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iRadiate.DataModel.NucMed
{
    /// <summary>
    /// An abstract from which ScanTask can be derived
    /// </summary>
    /// <remarks>
    /// The BasicTask does not include task commencement so this is a derived from which ScanTask
    /// and potentially others can be derived.
    /// </remarks>
    public abstract class BasicFiniteTask : BasicTask
    {
        #region private
        private DateTime _commencementTime;
        private bool _commenced;
        private DateTime _scheduledCommencementTime;
        private Room _inProgressRoom;
        #endregion

        #region events
        /// <summary>
        /// Fires when task is commenced
        /// </summary>
        public event EventHandler TaskCommenced;

        protected virtual void OnTaskCommenced(EventArgs e)
        {
            if (TaskCommenced != null)
                TaskCommenced(this, e);
        }
        #endregion

        #region constructor
        /// <summary>
        /// A no parameter constructor
        /// </summary>
        public BasicFiniteTask():base()
        {

        }

        /// <summary>
        /// A constructor with a parameter
        /// </summary>
        /// <param name="a">the appointment that this task belongs to</param>
        public BasicFiniteTask(Appointment a)
            : base(a)
        {

        }
        #endregion  

        /// <summary>
        /// The time at which this task was commenced
        /// </summary>
        [Auditable]
        public virtual DateTime CommencentTime
        {
            get { return _commencementTime; }
            set { _commencementTime = value; }
        }

        /// <summary>
        /// A value indicating whether this task was commenced or not
        /// </summary>
        [Auditable]
        public virtual bool Commenced 
        {
            get { return _commenced; }
            set { _commenced = value; }
        }

        /// <summary>
        /// The time at which this task was scheduled to be commenced
        /// </summary>
        /// <remarks>
        /// If ScheduledCompletionTime has not been set then it will set to
        /// ScheduledCommencementTime + 20 minutes to ensure there is always a non-negative duration.
        /// If the duration is non-negative then the ScheduledCompletionTime will be changed
        /// to get a constant duration.
        /// </remarks>
        public virtual DateTime ScheduledCommencementTime
        {
            get { return _scheduledCommencementTime; }
            set
            {
                if(ScheduledCompletionTime == new DateTime())
                {
                    ScheduledCompletionTime = value.AddMinutes(20);
                }
                else
                {
                    ScheduledCompletionTime = value.AddMinutes(Duration);
                }
                _scheduledCommencementTime = value;
            }
        }

        /// <summary>
        /// The time of the lastinteraction between a user and this tas
        /// </summary>
        /// <remarks>
        /// Returns either completed time or commencement time
        /// </remarks>
        public override DateTime? LastInteraction
        {
            get
            {
                if (Completed)
                {
                    return CompletionTime;
                }
                else if (Commenced)
                {
                    return CommencentTime;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns the room which this task is in
        /// </summary>
        /// <remarks>
        /// This property seems to meaningless since the Room property of BasicTask will already 
        /// have the room in which the task is in progress
        /// </remarks>
        public virtual Room InProgressRoom
        {
            get
            {
                return _inProgressRoom;
            }
            set
            {
                _inProgressRoom = value;
            }
        }

        /// <summary>
        /// A string describing the current status of the task
        /// </summary>
        /// <remarks>
        /// Will be "Started ..." or "Completed ..." or "Not started"
        /// </remarks>
        public override string Status
        {
            get
            {
                if (Commenced && !Completed)
                {
                    return TaskName + " Started " + LastInteraction.Value.ToShortTimeString();
                }
                else if (Completed)
                {
                    return TaskName + " Completed " + LastInteraction.Value.ToShortTimeString();
                }
                else
                {
                    return "Not started";
                }
            }
        }

        /// <summary>
        /// The duration of the task in minutes
        /// </summary>
        /// <remarks>
        /// I think this one might need work
        /// </remarks>
        public int Duration
        {
            get
            {
                if (Completed)
                {
                    TimeSpan ts = CompletionTime - CommencentTime;
                    return Convert.ToInt32(ts.TotalMinutes);

                }
                else
                {
                    if (ScheduledCompletionTime.Date != ScheduledCommencementTime.Date)
                    {
                        return 20;
                    }
                    if (ScheduledCompletionTime < ScheduledCommencementTime)
                    {
                        return 20;
                    }
                    TimeSpan ts = ScheduledCompletionTime - ScheduledCommencementTime;
                    return Convert.ToInt32(ts.TotalMinutes);
                }
            }
        }

        /// <summary>
        /// Returns either the time the task was commenced or the time it was scheduled to commence.
        /// </summary>
        public override DateTime ValidCommencementTime
        {
            get
            {
                if (Commenced)
                {
                    return CommencentTime;
                }
                else
                {
                    return ScheduledCommencementTime;
                }
            }
        }

       
        /// <summary>
        /// Returns a boolean indicating whether the task can be commenced at a proposed time
        /// </summary>
        /// <param name="proposedTime">The timing of the commencement</param>
        /// <returns>True if there are no constraints breached by commencing this at the proposed time</returns>
        public virtual bool CanCommence(DateTime proposedTime)
        {
            foreach (IConstraint ic in Constraints)
            {
                if (ic.CanCommence(proposedTime) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns a boolean indicating whether the task can be commenced now
        /// </summary>
        /// <returns>True if there are no constraints breached by commencing this</returns>
        public virtual bool CanCommence()
        {
            return CanCommence(DateTime.Now);
        }

        /// <summary>
        /// Returns ValidCommencementTime
        /// </summary>
        public override DateTime SchedulingTime
        {
            get
            {
                return ValidCommencementTime;
            }
        }

        /// <summary>
        /// Returns a TaskSatus for this task
        /// </summary>
        public override TaskStatus TaskStatus
        {
            get
            {
                if (!Completed)
                {
                    if (Commenced)
                    {
                        return NucMed.TaskStatus.Commenced;
                    }
                }
                return base.TaskStatus;
            }
        }

        public override DateTime SequenceTime
        {
            get
            {
                return ValidCommencementTime;
            }
        }

        public override bool CompleteTask()
        {
            return base.CompleteTask();
        }
    }
}
