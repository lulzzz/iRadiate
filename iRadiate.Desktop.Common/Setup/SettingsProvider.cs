using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Collections.ObjectModel;

using iRadiate.Common;
using iRadiate.DataModel;
using iRadiate.Desktop.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro;

namespace iRadiate.Desktop.Common.Setup
{
    public interface ISettingsProvider
    {
        string Name { get; }

        RelayCommand SaveCommand { get;  set; }
    }

    public abstract class SettingsProvider : ViewModelBase,  ISettingsProvider
    {
        public virtual string Name
        {
            get
            {
                return "SettingsProvier";
            }
        }

        public SettingsProvider()
        {
            SaveCommand = new RelayCommand(Save);
        }
        protected virtual void Save()
        {
            Properties.Settings.Default.Save();
        }
        public RelayCommand SaveCommand { get; set; }
    }

    [Export(typeof(ISettingsProvider))]
    [PreferredView("iRadiate.Desktop.Common.Setup.GeneralSettingsView","iRadiate.Desktop.Common")]
    public class GeneralSettingsProvider : SettingsProvider
    {
        //[ImportMany(typeof(IModuleLauncher))]
        //private List<IModuleLauncher> _moduleLaunchers;
        public override string Name
        {
            get
            {
                return "General Settings";
            }
        }

        public GeneralSettingsProvider():base()
        {
            OpenLogCommand = new RelayCommand(openLog);
            SaveModuleLaunchersCommand = new RelayCommand(saveModuleLaunchers);
            SaveAccountCommand = new RelayCommand(saveAccount);
            //var catalog = new AggregateCatalog();
            //catalog.Catalogs.Add(new DirectoryCatalog("."));
            //var container = new CompositionContainer(catalog);
            //container.ComposeParts(this);
        }

        public string LastLoginName
        {
            get
            {
                return Properties.Settings.Default.LastLoginName;
            }
            set
            {
                Properties.Settings.Default.LastLoginName = value;
            }
        }

        public MahApps.Metro.Accent AccentColor
        {
            get
            {
                return ThemeManager.GetAccent(Properties.Settings.Default.ColorTheme);
            }
            set
            {
                Properties.Settings.Default.ColorTheme = value.Name;
               
                ThemeManager.ChangeAppStyle(System.Windows.Application.Current.Resources, ThemeManager.GetAccent(value.Name),
                                    ThemeManager.GetAppTheme("BaseLight"));
                
                
                //Application.MainWindow.MyLayoutDocumentPaneGroup.Children.First().Content as UserControl
            }
        }

        public List<MahApps.Metro.Accent> AvailableAccents
        {
            get
            {
                return ThemeManager.Accents.ToList();
               
            }
        }

        public RelayCommand OpenLogCommand { get; set; }

        public RelayCommand SaveModuleLaunchersCommand { get; set; }

        public RelayCommand SaveAccountCommand { get; set; }
        private void openLog()
        {
            System.Diagnostics.Process.Start("log.txt");
            
        }

        private void saveModuleLaunchers()
        {
            foreach(IModuleLauncher m in ModuleLaunchers)
            {
                m.Save();
            }
        }

        private void saveAccount()
        {
            if(EnteredPassword != ConfirmedPasword)
            {
                DesktopApplication.ShowDialog("Error", "Passwords do not match");
                return;
            }

            if(EnteredPassword != null)
            {
                Platform.CurrentUser.Password = iRadiate.Common.Authentication.Authenticator.HashPassword(EnteredPassword);
                
            }

            if (Platform.Retriever.SaveItem(Platform.CurrentUser))
            {
                DesktopApplication.ShowDialog("Update", "Account changes saved");
            }else
            {
                DesktopApplication.ShowDialog("Error", "Failed to save account");
            }
        }

        public string LoginName
        {
            get
            {
                return Platform.CurrentUser.LoginName;
            }
            set
            {
                Platform.CurrentUser.LoginName = value;
                RaisePropertyChanged("LoginName");
            }
        }

        public string Surname
        {
            get
            {
                return Platform.CurrentUser.Surname;

            }
            set
            {
                Platform.CurrentUser.Surname = value;
            }
        }

        public string GivenNames
        {
            get { return Platform.CurrentUser.GivenNames; }
            set { Platform.CurrentUser.GivenNames = value; }
        }

        public string EnteredPassword { get; set; }

        public string ConfirmedPasword { get; set; }

        public ObservableCollection<IModuleLauncher> ModuleLaunchers
        {
            get { return DesktopApplication.MainViewModel.ModuleLaunchers; }
            set { DesktopApplication.MainViewModel.ModuleLaunchers = value;  RaisePropertyChanged("ModuleLaunchers"); }
        }

        public bool ConfirmPrinterForLabels
        {
            get { return Properties.Settings.Default.ConfirmPrinterForLabels; }
            set { Properties.Settings.Default.ConfirmPrinterForLabels = value; RaisePropertyChanged("ConfirmPrinterForLabels"); }
        }
    }
}
