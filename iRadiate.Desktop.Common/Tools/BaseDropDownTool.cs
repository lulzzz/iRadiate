using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Windows.Controls;
using ControlzEx;
using MahApps.Metro.IconPacks;
using Xceed.Wpf.Toolkit;

namespace iRadiate.Desktop.Common.Tools
{
    public abstract class BaseDropDownTool : BaseTool
    {
        protected ContentControl _dropDownContent;
        public BaseDropDownTool() : base()
        {

        }

       

        #region virtuals
        protected virtual ContentControl DropDownContent
        {
            get
            {
                return _dropDownContent;
            }
            set
            {
                _dropDownContent = value;
            }

        }
        #endregion

        #region overrides
        protected override PackIconBase Icon
        {
            get
            {
                var i = new PackIconModern();
                i.Kind = PackIconModernKind.Acorn;
                i.Width = DesktopApplication.IconWidth;
                i.Height = DesktopApplication.IconHeight;
                return i;
            }
        }

        public override void UIThreadInitialise()
        {

            _toolContent = new System.Windows.Controls.ContentControl();
            //_toolContent.Padding = new System.Windows.Thickness(3);
            //_toolContent.Margin = new System.Windows.Thickness(2);
            DropDownButton b = new DropDownButton();
            b.SetResourceReference(Control.StyleProperty, "IconDropDownButtonStyle");
           
            b.Content = Icon;

           
            //b.IsOpen = true;
            b.DropDownContent = DropDownContent;
            _toolContent.Content = b;
        }
        #endregion
    }
}
