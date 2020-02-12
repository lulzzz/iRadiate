using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.Radiopharmacy;
using iRadiate.Common;


namespace iRadiate.Desktop.Common.ViewModel
{
    
    public class RadioactiveInventoryItemViewModel : DataStoreItemViewModel
    {
        private DateTime _preCalTime;
        private Chemical _radiopharmaceutical;
        #region constructor
        public RadioactiveInventoryItemViewModel(DataStoreItem item) : base(item)
        {
            ReadActivityCommand = new RelayCommand(ReadActivity);
            _preCalTime = CalibrationDate;
        }

        public RadioactiveInventoryItemViewModel() : base()
        {
            ReadActivityCommand = new RelayCommand(ReadActivity);
            _preCalTime = CalibrationDate;
        }
        #endregion

        #region publicProperties
        public virtual double CurrentActivity
        {
            get
            {
                return ((IRadioactive)Item).CurrentActivity;
            }
        }

        public DateTime CalibrationDate
        {
            get
            {
                return ((IRadioactive)Item).CalibrationDate;
            }
            set
            {
                ((IRadioactive)Item).CalibrationDate = value;
                RaisePropertyChanged("CalibrationDate");
                RaisePropertyChanged("CurrentActivity");
                
            }
        }

        public virtual Chemical Radiopharmaceutical
        {
            get { return _radiopharmaceutical; }
            set { _radiopharmaceutical = value; }
        }

        public double CalibrationActivity
        {
            get
            {
                return ((IRadioactive)Item).CalibrationActivity;
            }
            set
            {
                ((IRadioactive)Item).CalibrationActivity = value;
                RaisePropertyChanged("CalibrationActivity");
                RaisePropertyChanged("CurrentActivity");
            }
        }

        public DateTime ExpiryDate
        {
            get
            {
                return ((IInventory)Item).ExpiryDate;
            }
            set
            {
                ((IInventory)Item).ExpiryDate = value;
                RaisePropertyChanged("ExpiryDate");
            }
        }

        public virtual DisposalStatus Disposed
        {
            get
            {
                return (Item as IInventory).Disposed;
            }
            set
            {
                (Item as IInventory).Disposed = value;
                (Item as IInventory).DisposalDate = DateTime.Now;
                RaisePropertyChanged("Disposed");
                RaisePropertyChanged("IsDisposed");

            }
        }

        public virtual Nullable<DateTime> DisposalDate
        {
            get
            {
                return (Item as IInventory).DisposalDate;
            }
            set
            {
                
                    (Item as IInventory).DisposalDate = value;
                RaisePropertyChanged("DisposalDate");
            }
        }

        public virtual bool IsDisposed
        {
            get
            {
                if(Disposed == DisposalStatus.NotDisposed)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public virtual DateTime PreCalTime
        {
            get
            {
                return _preCalTime;
            }
            set
            {
                _preCalTime = value;
                RaisePropertyChanged("PreCalTime");
                RaisePropertyChanged("PreCalActivity");
            }
        }
        public virtual double PreCalActivity
        {
            get
            {
                if (((BaseRadioactiveInventoryItem)Item).Isotope != null)
                {
                    return CalibrationActivity * Math.Exp((-1 * ((BaseRadioactiveInventoryItem)Item).Isotope.DecayConst) * (PreCalTime - CalibrationDate).TotalSeconds);
                }
                return 0;
                //return (PreCalTime - DateTime.Now).TotalSeconds;
            }
        }
        #endregion

        #region privateMethods
        protected virtual void ReadActivity()
        {
            //Get the dose calibrator
            IDoseCalibrator doseCalibrator = DesktopApplication.MainViewModel.DoseCalibrator;
            if (doseCalibrator.IsConnected && Radiopharmaceutical != null)
            {
                CalibrationActivity = doseCalibrator.ReadActivity(Radiopharmaceutical.Isotope);
                CalibrationDate = DateTime.Now;
                //DesktopApplication.ShowToastInformation("Dose calibrator read " + CalibrationActivity.ToString() + " MBq",DesktopApplication.NotificationPosition.BottomLeft);
            }
            else
            {
                DesktopApplication.ShowDialog("Error", "Dose calibrator is not connected");
            }
        }
        #endregion

        #region commands
        public RelayCommand ReadActivityCommand { get; set; }
        #endregion
    }
}
