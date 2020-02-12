using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using MahApps.Metro.IconPacks;

namespace iRadiate.Desktop.Common.Setup
{
    [Export(typeof(IModuleLauncher))]
    public class SetupModuleLauncher : ModuleLauncher
    {
        public override ContentControl ButtonContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.CogsSolid;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }
        public override int Order
        {
            get
            {
                return Properties.Settings.Default.SetupModuleLauncherOrder;
            }
            set
            {
                Properties.Settings.Default.SetupModuleLauncherOrder = value;
                RaisePropertyChanged("Order");
            }
        }

        public override bool Visible
        {
            get
            {
                return Properties.Settings.Default.SetupModuleLauncherVisible;
            }

            set
            {
                Properties.Settings.Default.SetupModuleLauncherVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override void Save()
        {
            Properties.Settings.Default.Save();
        }
        public override string Name
        {
            get
            {

                return "Settings";
            }
        }

        public string IconSource
        {
            get { return "/iRadiate.Desktop.Common;component/Images/SetupIcon2White.png"; }
        }


        public override void Launch()
        {
            DesktopApplication.MainViewModel.LaunchModule(typeof(SetupModule));

        }
    }
}
