using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.DataDictionary
{
    public abstract class BaseDataItem : DataStoreItem
    {
        private DataDictionaryEntry _dataDictionaryEntry;
        private User _recordingUser;
        private DateTime _dataDate;
        private string _comments;
        private string _dataValue;

        public BaseDataItem() : base()
        {

        }
        public DataDictionaryEntry DataDictionaryEntry
        {
            get { return _dataDictionaryEntry; }
            set { _dataDictionaryEntry = value; }
        }

        /// <summary>
        /// The User who recorded the information
        /// </summary>
        public User RecordingUser
        {
            get { return _recordingUser; }
            set { _recordingUser = value; }
        }

        /// <summary>
        /// The date on which the information was measured or obtained
        /// </summary>
        /// <remarks>
        /// This is not the date of when the data was entered, which is captured in the CreationDate property
        /// </remarks>
        public DateTime DataDate
        {
            get { return _dataDate; }
            set { _dataDate = value; }
        }

        /// <summary>
        /// Comments from the user about the data
        /// </summary>
        public string Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }

        public string DataValue
        {
            get { return _dataValue; }
            set { _dataValue = value; }
        }
    }
}
