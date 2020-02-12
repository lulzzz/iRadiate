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

using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Settings.Common.View
{
    /// <summary>
    /// Interaction logic for RoomsView.xaml
    /// </summary>
    public partial class RoomsView : UserControl
    {
        public RoomsView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            iRadiate.Desktop.Common.DesktopApplication.MakeModalDocument(new DataStoreItemViewModel(RoomsGrid.SelectedItem as Room), Desktop.Common.DesktopApplication.DocumentMode.Edit);
        }
    }
}
