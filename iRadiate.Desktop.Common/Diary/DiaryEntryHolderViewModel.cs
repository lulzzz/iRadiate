using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using NLog;
using iRadiate.Common;
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Common.Diary
{
    [Obsolete]
    public class DiaryEntryHolderViewModel : ViewModelBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private DateTime _startTime;
        private DateTime _endTime;
        private int _height;
        private AsyncObservableCollection<DataStoreItemViewModel> _tasks;
        private int _width = 400;
        private DateTime _minStart;
        private DateTime _maxStart;
        private ICollectionView _tasksView;
        private string _viewTitle;
        private bool _sliderVisible = true;
        private bool _isVisible = true;
        private bool _isEditable = false;
        private string _propertyName;
        private int _idNum;
        private DataStoreItemViewModel _thisItem;
        private int _order;
        private ObservableCollection<DataStoreItemViewModel> _alteredTasks;

        public void SaveChanges()
        {
            foreach (DataStoreItemViewModel dt in AlteredTasks)
            {
                dt.SaveItem();
            }
        }

        public ObservableCollection<DataStoreItemViewModel> AlteredTasks
        {
            get
            {
                if (_alteredTasks == null)
                {
                    _alteredTasks = new ObservableCollection<DataStoreItemViewModel>();
                }
                return _alteredTasks;
            }
            set
            {
                _alteredTasks = value;
                RaisePropertyChanged("AlteredTasks");
            }
        }

        public int Order
        {
            get
            {
                return _order;
            }
            set
            {
                _order = value;
                RaisePropertyChanged("Order");
            }
        }

        public DataStoreItemViewModel ThisItem
        {
            get
            {
                return _thisItem;
            }
            set
            {
                _thisItem = value;
                RaisePropertyChanged("ThisItem");
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
                RaisePropertyChanged("PropertyName");
            }
        }

        public int IdNum
        {
            get
            {
                return _idNum;
            }
            set
            {
                _idNum = value;
                RaisePropertyChanged("IdNum");
            }
        }

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                RaisePropertyChanged("IsVisible");
            }
        }

        public bool IsEditable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
                RaisePropertyChanged("IsEditable");
            }
        }

        public DateTime MinStart
        {
            get
            {
                return _minStart;
            }
            set
            {
                _minStart = value;
                RaisePropertyChanged("MinStart");
            }
        }

        public DateTime MaxEnd
        {
            get
            {
                return _maxStart;
            }
            set
            {
                _maxStart = value;
                RaisePropertyChanged("MaxEnd");
            }
        }

        private ObservableCollection<DiaryTimeTick> _ticks;

        public ObservableCollection<DiaryTimeTick> Ticks
        {
            get
            {
                if (_ticks == null)
                {
                    _ticks = new ObservableCollection<DiaryTimeTick>();
                }
                return _ticks;
            }
            set
            {
                _ticks = value;
                RaisePropertyChanged("Ticks");
            }
        }

        public DiaryEntryHolderViewModel(ICollectionView tasks, DateTime min, DateTime max)
        {
            logger.Trace("DiaryEntryHolderViewModel()...");
            MinStart = min;
            MaxEnd = max;
            _tasksView = tasks;
            logger.Trace("DiaryEntryHolderViewModel()...Done");
           
            
        }

        public bool SliderVisible
        {
            get
            {
                return _sliderVisible;
            }
            set
            {
                _sliderVisible = value;
                RaisePropertyChanged("SliderVisible");
            }
        }

        void tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
           
            
            //RefreshDiaryEvents();
        }

        public void InitializeTicks()
        {
            logger.Trace("InitializeTicks()...");

            for (int j = 0; j < 10; j=j+1)
            {
                DiaryTimeTick dtt = new DiaryTimeTick(this);
                dtt.Top = j * 80;
                Ticks.Add(dtt);
            }
            logger.Trace("InitializeTicks()...Done");
                
        }

       

        public void RefreshDiaryEvents()
        {

            logger.Trace("RefreshDiaryEvents()...");
            
            TasksView.Refresh();
           
            
            RaisePropertyChanged("TasksView");
            logger.Trace("RefreshDiaryEvents()...Done");
        }

        public ICollectionView TasksView
        {
            get
            {
                return _tasksView;
            }
            set
            {
                _tasksView = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                RaisePropertyChanged("StartTime");
                if (TasksView != null)
                {
                foreach (object devm in TasksView.SourceCollection)
                {
                    ((DataStoreItemViewModel)devm).RaisePropertyChanged("Duration");
                   ((DataStoreItemViewModel)devm).RaisePropertyChanged("SchedulingTime");

                }
                }
                
                foreach (DiaryTimeTick dtt in Ticks)
                {
                    dtt.RaisePropertyChanged("TickTime");
                }
            }
        }

        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
                RaisePropertyChanged("EndTime");

                if (TasksView != null)
                {
                    foreach (object devm in TasksView.SourceCollection)
                    {
                        ((DataStoreItemViewModel)devm).RaisePropertyChanged("Duration");
                        ((DataStoreItemViewModel)devm).RaisePropertyChanged("SchedulingTime");

                    }
                }
                
                foreach (DiaryTimeTick dtt in Ticks)
                {
                    dtt.RaisePropertyChanged("TickTime");
                }
            }
        }

        public double MinutesPerPixel
        {
            get
            {
                
                return (((_endTime - _startTime).TotalMinutes) / 800);
            }
        }

        public int Height
        {
            get
            {
                return 800;
            }
            set
            {
                _height = value;
                RaisePropertyChanged("Height");
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                RaisePropertyChanged("Width");
            }
        }

        public string ViewTitle
        {
            get
            {
                return _viewTitle;

            }
            set
            {
                _viewTitle = value;
                RaisePropertyChanged("ViewTitle");
            }
        }
    }

    [Obsolete]
    public class DiaryTimeTick : ViewModelBase
    {
        private DateTime _tickTime;
        private int _top;
        private DiaryEntryHolderViewModel _holder;

        public DiaryTimeTick(DiaryEntryHolderViewModel holder)
        {
            _holder = holder;
        }

        public DateTime TickTime
        {
            get 
            {
                return _holder.StartTime.AddMinutes((_holder.EndTime - _holder.StartTime).TotalMinutes * (Convert.ToDouble(_top) / 800));
                
            
            }
           
        }


        public int Top
        {
            get
            {
                return _top;
            }
            set { _top = value; RaisePropertyChanged("Top"); }
        }
    }

   

}
