using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.Desktop.Common;

namespace iRadiate.Settings.Common.ViewModel
{
    public class DoctorListViewModel : Module
    {
        private AsyncObservableCollection<IDataStoreItem> _doctors;
        public DoctorListViewModel() : base()
        {

        }

        public override void GetData()
        {
            List<RetrievalCriteria> criteria = new List<RetrievalCriteria>();
            Doctors = DesktopApplication.Librarian.GetItems(typeof(Doctor), criteria);
        }

        public AsyncObservableCollection<IDataStoreItem> Doctors
        {
            get
            {
                return _doctors;
            }
            set
            {
                _doctors = value;
            }
        }
    }
}
