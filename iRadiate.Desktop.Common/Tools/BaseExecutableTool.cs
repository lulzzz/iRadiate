using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;

namespace iRadiate.Desktop.Common.Tools
{
    public class BaseExecutableTool : BaseTool
    {
        protected virtual void Execute()
        {

        }
        public BaseExecutableTool() : base()
        {

        }
        public RelayCommand ExecuteCommand
        {
            get;set;
        }
        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            ExecuteCommand = new RelayCommand(Execute);
        }
        public override void UIThreadInitialise()
        {
            
            //base.UIThreadInitialise();
            _toolContent = new System.Windows.Controls.ContentControl();
            Button b = new Button();
           
            b.SetResourceReference(Control.StyleProperty, "SquareIconButton");
            b.Content = Icon;
            b.Command = ExecuteCommand;
            _toolContent.Content = b;
            
        }

        public override ContentControl ToolContent
        {
            get
            {
                return base.ToolContent;
            }
        }

    }
}
