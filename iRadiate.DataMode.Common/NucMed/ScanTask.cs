using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace iRadiate.DataModel.NucMed
{
    [Export("TaskType")]
    public class ScanTask : BasicFiniteTask
    {
        private List<PatientImage> _patientImages;

        public ScanTask() :base()
        {

        }
        public ScanTask(Appointment a)
            : base(a)
        {

        }
        

        public override string TaskName
        {
            get
            {
                if (SequenceNumber > 0)
                {
                    return "Scan " + SequenceNumber.ToString();
                }
                else
                {
                    return "Scan";
                }
            }
        }

        public override string Status
        {
            get
            {
                if(Completed)
                {
                    return TaskName + " Completed " + ((DateTime)LastInteraction).ToShortTimeString();
                }
                else if(Commenced)
                {
                    return TaskName + " Started " + ((DateTime)LastInteraction).ToShortTimeString();
                }
                else
                {
                    return TaskName + " not started";
                }
                    
            }
        }

        public override bool TaskTypesMatch(BasicTask otherTask)
        {
            if (otherTask is ScanTask)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(ScanTask);
            }
        }

        public override string DiaryName
        {
            get
            {
                return Patient.Surname + " - " + Appointment.Study.ShortName + " - " + TaskName;
            }
        }

        public override DateTime ValidCompletionTime
        {
            get
            {
                if (Completed)
                    return CompletionTime;
                if (!Commenced)
                    return ScheduledCompletionTime;

                return CommencentTime.AddMinutes((ScheduledCompletionTime - ScheduledCommencementTime).TotalMinutes);
            }
        }

        public virtual List<PatientImage> PatientImages
        {
            get
            {
                if (_patientImages == null)
                    _patientImages = new List<PatientImage>();
                return _patientImages;
            }
            set { _patientImages = value; }

        }
    }

    
}
