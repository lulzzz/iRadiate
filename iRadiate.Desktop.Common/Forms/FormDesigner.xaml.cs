using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iRadiate.Desktop.Common.Forms
{
    /// <summary>
    /// Interaction logic for FormDesigner.xaml
    /// </summary>
    public partial class FormDesigner : UserControl
    {
        public FormDesigner()
        {
            InitializeComponent();
            MyFormViewer.Width = Convert.ToDouble(FormWidthBox.Text);
            MyFormViewer.Height = Convert.ToDouble(FormHeightBox.Text);
        }

        private void TextBox_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            
        }

        private void MyFormViewer_ControlGotFocus(object sender, EventArgs e)
        {
            

        }

        private void FormWidthBox_LostFocus(object sender, RoutedEventArgs e)
        {
            MyFormViewer.Width = Convert.ToDouble(FormWidthBox.Text);
            MyFormViewer.Height = Convert.ToDouble(FormHeightBox.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DesktopApplication.ShowToastInformation("button clicked", DesktopApplication.NotificationPosition.BottomRight);
        }
    }
}
