using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using ControlzEx;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;
using MahApps.Metro.IconPacks.Converter;

namespace iRadiate.Desktop.Common.Tools
{
    public abstract class BaseTool : ViewModelBase, ITool
    {
        protected ContentControl _toolContent;
        private bool _available;

        public BaseTool()
        {

        }

        public virtual string Name
        {
            get
            {
                return "BaseTool";
            }
        }

        public RelayCommand SaveCommand
        {
            get;set;
        }

        public virtual string ToolTipText
        {
            get
            {
                return "BaseToolTipText";
            }
        }

        public virtual void NonUIThreadInitialise()
        {
            SetRelayCommands();
        }

        public virtual void Save()
        {
            throw new NotImplementedException();
        }

        public virtual void UIThreadInitialise()
        {
            _toolContent = new ContentControl();
            _toolContent.Content = Icon;
        }

        protected virtual void SetRelayCommands()
        {
            SaveCommand = new RelayCommand(Save);
        }

        /// <summary>
        /// The icon that represents the button, size is set by a user
        /// </summary>
        protected virtual PackIconBase Icon
        {
            get
            {
                PackIconMaterial m = new PackIconMaterial();
                m.Kind = PackIconMaterialKind.NintendoSwitch;
                m.Height = DesktopApplication.IconHeight;
                m.Width = DesktopApplication.IconWidth;
                return m;

            }
        }

        public bool Available
        {
            get
            {
                return _available;
            }

            set
            {
                _available = value;
                RaisePropertyChanged("Available");
            }
        }

        public virtual ContentControl ToolContent
        {
            get
            {
                return _toolContent;
            }
        }
    }
}
