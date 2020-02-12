using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


using MahApps.Metro.IconPacks;

using iRadiate.Desktop.Common;

namespace iRadiate.Desktop.Common.DataDictionary
{
    [Export(typeof(IModuleLauncher))]
    public class DictionaryLauncher : ModuleLauncher
    {
        public override string Name
        {
            get
            {
                return "Data Dictionary";
            }
        }

        public override bool Visible
        {
            get
            {
                return Properties.Settings.Default.DataDictionaryVisible;
            }

            set
            {
                Properties.Settings.Default.DataDictionaryVisible = value;
            }
        }

        public override int Order
        {
            get
            {
                return Properties.Settings.Default.DataDictionaryLauncherOrder;
            }

            set
            {
                Properties.Settings.Default.DataDictionaryLauncherOrder = value;
            }
        }

        public override ContentControl ButtonContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Dictionary;

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
            DesktopApplication.MainViewModel.LaunchModule(typeof(DataDictionaryModule));
        }
    }
}
