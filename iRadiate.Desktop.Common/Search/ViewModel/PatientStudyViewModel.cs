using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;
using NLog;

using iRadiate.Common.IO;
using iRadiate.Desktop.Common;
using iRadiate.DataModel;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;

using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Search.ViewModel
{
     [PreferredView("iRadiate.Desktop.Search.View.PatientStudyView", "iRadiate.Desktop.Common")]
    public class SearchViewModel : Module
    {

        public SearchViewModel()
         {
             _patientListViewModel = new PatientListViewModel();
             _studyListViewModel = new StudyListViewModel();
         }
        
        public override string IconSource
        {
            get { return "/iRadiate.Desktop.Common;component/Images/SearchIcon.png"; }
        }

        public override ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.AccountSearch;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }

        public override string Name
        {
            get
            {
                return "Search";
            }
            set
            {

            }
        }

        private PatientListViewModel _patientListViewModel;
        private StudyListViewModel _studyListViewModel;

        public PatientListViewModel PatientListViewModel
        {
            get
            {
                return _patientListViewModel;
            }
            set
            {
                _patientListViewModel = value;
                RaisePropertyChanged("PatientListViewModel");
            }
        }

        public StudyListViewModel StudyListViewModel
        {
            get
            {
                return _studyListViewModel;
            }
            set
            {
                _studyListViewModel = value;
                RaisePropertyChanged("StudyListViewModel");
            }
        }

        public override void GetData()
        {
            base.GetData();
            PatientListViewModel.GetData();
            StudyListViewModel.GetData();
        }

    }

     [Export(typeof(IModuleLauncher))]
     public class PatientListLauncher : ModuleLauncher
     {
         private static Logger logger = LogManager.GetCurrentClassLogger();

         public override string Name
         {
             get
             {
                 return "Search";
             }
         }

        public override ContentControl ButtonContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.AccountSearch;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }
        public override void Launch()
         {

             DesktopApplication.MainViewModel.LaunchModule(typeof(SearchViewModel));

         }

         public string IconSource
         {
             get { return "/iRadiate.Desktop.Common;component/Images/SearchIconWhite.png"; }
         }

         public override int Order
         {
             get
             {
                return Common.Properties.Settings.Default.SearchModuleLauncherOrder;
             }
            set
            {
                Common.Properties.Settings.Default.SearchModuleLauncherOrder = value;
                RaisePropertyChanged("Order");
            }
         }

        public override bool Visible
        {
            get
            {
                return Common.Properties.Settings.Default.SearchModuleLauncherVisible;
            }

            set
            {
                Common.Properties.Settings.Default.SearchModuleLauncherVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override void Save()
        {
            Common.Properties.Settings.Default.Save();
        }
    }
}
