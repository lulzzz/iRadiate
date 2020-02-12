using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using iRadiate.DataModel;
using iRadiate.Desktop.Common.Setup;
using iRadiate.Desktop.Common.ViewModel;


namespace iRadiate.Whiteboard.Common.ViewModel
{

    [Export(typeof(ISettingsProvider))]
    [PreferredView("iRadiate.Whiteboard.Common.View.WhiteboardSettingsView", "iRadiate.Whiteboard.Common")]
    class WhiteboardSettingsProvider : SettingsProvider
    {
        private List<ColumnSetting> _columns;
        [ImportMany(typeof(IWhiteboardTool))]
        private List<IWhiteboardTool> _tools;

        public override string Name
        {
            get
            {
                return "Whiteboard Settings";
            }
        }

        public WhiteboardSettingsProvider():base()
        {
            
            _columns = new List<ColumnSetting>();
            ColumnSetting csTransportType = new ColumnSetting();
            csTransportType.Name = "Transport Type";
            csTransportType.Visibility = Properties.Settings.Default.TransportTypeColumnVisibility;
            csTransportType.DisplayIndex = Properties.Settings.Default.TransportTypeColumnDisplayIndex;
            csTransportType.Width = Properties.Settings.Default.TransportTypeColumnWidth.Value;
            _columns.Add(csTransportType);

            

            ColumnSetting csAge = new ColumnSetting();
            csAge.Name = "Age";
            csAge.Visibility = Properties.Settings.Default.AgeColumnVisibility;
            csAge.DisplayIndex = Properties.Settings.Default.AgeColumnDisplayIndex;
            csAge.Width = Properties.Settings.Default.AgeColumnWidth.Value;
            _columns.Add(csAge);

            ColumnSetting csCamera = new ColumnSetting();
            csCamera.Name = "Camera";
            csCamera.Visibility = Properties.Settings.Default.CameraColumnVisibility;
            csCamera.DisplayIndex = Properties.Settings.Default.CameraColumnDisplayIndex;
            csCamera.Width = Properties.Settings.Default.CameraColumnWidth.Value;
            _columns.Add(csCamera);

            ColumnSetting csComments = new ColumnSetting();
            csComments.Name = "Comments";
            csComments.Visibility = Properties.Settings.Default.CommentsColumnVisibility;
            csComments.DisplayIndex = Properties.Settings.Default.CommentsColumnDisplayIndex;
            csComments.Width = Properties.Settings.Default.CommentsColumnWidth.Value;
            _columns.Add(csComments);

            ColumnSetting csCurrentTask = new ColumnSetting();
            csCurrentTask.Name = "Current Task";
            csCurrentTask.Visibility = Properties.Settings.Default.CurrentTaskColumnVisibility;
            csCurrentTask.DisplayIndex = Properties.Settings.Default.CurrentTaskColumnDisplayIndex;
            csCurrentTask.Width = Properties.Settings.Default.CurrentTaskColumnWidth.Value;
            _columns.Add(csCurrentTask);

            ColumnSetting csDOB = new ColumnSetting();
            csDOB.Name = "DOB";
            csDOB.Visibility = Properties.Settings.Default.DOBColumnVisibility;
            csDOB.DisplayIndex = Properties.Settings.Default.DOBColumnDisplayIndex;
            csDOB.Width = Properties.Settings.Default.DOBColumnWidth.Value;
            _columns.Add(csDOB);

            ColumnSetting csDuration = new ColumnSetting();
            csDuration.Name = "Duration";
            csDuration.Visibility = Properties.Settings.Default.DurationColumnVisibility;
            csDuration.DisplayIndex = Properties.Settings.Default.DurationColumnDisplayIndex;
            csDuration.Width = Properties.Settings.Default.DurationColumnWidth.Value;
            _columns.Add(csDuration);

            ColumnSetting csGivenNames = new ColumnSetting();
            csGivenNames.Name = "Given Names";
            csGivenNames.Visibility = Properties.Settings.Default.GivenNamesColumnVisibility;
            csGivenNames.DisplayIndex = Properties.Settings.Default.GivenNamesColumnDisplayIndex;
            csGivenNames.Width = Properties.Settings.Default.GivenNamesColumnWidth.Value;
            _columns.Add(csGivenNames);

            ColumnSetting csHospital = new ColumnSetting();
            csHospital.Name = "Hospital";
            csHospital.Visibility = Properties.Settings.Default.HospitalColumnVisibility;
            csHospital.DisplayIndex = Properties.Settings.Default.GivenNamesColumnDisplayIndex;
            csHospital.Width = Properties.Settings.Default.HospitalColumnWidth.Value;
            _columns.Add(csHospital);

            ColumnSetting csInjections = new ColumnSetting();
            csInjections.Name = "Injections";
            csInjections.Visibility = Properties.Settings.Default.InjectionsColumnVisibility;
            csInjections.DisplayIndex = Properties.Settings.Default.InjectionsColumnDisplayIndex;
            csInjections.Width = Properties.Settings.Default.InjectionsColumnWidth.Value;
            _columns.Add(csInjections);

            ColumnSetting csMRN = new ColumnSetting();
            csMRN.Name = "MRN";
            csMRN.Visibility = Properties.Settings.Default.MRNColumnVisibility;
            csMRN.DisplayIndex = Properties.Settings.Default.MRNColumnDisplayIndex;
            csMRN.Width = Properties.Settings.Default.MRNColumnWidth.Value;
            _columns.Add(csMRN);

            ColumnSetting csName = new ColumnSetting();
            csName.Name = "Name";
            csName.Visibility = Properties.Settings.Default.NameColumnVisibility;
            csName.DisplayIndex = Properties.Settings.Default.NameColumnDisplayIndex;
            csName.Width = Properties.Settings.Default.NameColumnWidth.Value;
            _columns.Add(csName);

            ColumnSetting csNextTask = new ColumnSetting();
            csNextTask.Name = "Next Task";
            csNextTask.Visibility = Properties.Settings.Default.NextTaskColumnVisibility;
            csNextTask.DisplayIndex = Properties.Settings.Default.NextTaskColumnDisplayIndex;
            csNextTask.Width = Properties.Settings.Default.NextTaskColumnWidth.Value;
            _columns.Add(csNextTask);

            ColumnSetting csPregnancyStatus = new ColumnSetting();
            csPregnancyStatus.Name = "Pregnancy Status";
            csPregnancyStatus.Visibility = Properties.Settings.Default.PregnancyStatusColumnVisibility;
            csPregnancyStatus.DisplayIndex = Properties.Settings.Default.PregnancyStatusColumnDisplayIndex;
            csPregnancyStatus.Width = Properties.Settings.Default.PregnancyStatusColumnWidth.Value;
            _columns.Add(csPregnancyStatus);

            ColumnSetting csRoom = new ColumnSetting();
            csRoom.Name = "Room";
            csRoom.Visibility = Properties.Settings.Default.RoomColumnVisibility;
            csRoom.DisplayIndex = Properties.Settings.Default.RoomColumnDisplayIndex;
            csRoom.Width = Properties.Settings.Default.RoomColumnWidth.Value;
            _columns.Add(csRoom);

            ColumnSetting csScans = new ColumnSetting();
            csScans.Name = "Scans";
            csScans.Visibility = Properties.Settings.Default.ScansColumnVisibility;
            csScans.DisplayIndex = Properties.Settings.Default.ScansColumnDisplayIndex;
            csScans.Width = Properties.Settings.Default.ScansColumnWidth.Value;
            _columns.Add(csScans);

            ColumnSetting csArrivalTime = new ColumnSetting();
            csArrivalTime.Name = "Scheduled Arrival Time";
            csArrivalTime.Visibility = Properties.Settings.Default.ScheduledArrivalTimeColumnVisibility;
            csArrivalTime.DisplayIndex = Properties.Settings.Default.ScheduledArrivalTimeColumnDisplayIndex;
            csArrivalTime.Width = Properties.Settings.Default.ScheduledArrivalTimeColumnWidth.Value;
            _columns.Add(csArrivalTime);

            ColumnSetting csSex = new ColumnSetting();
            csSex.Name = "Sex";
            csSex.Visibility = Properties.Settings.Default.SexColumnVisibility;
            csSex.DisplayIndex = Properties.Settings.Default.SexColumnDisplayIndex;
            csSex.Width = Properties.Settings.Default.SexColumnWidth.Value;
            _columns.Add(csSex);

            ColumnSetting csStatus = new ColumnSetting();
            csStatus.Name = "Status";
            csStatus.Visibility = Properties.Settings.Default.StatusColumnVisibility;
            csStatus.DisplayIndex = Properties.Settings.Default.StatusColumnDisplayIndex;
            csStatus.Width = Properties.Settings.Default.StatusColumnWidth.Value;
            _columns.Add(csStatus);

            ColumnSetting csSurname = new ColumnSetting();
            csSurname.Name = "Surname";
            csSurname.DisplayIndex = Properties.Settings.Default.SurnameColumnDisplayIndex;
            csSurname.Visibility = Properties.Settings.Default.SurnameColumnVisibility;            
            csSurname.Width = Properties.Settings.Default.SurnameColumnWidth.Value;
            _columns.Add(csSurname);

            ColumnSetting csTitle = new ColumnSetting();
            csTitle.Name = "Title";
            csTitle.DisplayIndex = Properties.Settings.Default.TitleColumnDisplayIndex;
            csTitle.Visibility = Properties.Settings.Default.TitleColumnVisibility;
            csTitle.Width = Properties.Settings.Default.TitleColumnWidth.Value;
            _columns.Add(csTitle);

            ColumnSetting csWard = new ColumnSetting();
            csWard.Name = "Ward";
            csWard.DisplayIndex = Properties.Settings.Default.WardColumnDisplayIndex;
            csWard.Visibility = Properties.Settings.Default.WardColumnVisibility;
            csWard.Width = Properties.Settings.Default.WardColumnWidth.Value;
            _columns.Add(csWard);

            SaveToolsCommand = new RelayCommand(saveTools);
        }

        public List<IWhiteboardTool> Tools
        {
            get
            {
                return _tools.OrderBy(x => x.WhiteboardPositionIndex).ToList();
            }
            set
            {
                _tools = value;
            }
        }
        public bool ExcludeCompleted
        {
            get { return Properties.Settings.Default.ExcludeCompleted; }
            set { Properties.Settings.Default.ExcludeCompleted = value; }
        }

        public bool HighlightArrived
        {
            get { return Properties.Settings.Default.HighlightArrived; }
            set { Properties.Settings.Default.HighlightArrived = value; RaisePropertyChanged("HighlightArrived"); }
        }
        public bool ExcludeCancelled
        {
            get { return Properties.Settings.Default.ExcludeCancelled; }
            set { Properties.Settings.Default.ExcludeCancelled = value; }
        }
        protected override void Save()
        {
            
            foreach(ColumnSetting cs in Columns.OrderBy(x=>x.DisplayIndex))
            {
                
                
                cs.UpdateSetting(ColumnSetting.ColumnAttribute.DisplayIndex);
                cs.UpdateSetting(ColumnSetting.ColumnAttribute.Visibility);
                cs.UpdateSetting(ColumnSetting.ColumnAttribute.Width);
            }
            base.Save();
        }
        public List<ColumnSetting> Columns
        {
            get
            {


                return _columns;
            }
            set
            {
                _columns = value;
            }
        }

        private void saveTools()
        {
            foreach(var t in Tools)
            {
                t.SaveCommand.Execute(null);
            }
        }

        public RelayCommand SaveToolsCommand { get; set; }
    }

    public class ColumnSetting
    {
        private int _displayIndex;
        private double _width;
        private bool _visibility;
        private string _name;
        public int DisplayIndex
        {
            get { return _displayIndex; }
            set { _displayIndex = value;
                //UpdateSetting(ColumnAttribute.DisplayIndex);
            }
        }

        public bool Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {

                _visibility = value;
                //UpdateSetting(ColumnAttribute.Visibility);
            }
        }

        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                //UpdateSetting(ColumnAttribute.Width);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public enum ColumnAttribute { DisplayIndex, Visibility, Width};

        public void UpdateSetting(ColumnAttribute att)
        {
            switch (att)
            {
                case ColumnAttribute.Visibility:
                    Properties.Settings.Default[Regex.Replace(Name, @"\s+", "") + "ColumnVisibility"] = Visibility;
                    break;
                case ColumnAttribute.DisplayIndex:
                    //Properties.Settings.Default[Regex.Replace(Name, @"\s+", "") + "ColumnDisplayIndex"] = DisplayIndex;
                    break;
                case ColumnAttribute.Width:
                    Properties.Settings.Default[Regex.Replace(Name, @"\s+", "") + "ColumnWidth"] = new DataGridLength(Width);
                    break;
            }
            
        }
    }
}
