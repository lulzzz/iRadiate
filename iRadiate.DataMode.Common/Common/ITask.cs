using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;


namespace iRadiate.DataModel.Common
{

    public interface ITask
    {
                
        DateTime CompletionTime
        {
            get;
            set;
        }
       
        bool Completed
        {
            get;
            set;
        }

        bool Cancelled
        {
            get;
            set;
        }
       
        DateTime ScheduledCompletionTime
        {
            get;
            set;
        }
       
        string TaskName
        {
            get;
            
        }              

        User Assignee
        {
            get;
            set;
        }

        DateTime? LastInteraction
        {
            get;
        }

        string Status
        {
            get;
        }

        bool CompleteTask();

        bool IsCancelled { get; }
    }

    public class TaskArgs : EventArgs
    {
        public ITask Task { get; set; }
        public DateTime TaskTime { get; set; }
        public User User { get; set; }
    }
}
