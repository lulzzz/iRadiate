using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Settings.Common.ViewModel
{
    public class StudyTypesViewModel : Module
    {
        private AsyncObservableCollection<IDataStoreItem> _studyTypes;


        public StudyTypesViewModel()
        {
            StudyTypes = DesktopApplication.Librarian.GetItems(typeof(StudyType), new List<RetrievalCriteria>());
            AddNewStudyTypeCommand = new RelayCommand(AddNewStudyType);
        }

        public AsyncObservableCollection<IDataStoreItem> StudyTypes
        {
            get
            {
                if (_studyTypes == null)
                {
                    _studyTypes = new AsyncObservableCollection<IDataStoreItem>();
                }
                return _studyTypes;
            }
            set
            {
                _studyTypes = value;
            }
        }

        public RelayCommand AddNewStudyTypeCommand
        {
            get;
            private set;
        }

        private void AddNewStudyType()
        {
            StudyType st = new StudyType();
            st.NucMedPractice = DesktopApplication.CurrentPratice;
            StudyTypes.Add(st);
            
            DataStoreItemViewModel stvm = new DataStoreItemViewModel(st);
            DesktopApplication.MakeModalDocument(stvm, DesktopApplication.DocumentMode.New);
        }

        public override void GetData()
        {
            StudyTypes = DesktopApplication.Librarian.GetItems(typeof(StudyType), new List<RetrievalCriteria>());
        }
    }
}
