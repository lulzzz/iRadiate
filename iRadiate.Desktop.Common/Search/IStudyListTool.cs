using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;


using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Desktop.Search.ViewModel;


using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NLog;
using MahApps.Metro.IconPacks;

namespace iRadiate.Desktop.Search
{
    public interface IStudyListTool
    {
        void Execute();

        bool Available{ get; }

        string IconSource { get; }

        string Name { get; }

        ContentControl ContentSource { get; set; }

        void SetStudyList(StudyListViewModel studyList);

    }

    public class BaseStudyListTool : ViewModelBase, IStudyListTool
    {
        
        private StudyListViewModel _studyList;
        private bool _available;

        public BaseStudyListTool()
        {
            ExecuteCommand = new RelayCommand(Execute);
        }
        
        

       

        public StudyListViewModel StudyList
        {
            get
            {
                return _studyList;
            }
        }
        public virtual void Execute()
        {
            throw new NotImplementedException();
        }

        public bool Available
        {
            get
            {
                return _available;
            }
            set
            {
                _available = value;
                RaisePropertyChanged("Available");
            }

        }

        public virtual string IconSource
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        
        public virtual string Name
        {
            get
            {
                return "Base";
            }
        }
        public RelayCommand ExecuteCommand
        {
            get;
            set;
        }

        public virtual ContentControl ContentSource
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual void SetStudyList(StudyListViewModel list)
        {
            _studyList = list;
            _studyList.SelectionChanged += _studyList_SelectionChanged;
            if (StudyList.SelectedStudy != null)
            {
                Available = true;
            }
            else
            {
                Available = false;
            }
        }

        protected virtual void _studyList_SelectionChanged(object sender, EventArgs e)
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

    [Export(typeof(IStudyListTool))]
    public class PatientDetailsTool : BaseStudyListTool
    {
       
        public override string IconSource
        {
            get
            {
                return "/iRadiate.Desktop.Common;component/Images/DetailsIcon.png";
            }
        }

        public override string Name
        {
            get
            {
                return "Patient Details";
            }
        }

        public override void Execute()
        {

            if (StudyList.SelectedStudy == null)
            {
                return;
            }
            
            Study s = StudyList.SelectedStudy as Study;

            PatientViewModel p = new PatientViewModel(s.Patient);
            p.SelectedStudy = p.Studies.Where(x => x.Item.ID == StudyList.SelectedStudy.ID).First();
           
            DesktopApplication.MakeModalDocument(p);
            

           
            

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

        public override ContentControl ContentSource
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                
                icon.Kind = PackIconMaterialKind.Account;
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

    }

    [Export(typeof(IStudyListTool))]
    public class StudyReportTool : BaseStudyListTool
    {

       

        public override string Name
        {
            get
            {
                return "Study Report";
            }
        }

        public override void Execute()
        {

            if (StudyList.SelectedStudy == null)
            {
                return;
            }
            if ((StudyList.SelectedStudy as Study).Report == null)
                return;

            StudyReportViewModel viewModel = new StudyReportViewModel((StudyList.SelectedStudy as Study).Report);
            DesktopApplication.MakeModalDocument(viewModel);

           





        }
        protected override void _studyList_SelectionChanged(object sender, EventArgs e)
        {
            if (StudyList.SelectedStudy != null)
            {
                if((StudyList.SelectedStudy as Study).Report != null)
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

        public override ContentControl ContentSource
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconFontAwesome icon = new PackIconFontAwesome();

                icon.Kind = PackIconFontAwesomeKind.FileAltRegular;
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

    }

    [Export(typeof(IStudyListTool))]
    public class PrintStudyListTool: BaseStudyListTool
    {
        public override string Name
        {
            get
            {
                return "Print Studies";
            }
        }

        public override void Execute()
        {

            PrintDG print = new PrintDG();
            string[] columns = new string[6];
            columns[0] = "Date";
            columns[1] = "Patient.Surname";
            columns[2] = "Patient.GivenNames";
            columns[3] = "Name";
            columns[4] = "Request.Referrer.FullName";
            columns[5] = "Ward.AbbreviatedFullName";

            string[] columnNames = new string[6];
            columnNames[0] = "Date & Time";
            columnNames[1] = "Surname";
            columnNames[2] = "Given Names";
            columnNames[3] = "Appointment";
            columnNames[4] = "Referrer";
            columnNames[5] = "Ward";

            int[] columnWidths = new int[6];
            columnWidths[0] = 175;
            columnWidths[1] = 100;
            columnWidths[2] = 100;
            columnWidths[3] = 200;
            columnWidths[4] = 250;
            columnWidths[5] = 200;

            var studies = CollectionViewSource.GetDefaultView(StudyList.RetrievedStudies);
            
            
            print.printDG(studies, "Appointment List", columns, columnNames, columnWidths);
            
            

            //print.printDG(datagridName, "Title");







        }
        protected override void _studyList_SelectionChanged(object sender, EventArgs e)
        {
            Available = true;
        }

        public override ContentControl ContentSource
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconFontAwesome icon = new PackIconFontAwesome();

                icon.Kind = PackIconFontAwesomeKind.PrintSolid;
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
    }

}
