using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel.Composition;

using MahApps.Metro.IconPacks;

using iRadiate.Desktop.Common;
using iRadiate.QA.ViewModel;

namespace iRadiate.QA
{
    [Export(typeof(IModuleLauncher))]
    public class QAModuleLauncher : ModuleLauncher
    {
        public override string Name
        {
            get
            {
                return "Quality Assurance";
            }
        }

        public override bool Visible
        {
            get
            {
                return Properties.Settings.Default.QALauncherVisible;
            }

            set
            {
                Properties.Settings.Default.QALauncherVisible = value;
            }
        }

        public override int Order
        {
            get
            {
                return Properties.Settings.Default.QALauncherOrder;
            }

            set
            {
                Properties.Settings.Default.QALauncherOrder = value;
            }
        }

        public override ContentControl ButtonContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.BullseyeArrow;
                
                 icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }

        public override void Save()
        {
            Properties.Settings.Default.Save();
        }

        public override void Launch()
        {
            DesktopApplication.MainViewModel.LaunchModule(typeof(QAModule));
        }
    }
}
