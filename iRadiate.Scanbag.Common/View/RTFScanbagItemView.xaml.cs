using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Documents;
using System.Xml;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iRadiate.Scanbag.Common.View
{
    /// <summary>
    /// Interaction logic for RTFScanbagItemView.xaml
    /// </summary>
    public partial class RTFScanbagItemView : UserControl
    {
        public RTFScanbagItemView()
        {
            InitializeComponent();
        }

        private void FlowDocReader_Unloaded(object sender, RoutedEventArgs e)
        {
            FlowDocReader.Document = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoThePrint(FlowDocReader.Document);
        }

        private void DoThePrint(System.Windows.Documents.FlowDocument document)
        {
           

            var str = XamlWriter.Save(document);
            var stringReader = new System.IO.StringReader(str);
            var xmlReader = XmlReader.Create(stringReader);
            var CloneDoc = XamlReader.Load(xmlReader) as FlowDocument;

            //Now print using PrintDialog
            var pd = new PrintDialog();

            if (pd.ShowDialog().Value)
            {
                CloneDoc.PageHeight = pd.PrintableAreaHeight;
                CloneDoc.PageWidth = pd.PrintableAreaWidth;
                
                CloneDoc.ColumnGap = 0;
                CloneDoc.ColumnWidth = pd.PrintableAreaWidth;
                CloneDoc.PagePadding = new Thickness(50);
                IDocumentPaginatorSource idocument = CloneDoc as IDocumentPaginatorSource;
                idocument.DocumentPaginator.PageSize = new Size(96 * 8.5, 96 * 11);
                pd.PrintDocument(idocument.DocumentPaginator, "Printing FlowDocument");
            }

        }
    }
}
