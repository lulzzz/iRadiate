using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;

using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Desktop.Search;
using iRadiate.Diary.Common;
using iRadiate.Whiteboard.Common;
using ControlzEx;

namespace iRadiate.Scanbag.Common.ViewModel
{

    public delegate void CurrentItemChangedEventHandler(object sender, EventArgs e);

    [PreferredView("iRadiate.Scanbag.Common.View.ScanBagView", "iRadiate.Scanbag.Common")]
    public class ScanBagViewModel : Module
    {
        #region privateFields
        private Patient _patient;
        private List<ScanBagSection> _scanBagSections;
        private ScanBagItem _currentItem;
        private Study _currentStudy;
        private ScanBagSection _currentSection;
        #endregion

        [ImportMany(typeof(IScanbagTool))]
        private List<IScanbagTool> _tools;

        #region overrides
        public override string Name
        {
            get
            {
                return _patient.FullName;
            }
            set
            {
               
            }
        }
        public override string IconSource
        {
            get { return "/iRadiate.ScanBag.Common;component/Images/ScanBagIcon.png"; }
        }

        public override ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconModern icon = new PackIconModern();
                icon.Kind = PackIconModernKind.FolderPeople;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }
        #endregion

        #region constructors
        public ScanBagViewModel()
        {
            
        }

        public ScanBagViewModel(Patient patient)
        {
            _patient = patient;
            
            GetData();
            CreateScanBagSections();
            UIThreadInitialize();
        }

        public override void GetData()
        {
            
        }

        public override void UIThreadInitialize()
        {
            base.UIThreadInitialize();
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            foreach (IScanbagTool t in Tools)
            {
                t.SetScanbag(this);
                t.NonUIThreadInitialise();
                t.UIThreadInitialise();
            }
        }
        #endregion

        #region publicProperties
        public Patient Patient
        {
            get
            {
                return _patient;
            }
            set
            {
                _patient = value;
                RaisePropertyChanged("Patient");
                CreateScanBagSections();
            }
        }

        public List<ScanBagSection> ScanBagSections
        {
            get
            {
                if (_scanBagSections == null)
                {
                    _scanBagSections = new List<ScanBagSection>();
                }
                return _scanBagSections;
            }
            set
            {
                _scanBagSections = value;
                RaisePropertyChanged("ScanBagSections");
            }
        }

        public ScanBagItem CurrentItem
        {
            get
            {
                return _currentItem;
            }
            set
            {
                _currentItem = value;
                RaisePropertyChanged("CurrentItem");
                OnChanged(new EventArgs());
            }
        }

        public Study CurrentStudy
        {
            get
            {
                return _currentStudy;
            }
            set
            {
                _currentStudy = value;
                RaisePropertyChanged("CurrentStudy");
                OnChanged(new EventArgs());
            }
        }

        public ScanBagSection CurrentSection
        {
            get
            {
                return _currentSection;
            }
            set
            {
                _currentSection = value;
                RaisePropertyChanged("CurrentSection");

            }
        }
        

        public List<IScanbagTool> Tools
        {
            get
            {
                return _tools;
            }
            set
            {
                _tools = value;
                RaisePropertyChanged("Tools");
            }
        }
        
        #endregion

        #region privateMethods
        private void AddScanBagItem(ScanBagItem newItem)
        {

        }
        private void CreateScanBagSections()
        {
            if (_patient == null)
            {
                return;
            }
            if (!_patient.Studies.Any())
            {
                return;
            }

            foreach(Study s in Patient.Studies.Where(x=>x.Deleted == false))
            {
                ScanBagSection sec = new ScanBagSection(s);
                ScanBagSections.Add(sec);
                sec.ScanBag = this;

            }
        }
        #endregion

        #region events
        public event CurrentItemChangedEventHandler CurrentItemChanged;

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnChanged(EventArgs e)
        {
            if (CurrentItemChanged != null)
                CurrentItemChanged(this, e);
        }
        #endregion
    }

    [Export(typeof(IWhiteboardTool))]
    public class OpenScanBagTool : BaseWhiteboardTool
    {
        public OpenScanBagTool()
            : base()
        {

        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconModern icon = new PackIconModern();
                icon.Kind = PackIconModernKind.FolderPeople;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }
       
        public override string Name
        {
            get
            {
                return "Open scan bag";
            }
        }
        protected override void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            if (Whiteboard.SelectedAppointment != null)
            {
                Available = true;
            }
            else
            {
                Available = false;
            }
        }
       
        protected override void Execute()
        {
            //Application.ShowDialog("Info", "Execute()...");
            base.Execute();
            Study s = (Whiteboard.SelectedAppointment as Appointment).Study;
            ScanBagViewModel svm = new ScanBagViewModel(s.Patient);
            
            DesktopApplication.MakeDocument(svm);
        }
        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.WhiteboardScanbagToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.WhiteboardScanbagToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }
        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.WhiteboardScanbagToolVisible;
            }

            set
            {
                Properties.Settings.Default.WhiteboardScanbagToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }
    }

    [Export(typeof(IStudyListTool))]
    public class ScanbagStudyListTool : BaseStudyListTool
    {
        public ScanbagStudyListTool()
            : base()
        {

        }

        public override ContentControl ContentSource
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconModern icon = new PackIconModern();
                icon.Kind = PackIconModernKind.FolderPeople;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }

            set
            {
                //base.ContentSource = value;
            }
        }
        public override string IconSource
        {
            get
            {
                return "/iRadiate.Scanbag.Common;component/Images/ScanBagIcon.png";
            }
        }

        public override string Name
        {
            get
            {
                return "Patient scan bag";
            }
        }

        public override void Execute()
        {
            //Application.ShowDialog("Info", "Execute()...");
            //base.Execute();
            Study s = (Study)(StudyList.SelectedStudy);
            ScanBagViewModel svm = new ScanBagViewModel(s.Patient);
            //Application.ShowDialog("Info", "sVM instantiated");
            DesktopApplication.MakeDocument(svm);
        }

        protected override void _studyList_SelectionChanged(object sender, EventArgs e)
        {
            if (StudyList.SelectedStudy != null)
            {
                Available = true;
            }
            else
            {
                Available = false;
            }
        }
    }

    [Export(typeof(IPatientListTool))]
    public class ScanbagPatientListTool : BasePatientListTool
    {
        public ScanbagPatientListTool()
            : base()
        {

        }
        public override string IconSource
        {
            get
            {
                return "/iRadiate.Scanbag.Common;component/Images/ScanBagIcon.png";
            }
        }

        public override string Name
        {
            get
            {
                return "Patient scan bag";
            }
        }

        public override void Execute()
        {
            //Application.ShowDialog("Info", "Execute...");
            Patient p = (Patient)PatientList.SelectedPatient;
            ScanBagViewModel svm = new ScanBagViewModel(p);
            //Application.ShowDialog("Info", "sVM instantiated");
            DesktopApplication.MakeDocument(svm);
        }
    }

    [Export(typeof(IDiaryTool))]
    public class DiaryOpenScanBagTool : BaseDiaryTool
    {
        public DiaryOpenScanBagTool() : base()
        {
            Available = false;
        }

        protected override void DiarySelectedItemChanged(object sender, EventArgs e)
        {
            if(DiaryViewModel.SelectedEvent == null)
            {
                Available = false;
            }
            else
            {
                Available = true;
            }
        }

        public override int DiaryPositionIndex
        {
            get
            {
                return Properties.Settings.Default.DiaryScanbagToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.DiaryScanbagToolPositionIndex = value;
                RaisePropertyChanged("DiaryPositionIndex");
            }
        }

        public override bool DiaryVisible
        {
            get
            {
                return Properties.Settings.Default.DiaryScanbagToolVisible;
            }

            set
            {
                Properties.Settings.Default.DiaryScanbagToolVisible = value;
                RaisePropertyChanged("DiaryVisible");
            }
        }

        protected override void Execute()
        {
            
            var s = DiaryViewModel.SelectedEvent;
            if(s.TypeOfevent == Diary.Common.ViewModel.DiaryEventWrapper.EventType.Scan)
            {
                var scan = s.Event as ScanTask;
                ScanBagViewModel svm = new ScanBagViewModel(scan.Patient);

                DesktopApplication.MakeDocument(svm);
            }else
            {
                DesktopApplication.ShowDialog("Info", "Could not execute");
            }
            
           
        }

        public override string Name
        {
            get
            {
                return "Scanbag tool";
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Open scanbag";
            }
        }

        protected override PackIconBase Icon
        {
            get
            {
                var i = new PackIconModern();
                i.Kind = PackIconModernKind.FolderPeople;
                i.Width = DesktopApplication.IconWidth;
                i.Height = DesktopApplication.IconHeight;
                return i;
            }
        }
    }
}
