using System;

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using MahApps.Metro.IconPacks;

using iRadiate.Desktop.Common;

namespace iRadiate.Interfaces.DICOM
{
    [Export(typeof(IModuleLauncher))]
    public class DicomModuleLauncher : ModuleLauncher
    {
        public DicomModuleLauncher() : base()
        {

        }

        public override ContentControl ButtonContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.XRaySolid;
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
                return "Dicom Interface";
                    
            }
        }

        public override int Order
        {
            get
            {
                return Properties.Settings.Default.DicomModuleLauncherOrder;
            }
            set
            {
                Properties.Settings.Default.DicomModuleLauncherOrder = value;
                RaisePropertyChanged("Order");
            }
        }

        public override bool Visible
        {
            get
            {
                return Properties.Settings.Default.DicomModuleLauncherVisible;
            }

            set
            {
                Properties.Settings.Default.DicomModuleLauncherVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override void Save()
        {
            Properties.Settings.Default.Save();
        }
        public override void Launch()
        {
            DesktopApplication.MainViewModel.LaunchModule(typeof(DicomModule));
        }
    }
}
