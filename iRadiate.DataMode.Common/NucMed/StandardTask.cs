using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.NucMed
{
    public class StandardTask : BasicTask
    {
        private StandardTaskType _taskType;

        public StandardTask():base()
        {

        }

        public StandardTask(Appointment a):base(a)
        {
            
        }

        public virtual StandardTaskType TaskType
        {
            get { return _taskType; }
            set { _taskType = value; }
        }

        public override DateTime? LastInteraction
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

        public override bool TaskTypesMatch(BasicTask otherTask)
        {
            if (otherTask is StandardTask)
            {
                if (((StandardTask)otherTask).TaskType.Name == TaskType.Name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override string TaskName
        {
            get
            {
                if (SequenceNumber > 0)
                {
                    return TaskType.Name + " " + SequenceNumber.ToString();
                }
                else
                {
                    return TaskType.Name;
                }
                
            }
        }

        public override string Status
        {
            get
            {
                if (Completed)
                {
                    return TaskName + " Completed " + LastInteraction.Value.ToShortTimeString();
                }
                else
                {
                    return TaskName + " Pending";
                }
            }
        }

         public override Type ConcreteType
        {
            get
            {
                return typeof(StandardTask);
            }
        }

         public override string Description
         {
             get
             {
                 if (_taskType == null)
                 {
                     return null;
                 }
                 return _taskType.Description;
             }
         }
    
    }

    public class StandardTaskType : DataStoreItem
    {
        private string _name;
        private string _code;
        private string _description;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        
        
    }
}
