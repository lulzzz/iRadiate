using System;
using System.ComponentModel.Composition;

using iRadiate.DataModel.Common;
namespace iRadiate.DataModel.NucMed
{
    /// <summary>
    /// A constraint the constrains one task in relation to another
    /// </summary>
    /// <remarks>
    /// In this model the tasks do not monitor on another instead the iConstraint monitors one task
    /// and rearranges the other if necessary. The constraint also provides methods to indicate whether the
    /// constrained task is able to commence or complete at a given time.
    /// </remarks>
    public interface IConstraint
    {
        BasicTask Constrainee { get; set; }

        BasicTask Constrainor { get; set; }

        string Name { get; }

        string ConstraintType {get;}

        bool CanComplete();

        bool CanComplete(DateTime proposedTime);

        bool CanCommence();

        bool CanCommence(DateTime proposedTime);

        void RescheduleConstrainor();

        void RescheduleConstrainee();
        
        
    }

    public abstract class BaseConstraint : DataStoreItem, IConstraint
    {
        private BasicTask _constrainee;
        private BasicTask _constrainor;

        /// <summary>
        /// The task which is constrained in the worklow
        /// </summary>
        /// <remarks>
        /// The constrainor will react to a change in the constrainee and vice versa
        /// </remarks>
        public BasicTask Constrainee
        {
            get
            {
                return _constrainee;
            }
            set
            {
                _constrainee = value;
            }
        }

        /// <summary>
        /// The task which is constraining the workflow
        /// </summary>
        /// <remarks>
        /// The constrainor will react to a change in the constrainee and vice versa
        /// </remarks>
        public BasicTask Constrainor
        {
            get
            {
                return _constrainor;
            }
            set
            {
                _constrainor = value;
                Subscribe();
                
            }
        }

        public void Subscribe()
        {
            _constrainor.TaskRescheduled += _constrainor_TaskRescheduled;
            _constrainor.TaskReassigned += _constrainor_TaskReassigned;
            _constrainor.TaskRoomMoved += _constrainor_TaskRoomMoved;
        }

        protected virtual void _constrainor_TaskRoomMoved(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void _constrainor_TaskReassigned(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void _constrainor_TaskRescheduled(object sender, EventArgs e)
        {
            //When the constrainor rescheuled we need to move the constrainor as well
            throw new NotImplementedException();
        }

        /// <summary>
        /// The name of this constraint
        /// </summary>
        public virtual string Name
        {
            get { throw new NotImplementedException(); }
        }
    
        public virtual string ConstraintType
        {
	        get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Checkes whether the constrainee can complete
        /// </summary>
        /// <returns>Returns true if the constraint does not prohibit the Constrainee from completing</returns>
        public virtual bool CanComplete()
        {
            return CanComplete(DateTime.Now);
        }

        /// <summary>
        /// Checkes whether the constrainee can complete at a proposed time
        /// </summary>
        /// <param name="proposedTime">Time for which we are checking if can complete</param>
        /// <returns>Returns true if the constraint does not prohibit the Constrainee from completing at the proposed time</returns>
        public virtual bool CanComplete(DateTime proposedTime)
        {
            return true;
        }

        public virtual bool CanCommence()
        {
            return CanCommence(DateTime.Now);
        }

        public virtual bool CanCommence(DateTime proposedTime)
        {
            return true;
        }

        /// <summary>
        /// Reschedules the Constrainor to fit within this constraint
        /// </summary>
        public void RescheduleConstrainor()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rescheduled the Constrainee to fit within this constraint
        /// </summary>
        public void RescheduleConstrainee()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// A constraint that delays the Constrainee in regard to the Constrainor
    /// </summary>
    /// <remarks>
    /// This constraint is driven by a DelayTime which species how long after the constrainor
    /// has completed before the constrainee can commence or complete.
    /// DelayTime cannot be negative.
    /// </remarks>
    [Export(typeof(IConstraint))]
    public class DelayConstraint : BaseConstraint
    {
        
        private int _delayTime;

        public DelayConstraint():base()
        {
           
        }
    
        public override string ConstraintType
        {
            get
            {
                return "Delay Constraint";
            }
        }

        public int DelayTime
        {
            get
            {
                return _delayTime;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Delay time cannot be less than 0");
                }
                _delayTime = value;
            }
        }
        public override string Name
        {
            get
            {
                if (Constrainee != null && Constrainor != null)
                {
                    return "Delay of " + Constrainee.TaskName + " until  " + DelayTime.ToString() + " minutes  after " + Constrainor.TaskName;
                }
                else
                {
                    return ConstraintType;
                }
                
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(DelayConstraint);
            }
        }

        public override bool CanComplete(DateTime proposedTime)
        {
            //if the constrainor is not completed
            if (Constrainor.Deleted)
            {
                return true;
            }
            if (Constrainor.Completed == false)
            {
                return false;
            }
            if (Constrainor.CompletionTime.AddMinutes(DelayTime) > proposedTime)
            {
                return false;
            }
            return true;
        }

        protected override void _constrainor_TaskRescheduled(object sender, EventArgs e)
        {
            if (Constrainee.Completed)
            {
                return;
            }

            if(Constrainee is BasicFiniteTask)
            {
                BasicFiniteTask bft = Constrainee as BasicFiniteTask;
                bft.ScheduledCommencementTime = Constrainor.ScheduledCompletionTime.AddMinutes(DelayTime);
                Constrainor.LinkedItems.Add(bft);
            }
            else
            {
                Constrainee.ScheduledCompletionTime = Constrainor.ScheduledCompletionTime.AddMinutes(DelayTime);
                Constrainor.LinkedItems.Add(Constrainee);
            }
        }

    }

    /// <summary>
    /// This constraint requires that the constrainee happen during the constrainor task delayed
    /// </summary>
    /// <remarks>
    /// TimeDelay is the ideal delay between the commencement of the constrainor and the completion of the constrainee.
    /// Constrainor must be BasicFiniteTask and Constrainee must not be BasicFiniteTask
    /// </remarks>
    [Export(typeof(IConstraint))]
    public class DuringConstraint :BaseConstraint
    {
        private int _delayTime;

        public override string ConstraintType
        {
            get
            {
                return "During Constraint";
            }
        }

        public int DelayTime
        {
            get
            {
                return _delayTime;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Delay time cannot be less than 0");
                }
                _delayTime = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(DuringConstraint);
            }
        }

        protected override void _constrainor_TaskRescheduled(object sender, EventArgs e)
        {
            if (Constrainee.Completed)
            {
                return;
            }
            if (Constrainor.Completed)
            {
                return;

            }

            if (Constrainee is BasicFiniteTask)
            {
                if(!(Constrainor is BasicFiniteTask))
                {
                    int duration = (Constrainor as BasicFiniteTask).Duration;
                    if(duration < DelayTime)
                    {
                        Constrainee.ScheduledCompletionTime = (Constrainor as BasicFiniteTask).ScheduledCommencementTime.AddMinutes(duration);
                    }
                    else
                    {
                        Constrainee.ScheduledCompletionTime = (Constrainor as BasicFiniteTask).ScheduledCommencementTime.AddMinutes(DelayTime);
                    }
                }
                
            }
        }
    }

   
}
