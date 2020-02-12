using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx;
using MahApps.Metro.IconPacks;
using Xceed.Wpf.Toolkit;

namespace iRadiate.Desktop.Common.Tools
{
    public class BaseSplitButtonTool : BaseExecutableTool
    {
        protected ContentControl _dropDownContent;

        public BaseSplitButtonTool() : base()
        {

        }

        #region publicProperties
        public override void UIThreadInitialise()
        {
            
            _toolContent = new System.Windows.Controls.ContentControl();
            //_toolContent.Padding = new System.Windows.Thickness(3);
            //_toolContent.Margin = new System.Windows.Thickness(2);
            SplitButton b = new SplitButton();
            b.SetResourceReference(Control.StyleProperty, "IconSplitButtonStyle");
           
            b.Content = Icon;
            b.Command = ExecuteCommand;
           
           
            //Border r = new Border();
            //r.BorderThickness = new System.Windows.Thickness(1);
            //r.BorderBrush = Brushes.Red;
            //r.Background = Brushes.LightGreen;
            //r.Height = 100;
            //r.Width = 100;

            //DropDownContent = new ContentControl();
            //DropDownContent.Content = r;
            //DropDownContent.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            b.DropDownContent = DropDownContent;
            _toolContent.Content = b;
        }
        #endregion

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
        #endregion
    }
}
