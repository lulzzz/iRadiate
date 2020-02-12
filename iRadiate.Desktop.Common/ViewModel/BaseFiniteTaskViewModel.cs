using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{
   
    public abstract class BaseFiniteTaskViewModel : BaseTaskViewModel
    {
        #region constructors
        public BaseFiniteTaskViewModel():base()
        {

        }

        public BaseFiniteTaskViewModel(IDataStoreItem item):base(item)
        {

        }
        #endregion

        #region virtuals
        public virtual bool Commenced
        {
            get
            {
                return ((BasicFiniteTask)Item).Commenced;
            }
            set
            {
                ((BasicFiniteTask)Item).Commenced = value;
                if (value)
                {
                    ((BasicFiniteTask)Item).CommencentTime = DateTime.Now;
                }
                
                RaisePropertyChanged("Commenced");
                RaisePropertyChanged("WorkflowStatus");
                RaisePropertyChanged("CommencementTime");
            }
        }
        public virtual DateTime ValidCommencementTime
        {
            get
            {
                if (Commenced)
                {
                    return CommencementTime;
                }
                else
                {
                    return ScheduledCommencementTime;
                }
            }
            set
            {
                if (Commenced)
                {
                    CommencementTime = value;
                }
                else
                {
                    ScheduledCommencementTime = value;
                }
            }
        }
        #endregion

        #region publicProperties
        public bool UnCommenced
        {
            get
            {
                return !Commenced;
            }
        }
        public DateTime ScheduledCommencementTime
        {
            get
            {
                return ((BasicFiniteTask)Item).ScheduledCommencementTime;
            }
            set
            {
                int d = ((BasicFiniteTask)Item).Duration;
                ((BasicFiniteTask)Item).ScheduledCommencementTime = value;
                ((BasicFiniteTask)Item).ScheduledCompletionTime = ((BasicFiniteTask)Item).ScheduledCommencementTime.AddMinutes(d); ;
                RaisePropertyChanged("ScheduledCommencementTime");
                RaisePropertyChanged("ScheduledCompletionTime");
                RaisePropertyChanged("SchedulingTime");
            }
        }
        public DateTime CommencementTime
        {
            get
            {
                return ((BasicFiniteTask)Item).CommencentTime;
            }
            set
            {
                ((BasicFiniteTask)Item).CommencentTime = value;
                RaisePropertyChanged("CommencementTime");
                RaisePropertyChanged("SchedulingTime");
            }
        }

        public int Duration
        {
            get
            {
                return ((BasicFiniteTask)Item).Duration;
            }
        }

        #endregion

        #region overrides
        public override bool Completed
        {
            get
            {
                return base.Completed;
            }
            set
            {
                ((BasicFiniteTask)Item).CompletionTime = DateTime.Now;
                ((BasicFiniteTask)Item).Completed = value;
                RaisePropertyChanged("Completed");
                RaisePropertyChanged("CompletionTime");
                RaisePropertyChanged("WorkflowStatus");
            }
        }

        public override DateTime SchedulingTime
        {
            get
            {
                return ValidCommencementTime;
            }
            set
            {
                ValidCommencementTime = value;
                RaisePropertyChanged("SchedulingTime");
            }

        }

        public override WorkflowStatus WorkflowStatus
        {
            get
            {
                if (Deleted)
                {
                    return ViewModel.WorkflowStatus.Deleted;
                }
                if (Completed)
                {
                    return ViewModel.WorkflowStatus.Completed;
                }
                if (Commenced)
                {
                    return ViewModel.WorkflowStatus.Commenced;
                }
                return ViewModel.WorkflowStatus.Pending;
            }
        }
        #endregion
    }
}
