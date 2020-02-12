using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Whiteboard.Common;

namespace iRadiate.Scanbag.Common.ViewModel
{
    [PreferredView("iRadiate.Scanbag.Common.View.UploadFile", "iRadiate.Scanbag.Common")]
    public class UploadFileViewModel : Module
    {
        private string _uploadDescription;
        private string _uploadFileName;
        private ScanBagSection _scanbagSection;

        public UploadFileViewModel()
        {
            ChooseFileCommand = new RelayCommand(ChooseFile);
            UploadFileCommand = new RelayCommand(uploadFile);
        }

        public UploadFileViewModel(ScanBagSection sec)
        {

            _scanbagSection = sec;
            ChooseFileCommand = new RelayCommand(ChooseFile);
            UploadFileCommand = new RelayCommand(uploadFile);
        }
        public string UploadDescription
        {
            get
            {
                return _uploadDescription;
            }
            set
            {
                _uploadDescription = value;
                RaisePropertyChanged("UploadDescription");
            }
        }
        public string UploadFileName
        {
            get
            {
                return _uploadFileName;
            }
            set
            {
                _uploadFileName = value;
                RaisePropertyChanged("UploadFileName");
            }
        }

        private void ChooseFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                UploadFileName = ofd.FileName;
            }
        }

        public RelayCommand ChooseFileCommand
        {
            get;
            private set;
        }

        public RelayCommand UploadFileCommand
        {
            get;
            private set;
        }

        public string ScanBagName
        {
            get
            {
                return _scanbagSection.ScanBag.Patient.FullName;
            }
        }

        public string SectionName
        {
            get 
            {
                return _scanbagSection.Description;
            }
        }

        private void uploadFile()
        {
            _scanbagSection.UploadFile(UploadFileName, UploadDescription);
            Close();

        }

        public override string Name
        {
            get
            {
                return "Upload File";
            }

            set
            {
                base.Name = value;
            }
        }
    }
}
