using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;

using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Radiopharmacy;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Whiteboard.Common;
using ControlzEx;

namespace iRadiate.Radiopharmacy
{

    
    public class PYPAdmininistrationTool : BaseWhiteboardTool
    {
        
        public PYPAdmininistrationTool()
            : base()
        {

        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Gate;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }
        
        protected override void Execute()
        {
            base.Execute();
           
            Appointment a = Whiteboard.SelectedAppointment as Appointment;
            a.Comments = "PYP Injected @ " + DateTime.Now + "; " + a.Comments;
            AppointmentViewModel avm = new AppointmentViewModel(a);
            DesktopApplication.MakeModalDocument(avm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.CommentsView");

        }

        public override string Name
        {
            get
            {
                return "Record PYP injetion";
            }
        }

        protected override void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            if (Whiteboard.SelectedAppointment != null)
            {
                Appointment a = Whiteboard.SelectedAppointment as Appointment;
                if(a.Completed)
                {
                    Available = false;
                    return;
                }
                if (a.Cancelled)
                {
                    Available = false;
                    return;
                }
                Available = true;
            }
            else
            {
                Available = false;
            }
        }

        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.PYPAdministrationToolPositionIndex;
            }
            set
            {
                Properties.Settings.Default.PYPAdministrationToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.PYPAdministrationToolVisible;
            }

            set
            {
                Properties.Settings.Default.PYPAdministrationToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }
    }

    [Export(typeof(IWhiteboardTool))]
    public class GasAdmininistrationTool : BaseWhiteboardTool
    {

        public GasAdmininistrationTool()
            : base()
        {

        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.GasCylinder;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }
        
        protected override void Execute()
        {
            base.Execute();
            Appointment a = Whiteboard.SelectedAppointment as Appointment;
            DoseAdministrationTask dat = a.Tasks.Where(x => x.Deleted == false && x.Completed == false && x is DoseAdministrationTask).Where(j => (j as DoseAdministrationTask).PrescribedRadioPharmaceutical.IsGaseous == true).First() as DoseAdministrationTask;
            RadioactiveGasModule mod = new RadioactiveGasModule(dat);
            DesktopApplication.MakeModalDocument(mod);

           //Figure this out later

        }

        public override string Name
        {
            get
            {
                return "Administer radioactive gas";
            }
        }

        protected override void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            if (Whiteboard.SelectedAppointment != null)
            {
                Appointment a = Whiteboard.SelectedAppointment as Appointment;
                if (a.Completed)
                {
                    Available = false;
                    return;
                }
                if (a.Cancelled)
                {
                    Available = false;
                    return;
                }
                if (a.Tasks.Where(x=>x.Deleted == false && x.Completed==false && x is DoseAdministrationTask).Where(j=>(j as DoseAdministrationTask).PrescribedRadioPharmaceutical.IsGaseous == true).Any())
                {
                    Available = true;
                }
                else
                {
                    Available = false;
                }
                
            }
            else
            {
                Available = false;
            }
        }

        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.GasAdministrationToolPositionIndex;
            }
            set
            {
                Properties.Settings.Default.GasAdministrationToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.GasAdministrationToolVisible;
            }

            set
            {
                Properties.Settings.Default.GasAdministrationToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Administer radioactive gas to patient";
            }
        }
    }

   [Export(typeof(IWhiteboardTool))]
    public class DrawDoseWhiteboardTool : BasesplitButtonWhiteboardTool
    {
        protected DoseAdministrationTask dat;
        protected BaseBulkDose bulkDose;
        public DrawDoseWhiteboardTool() : base()
        {

        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconModern icon = new PackIconModern();
                icon.Kind = PackIconModernKind.DrawPencilReflection;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }
      
        protected override void Execute()
        {
            base.Execute();
            DrawDoseViewModel vm = new DrawDoseViewModel((DataStoreItem)bulkDose);

            DesktopApplication.MakeModalDocument(vm, "iRadiate.Radiopharmacy", "iRadiate.Radiopharmacy.View.DrawDoseView");
            vm.SelectedTask = dat as IDataStoreItem;

            

        }

        public override string Name
        {
            get
            {
                return "Draw dose for patient";
            }
        }

        protected override void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            if (Whiteboard.SelectedAppointment != null)
            {
                Appointment a = Whiteboard.SelectedAppointment as Appointment;
                if (a.Completed)
                {
                    Available = false;
                    return;
                }
                if (a.Cancelled)
                {
                    Available = false;
                    return;
                }
                if (a.Tasks.Where(x => x.Deleted == false && x.IsCancelled == false && x.Completed == false && x is DoseAdministrationTask).Where(y=>(y as DoseAdministrationTask).UnitDose == null).Any())
                {
                    //do we have a bulk dose to draw from?
                    //Get the next dose administration task
                    dat = a.Tasks.Where(x => x.Deleted == false && x.IsCancelled == false && x.Completed == false && x is DoseAdministrationTask).Where(y => (y as DoseAdministrationTask).UnitDose == null).OrderBy(y => y.SchedulingTime).First() as DoseAdministrationTask;
                    //Get the list of bulk doses which are not expired and have the right radiopharmaceuical
                    RetrievalCriteria rc1 = new RetrievalCriteria("Expired", CriteraType.Equals, false);
                    RetrievalCriteria rc2 = new RetrievalCriteria("Radiopharmaceutical", CriteraType.Equals, dat.PrescribedRadioPharmaceutical);
                    RetrievalCriteria rc3 = new RetrievalCriteria("IsDisposed", CriteraType.Equals, false);
                    List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
                    rcList.Add(rc1);
                    rcList.Add(rc2);
                    rcList.Add(rc3);
                    var bulkDoses = DesktopApplication.Librarian.DataRetriever.RetrieveItems(typeof(BaseBulkDose), rcList);
                    if (bulkDoses.Any())
                    {
                        bulkDose = bulkDoses.First() as BaseBulkDose;
                        Available = true;
                    }
                    else
                    {
                        Available = false;
                    }
                    
                }
                else
                {
                    Available = false;
                }

            }
            else
            {
                Available = false;
            }
        }

        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.DrawDoseWhiteboardToolPositionIndex;
            }
            set
            {
                Properties.Settings.Default.DrawDoseWhiteboardToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.DrawDoseWhiteboardToolVisible;
            }

            set
            {
                Properties.Settings.Default.DrawDoseWhiteboardToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Draw a dose for the patient";
            }
        }

        public override void UIThreadInitialise()
        {
            Border b = new Border();
            b.Background = Brushes.White;
            b.BorderBrush = Brushes.LightGray;
            b.BorderThickness = new System.Windows.Thickness(3);
            b.Padding = new System.Windows.Thickness(3);
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;


            Button splitButton = new Button();
            splitButton.FontSize = 14;
            splitButton.Content = "Split dose";
            splitButton.Command = DrawSplitDoseCommand;
           
            splitButton.Padding = new System.Windows.Thickness(3);
            splitButton.Margin = new System.Windows.Thickness(2);
            splitButton.Background = Brushes.Transparent;
            splitButton.BorderBrush = Brushes.LightGray;
            splitButton.SetResourceReference(Control.StyleProperty, "MetroFlatButtonStyle");
            sp.Children.Add(splitButton);


            b.Child = sp;
            _dropDownContent = new ContentControl();
            _dropDownContent.Content = b;
            base.UIThreadInitialise();
        }

        protected override void SetRelayCommands()
        {
            DrawSplitDoseCommand = new RelayCommand(drawSplitDose);
            base.SetRelayCommands();
        }

        private void drawSplitDose()
        {
            SplitUnitDose d = new SplitUnitDose();
            d.CalibrationDate = DateTime.Now;
            d.ExpiryDate = DateTime.Now.AddHours(12);
            d.BulkDose = bulkDose;
            d.DoseAdministrationTask = dat;
            d.Radiopharmaceutical = bulkDose.Radiopharmaceutical;
            DesktopApplication.MakeModalDocument(new SplitUnitDoseViewModel(d));
        }
        public RelayCommand DrawSplitDoseCommand { get; set; }
    }

    [Export(typeof(IWhiteboardTool))]
    public class AddUnitDoseWhiteboardTool : BaseDropDownWhiteboardTool
    {
        private List<string> _unitDoseTypes;
        private string _unitDoseType;
        private DoseAdministrationTask _task;
        public AddUnitDoseWhiteboardTool() : base()
        {
            UnitDoseTypes.Add("Syringe dose");
            UnitDoseTypes.Add("Capsule dose");
        }
        protected override PackIconBase Icon
        {
            get
            {
                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.PrescriptionBottleAltSolid;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }

        public override string Name
        {
            get
            {
                return "Add unit dose for patient";
            }
        }

        protected override void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            if (Whiteboard.SelectedAppointment != null)
            {
                Appointment a = Whiteboard.SelectedAppointment as Appointment;
                if (a.Completed)
                {
                    Available = false;
                    return;
                }
                if (a.Cancelled)
                {
                    Available = false;
                    return;
                }
                if (a.Tasks.Where(x => x.Deleted == false && x.IsCancelled == false && x.Completed == false && x is DoseAdministrationTask).Where(y => (y as DoseAdministrationTask).UnitDose == null).Any())
                {
                    Available = true;
                    _task = a.Tasks.Where(x => x.Deleted == false && x.IsCancelled == false && x.Completed == false && x is DoseAdministrationTask).Where(y => (y as DoseAdministrationTask).UnitDose == null).First() as DoseAdministrationTask;
                }
                else
                {
                    Available = false;
                }

            }
            else
            {
                Available = false;
            }
        }

        public override void UIThreadInitialise()
        {
            Border b = new Border();
            b.Background = Brushes.White;
            b.BorderBrush = Brushes.LightGray;
            b.BorderThickness = new System.Windows.Thickness(3);
            b.Padding = new System.Windows.Thickness(3);
           
            ListBox l = new ListBox();
            l.FontSize = 14;
            l.ItemsSource = UnitDoseTypes;
            Binding myBinding = new Binding();
            myBinding.Source = this;
            myBinding.Path = new System.Windows.PropertyPath("SelectedUnitDoseType");                
            BindingOperations.SetBinding(l, ListBox.SelectedItemProperty, myBinding);
            //l.SelectedItem = SelectedUnitDoseType;
            l.SelectionChanged += L_SelectionChanged;
            b.Child = l;

            DropDownContent = new ContentControl();
            DropDownContent.Content = b;
            base.UIThreadInitialise();
        }

        private void L_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //
        }

        public List<string> UnitDoseTypes
        {
            get
            {
                if (_unitDoseTypes == null)
                    _unitDoseTypes = new List<string>();
                return _unitDoseTypes;
            }
            set { _unitDoseTypes = value; }
        }

        public string SelectedUnitDoseType
        {
            get { return _unitDoseType; }
            set
            {
                _unitDoseType = value;
                if(_unitDoseType == "Syringe dose")
                {
                    SyringeUnitDose m = new SyringeUnitDose();
                    m.DoseAdministrationTask = _task;
                    SyringeUnitDoseViewModel vm = new SyringeUnitDoseViewModel(m);
                    DesktopApplication.MakeModalDocument(vm);
                }
                else if(_unitDoseType == "Capsule dose")
                {
                    CapsuleUnitDose d = new CapsuleUnitDose();
                    d.DoseAdministrationTask = _task;
                    CapsuleUnitDoseViewModel vm = new CapsuleUnitDoseViewModel(d);
                    DesktopApplication.MakeModalDocument(vm);
                }
            }
        }


    }
}
