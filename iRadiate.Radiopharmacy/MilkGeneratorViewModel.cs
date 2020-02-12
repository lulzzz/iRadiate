using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.Radiopharmacy;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;


namespace iRadiate.Radiopharmacy
{
    [PreferredView("iRadiate.Radiopharmacy.View.MilkGeneratorView", "iRadiate.Radiopharmacy")]
    public class MilkGeneratorViewModel : GenericViewModel
    {
        private Generator _generator;
        private double _volume, _breakthrough, _activity;
        private DateTime _elutionDate, _expiryDate;
        private string _batchNumber;
  

        public MilkGeneratorViewModel(Generator g)
        {
            _generator = g;
            _volume = 0;
            _breakthrough = 0;
            _activity = 0;
            _elutionDate = DateTime.Now;
            _expiryDate = DateTime.Now.AddDays(1);

            SaveElutionCommand = new RelayCommand(SaveElution);
        }
        public Generator Generator
        {
            get { return _generator; }
            set { _generator = value;  RaisePropertyChanged("Generator"); }
        }

        public RelayCommand SaveElutionCommand { get; set; }
        
        private void SaveElution()
        {
            Generator.Elute(_volume, _activity, _breakthrough, _elutionDate, _expiryDate, _batchNumber);
            DesktopApplication.Librarian.SaveItem(Generator);
            OnViewModelClosing();
        } 

        public double Volume
        {
            get { return _volume; }
            set { _volume = value; RaisePropertyChanged("Volume"); }
        }

        public double Breakthrough
        {
            get { return _breakthrough; }
            set { _breakthrough = value;  RaisePropertyChanged("Breakthrough"); }
        }

        public double Activity
        {
            get { return _activity; }
            set { _activity = value; RaisePropertyChanged("Activity"); }
        }

        public string BatchNumber
        {
            get { return _batchNumber; }
            set { _batchNumber = value; RaisePropertyChanged("BatchNumber"); }            
        }

        public DateTime ElutionDate
        {
            get { return _elutionDate; }
            set { _elutionDate = value; RaisePropertyChanged("ElutionDate"); }
        }

        public DateTime ExpiryDate
        {
            get { return _expiryDate; }
            set { _expiryDate = value; RaisePropertyChanged("ExpiryDate"); }
        }
    }
}
