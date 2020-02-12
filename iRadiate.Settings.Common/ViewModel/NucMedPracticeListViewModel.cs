using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.Common;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.Common.IO;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Settings.Common.ViewModel
{
    public class NucMedPracticeListViewModel : Module
    {
        private NucMedPractice _selectedNucMedPractice;
        private AsyncObservableCollection<IDataStoreItem> _practices;


        public NucMedPractice SelectedNucMedPractice
        {
            get { return _selectedNucMedPractice; }
            set { _selectedNucMedPractice = value;
            RaisePropertyChanged("SelectedNucMedPractice");
            }
        }

        public AsyncObservableCollection<IDataStoreItem> Practices
        {
            get
            {
                return _practices;
            }
            set
            {
                _practices = value;
            }
        }

        public override void GetData()
        {
            List<RetrievalCriteria> criteria = new List<RetrievalCriteria>();
            Practices = DesktopApplication.Librarian.GetItems(typeof(NucMedPractice), criteria);
            RaisePropertyChanged("Practices");
        }
        public override void AddNew()
        {
            NucMedPractice newPractice = new NucMedPractice();
            newPractice.Name = "Enter name";
            SelectedNucMedPractice = newPractice;
            Practices.Add(newPractice);
           
            RaisePropertyChanged("ViewModelCollection");
        }
    }
}
