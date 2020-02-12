using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO.Ports;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.Setup;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.Radiopharmacy;
using GalaSoft.MvvmLight.Command;


namespace iRadiate.Interfaces.Capintec
{
    [Export(typeof(ISettingsProvider))]
    [PreferredView("iRadiate.Interfaces.Capintec.View.CapintecSettingsView", "iRadiate.Interfaces.Capintec")]
    public class CapintecSettingsProvider : SettingsProvider
    {
       
        private string _output, _input, _portStatus, _currentActivity;
        private CapintecDoseCalibrator _doseCalibrator;
        private ObservableCollection<IsotopeChannelNumber> _isotopeChannelNumbers;
        private List<IDataStoreItem> _availableIsotopes;

        #region constructor
        public CapintecSettingsProvider() : base()
        {
            OpenPortCommand = new RelayCommand(openPort);
            WriteToPortCommand = new RelayCommand(WriteToPort);
            ReadActivityCommand = new RelayCommand(ReadActivity);
            AddIsotopeCommand = new RelayCommand(AddIsotope);
            SaveChannelsCommand = new RelayCommand(SaveChannels);

            string [] portName = SerialPort.GetPortNames();
            foreach(string s in portName)
            {
                Output = Output + s + System.Environment.NewLine;
            }
            DoseCalibrator = DesktopApplication.MainViewModel.DoseCalibrator as CapintecDoseCalibrator;
            foreach(var c in DoseCalibrator.IsotopeChannelNumbers)
            {
                IsotopeChannelNumbers.Add(c);
            }
            _availableIsotopes = Platform.Retriever.RetrieveItems(typeof(Isotope), new List<RetrievalCriteria>());
            
        }
        #endregion

        #region publicProperties
        
        public ObservableCollection<IsotopeChannelNumber> IsotopeChannelNumbers
        {
            get
            {
                if (_isotopeChannelNumbers == null)
                    _isotopeChannelNumbers = new ObservableCollection<IsotopeChannelNumber>();
                return _isotopeChannelNumbers;
            }
            set
            {
                _isotopeChannelNumbers = value;
                RaisePropertyChanged("IsotopeChannelNumbers");
            }
        }
        public string Output
        {
            get { return _output; }
            set { _output = value; RaisePropertyChanged("Output"); }
        }

        public string Input
        {
            get { return _input; }
            set { _input = value; RaisePropertyChanged("Input"); }
        }

        public string PortStatus
        {
            get
            {
                if (DoseCalibrator == null)
                    return "No calibrator";
                if (DoseCalibrator.Port == null)
                    return "Port is null";
                if (DoseCalibrator.PortOpen)
                    return "Port open";

                return "Port closed";
            }
           
        }

        public string CurrentActivity
        {
            get { return _currentActivity; }
            set { _currentActivity = value; RaisePropertyChanged("CurrentActivity"); }
        }

        public CapintecDoseCalibrator DoseCalibrator
        {
            get { return _doseCalibrator; }
            set { _doseCalibrator = value;  RaisePropertyChanged("DosecCalibrator"); }
        }
        public List<IDataStoreItem> AvailableIsotopes
        {
            get { return _availableIsotopes; }
            set { _availableIsotopes = value; }
        }
        #endregion

        #region overrides
        public override string Name
        {
            get
            {
                return "Capintec Settings";
            }
        }

       
        #endregion

        #region privateMethods
        private void openPort()
        {
            DoseCalibrator.OpenPort();
            RaisePropertyChanged("PortStatus");
            
            
            
        }

        private void WriteToPort()
        {
            try
            {
                DoseCalibrator.IssueCommand(Input);
                
            }
            catch
            {
                Output = "Unable to write to port";
            }
            try
            {
                Output = DoseCalibrator.ReadLine();
            }
            catch
            {
                Output = "Unable to read from port";
            }
            
        }

        private void ReadActivity()
        {
            CurrentActivity =  DoseCalibrator.ReadActivity().ToString() + " MBq";
        }

        private void AddIsotope()
        {
            IsotopeChannelNumber n = new IsotopeChannelNumber();
            IsotopeChannelNumbers.Add(n);
            DoseCalibrator.IsotopeChannelNumbers.Add(n);
        }

        private void SaveChannels()
        {
            DoseCalibrator.SaveIsotopeChamberNumbers();
        }
        #endregion

        #region commands
        public RelayCommand OpenPortCommand { get; set; }

        public RelayCommand WriteToPortCommand { get; set; }

        public RelayCommand ReadActivityCommand { get; set; }

        public RelayCommand AddIsotopeCommand { get; set; }

        public RelayCommand SaveChannelsCommand { get;set;}
        #endregion

    }

}
