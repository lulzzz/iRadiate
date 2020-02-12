using System;
using System.Collections.Generic;
using iRadiate.DataModel.Common;


namespace iRadiate.Common.IO
{
    /// <summary>
    /// An automated librarian that manages interaction with the database
    /// </summary>
    /// <remarks>
    /// The iDataLibrarian goes further than the IDataRetriever by actively updating DataStoreItems from the database
    /// and will 
    /// </remarks>
    public interface IDataLibrarian
    {
        IDataStoreItem GetItem(int id, Type type);

        AsyncObservableCollection<IDataStoreItem> GetItems(Type type, ICollection<RetrievalCriteria> criteria);

        void SaveItems(IEnumerable<IDataStoreItem> items);

        void SaveItem(IDataStoreItem item);

        void ReloadItem(IDataStoreItem item);

        void DeleteItem(IDataStoreItem item);

        void UndeleteItem(IDataStoreItem item);

        AsyncObservableCollection<IDataStoreItem> GetAppointments(DateTime selectedDate);

        AsyncObservableCollection<IDataStoreItem> GetTasks(DateTime selectedDate);

        void UpdateAppointments();

        bool AutoRefresh{get;set;}

        void SwitchOnAutoDetect();

        void SwitchOffAutoDetect();

        void GetAlterations(IDataStoreItem item);

        [Obsolete("Use Platform.Retriever insread")]
        IDataRetriever DataRetriever { get; set; }

        event EventHandler LibraryRefreshing;

        event EventHandler RefreshCompleted;

    }

    
}
