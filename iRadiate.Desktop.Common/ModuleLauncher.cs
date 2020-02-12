using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;

namespace iRadiate.Desktop.Common
{

    public interface IModuleLauncher
    {
        RelayCommand LaunchCommand {get;set;}
        string Name { get; }
        void Launch();
        int Order { get; set; }
        bool Visible { get; set; }
        ContentControl ButtonContent { get; }

        /// <summary>
        /// Saves changes made to the module launcher
        /// </summary>
        void Save();
    }

    public abstract class ModuleLauncher : ViewModelBase, IModuleLauncher
    {

        protected Module _module;
        private RelayCommand _launchCommand;



        public RelayCommand LaunchCommand
        {
            get
            {
                if (_launchCommand == null)
                {
                    _launchCommand = new RelayCommand(Launch);
                }
                return _launchCommand;
            }
            set
            {
                _launchCommand = value;
            }
        }
        public virtual string Name
        {
            get
            {
                return _module.Name;
            }
        }

        public virtual void Launch()
        {
            throw new NotImplementedException();
        }

        public virtual void Save()
        {
            throw new NotImplementedException();
        }

        public virtual ContentControl ButtonContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.NintendoSwitch;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }

        public virtual int Order
        {
            get
            {
               
                return 0;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool Visible
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        
    }
}
