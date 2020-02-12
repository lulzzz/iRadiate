using System;
using System.Collections.Generic;
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
    [PreferredView("iRadiate.Scanbag.Common.View.XPSScanBagItemView", "iRadiate.Scanbag.Common")]
    public class XPSScanbagItem : FileScanBagItem
    {
        public XPSScanbagItem(iRadiate.DataModel.Common.File f)
            : base(f)
        {

        }

         
    }
}
