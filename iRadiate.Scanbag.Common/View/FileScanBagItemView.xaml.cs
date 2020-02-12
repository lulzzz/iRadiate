using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iRadiate.Scanbag.Common.ViewModel;
using PdfiumViewer;

namespace iRadiate.Scanbag.Common.View
{
    /// <summary>
    /// Interaction logic for FileScanBagItemView.xaml
    /// </summary>
    public partial class FileScanBagItemView : UserControl
    {
        public FileScanBagItemView()
        {
            InitializeComponent();
            
        }

        private void loadDocument()
        {
            FileScanBagItem con = (FileScanBagItem)this.DataContext;
            MyPdfViewer.Document = PdfiumViewer.PdfDocument.Load(new MemoryStream(con.FileArray));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadDocument();
        }
    }

    public static class WebBrowserUtility
    {
        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = o as WebBrowser;
            if (browser != null)
            {
                string uri = e.NewValue as string;
                browser.Source = !String.IsNullOrEmpty(uri) ? new Uri(uri) : null;
            }
        }

    }
}
