using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using GalaSoft.MvvmLight;
using MahApps.Metro.IconPacks;

using iRadiate.Desktop.Common;

namespace Reporting
{
    [Export(typeof(IModuleLauncher))]
    public class ReportingModuleLauncher : ModuleLauncher
    {
        public override string Name
        {
            get
            {
                return "Reporting";
            }
        }

        public override void Launch()
        {
            DesktopApplication.MainViewModel.LaunchModule(typeof(Reporting.ViewModel.ReportingModule));

        }

        public override System.Windows.Controls.ContentControl ButtonContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.FileChart;
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
                return Properties.Settings.Default.ReportingLauncherOrder;
            }

            set
            {
                Properties.Settings.Default.ReportingLauncherOrder = value;
            }
        }

        public override bool Visible
        {
            get
            {
                return Properties.Settings.Default.ReportingLauncherVisible;
            }

            set
            {
                Properties.Settings.Default.ReportingLauncherVisible = value;
            }
        }
    }
}
