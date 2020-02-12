using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight.Command;

using iRadiate.Common;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.Radiopharmacy;

namespace iRadiate.Desktop.Common.ViewModel
{
    [PreferredView("iRadiate.Desktop.Common.View.ReconstitutedColdKitView", "iRadiate.Desktop.Common")]
    public class BulkDoseViewModel : RadioactiveInventoryItemViewModel
    {

        #region privateFields
        private AsyncObservableCollection<IDataStoreItem> _unitDoses;
        private double _drawnUpActivity, _drawnUpVolume;
        private DateTime _preCalTime;
        #endregion

        #region constructors
        public BulkDoseViewModel() : base()
        {
            System.Diagnostics.Debug.WriteLine("BulkDoseViewModel()....");
            AddUnitDoseCommand = new RelayCommand(MakeUnitDose);
            //PreCalTime = DateTime.Now.AddHours(0);
            
            
        }

        public BulkDoseViewModel(DataStoreItem item) : base(item)
        {
          
            AddUnitDoseCommand = new RelayCommand(MakeUnitDose);
            //PreCalTime = DateTime.Now.AddHours(0);
           foreach(BaseUnitDose u in ((BaseBulkDose)item).UnitDoses)
            {
                UnitDoses.Add(u);
            }
            
        }
        #endregion

        #region publicProperties
        public AsyncObservableCollection<IDataStoreItem> UnitDoses
        {
            get
            {
                if(_unitDoses == null)
                {
                    _unitDoses = new AsyncObservableCollection<IDataStoreItem>();
                    _unitDoses._synchronizationContext = Platform.SynchronizationContext;
                }
                return _unitDoses;
            }
            set
            {
                _unitDoses = value;
            }
        }

        public override Chemical Radiopharmaceutical
        {
            get
            {
                if(((BaseBulkDose)Item).Radiopharmaceutical != null)
                {
                    return ((BaseBulkDose)Item).Radiopharmaceutical;
                }

                return null;
            }
            set
            {
                ((BaseBulkDose)Item).Radiopharmaceutical = value;
                RaisePropertyChanged("Radiopharmaceutical");
                RaisePropertyChanged("CurrentActivity");
            }
        }
        public double DrawnUpActivity
        {
            get
            {
                return _drawnUpActivity;
            }
            set
            {
                _drawnUpActivity = value;
                if (value > 0 && ((BaseBulkDose)Item).Volume > 0)
                {
                    var v = (Item as BaseBulkDose).Volume;
                    var a = iRadiate.Common.Misc.DecayCorrecter.Decay((Item as BaseBulkDose).CalibrationDate, PreCalTime, (Item as BaseBulkDose).Isotope.HalfLife / 3600, (Item as BaseBulkDose).CalibrationActivity);
                    var c = a / v;
                    DrawnUpVolume = value / c;
                    //DrawnUpVolume = c;
                }
                
                RaisePropertyChanged("DrawnUpActivity");
                RaisePropertyChanged("PreCalActivity");
                
            }
        }

        public double DrawnUpVolume
        {
            get
            {
                return _drawnUpVolume;
            }
            set
            {
                if(value > 0)
                {
                    _drawnUpVolume = value;
                }
                
                RaisePropertyChanged("DrawnUpVolume");
            }
        }

        public override double PreCalActivity
        {
            get
            {
                if (((BaseBulkDose)Item).Isotope != null)
                {
                    return DrawnUpActivity * Math.Exp((-1 * ((BaseBulkDose)Item).Isotope.DecayConst) * (PreCalTime - DateTime.Now).TotalSeconds);
                }
                return 0;
                //return (PreCalTime - DateTime.Now).TotalSeconds;
            }
        }
        public RelayCommand AddUnitDoseCommand { get; set; }



        public virtual bool ShowQC
        {
            get { return false; }
        }
        #endregion

        #region privateMethods
        protected virtual void MakeUnitDose()
        {
            
            BaseUnitDose u = (Item as BaseBulkDose).DrawDose(DrawnUpActivity, DrawnUpVolume, DateTime.Now, DateTime.Now.AddHours(12),"", DrawnUpVolume);
            UnitDoses.Add(u);
        }

        #endregion

        #region overrides
        public override void SaveItem()
        {
            if (((BaseBulkDose)Item).UnitDoses.Where(x => x.ID == 0).Any())
            {
                foreach(BaseUnitDose u in ((BaseBulkDose)Item).UnitDoses.Where(x => x.ID == 0))
                {
                    DesktopApplication.Librarian.SaveItem(u);
                }
            }
            base.SaveItem();
        }
        #endregion
    }

    [PreferredView("iRadiate.Desktop.Common.View.ReconstitutedColdKitView", "iRadiate.Desktop.Common")]
    public class ElutionViewModel: BulkDoseViewModel
    {
        public ElutionViewModel() : base()
        {

        }

        public ElutionViewModel(DataStoreItem item) : base(item)
        {

        }
    }

    [PreferredView("iRadiate.Desktop.Common.View.ReconstitutedColdKitView", "iRadiate.Desktop.Common")]
    public class ReconstitutedColdKitViewModel : BulkDoseViewModel
    {
        private ObservableCollection<RadiochemicalPurityMeasurement> _measurements;
        private bool _qcExists;

        #region Constructors
        public ReconstitutedColdKitViewModel() : base()
        {

        }

        public ReconstitutedColdKitViewModel(DataStoreItem item) : base(item)
        {
            InsertQCAnalysisCommand = new RelayCommand(CreatQCAnalysis);
            AddQCMeasurementCommand = new RelayCommand(AddQCMeasurement);
            if ((Item as ReconstitutedColdKit).QCAnalysis != null)
                QCExists = true;

            if((Item as ReconstitutedColdKit).QCAnalysis != null)
            {
                foreach (RadiochemicalPurityMeasurement m in ((Item as ReconstitutedColdKit).QCAnalysis.Measurements))
                {
                    Measurements.Add(m);
                }
            }
            
        }
        #endregion
         
        #region privateMethods
        private void CreatQCAnalysis()
        {
            RadiochemicalPurityAnalysis analysis;
            if((Item as ReconstitutedColdKit).QCAnalysis == null)
            {
                (Item as ReconstitutedColdKit).QCAnalysis = new RadiochemicalPurityAnalysis();

            }
            analysis = (Item as ReconstitutedColdKit).QCAnalysis;
            QCExists = true;
        }
        private void AddQCMeasurement()
        {
            if ((Item as ReconstitutedColdKit).QCAnalysis == null)
            {
                return;

            }

            RadiochemicalPurityMeasurement m = new RadiochemicalPurityMeasurement();
            m.Impurity = "???";
            (Item as ReconstitutedColdKit).QCAnalysis.Measurements.Add(m);
            Measurements.Add(m);
            RaisePropertyChanged("Measurements");
        }
        #endregion

        #region publicProperties
        public double PurityValue
        {
            get
            {
                if ((Item as ReconstitutedColdKit).QCAnalysis == null)
                {
                    return 0;

                }
                else
                {
                    RadiochemicalPurityAnalysis analysis = (Item as ReconstitutedColdKit).QCAnalysis;
                    return 100 - analysis.Measurements.Sum(x => x.ImpurityFraction);
                }
            }
        }

        public bool RadiochemicalPurityResult
        {
            get
            {
                if ((Item as ReconstitutedColdKit).QCAnalysis != null)
                    return (Item as ReconstitutedColdKit).QCAnalysis.Pass;
                else
                    return false;
            }
            set
            {
                (Item as ReconstitutedColdKit).QCAnalysis.Pass = value;
                RaisePropertyChanged("RadiochemicalPurityResult");
            }
        }

        public override bool ShowQC
        {
            get
            {
                return true;
            }
        }

        public virtual ObservableCollection<RadiochemicalPurityMeasurement> Measurements
        {
            get
            {
                if (_measurements == null)
                    _measurements = new ObservableCollection<RadiochemicalPurityMeasurement>();
                return _measurements;
            }
            set
            {
                _measurements = value;
            }
        }

        public bool QCExists
        {
            get { return _qcExists; }
            set { _qcExists = value;  RaisePropertyChanged("QCExists"); RaisePropertyChanged("QCNotExists"); }
        }

        public bool QCNotExists
        {
            get { return !QCExists; }
        }
        #endregion

        #region commands
        public RelayCommand InsertQCAnalysisCommand { get; set; }

        public RelayCommand AddQCMeasurementCommand { get; set; }
        #endregion
    }
}
