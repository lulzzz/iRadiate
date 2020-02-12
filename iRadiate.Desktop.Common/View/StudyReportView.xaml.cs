using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for StudyReportView.xaml
    /// </summary>
    public partial class StudyReportView : UserControl
    {
        public StudyReportView()
        {
            InitializeComponent();
        }

        private void FlowDocumentReader_Unloaded(object sender, RoutedEventArgs e)
        {
            FlowDocReader.Document = null;
        }
    }
}
