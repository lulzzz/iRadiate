using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using iRadiate.DataModel;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace iRadiate.Desktop.Common.Setup
{
    [PreferredView("iRadiate.Desktop.Common.Setup.SetupView","iRadiate.Desktop.Common")]
    public class SetupModule : Module
    {
        private List<ISettingsProvider> _settingsProviders;
        public override string Name
        {
            get
            {
                return "Settings";
            }
            set
            {

            }
        }

        public override string IconSource
        {
            get { return "/iRadiate.Desktop.Common;component/Images/SetupIcon2.png"; }
        }

        public override ContentControl IconContent
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

        public override void GetData()
        {
            base.GetData();
            
        }

        public override void UIThreadInitialize()
        {
            base.UIThreadInitialize();

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        [ImportMany(typeof(ISettingsProvider))]
        public List<ISettingsProvider> SettingsProviders
        {
            get
            {
                if(_settingsProviders == null)
                {
                    _settingsProviders = new List<ISettingsProvider>();
                }
                return _settingsProviders;
            }
            set
            {
                _settingsProviders = value;
            }
        }
    }
}
