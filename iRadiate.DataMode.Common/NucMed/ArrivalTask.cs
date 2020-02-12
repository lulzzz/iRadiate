using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using iRadiate.DataModel;

namespace iRadiate.DataModel.NucMed
{
    /// <summary>
    /// The task of arriving the patient
    /// </summary>
    /// <remarks>
    /// When a patient is arrived it is a signal to everyone lese that the patient
    /// is now ready to start progressing through the workflow
    /// </remarks>
    [Export("TaskType")]
    [PreferredView("iRadiate.Desktop.Common.View.DoseAdministrationTaskView", "iRadiate.Desktop.Common")]
    public class ArrivalTask : BasicTask
    {
        /// <summary>
        /// An empty constructor
        /// </summary>
        public ArrivalTask()
            : base()
        {

        }

        /// <summary>
        /// A construct with appoinemt
        /// </summary>
        /// <param name="A">The appointment that this task belongs to</param>
        public ArrivalTask(Appointment A)
            : base(A)
        {

        }

        /// <summary>
        /// Returns Arrival or Arrival + SequenceNumber
        /// </summary>
        /// <remarks>
        /// Multiple arivals is counter intuitive but in the case of bone scans the patient can arrive multiple times
        /// </remarks>
        public override string TaskName
        {
            get
            {
                if (SequenceNumber > 0)
                {
                    return "Arrival " + SequenceNumber.ToString();
                }
                else
                {
                    return "Arrival";
                }
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(ArrivalTask);
            }
        }

        /// <summary>
        /// Returns a string represinting the status of this tsak
        /// </summary>
        /// <remarks>
        /// Will be either Arrive @ 8:30 AM or Arrived (2) @ 8:30 AM or Arrival Pending
        /// </remarks>
        public override string Status
        {
            get
            {
                if (Completed)
                {
                    if (SequenceNumber > 0)
                    {
                        return "Arrived (" + SequenceNumber.ToString() + ") @ " + CompletionTime.ToShortTimeString();
                    }
                    else
                    {
                        return "Arrived @ " + CompletionTime.ToShortTimeString();
                    }

                }
                else
                {
                    return TaskName + " Pending";
                }
            }
        }

        /// <summary>
        /// Method to determine if this task matches another BasicTask
        /// </summary>
        /// <param name="otherTask">The task to compare with</param>
        /// <returns>True if otherTask is ArivalTask</returns>
        public override bool TaskTypesMatch(BasicTask otherTask)
        {
            if (otherTask is ArrivalTask)
            {
                return true;
            }

            return false;
        }

        public override bool Completed
        {
            get
            {
                return base.Completed;
            }

            set
            {
                if(base.Completed == false && value == true)
                {
                    base.Completed = true;
                    if(Appointment != null)
                        Appointment.FireArrivalEvent();
                }
                else
                {
                    base.Completed = value;
                }
                
            }
        }
    }
}
