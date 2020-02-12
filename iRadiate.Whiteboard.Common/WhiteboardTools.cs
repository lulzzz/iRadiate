using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
//using System.Spatial;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

using iRadiate.Whiteboard.Common.ViewModel;
using iRadiate.DataModel.Common;
using iRadiate.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Desktop.Common.Tools;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NLog;
using MahApps.Metro.IconPacks;
using Xceed.Wpf.Toolkit;
using ControlzEx;

namespace iRadiate.Whiteboard.Common
{
    public interface IWhiteboardTool : ITool
    {
    
        void SelectedAppointmentChanged();

        void SetWhiteboard(WhiteboardViewModel wb);

        int WhiteboardPositionIndex { get; set; }

        bool WhiteboardVisible { get; set; }

    }

    public abstract class BaseWhiteboardTool : BaseExecutableTool, IWhiteboardTool
    {
        #region privateFields
        private WhiteboardViewModel _whiteboard;
        private bool _available = false;
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region constructor
        public BaseWhiteboardTool():base()
        {
            
        }
        #endregion

        #region virtuals
        protected virtual void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            //
        }
        public virtual void SelectedAppointmentChanged()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region publicMethods
        public void SetWhiteboard(WhiteboardViewModel wb)
        {
            _whiteboard = wb;
            _whiteboard.SelectionChanged += _whiteboard_SelectionChanged;
        }

        public override void Save()
        {
            Properties.Settings.Default.Save();
        }
        #endregion

        #region protected
        protected WhiteboardViewModel Whiteboard
        {
            get
            {
                return _whiteboard;
            }
        }
        #endregion

        #region publicProperties
        public virtual int WhiteboardPositionIndex
        {
            get
            {
                return 1;
            }
            set
            {

            }
        }

        public virtual bool WhiteboardVisible
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

    public abstract class BasesplitButtonWhiteboardTool : BaseSplitButtonTool, IWhiteboardTool
    {

        private WhiteboardViewModel _whiteboard;
        private bool _available = false;

        #region constructor
        public BasesplitButtonWhiteboardTool() : base()
        {

        }
        #endregion

        #region public
       
        public void SelectedAppointmentChanged()
        {
            throw new NotImplementedException();
        }

        public void SetWhiteboard(WhiteboardViewModel wb)
        {
            _whiteboard = wb;
            _whiteboard.SelectionChanged += _whiteboard_SelectionChanged;
        }
        #endregion

        #region virtual
        protected virtual void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            //
        }
        public virtual int WhiteboardPositionIndex
        {
            get
            {
                return 1;
            }

            set
            {

            }
        }

        public virtual bool WhiteboardVisible
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

        #region protected
        protected WhiteboardViewModel Whiteboard
        {
            get
            {
                return _whiteboard;
            }
        }
        #endregion
        

    }

    public abstract class BaseDropDownWhiteboardTool : BaseDropDownTool, IWhiteboardTool
    {
        private WhiteboardViewModel _whiteboard;
        private bool _available = false;

        #region constructor
        public BaseDropDownWhiteboardTool() : base()
        {

        }
        #endregion

        #region public

        public void SelectedAppointmentChanged()
        {
            throw new NotImplementedException();
        }

        public void SetWhiteboard(WhiteboardViewModel wb)
        {
            _whiteboard = wb;
            _whiteboard.SelectionChanged += _whiteboard_SelectionChanged;
        }
        #endregion

        #region virtual
        protected virtual void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            //
        }
        public virtual int WhiteboardPositionIndex
        {
            get
            {
                return 1;
            }

            set
            {

            }
        }

        public virtual bool WhiteboardVisible
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

        #region protected
        protected WhiteboardViewModel Whiteboard
        {
            get
            {
                return _whiteboard;
            }
        }
        #endregion
    }

    [Export(typeof(IWhiteboardTool))]
    public class PatientDetailsTool : BaseWhiteboardTool
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public PatientDetailsTool()
            : base()
        {

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
           
            Appointment a = Whiteboard.SelectedAppointment as Appointment;
            
            PatientViewModel p = new PatientViewModel(a.Patient);
            p.SelectedStudy = p.Studies.Where(x => x.Item.ID == a.Study.ID).First();
            p.SelectedStudy.SelectedAppointment = p.SelectedStudy.Appointments.Where(y => y.Item.ID == a.ID).First();
            DesktopApplication.MakeModalDocument(p);
            
        }

        public override string Name
        {
            get
            {
                return "Patient Details";
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

        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.PatientDetailsToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.PatientDetailsToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.PatientDetailsToolVisible;
            }

            set
            {
                Properties.Settings.Default.PatientDetailsToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "View the patient's details";
            }
        }
    }

    [Export(typeof(IWhiteboardTool))]
    public class CommentsTool : BasesplitButtonWhiteboardTool
    {
       
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public CommentsTool()
            : base()
        {

        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.CommentAltRegular;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }
        
        protected override void Execute()
        {
            //base.Execute();
            AppointmentViewModel avm = new AppointmentViewModel(Whiteboard.SelectedAppointment as Appointment);
            DesktopApplication.MakeModalDocument(avm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.CommentsView");

        }

        public override string Name
        {
            get
            {
                return "Comments";
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

        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.CommentsToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.CommentsToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.CommentsToolVisible;
            }

            set
            {
                Properties.Settings.Default.CommentsToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Edit the comments about the appointment";
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
            

            Button pypButton = new Button();
            pypButton.FontSize = 14;
            pypButton.Content = "PYP Injected";
            pypButton.Command = RecordPYPCommand;
            pypButton.Padding = new System.Windows.Thickness(3);
            pypButton.Margin = new System.Windows.Thickness(2);
            pypButton.Background = Brushes.Transparent;
            pypButton.BorderBrush = Brushes.LightGray;
            pypButton.SetResourceReference(Control.StyleProperty, "MetroFlatButtonStyle");
            sp.Children.Add(pypButton);


            b.Child = sp;
            _dropDownContent = new ContentControl();
            _dropDownContent.Content = b;
            base.UIThreadInitialise();
        }

        private void recordPYP()
        {
            (Whiteboard.SelectedAppointment as Appointment).Comments += "PYP Injected @ " + DateTime.Now.ToShortTimeString();
            var a = Whiteboard.SelectedAppointment;
            //DesktopApplication.Librarian.SaveItem(Whiteboard.SelectedAppointment);
            AppointmentViewModel avm = new AppointmentViewModel(a);
            DesktopApplication.MakeModalDocument(avm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.CommentsView");
        }

        protected override void SetRelayCommands()
        {
            RecordPYPCommand = new RelayCommand(recordPYP);
            base.SetRelayCommands();

        }
        public RelayCommand RecordPYPCommand { get; set; }
    }

    [Export(typeof(IWhiteboardTool))]
    public class ArrivePatientTool : BaseWhiteboardTool
    {
        public ArrivePatientTool()
            : base()
        {

        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.AirplaneLanding;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }
       
        public override string Name
        {
            get
            {
                return "Arrive tool";
            }
        }
        protected override void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            if (Whiteboard.SelectedAppointment != null)
            {
                if((Whiteboard.SelectedAppointment as Appointment).Completed)
                {
                    Available = false;
                    return;
                }

                if (!((Appointment)Whiteboard.SelectedAppointment).HasPatientArrived)
                    Available = true;
                else
                    Available = false;
            }
            else
            {
                Available = false;
            }
        }

        protected override void Execute()
        {
            base.Execute();
            Appointment a = (Appointment)Whiteboard.SelectedAppointment;
            //go through the appointment and check for arrivals that haven't been completed
            foreach (BasicTask t in a.Tasks)
            {
                if (t is ArrivalTask)
                {
                    if (t.Completed != true)
                    {
                        ArrivalTaskViewModel atvm = new ArrivalTaskViewModel(t);
                        DesktopApplication.MakeModalDocument(atvm);
                        return;
                    }
                    
                }
            }

            //If we reach this point it means there has not been an arrival task ready to complete
            //so we must make out own
            ArrivalTask at = new ArrivalTask(a);
            at.ScheduledCompletionTime = DateTime.Now;
            ArrivalTaskViewModel atvm1 = new ArrivalTaskViewModel(at);
            DesktopApplication.MakeModalDocument(atvm1);
            return;
            
        }

        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.ArrivePatientToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.ArrivePatientToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.ArrivePatientToolVisible;
            }

            set
            {
                Properties.Settings.Default.ArrivePatientToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Mark the patient as 'Arrived'";
            }
        }
    }

    [Export(typeof(IWhiteboardTool))]
    public class InjectPatientTool : BaseWhiteboardTool
    {
        public InjectPatientTool()
            : base()
        {

        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Needle;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }

        public override string Name
        {
            get
            {
                return "Inject patient";
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
                if(a.Tasks.Where(x=> x is DoseAdministrationTask && !x.Cancelled && !x.Completed).Any())
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
        protected override void Execute()
        {
            base.Execute();
            base.Execute();
            Appointment a = (Appointment)Whiteboard.SelectedAppointment;
            //go through the appointment and check for arrivals that haven't been completed
            foreach (BasicTask t in a.Tasks.OrderBy(y=>y.ScheduledCompletionTime))
            {
                if (t is DoseAdministrationTask)
                {
                    if (!t.Completed)
                    {
                        DoseAdministrationTaskViewModel atvm = new DoseAdministrationTaskViewModel(t);
                        DesktopApplication.MakeModalDocument(atvm);
                        return;
                    }

                }
            }

            //We will only use this method to complete existing injection tasks, so if there is not one available, return
            return;
        }

        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.InjectPatientToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.InjectPatientToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.InjectPatientToolVisible;
            }

            set
            {
                Properties.Settings.Default.InjectPatientToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Administer dose to the patient";
            }
        }
    }

    [Export(typeof(IWhiteboardTool))]
    public class CompleteAppointmentTool: BaseWhiteboardTool
    {
        public CompleteAppointmentTool()
            : base()
        {

        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconOcticons icon = new PackIconOcticons();
                icon.Kind = PackIconOcticonsKind.IssueClosed;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }
 
        public override string Name
        {
            get
            {
                return "Complete appointment";
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
                }
                else
                {
                    Available = true;
                }
            }
            else
            {
                Available = false;
            }
        }
        protected override void Execute()
        {
            base.Execute();
            base.Execute();
            Appointment a = (Appointment)Whiteboard.SelectedAppointment;
            a.Completed = true;
            a.CompletionTime = DateTime.Now;
        }

        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.CompleteAppointmentToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.CompleteAppointmentToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.CompleteAppointmentToolVisible;
            }

            set
            {
                Properties.Settings.Default.CompleteAppointmentToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Mark appointment as completed";
            }
        }
    }
    

    [Export(typeof(IWhiteboardTool))]
    public class SaveLayoutTool : BaseWhiteboardTool
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public SaveLayoutTool()
            : base()
        {
            Available = true;
        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.TableColumnWidth;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }
       
        protected override void Execute()
        {
            base.Execute();
            logger.Trace("Execute() ...");

            Whiteboard.SaveLayout();
            logger.Trace("Execute() ...");
        }

        public override string Name
        {
            get
            {
                return "Save Whiteboard Layout";
            }
        }

        protected override void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            Available = true;
        }
        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.SaveLayoutToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.SaveLayoutToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.SaveLayoutToolVisible;
            }

            set
            {
                Properties.Settings.Default.SaveLayoutToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Save the layout of the whiteboard";
            }
        }

    }

    [Export(typeof(IWhiteboardTool))]
    public class RefreshTool : BaseWhiteboardTool
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public RefreshTool()
            : base()
        {
            Available = true;
        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Sort;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }
       
        protected override void Execute()
        {
            base.Execute();
            logger.Trace("Execute() ...");

            Whiteboard.AppointmentsView.View.Refresh();
            logger.Trace("Execute() ...");
        }

        public override string Name
        {
            get
            {
                return "Refresh";
            }
        }

        protected override void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            Available = true;
        }

        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.RefreshWhiteboardToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.RefreshWhiteboardToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.RefreshWhiteboardToolVisible;
            }

            set
            {
                Properties.Settings.Default.RefreshWhiteboardToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Refresh the contents of the whiteboard";
            }
        }
    }

    [Export(typeof(IWhiteboardTool))]
    public class StudyReportTool : BaseWhiteboardTool
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();
        public StudyReportTool()
            : base()
        {

        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.FileAltRegular;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }
       
        protected override void Execute()
        {
            base.Execute();
            if (Whiteboard.SelectedAppointment == null)
                return;
            if ((Whiteboard.SelectedAppointment as Appointment).Study.Report == null)
                return;
            StudyReportViewModel viewModel = new StudyReportViewModel((Whiteboard.SelectedAppointment as Appointment).Study.Report);
            DesktopApplication.MakeModalDocument(viewModel);
        }

        public override string Name
        {
            get
            {
                return "View report";
            }
        }

        protected override void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            if (Whiteboard.SelectedAppointment == null)
            {
                Available = false;
                return;
            }
                
            if ((Whiteboard.SelectedAppointment as Appointment).Study.Report == null)
            {
                Available = false;
                return;
            }
                
            Available = true;
        }

        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.StudyReportToolPotisionIndex;
            }

            set
            {
                Properties.Settings.Default.StudyReportToolPotisionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.StudyReportToolVisible;
            }

            set
            {
                Properties.Settings.Default.StudyReportToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "View the report for this study";
            }
        }
    }

    [Export(typeof(IWhiteboardTool))]
    public class TransplantTasksTool : BaseWhiteboardTool
    {
        public TransplantTasksTool() : base()
        {

        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.PeopleCarrySolid;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }
        
        protected override void Execute()
        {
            logger.Debug("Executing Transplant Tasks tool");
            (Whiteboard.SelectedAppointment as Appointment).TransplantTasks();
            logger.Debug("Saving appointmnt now that tasks have been transplanted");
            foreach(BasicTask b in (Whiteboard.SelectedAppointment as Appointment).Tasks)
            {
                Platform.Retriever.SaveItem(b);
            }
            
            //PlatformID.R.SaveItem(Whiteboard.SelectedAppointment as Appointment);
            DesktopApplication.ShowDialog("Info", "Tasks have been rescheduled to match appointment");
            logger.Debug("Executing Transplant Tasks tool completed");

        }

        public override string Name
        {
            get
            {
                return "Reschedule tasks";
            }
        }

        protected override void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            if (Whiteboard.SelectedAppointment == null)
            {
                Available = false;
                return;
            }
            Appointment a = Whiteboard.SelectedAppointment as Appointment;
            if(a == null)
            {
                Available = false;
                return;
            }
            if (a.Completed)
            {
                Available = false;
                return;
            }
            if ((Whiteboard.SelectedAppointment as Appointment).Tasks.Where(x => x.ScheduledCompletionTime.Date != (Whiteboard.SelectedAppointment as Appointment).ScheduledArrivalTime.Date).Any())
            {
                Available = true;
                return;
            }

            Available = false;
        }
        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.TransplantTasksToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.TransplantTasksToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }

        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.TransplantTasksToolVisible;
            }

            set
            {
                Properties.Settings.Default.TransplantTasksToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Reschedule tasks to match appointmment";
            }
        }
    }

    [Export(typeof(IWhiteboardTool))]
    public class AddressLabelTool : BaseWhiteboardTool
    {
        public AddressLabelTool() : base()
        {

        }

        protected override PackIconBase Icon
        {
            get
            {
                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.AddressCardRegular;
                icon.Height = DesktopApplication.IconHeight;
                icon.Width = DesktopApplication.IconWidth;
                return icon;
            }
        }

        protected override void Execute()
        {
            Appointment a = Whiteboard.SelectedAppointment as Appointment;
            if (a == null)
                return;
            if (a.Patient == null)
                return;

            var vm = new PatientViewModel(a.Patient);
            DesktopApplication.MakeModalDocument(vm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.PatientLabelPrintView");


        }

        public override string Name
        {
            get
            {
                return "Address Label";
            }
        }

        protected override void _whiteboard_SelectionChanged(object sender, EventArgs e)
        {
            if (Whiteboard.SelectedAppointment == null)
            {
                Available = false;
                return;
            }
            
            Available = true;
        }
        public override int WhiteboardPositionIndex
        {
            get
            {
                return Properties.Settings.Default.AddressLabelToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.AddressLabelToolPositionIndex = value;
                RaisePropertyChanged("PositionIndex");
            }
        }
        public override bool WhiteboardVisible
        {
            get
            {
                return Properties.Settings.Default.AddressLabelToolVisible;
            }

            set
            {
                Properties.Settings.Default.AddressLabelToolVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Print patient address label";
            }
        }

    }
 

}
