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
    /// Interaction logic for SplitUnitDosePrintView.xaml
    /// </summary>
    public partial class SplitUnitDosePrintView : UserControl
    {
        public SplitUnitDosePrintView()
        {
            InitializeComponent();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PreCalTimeBox.Visibility = Visibility.Visible;
            PreCalTimePicker.Visibility = Visibility.Hidden;

            var printDialog = new PrintDialog();
            if (Properties.Settings.Default.ConfirmPrinterForLabels)
            {
                if (printDialog.ShowDialog() == true)
                {
                    MainDockPanel.Children.Clear();
                    FlowDocument fd = new FlowDocument();
                    fd.DataContext = MainDockPanel.DataContext;
                    fd.Blocks.Add(new BlockUIContainer(LabelGrid));
                    fd.PagePadding = new Thickness(20);
                    fd.PageHeight = printDialog.PrintableAreaHeight;
                    fd.PageWidth = printDialog.PrintableAreaWidth;
                    var paginator = ((IDocumentPaginatorSource)fd).DocumentPaginator;
                    printDialog.PrintDocument(paginator, "Dose Label");
                    DesktopApplication.CloseActiveWindow();
                }
            }
            else
            {
                MainDockPanel.Children.Clear();
                FlowDocument fd = new FlowDocument();
                fd.DataContext = MainDockPanel.DataContext;
                fd.Blocks.Add(new BlockUIContainer(LabelGrid));
                fd.PagePadding = new Thickness(20);
                fd.PageHeight = printDialog.PrintableAreaHeight;
                fd.PageWidth = printDialog.PrintableAreaWidth;
                var paginator = ((IDocumentPaginatorSource)fd).DocumentPaginator;
                printDialog.PrintDocument(paginator, "Dose Label");
                DesktopApplication.CloseActiveWindow();
            }
        }
    }
}
