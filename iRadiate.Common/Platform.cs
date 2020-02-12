using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

using NLog;

using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Common
{
    /// <summary>
    /// A collection of static methods that any part of the codebase can used
    /// </summary>
    /// <remarks>
    /// This has been moved to the common assembly so that we can decouple the gui form the core model.
    /// This class references the datamodel - not the othe way around.
    /// The intention is here to provide access to information so that a new application can be created out of the 
    /// data model and the common assembly.
    /// </remarks>
    public class Platform
    {
        #region privateStatic
        private static User _currentUser;
        private static List<AuthorityTokenGroup> _authorityTokenGroups;
        private static List<AuthorityToken> _authorityTokens;
        private static Workstation _currentWorkstation;
        private static NucMedPractice _currentNucMedPractice;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static IDataRetriever _retriever;
        private static bool _isApplicationRunning = false;
        private static IDoseCalibrator _doseCalibrator;
        #endregion

        /// <summary>
        /// Gets or sets the user currently logged in
        /// </summary>
        /// <remarks>
        /// Returns null if it has not been set by the application.
        /// </remarks>
        public static User CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
                DataStoreItem.CurrentUser = _currentUser;
            }
        }
        
        /// <summary>
        /// Gets or sets the workstation currently being used
        /// </summary>
        /// <remarks>
        /// This property only makes sense for user driven interaction
        /// </remarks>
        public static Workstation CurrentWorkstation
        {
            get
            {
                return _currentWorkstation;
            }
            set
            {
                _currentWorkstation = value;
                DataStoreItem.CurrentWorkstation = _currentWorkstation;
            }
        }

        /// <summary>
        /// Gets or sets the current NucMedPractice that the use is logged into
        /// </summary>
        public static NucMedPractice CurrentNucMedPractice
        {
            get
            {
                return _currentNucMedPractice;
            }
            set
            {
                _currentNucMedPractice = value;

            }
        }

        /// <summary>
        /// Gets or sets the UI synchronizationcontext for creating AsyncObvservableCollections
        /// </summary>
        public static SynchronizationContext SynchronizationContext { get; set; }

        /// <summary>
        /// Returns an AsyncObservableCollection with the synchronizationContext set to Platform.SynchronizationContext
        /// </summary>
        /// <returns></returns>
        [Obsolete("AysncObservableCollection construct uses the UI Synchronization Context")]
        public static AsyncObservableCollection<IDataStoreItem> CreateCollection()
        {
            logger.Trace("CreateCollection()...");
            AsyncObservableCollection<IDataStoreItem> res = new AsyncObservableCollection<IDataStoreItem>();
            res._synchronizationContext = SynchronizationContext;

            logger.Trace("CreateCollection()...Completed");
            return res;
        }

        /// <summary>
        /// Gets or sets the singleton IDataRetriever
        /// </summary>
        /// <remarks>
        /// If necessary, the get method will instantiate a retriever
        /// </remarks>
        public static IDataRetriever Retriever
        {
            get
            {
                if (_retriever == null)
                    _retriever = new EFDataRetriever();

                return _retriever;
            }
            set
            {
                _retriever = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the application is running
        /// </summary>
        /// <remarks>
        /// This is intended to allow modules to check if they need to perform intialization code 
        /// or if it can be assumed that everything has been done
        /// </remarks>
        public static bool IsApplicationRunning
        {
            get
            {
                return _isApplicationRunning;
            }
            set { _isApplicationRunning = value; }
        }

        /// <summary>
        /// Gets or sets the IDoseCalibrator that is configured for this workstation
        /// </summary>
        public static IDoseCalibrator DoseCalibrator
        {
            get { return _doseCalibrator; }
            set { _doseCalibrator = value; }
        }
    }
}
