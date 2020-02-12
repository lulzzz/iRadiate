using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.DataDictionary;
using iRadiate.DataModel.Equipment;

namespace iRadiate.DataModel.Forms
{
    public class FormInstance : DataStoreItem
    {
        private FormTemplate _formVersion;
        private DateTime _formCommencementDate;
        private bool _formCompleted;
        private DateTime _formCompletionDate;
        private User _formCompletionUser;
        private List<BaseDataItem> _dataItems;

        public FormInstance() : base()
        {

        }

        public virtual FormTemplate FormTemplate
        {
            get { return _formVersion; }
            set { _formVersion = value; }
        }

        public DateTime FormCommencementDate
        {
            get { return _formCommencementDate; }
            set { _formCommencementDate = value; }
        }

        /// <summary>
        /// Gets or sets when the form has been completed, and should not recieve any new data;
        /// </summary>
        public bool FormCompleted
        {
            get { return _formCompleted; }
            set { _formCompleted = value; }
        }
        
        /// <summary>
        /// Gets or sets the date on which the form was marked as completed
        /// </summary>
        public DateTime FormCompletionDate
        {
            get { return _formCompletionDate; }
            set { _formCompletionDate = value; }
        }

        /// <summary>
        /// The user who marked the form as completed
        /// </summary>
        public virtual User FormCompletionUser
        {
            get { return _formCompletionUser; }
            set { _formCompletionUser = value; }
        }

        /// <summary>
        /// The list of data items that are lnked to this instance of the form
        /// </summary>
        public virtual List<BaseDataItem> DataItems
        {
            get
            {
                if (_dataItems == null)
                    _dataItems = new List<BaseDataItem>();
                return _dataItems;
            }
            set
            {
                _dataItems = value;
            }
        }

        
    }

    
}
