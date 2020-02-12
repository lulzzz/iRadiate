using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PdfiumViewer;

using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Whiteboard.Common;


namespace iRadiate.Scanbag.Common.ViewModel
{
    [PreferredView("iRadiate.Scanbag.Common.View.FileScanBagItemView","iRadiate.Scanbag.Common")]
    public class FileScanBagItem : ScanBagItem
    {
        protected iRadiate.DataModel.Common.File _f;
        private string _fileLocation;

        public FileScanBagItem(iRadiate.DataModel.Common.File f)
        {
            
            _f = f;
            
        }
        public override string Description
        {
            get
            {
                return _f.Description;
            }
            set
            {
                
            }
        }

        public DateTime UploadDate
        {
            get
            {
                return _f.CreationDate;
            }
        }

        public byte[] FileArray
        {
            get
            {
                return _f.Data;
            }
        }
        public string FileLocation
        {
            get
            {
                return _fileLocation;
            }
            set
            {
                _fileLocation = value;
                RaisePropertyChanged("FileLocation");
            }
        }
    }
}
