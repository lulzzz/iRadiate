using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;


using GalaSoft.MvvmLight.Command;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Desktop.Common;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.Radiopharmacy;

namespace iRadiate.Radiopharmacy
{
    [PreferredView("iRadiate.Radiopharmacy.View.ReconstitutionView", "iRadiate.Radiopharmacy")]
    public class ReconstitutionViewModel : DataStoreItemViewModel
    {

        #region privateFields
        private AsyncObservableCollection<IDataStoreItem> _potentialIngredients;
        private BaseBulkDose _selectedIngredient;
        private bool _isIngredientSelected;
        private TimeSpan _calibrationDate;
        private double _bdCalibrationActivity;
        private string _batchNumber;
        private double _drawnVolume, _totalVolume;
        private DateTime _bdExpiryDate;
        private double _targetActivity;
        #endregion

        #region constructor
        public ReconstitutionViewModel() : base()
        {
            
        }

        public ReconstitutionViewModel(DataStoreItem item) : base(item)
        {
            if(item is Kit)
            {
                (item as Kit).ColdKitReconstituted += ColdKitReconstituted;
            }
            RetrievalCriteria rc = new RetrievalCriteria("Radiopharmaceutical", CriteraType.Equals, (item as Kit).KitDefinition.RadioactiveIngredient);
            RetrievalCriteria rc1 = new RetrievalCriteria("ExpiryDate", CriteraType.GreaterThan, DateTime.Now);
            RetrievalCriteria rc2 = new RetrievalCriteria("IsDisposed", CriteraType.Equals, false);
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc);
            rcList.Add(rc1);
            rcList.Add(rc2);
            _potentialIngredients = DesktopApplication.Librarian.GetItems(typeof(BaseBulkDose), rcList);
            SaveDoseCommand = new RelayCommand(SaveBulkDose);
            ReadActivityCommand = new RelayCommand(ReadActivity);
            BdExpiryDate = DateTime.Now.AddDays(1);
        }

        private void ColdKitReconstituted(object sender, NewDataStoreItemEventArgs e)
        {
            //I don't think we need to do anything here
        }
        #endregion

        #region PublicProperties
        public AsyncObservableCollection<IDataStoreItem> PotentialIngredients
        {
            get
            {
                return _potentialIngredients;
            }
            set
            {
                _potentialIngredients = value;
                RaisePropertyChanged("PotentialIngredients");
            }
        }

        public BaseBulkDose SelectedIngredient
        {
            get
            {
                return _selectedIngredient;
            }
            set
            {
                _selectedIngredient = value;
                RaisePropertyChanged("SelectedIngredient");
                RaisePropertyChanged("IsIngredientSelected");
                CalibrationTime = DateTime.Now - DateTime.Today;
                BdCalibrationActivity = SelectedIngredient.CurrentActivity;
            }
        }

        public bool IsIngredientSelected
        {
            get
            {
                if(SelectedIngredient != null)
                {
                    return true;
                }
                return false;
            }
        }

        public TimeSpan CalibrationTime
        {
            get
            {
                return _calibrationDate;
            }
            set
            {
                _calibrationDate = value;
                RaisePropertyChanged("CalibrationTime");
            }
        }

        public double BdCalibrationActivity
        {
            get
            {
                return _bdCalibrationActivity;
            }
            set
            {
                _bdCalibrationActivity = value;
                RaisePropertyChanged("BdCalibrationActivity");
            }
        }

        public string BatchNumber
        {
            get
            {
                return _batchNumber;
            }
            set
            {
                _batchNumber = value;
                RaisePropertyChanged("BatchNumber");
            }
        }

        public double DrawnVolume
        {
            get
            {
                return _drawnVolume;
            }
            set
            {
                _drawnVolume = value;
                RaisePropertyChanged("DrawnVolume");
                if(_drawnVolume > TotalVolume)
                {
                    TotalVolume = _drawnVolume;
                }
            }
        }

        public double TotalVolume
        {
            get
            {
                return _totalVolume;
            }
            set
            {
                _totalVolume = value;
                RaisePropertyChanged("TotalVolume");
                if(_totalVolume < _drawnVolume)
                {
                    DrawnVolume = TotalVolume;
                }
            }
        }

        public DateTime BdExpiryDate
        {
            get
            {
                return _bdExpiryDate;
            }
            set
            {
                _bdExpiryDate = value;
                RaisePropertyChanged("BdExpiryDate");
            }
        }

        public double TargetActivity
        {
            get
            {
                return _targetActivity;
            }
            set
            {
                _targetActivity = value;
                RaisePropertyChanged("TargetActivity");
                RaisePropertyChanged("TargetVolume");
            }
        }

        public double TargetVolume
        {

            get
            {
                var x = SelectedIngredient.CalibrationActivity / SelectedIngredient.Volume;
                var k = 0.693 / SelectedIngredient.Isotope.HalfLife;
                var timeGap = (DateTime.Now - SelectedIngredient.CalibrationDate).TotalSeconds;
                var decayFactor = Math.Exp(-1 * k * timeGap);
                var currentConcentration = x * decayFactor;
                return TargetActivity / currentConcentration;
            }
        }
        #endregion

        #region overrides

        #endregion

        #region privateMethods
        private void SaveBulkDose()
        {
          
            ReconstitutedColdKit bd = (Item as Kit).ReconstituteColdKit(BdCalibrationActivity, DateTime.Today.Add(CalibrationTime), BdExpiryDate, "", DrawnVolume, SelectedIngredient, TotalVolume);
            DesktopApplication.Librarian.SaveItem(bd);
            //SelectedIngredient.Volume = SelectedIngredient.Volume - DrawnVolume;
            //SelectedIngredient.CalibrationActivity = SelectedIngredient.CalibrationActivity - BdCalibrationActivity;
            //SelectedIngredient.CalibrationDate = DateTime.Today.Add(CalibrationTime);
            //Platform.Retriever.SaveItem(SelectedIngredient);
            Close();
        }

        protected void ReadActivity()
        {
            IDoseCalibrator doseCalibrator = DesktopApplication.MainViewModel.DoseCalibrator;
            if (doseCalibrator.IsConnected)
            {
                BdCalibrationActivity = doseCalibrator.ReadActivity((Item as Kit).KitDefinition.Product.Isotope);

            }
            else
            {
                DesktopApplication.ShowDialog("Error", "Dose calibrator is not connected");
            }
        }
        #endregion

        #region Commands
        public RelayCommand SaveDoseCommand { get; set; }

        public RelayCommand ReadActivityCommand { get; set; }
        #endregion
    }
}
