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

using iRadiate.Common.IO;
using iRadiate.DataModel.Radiopharmacy;
using iRadiate.Desktop.Common;

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for BulkDoseView.xaml
    /// </summary>
    public partial class ReconstitutedColdKitView : UserControl
    {
        public ReconstitutedColdKitView()
        {
            InitializeComponent();
            CollectionViewSource cvs = FindResource("Radiopharmaceuticals") as CollectionViewSource;
            cvs.Source = DesktopApplication.Librarian.GetItems(typeof(Chemical), new List<RetrievalCriteria>());
        }

        private void ExpiryDateListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            switch(ExpiryDateListBox.SelectedValue.ToString())
            {
                case "System.Windows.Controls.ListBoxItem: Now":
                    ExpiryDatePicker.SelectedDate = DateTime.Now;
                    break;
                case "System.Windows.Controls.ListBoxItem: +6 hours":
                    ExpiryDatePicker.SelectedDate = DateTime.Now.AddHours(6);
                    break;
                case "System.Windows.Controls.ListBoxItem: +12 hours":
                    ExpiryDatePicker.SelectedDate = DateTime.Now.AddHours(12);
                    break;
                case "System.Windows.Controls.ListBoxItem: +24 hours":
                    ExpiryDatePicker.SelectedDate = DateTime.Now.AddHours(24);
                    break;
                default:
                    ExpiryDatePicker.SelectedDate = DateTime.Now.AddHours(-6);
                    break;
            }
            ExpiryDateDropDownButton.IsOpen = false;
        }
    }
}
