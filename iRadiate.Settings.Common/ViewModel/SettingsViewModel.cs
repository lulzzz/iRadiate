using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using MahApps.Metro.IconPacks;
using NLog;
using iRadiate.DataModel;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;


namespace iRadiate.Settings.Common.ViewModel
{
    [PreferredView("iRadiate.Settings.Common.View.SettingsView","iRadiate.Settings.Common")]
    public class SettingsViewModel : Module 
    {
        //Ummm what goes here?
        //1. Users
        //2. StaffMemberRoles
        //3. Hospitals (Wards)
        //4. Practices (Rooms)
        //5. Study Types
        public UserListViewModel ulvm { get; set; }

        public DoctorListViewModel dlvm { get; set; }
        public StaffMemberRoleListViewModel smrvm { get; set; }
        public HospitalListViewModel hlvm { get; set; }
        public NucMedPracticeListViewModel nmplvm { get; set; }
       
        public RoomsViewModel roomsViewModel { get; set; }
        public StudyTypesViewModel studyTypes { get; set; }
        public StaffMemberRoleListViewModel roles { get; set; }

        public ElementListViewModel elements { get; set; }

        public SettingsViewModel():base()
        {
            logger.Trace("SettingsViewModel() ...");
            ulvm = new UserListViewModel();
            smrvm = new StaffMemberRoleListViewModel();
            hlvm = new HospitalListViewModel();
            nmplvm = new NucMedPracticeListViewModel();
            dlvm = new DoctorListViewModel();
            roomsViewModel = new RoomsViewModel();
            studyTypes = new StudyTypesViewModel();
            roles = new StaffMemberRoleListViewModel();
            elements = new ElementListViewModel();
            logger.Trace("SettingsViewModel() ...Done");
        }

        public override void GetData()
        {
            logger.Trace("GetData() ...");
            ulvm.GetData();
            dlvm.GetData();
            hlvm.GetData();
            nmplvm.GetData();
            roomsViewModel.GetData();
            elements.GetData();
            logger.Trace("GetData() ... Done");
        }

        public override string Name
        {
            get
            {
                return "Setup";
            }
            set
            {
                
            }
        }

        public override string IconSource
        {
            get { return "/iRadiate.Settings.Common;component/Images/SettingsIcon.png"; }
        }

        public override ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Database;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }
    }

    [Export(typeof(IModuleLauncher))]
    public class SettingsLauncher : ModuleLauncher
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override string Name
        {
            get
            {
                return "Setup";
            }
        }

        public override ContentControl ButtonContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Database;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }
        public override void Launch()
        {
            logger.Trace("SettingsLauncher.Launch()");
            DesktopApplication.MainViewModel.LaunchModule(typeof(SettingsViewModel));
        
        }

        public string IconSource
        {
            get { return "/iRadiate.Settings.Common;component/Images/SettingsIconWhite.png"; }
        }

        public override int Order
        {
            get
            {
                return Properties.Settings.Default.SettingsLauncherOrder;
            }
            set
            {
                Properties.Settings.Default.SettingsLauncherOrder = value;
                RaisePropertyChanged("Order");
            }
        }

        public override bool Visible
        {
            get
            {
                return Properties.Settings.Default.SettingsLauncherVisible;
            }

            set
            {
                Properties.Settings.Default.SettingsLauncherVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
