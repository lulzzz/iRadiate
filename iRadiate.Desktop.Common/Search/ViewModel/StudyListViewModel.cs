using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Data;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NLog;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.Desktop.Common;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;

using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Search.ViewModel
{
    public class StudyListViewModel : Module
    {
        #region privateFields
        private string _selectedDateRange;
        private DateTime _startDate, _endDate;
        private List<string> _dateRanges;
        private AsyncObservableCollection<IDataStoreItem> _retrievedStudies;
        private bool _customDatesVisible;
        private StudyType _selectedStudyType;
        private IDataStoreItem _selectedStudy;
        private ObservableCollection<IStudyListTool> _studyTools;
        private ICollectionView _studiesView;
        #endregion

        #region constructor
        public StudyListViewModel()
        {
            SearchCommand = new RelayCommand(Search);
            
        }
        #endregion

        public string SelectedDateRange
        {
            get
            {
                return _selectedDateRange;
            }
            set
            {
                _selectedDateRange = value;
                RaisePropertyChanged("SelectedDateRange");
                SetDateRange();
            }
        }
        public bool CustomDatesVisible
        {
            get
            {
                return _customDatesVisible;
            }
            set
            {
                _customDatesVisible = value;
                RaisePropertyChanged("CustomDatesVisible");
            }
        }

        public StudyType SelectedStudyType
        {
            get
            {
                return _selectedStudyType;
            }
            set
            {
                _selectedStudyType = value;
                RaisePropertyChanged("SelectedStudyType");
            }
        }
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                RaisePropertyChanged("StartDate");
            }
        }
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                RaisePropertyChanged("EndDate");
            }
        }
        public List<string> DateRanges
        {
            get
            {
                if (_dateRanges == null)
                {
                    _dateRanges = new List<string>();
                    _dateRanges.Add("Today");
                    _dateRanges.Add("Tomorrow");
                    _dateRanges.Add("This Week");
                    _dateRanges.Add("Next Week");
                    _dateRanges.Add("This Month");
                    _dateRanges.Add("Next Month");
                    _dateRanges.Add("This Year");
                    _dateRanges.Add("Custom");
                }

                return _dateRanges;
            }
        }
        public List<StudyType> StudyTypes
        {
            get
            {
                return DesktopApplication.CurrentPratice.StudyTypes;
            }
        }
        public AsyncObservableCollection<IDataStoreItem> RetrievedStudies
        {
            get
            {
                return _retrievedStudies;
            }
            set
            {
                _retrievedStudies = value;
                RaisePropertyChanged("RetrievedStudies");
            }
        }
        public RelayCommand SearchCommand { get; private set; }
        
        public IDataStoreItem SelectedStudy
        {
            get
            {
                return _selectedStudy;
            }
            set
            {
                _selectedStudy = value;
                RaisePropertyChanged("SelectedStudy");
                OnSelectionChanged(EventArgs.Empty);
            }
        }

        public delegate void SelectionChangedHandler(object sender, EventArgs e);

        [ImportMany(typeof(IStudyListTool))]
        public ObservableCollection<IStudyListTool> StudyTools
        {
            get
            {
                if (_studyTools == null)
                {
                    _studyTools = new ObservableCollection<IStudyListTool>();

                }
                return _studyTools;
            }
            set
            {
                _studyTools = value;
                RaisePropertyChanged("StudyTools");
            }
        }
        public event SelectionChangedHandler SelectionChanged;

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        private void SetDateRange()
        {
            switch (SelectedDateRange)
            {
                case "Today":
                    StartDate = DateTime.Today;
                    EndDate = DateTime.Today.AddDays(1);
                    CustomDatesVisible = false;
                    break;
                case "Tomorrow":
                    StartDate = DateTime.Today.AddDays(1);
                    EndDate = DateTime.Today.AddDays(2);
                    CustomDatesVisible = false;
                    break;
                case "This Month":
                    StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month+1, 1);
                    CustomDatesVisible = false;
                    break;
                case "This Week":
                    StartDate = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                    EndDate = StartDate.AddDays(7);
                    CustomDatesVisible = false;
                    break;
                case "Next Week":
                    StartDate = DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(7);
                    EndDate = StartDate.AddDays(7);
                    CustomDatesVisible = false;
                    break;
                case "Next Month":
                     StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month+1, 1);
                    EndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month+2, 1);
                    CustomDatesVisible = false;
                    break;
                case "This Year":
                    StartDate = new DateTime(DateTime.Today.Year, 1, 1);
                    EndDate = new DateTime(DateTime.Today.Year + 1, 1, 1);
                    CustomDatesVisible = false;
                    break;
                case "Custom":
                    CustomDatesVisible = true;
                    break;
            }
        }
        private void Search()
        {
            RetrievalCriteria rc = new RetrievalCriteria("Date", CriteraType.GreaterThan, _startDate);
            RetrievalCriteria rc1 = new RetrievalCriteria("Date", CriteraType.LessThan, _endDate);
            

            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
           
            rcList.Add(rc);
            rcList.Add(rc1);
          
            if (SelectedStudyType != null)
            {
                RetrievalCriteria rc2 = new RetrievalCriteria("StudyTypeID", CriteraType.Equals, SelectedStudyType.ID);
                rcList.Add(rc2);
            }
           
            RetrievedStudies = DesktopApplication.GetLibrarian().GetItems(typeof(Study), rcList);
            //RetrievedStudies =  Application.GetLibrarian().GetViewModels(typeof(Study), rcList);
            StudiesView = CollectionViewSource.GetDefaultView(RetrievedStudies);
            StudiesView.Filter = new Predicate<object>(FilterStudy);
        }
        public override void GetData()
        {
            base.GetData();
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);


            foreach (IStudyListTool t in StudyTools)
            {
                t.SetStudyList(this);
            }
        }

        public ICollectionView StudiesView
        {
            get
            {
                return _studiesView;
            }
            set
            {
                _studiesView = value;
                RaisePropertyChanged("StudiesView");
            }
        }

        private bool FilterStudy(object o)
        {
            Study s = (Study)o;
            if (s.IsCancelled)
                return false;
            else
                return true;
        }

    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
