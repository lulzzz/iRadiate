using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{
    public class TaskTypeWrapper 
    {
        private Type _taskType;
        private StandardTaskType _nonFiniteType;
        private StandardFiniteTaskType _finiteType;

        public StandardTaskType NonFiniteTaskType
        {
            get
            {
                return _nonFiniteType;
            }
            set
            {
                _nonFiniteType = value;
            }
        }

        public StandardFiniteTaskType FiniteType
        {
            get
            {
                return _finiteType;
            }
            set
            {
                _finiteType = value;
            }
        }



        public string Name
        {
            get;
            set;
        }

        public Type TaskType
        {
            get
            {
                return _taskType;
            }
            set
            {
                _taskType = value;
            }
        }

        public BasicTask GetTask(DateTime tasktime)
        {

            if (TaskType == typeof(StandardTaskType))
            {
                StandardTask t = new StandardTask();
                t.TaskType = NonFiniteTaskType;
                return t;
            }
            else if (TaskType == typeof(StandardFiniteTaskType))
            {
                StandardFiniteTask t = new StandardFiniteTask();
                t.TaskType = FiniteType;
                return t;
            }
            else
            {
                ObjectHandle handle = Activator.CreateInstance(TaskType.Assembly.FullName, TaskType.FullName);
                object p = handle.Unwrap();


                return (BasicTask)p;
            }
            
        }
    }
}
