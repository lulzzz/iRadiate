using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Dicom;
using Dicom.Imaging;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Whiteboard.Common;

using iRadiate.DataModel;

namespace iRadiate.Scanbag.Common.ViewModel
{

    [PreferredView("iRadiate.Scanbag.Common.View.RTFScanbagItemView", "iRadiate.Scanbag.Common")]
    public class RTFScanbagItem : FileScanBagItem
    {
        private FlowDocument _document;
        public RTFScanbagItem(iRadiate.DataModel.Common.File f)
            : base(f)
        {
           
        }

        public FlowDocument Document
        {
            get
            {
                if (_document == null)
                {
                   
                    MemoryStream fileStream = new MemoryStream(FileArray);
                    
                    _document = new FlowDocument();

                    TextRange textRange = new TextRange(_document.ContentStart, _document.ContentEnd);
                    if (textRange.CanLoad(DataFormats.Rtf))
                    {
                        //iRadiate.Desktop.Common.Application.ShowDialog("Info", "textRange.CanLoad(DataFormats.Rtf) = true");
                    }
                    else
                    {
                        //iRadiate.Desktop.Common.Application.ShowDialog("Info", "textRange.CanLoad(DataFormats.Rtf) = false");
                    }
                    
                    textRange.Load(fileStream, DataFormats.Rtf);
                    
                }
                return _document;
            }
            
        }

        
    }

    [PreferredView("iRadiate.Scanbag.Common.View.ScreencapScanbagItemView", "iRadiate.Scanbag.Common")]
    public class ScreencapScanBagItem : FileScanBagItem
    {
        private ImageSource _imageSource;
        public ScreencapScanBagItem(iRadiate.DataModel.Common.File f) : base(f)
        {

        }

        public virtual ImageSource ImageSource
        {
            get
            {

                BitmapImage biImg = new BitmapImage();
                MemoryStream ms = new MemoryStream(FileArray);
                biImg.BeginInit();
                biImg.StreamSource = ms;
                biImg.EndInit();

                ImageSource imgSrc = biImg as ImageSource;

                return imgSrc;
            }
        }
    }
}
