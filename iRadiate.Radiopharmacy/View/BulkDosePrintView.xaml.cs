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

using iRadiate.Desktop.Common;

namespace iRadiate.Radiopharmacy.View
{
    /// <summary>
    /// Interaction logic for BulkDosePrintView.xaml
    /// </summary>
    public partial class BulkDosePrintView : UserControl
    {
        public BulkDosePrintView()
        {
            InitializeComponent();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {

            var printDialog = new PrintDialog();
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
    }
}
