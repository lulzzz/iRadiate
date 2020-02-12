using System.ComponentModel.Composition;
using System.Windows.Controls;

using MahApps.Metro.IconPacks;

using iRadiate.Desktop.Common;

namespace iRadiate.Radiopharmacy
{
    [Export(typeof(IModuleLauncher))]
    public class RadiopharmacyModuleLauncher : ModuleLauncher
    {
        public override string Name
        {
            get
            {
                return "Radiopharmacy";
                   
            }
        }

        public override ContentControl ButtonContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Pharmacy;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }

        public override void Launch()
        {
            DesktopApplication.MainViewModel.LaunchModule(typeof(RadiopharmacyModule));
        }

        public string IconSource
        {
            get { return "/iRadiate.Radiopharmacy;component/Images/RadiopharmacyIconWhite.png"; }
        }

        public override int Order
        {
            get
            {
                return Properties.Settings.Default.RadiopharmacyLauncherOrder;
            }

            set
            {
                Properties.Settings.Default.RadiopharmacyLauncherOrder = value;
                RaisePropertyChanged("Order");
            }
        }

        public override bool Visible
        {
            get
            {
                return Properties.Settings.Default.RadiopharmacyLauncerVisible;
            }

            set
            {
                Properties.Settings.Default.RadiopharmacyLauncerVisible = value;
                RaisePropertyChanged("Visible");
            }
        }

        public override void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
