using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

using NLog;



namespace iRadiate.Common.IO
{
    /// <summary>
    /// The default implentation of IDataLibrarian
    /// </summary>
    public class StandardDataLibrarian : IDataLibrarian
    {
        #region privateFields
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private System.Timers.Timer timer;
        private IDataRetriever retriever;
        private List<AppointmentList> AppointmentLists;
        private List<TasksList> TasksLists;
        private Object thisLock = new Object();
        [DllImport("User32.dll")]
        public static extern bool LockWorkStation();
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO Dummy);
        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();
        private bool _autoRefresh = true;
        private bool _refreshInProgress = false;

        

        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        #endregion

        #region privateMethods
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                logger.Trace("timer_Elapsed...");
                //TimeSpan elaped = DateTime.Now - Application.MainWindow.LastMouseMove;
                    if (_refreshInProgress)
                    {
                        logger.Trace("_refreshInProgress = true - do not refresh");
                        return;
                    }
                    else
                    {
                        logger.Trace("_refreshInProgress = false - we are good for refresh");
                    }
                    if (AutoRefresh)
                    {
                        OnLibraryRefreshing();
                        
                     
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.DoWork += bw_DoWork;
                        bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                        bw.RunWorkerAsync();
                        
                    }
                logger.Trace("timer_Elapsed...done");
            }
            catch
            {

            }
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _refreshInProgress = false;
            OnRefreshCompleted();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            _refreshInProgress = true;
            UpdateAppointments();
        }

        public static uint GetIdleTime()
        {
            LASTINPUTINFO LastUserAction = new LASTINPUTINFO();
            LastUserAction.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(LastUserAction);
            GetLastInputInfo(ref LastUserAction);
            return ((uint)Environment.TickCount - LastUserAction.dwTime);
        }

        #endregion

        #region constructors
        public StandardDataLibrarian()
        {
            logger.Trace("StandardDataLibrarian()");
            logger.Trace("threadID = " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            retriever = new EFDataRetriever();
            Platform.Retriever = retriever;
            AppointmentLists = new List<AppointmentList>();
            //TasksLists = new List<TasksList>();
            timer = new System.Timers.Timer(Properties.Settings.Default.LibraryRefreshTime * 1000);
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
            timer.AutoReset = true;
            
        }
        #endregion

        #region publicMethods
        public IDataStoreItem GetItem(int ID, Type t)
        {
            return retriever.RetrieveItem(ID, t);
        }
     
        public AsyncObservableCollection<IDataStoreItem> GetItems(Type type, ICollection<RetrievalCriteria> criteria)
        {
            logger.Trace("GetItems( " + type.Name + ", criteria...");
            AsyncObservableCollection<IDataStoreItem> result = new AsyncObservableCollection<IDataStoreItem>();
            List<IDataStoreItem> items = retriever.RetrieveItems(type, criteria);
            logger.Trace("Results includes " + items.Count + " elements");
            foreach(IDataStoreItem i in items)
            {
                logger.Trace("Added item to result");
                result.Add(i);
                
            }
            return result;
        }

        public void SaveItems(IEnumerable<IDataStoreItem> items)
        {
            throw new NotImplementedException();
        }

        public void ReloadItem(IDataStoreItem item)
        {
           
        }

        public void DeleteItem(IDataStoreItem item)
        {
            retriever.DeleteItem(item);
        }

        public void UndeleteItem(IDataStoreItem item)
        {
            retriever.UnDeleteItem(item);
        }

        AsyncObservableCollection<IDataStoreItem> IDataLibrarian.GetAppointments(DateTime selectedDate)
        {
            if (AppointmentLists.Where(x => x.SelectedDate.Date == selectedDate.Date).Any())
            {
               
                return AppointmentLists.Where(x => x.SelectedDate.Date == selectedDate.Date).First().Appointments;
            }

            List<RetrievalCriteria> rc = new List<RetrievalCriteria>();
            RetrievalCriteria first = new RetrievalCriteria("ScheduledArrivalTime", CriteraType.GreaterThan, selectedDate.Date);
            rc.Add(first);
            RetrievalCriteria second = new RetrievalCriteria("ScheduledArrivalTime", CriteraType.LessThan, selectedDate.Date.AddDays(1));
            rc.Add(second);
            AppointmentList ls = new AppointmentList();
            
            ls.Appointments = GetItems(typeof(Appointment), rc);
           
            ls.SelectedDate = selectedDate;
            
            AppointmentLists.Add(ls);
         
            return ls.Appointments;
        }

        AsyncObservableCollection<IDataStoreItem> IDataLibrarian.GetTasks(DateTime selectedDate)
        {
            throw new NotImplementedException();
        }

        public void UpdateAppointments()
        {
         
            DateTime start = DateTime.Now;
            lock (thisLock)
            {

                try
                {
                    foreach (AppointmentList al in AppointmentLists)
                    {

                        try
                        {
                           
                            List<RetrievalCriteria> rc = new List<RetrievalCriteria>();
                            RetrievalCriteria first = new RetrievalCriteria("ScheduledArrivalTime", CriteraType.GreaterThan, al.SelectedDate.Date);
                            rc.Add(first);
                            RetrievalCriteria second = new RetrievalCriteria("ScheduledArrivalTime", CriteraType.LessThan, al.SelectedDate.Date.AddDays(1));
                            rc.Add(second);

                            //Create a new retriever item
                            EFDataRetriever spareRetriever = new EFDataRetriever();
                            
                                List<IDataStoreItem> items = spareRetriever.RetrieveItems(typeof(Appointment), rc, true);
                           
                            foreach (IDataStoreItem dt in items)
                            {
                                //rmember dt = item fress from database
                                //check if this already exists in the list of appointments
                                if (al.Appointments.Where(x => x.ID == dt.ID).Any())
                                {

                                    //this means this items is in the appointment list, we must check for updates
                                    IDataStoreItem oldItem = al.Appointments.Where(x => x.ID == dt.ID).First();
                                    

                                    if (oldItem.LastEditDate != dt.LastEditDate)
                                    {
                                        logger.Trace("Found appointment in current list -- Needs to be updated");
                                       
                                        retriever.UpdateItem(oldItem);

                                    }
                                    else
                                    {
                                       
                                    }

                                    //Now we check all the tasks for this appointment
                                    foreach (BasicTask bt in ((Appointment)dt).Tasks)
                                    {
                                        try
                                        {


                                            if (((Appointment)oldItem).Tasks.Where(x => x.ID == bt.ID).Any())
                                            {
                                                //this task is already in the appointment - check for updates needed
                                                BasicTask ot = ((Appointment)oldItem).Tasks.Where(x => x.ID == bt.ID).First();
                                                if (ot.LastEditDate != bt.LastEditDate)
                                                {
                                                    retriever.UpdateItem(ot);
                                                }
                                                //foreach (BaseConstraint c in bt.Constraints)
                                                //{
                                                //    if (ot.Constraints.Where(x => x.ID == c.ID).Any())
                                                //    {
                                                //        //this constraint exists in our version
                                                //        if (ot.Constraints.Where(x => x.ID == c.ID).First().LastEditDate != c.LastEditDate)
                                                //        {
                                                //            retriever.UpdateItem(ot.Constraints.Where(x => x.ID == c.ID).First());

                                                //        }
                                                //    }
                                                //    else
                                                //    {



                                                //    }
                                                //}
                                            }
                                            else
                                            {
                                                //need to add this task
                                                //(oldItem as Appointment).Tasks.Add(bt);
                                                
                                                retriever.UpdateItem(oldItem);
                                                System.Diagnostics.Debug.WriteLine("New task found ... appointment reloaded");
                                                if (bt is ArrivalTask)
                                                {
                                                    System.Diagnostics.Debug.WriteLine("task being added is arrivalTask");
                                                    (oldItem as Appointment).FireArrivalEvent();
                                                }
                                                else
                                                {
                                                    System.Diagnostics.Debug.WriteLine("task being added is NOT arrivalTask");
                                                }
                                                retriever.SaveItem(oldItem);
                                               
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    //This one is not in the list of appointments must add it.
                                    logger.Trace("Could not find this appointment in the current list");
                                    
                                    al.Appointments.Add(dt);
                                   
                                }
                               
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Caught exception during appointment refresh: " + ex.Message);
                        }
                    }
                }
                catch
                {

                }

                logger.Trace("UpdateAppointments()...done");
            }
        }
      

        public IDataRetriever DataRetriever
        {
            get
            {
                return retriever;
            }

            set
            {
                retriever = value;
            }
        }

        public void SwitchOnAutoDetect()
        {
            retriever.SwitchOnAutoDetect();
        }

        public void SwitchOffAutoDetect()
        {
            retriever.SwitchOffAutoDetect();
        }

        public void SaveItem(IDataStoreItem item)
        {
            item.FireSavingEvent();
            retriever.SaveItem(item);
            item.FireSavedEvent();
        }

        public void GetAlterations(IDataStoreItem item)
        {
            item.Alterations.Clear();
            System.Diagnostics.Debug.Print("GetAlterations(DatastoreItem item)....");
            RetrievalCriteria rc1 = new RetrievalCriteria("DataStoreItemName", CriteraType.ExactTextMatch, item.ConcreteType.Name);
            RetrievalCriteria rc2 = new RetrievalCriteria("ItemIDNumber", CriteraType.Equals, item.ID);
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc1);
            rcList.Add(rc2);
            System.Diagnostics.Debug.Print("    abou to get tmpList");
            AsyncObservableCollection<IDataStoreItem> tmpList = GetItems(typeof(DataStoreItemAlteration), rcList);
            System.Diagnostics.Debug.Print("    tmpList retrieved");
            foreach (IDataStoreItem i in tmpList)
            {
                System.Diagnostics.Debug.Print("         " + i.ConcreteType.Name);
                item.Alterations.Add(i as DataStoreItemAlteration);
            }
            System.Diagnostics.Debug.Print("GetAlterations(DatastoreItem item....Complete!");
        }
        #endregion

        #region publicProperties
        public bool AutoRefresh
        {
            get
            {
                return _autoRefresh;
            }
            set
            {
                _autoRefresh = value;
            }
        }
        #endregion

        #region events
        public event EventHandler LibraryRefreshing;
        public event EventHandler RefreshCompleted;

        protected virtual void OnLibraryRefreshing()
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler handler = LibraryRefreshing;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                
                handler(this, new EventArgs());
            }
        }

        protected virtual void OnRefreshCompleted()
        { 
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler handler = RefreshCompleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {

                handler(this, new EventArgs());
            }

        }
        #endregion
    }

    public class AppointmentList
    {
        public AppointmentList()
        {
            Appointments = Platform.CreateCollection();
        }
        public DateTime SelectedDate { get; set; }
        public AsyncObservableCollection<IDataStoreItem> Appointments { get; set; }
    }

    public class TasksList
    {
        public DateTime SelectedDate { get; set; }
        public TasksList()
        {
            Tasks = Platform.CreateCollection();
        }
        public AsyncObservableCollection<IDataStoreItem> Tasks
        {
            get;
            set;
        }
    }
}
