using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.IO;
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
using Microsoft.Win32;

using iRadiate.Desktop.Common;
using iRadiate.Scanbag.Common.ViewModel;

namespace iRadiate.Scanbag.Common.View
{
    /// <summary>
    /// Interaction logic for DocumentWriterView.xaml
    /// </summary>
    public partial class DocumentWriterView : UserControl
    {
        

        public DocumentWriterView()
        {
            InitializeComponent();
           
        }

        public void ToggleBold()
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IStudyDataProvider provider = DataProviderComboBox.SelectedItem as IStudyDataProvider;
            if (provider.IsFormattable)
            {
                string format = "{0:"+((ComboBoxItem)FormatSelectorComboBox.SelectedValue).Content.ToString()+"}";
                rtb.CaretPosition.InsertTextInRun(String.Format(format,provider.GetData()));
               
                
            }
            else
            {
                DocumentWriterViewModel vm = this.DataContext as DocumentWriterViewModel;

                rtb.CaretPosition.InsertTextInRun(vm.CurrentProvider.GetData().ToString());
            }
            
            rtb.Focus();
        }

        private void DataProviderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DocumentWriterViewModel vm = this.DataContext as DocumentWriterViewModel;
            if ((vm.Description == null) || (vm.Description.Trim() == ""))
            {
                iRadiate.Desktop.Common.DesktopApplication.ShowDialog("Error", "Cannot save document without description");
                return;
            }
            var content = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            if (content.CanSave(DataFormats.Rtf))
            {
                using (var stream = new MemoryStream())
                {
                    content.Save(stream, DataFormats.Rtf);
                    iRadiate.DataModel.Common.File f = new DataModel.Common.File();

                    f.Data = stream.ToArray();
                    f.Extension = "rtf";
                    f.Study = vm.Study;
                    f.Description = vm.Description;
                    iRadiate.Desktop.Common.DesktopApplication.GetLibrarian().SaveItem(f);
                }
                
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "rtf";
            openFileDialog.Filter = "rtf files (*.rtf)|*.rtf";
            if (openFileDialog.ShowDialog() == true)
            {
                FlowDocument flowDocument = new FlowDocument();
                FileStream fs = new FileStream(openFileDialog.FileName,FileMode.Open);
                TextRange textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                textRange.Load(fs, DataFormats.Rtf);
                rtb.Document = flowDocument;
            }
        }

        
    }
}
