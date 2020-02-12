using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;
using NLog;

using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.Tools;
using iRadiate.Scanbag.Common.ViewModel;
using iRadiate.Desktop.Common.ViewModel;
using ControlzEx;

namespace iRadiate.Scanbag.Common
{
    public interface IScanbagTool : ITool
    {
        void ScanbagSectionChanged();

        void SetScanbag(ScanBagViewModel sb);

        int ScanbagPositionIndex { get; set; }

        bool ScanbagVisible { get; set; }

       

        
    }

    public abstract class BaseScanbagTool : BaseExecutableTool, IScanbagTool
    {
        #region privateFields
        protected ScanBagViewModel _scanbag;
        protected bool _available = false;
        #endregion

        #region constuctor
        public BaseScanbagTool():base()
        {
           
        }
        #endregion

        #region virtualMethods
        protected virtual void ScanbagSectionChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region overrides
        public override string Name
        {
            get { return "BaseScanBagTool"; }
        }
        #endregion

        #region publicMethods
        public void SetScanbag(ScanBagViewModel sb)
        {
            _scanbag = sb;
            sb.CurrentItemChanged += ScanbagSectionChanged;
        }

        public void ScanbagSectionChanged()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region protected
        protected ScanBagViewModel Scanbag
        {
            get { return _scanbag; }
        }

        #endregion


        #region publicProperties
        public virtual int ScanbagPositionIndex
        {
            get
            {
                return 1;
            }

            set
            {
                
            }
        }

        public virtual bool ScanbagVisible
        {
            get
            {
                return true;
            }
            set
            {

            }
        }
        #endregion
    }



    [Export(typeof(IScanbagTool))]
    public class PatientDetailsTool : BaseScanbagTool
    {

        public PatientDetailsTool() : base()
        {
            Available = true;
            
        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Account;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;

                return icon;
            }
        }

        protected override void Execute()
        {
            PatientViewModel vm = new PatientViewModel(_scanbag.Patient);
            DesktopApplication.MakeModalDocument(vm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.PatientView");
        }

        public override string Name
        {
            get
            {
                return "Patient details";
            }
        }

        protected override void ScanbagSectionChanged(object sender, EventArgs e)
        {
        }


        public override int ScanbagPositionIndex
        {
            get
            {
                return base.ScanbagPositionIndex;
            }

            set
            {
                base.ScanbagPositionIndex = value;
            }
        }

        public override bool ScanbagVisible
        {
            get
            {
                return true; 
            }

            set
            {
                base.ScanbagVisible = value;
            }
        }


        public override string ToolTipText
        {
            get
            {
                return "View patient";
            }
        }

    }


    //[Export(typeof(IScanbagTool))]
    //public class DeleteScanbagItemTool : BaseScanbagTool
    //{
    //    public override string Name
    //    {
    //        get
    //        {
    //            return "Delete Item";
    //        }
    //    }

    //    public override string IconSource
    //    {
    //        get
    //        {
    //            return "/iRadiate.Desktop.Common;component/Images/DeleteIcon.png";
    //        }
    //    }

    //    public override void Execute()
    //    {

    //    }

    //    protected override void ScanbagSectionChanged(object sender, EventArgs e)
    //    {
    //        base.ScanbagSectionChanged(sender, e);
    //        if (((ScanBagViewModel)sender).CurrentItem is ReportScanBagItem)
    //        {
    //            _available = false;
    //            RaisePropertyChanged("Available");
    //        }
    //        else if (((ScanBagViewModel)sender).CurrentItem is FileScanBagItem)
    //        {
    //            _available = true;
    //            RaisePropertyChanged("Available");
    //        }
    //        else if (((ScanBagViewModel)sender).CurrentItem == null)
    //        {
    //            _available = false;
    //            RaisePropertyChanged("Available");
    //        }
    //        else if (((ScanBagViewModel)sender).CurrentItem is ScanBagItem)
    //        {
    //            _available = false;
    //            RaisePropertyChanged("Available");
    //        }

    //    }
    //}


    //public class UploadFileTool : //BaseScanbagTool
    //{
    //    public override string Name
    //    {
    //        get
    //        {
    //            return "Upload file";
    //        }
    //    }

    //    public override string IconSource
    //    {
    //        get
    //        {
    //            return "/iRadiate.Scanbag.Common;component/Images/UploadIcon.png";
    //        }
    //    }

    //    public override void Execute()
    //    {

    //        UploadFileViewModel ufvm = new UploadFileViewModel(_scanbag.CurrentSection);
    //        iRadiate.Desktop.Common.DesktopApplication.MakeModalDocument(ufvm);



    //    }

    //    protected override void ScanbagSectionChanged(object sender, EventArgs e)
    //    {
    //        base.ScanbagSectionChanged(sender, e);
    //        if (((ScanBagViewModel)sender).CurrentItem is ReportScanBagItem)
    //        {
    //            _available = false;
    //            RaisePropertyChanged("Available");
    //        }
    //        else if (((ScanBagViewModel)sender).CurrentItem is FileScanBagItem)
    //        {
    //            _available = false;
    //            RaisePropertyChanged("Available");
    //        }
    //        else if (((ScanBagViewModel)sender).CurrentItem == null)
    //        {
    //            _available = true;
    //            RaisePropertyChanged("Available");
    //        }
    //        else if (((ScanBagViewModel)sender).CurrentItem is ScanBagItem)
    //        {
    //            _available = false;
    //            RaisePropertyChanged("Available");
    //        }

    //    }
    //}

    [Export(typeof(IScanbagTool))]
    public class NoteTool : BaseScanbagTool
    {
        public override string Name
        {
            get
            {
                return "Add note";
            }
        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconEntypo icon = new PackIconEntypo();
                icon.Kind = PackIconEntypoKind.NewMessage;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;

                return icon;
            }
        }

        protected override void Execute()
        {
            /*Note n = new Note();
            n.Study = _scanbag.CurrentStudy;
            n.User = iRadiate.Desktop.Common.DesktopApplication.CurrentUser;

            NoteViewModel nvm = new NoteViewModel(n);
            nvm.SaveButtonVisible = true;
            nvm.EditButtonVisible = false;
            nvm.DeleteButtonVisible = false;
            nvm.ViewDetails();*/
            

        }

        protected override void ScanbagSectionChanged(object sender, EventArgs e)
        {
            base.ScanbagSectionChanged(sender, e);
            if (((ScanBagViewModel)sender).CurrentItem is ReportScanBagItem)
            {
                _available = false;
                RaisePropertyChanged("Available");
            }
            else if (((ScanBagViewModel)sender).CurrentItem is FileScanBagItem)
            {
                _available = false;
                RaisePropertyChanged("Available");
            }
            else if (((ScanBagViewModel)sender).CurrentItem == null)
            {
                _available = true;
                RaisePropertyChanged("Available");
            }
            else if (((ScanBagViewModel)sender).CurrentItem is ScanBagItem)
            {
                _available = false;
                RaisePropertyChanged("Available");
            }

        }

        public override string ToolTipText
        {
            get
            {
                return "Add a note";
            }
        }
    }

    [Export(typeof(IScanbagTool))]
    public class StudySummaryTool : BaseScanbagTool
    {
        public StudySummaryTool() : base()
        {

        }

        public override string ToolTipText
        {
            get
            {
                return "Get printable summary";
            }
        }

        public override string Name
        {
            get
            {
                return "Study summary tool";
            }
        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconOcticons icon = new PackIconOcticons();
                icon.Kind = PackIconOcticonsKind.File;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;

                return icon;
            }
        }

        protected override void Execute()
        {
            
        }

        public override int ScanbagPositionIndex
        {
            get
            {
                return 2;
            }

            set
            {
                base.ScanbagPositionIndex = value;
            }
        }

        public override bool ScanbagVisible
        {
            get
            {
                return true;
            }

            set
            {
                base.ScanbagVisible = value;
            }
        }

        protected override void ScanbagSectionChanged(object sender, EventArgs e)
        {
            
        }
    }

    //[Export(typeof(IScanbagTool))]
    //public class DocumentWriterTool : BaseScanbagTool
    //{
    //    public override string Name
    //    {
    //        get
    //        {
    //            return "Create document";
    //        }
    //    }

    //    public override string IconSource
    //    {
    //        get
    //        {
    //            return "/iRadiate.Scanbag.Common;component/Images/DocumentWriterIcon.png";
    //        }
    //    }

    //    public override void Execute()
    //    {
    //        DocumentWriterViewModel d = new DocumentWriterViewModel(_scanbag.CurrentStudy);
    //        iRadiate.Desktop.Common.DesktopApplication.MakeDocument(d);



    //    }

    //    protected override void ScanbagSectionChanged(object sender, EventArgs e)
    //    {
    //        base.ScanbagSectionChanged(sender, e);
    //        if (((ScanBagViewModel)sender).CurrentItem is ReportScanBagItem)
    //        {
    //            _available = true;
    //            RaisePropertyChanged("Available");
    //        }
    //        else if (((ScanBagViewModel)sender).CurrentItem is FileScanBagItem)
    //        {
    //            _available = true;
    //            RaisePropertyChanged("Available");
    //        }
    //        else if (((ScanBagViewModel)sender).CurrentItem == null)
    //        {
    //            _available = true;
    //            RaisePropertyChanged("Available");
    //        }
    //        else if (((ScanBagViewModel)sender).CurrentItem is ScanBagItem)
    //        {
    //            _available = true;
    //            RaisePropertyChanged("Available");
    //        }

    //    }
    //}
}
