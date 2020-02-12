using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Radiopharmacy;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace iRadiate.Settings.Common.ViewModel
{
    public class ElementListViewModel : Module
    {
        private AsyncObservableCollection<IDataStoreItem> _elements;
        private AsyncObservableCollection<IDataStoreItem> _isotopes;
        public ElementListViewModel()
        {
            
        }

        public override void GetData()
        {
            _elements = DesktopApplication.Librarian.GetItems(typeof(Element), new List<RetrievalCriteria>());
            _isotopes = DesktopApplication.Librarian.GetItems(typeof(Isotope), new List<RetrievalCriteria>());
        }

        public AsyncObservableCollection<IDataStoreItem> Elements
        {
            get
            {
                return _elements;
            }
            set
            {
                _elements = value;
            }
        }

        public AsyncObservableCollection<IDataStoreItem> Isotopes
        {
            get
            {
                return _isotopes;
            }
            set
            {
                _isotopes = value;
            }
        }

    }
}
