
using System.Collections.Generic;

using System.Collections.ObjectModel;
using System.Threading;
using System.Collections.Specialized;
using System.ComponentModel;
using NLog;

namespace iRadiate.Common
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        //private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        public SynchronizationContext _synchronizationContext {get;set;}

        public AsyncObservableCollection():base()
        {
            //_synchronizationContext = SynchronizationContext.Current;
            logger.Trace("AsyncObservableCollectio()  -- threadID = " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            try
            {
                _synchronizationContext = Platform.SynchronizationContext;
            }
            catch
            {
                logger.Error("Caught an error tryig to set _synchronizationContext from AsyncObservableCollection()");
            }
        }

        public AsyncObservableCollection(IEnumerable<T> list)
            : base(list)
        {
           
            try
            {
                _synchronizationContext = Platform.SynchronizationContext;
            }
            catch
            {
                logger.Error("Caught an error tryig to set _synchronizationContext from AsyncObservableCollection(IEnumerable<T> list)");
            }
            //_synchronizationContext = SynchronizationContext.Current;
            logger.Trace("AsyncObservableCollection(IEnumerable<T>)  -- threadID = " + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Execute the CollectionChanged event on the current thread
                RaiseCollectionChanged(e);
                
            }
            else
            {
                // Raises the CollectionChanged event on the creator thread
                _synchronizationContext.Send(RaiseCollectionChanged, e);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                RaisePropertyChanged(e);
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                _synchronizationContext.Send(RaisePropertyChanged, e);
            }
        }

        private void RaisePropertyChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }
    }
  
    
}