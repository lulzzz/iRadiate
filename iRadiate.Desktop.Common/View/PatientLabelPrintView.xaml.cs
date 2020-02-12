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

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for PatientLabelPrintView.xaml
    /// </summary>
    public partial class PatientLabelPrintView : UserControl
    {
        public PatientLabelPrintView()
        {
            InitializeComponent();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                PrintButton.Visibility = Visibility.Hidden;
                MainGrid.Children.Clear();
                FlowDocument fd = new FlowDocument();
                fd.DataContext = MainGrid.DataContext;
                fd.Blocks.Add(new BlockUIContainer(LabelGrid));
                fd.PagePadding = new Thickness(20);
                fd.PageHeight = printDialog.PrintableAreaHeight;
                fd.PageWidth = printDialog.PrintableAreaWidth;
                var paginator = ((IDocumentPaginatorSource)fd).DocumentPaginator;
                printDialog.PrintDocument(paginator, "Address Label");
                DesktopApplication.CloseActiveWindow();
            }
        }
    }
}
