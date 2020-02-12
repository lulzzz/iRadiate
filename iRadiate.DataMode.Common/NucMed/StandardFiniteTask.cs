using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
namespace iRadiate.DataModel.NucMed
{
    public class StandardFiniteTask : BasicFiniteTask
    {
        private StandardFiniteTaskType _taskType;

        public StandardFiniteTask()
            : base()
        {

        }
        public StandardFiniteTask(Appointment a)
            : base(a)
        {

        }

        public virtual StandardFiniteTaskType TaskType
        {
            get { return _taskType; }
            set { _taskType = value; }
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

        public override Type ConcreteType
        {
            get
            {
                return typeof(StandardFiniteTask);
            }
        }
    }

    public class StandardFiniteTaskType : DataStoreItem
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
