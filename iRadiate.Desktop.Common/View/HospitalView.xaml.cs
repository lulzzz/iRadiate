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

using iRadiate.DataModel.HealthCare;
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for HospitalView.xaml
    /// </summary>
    public partial class HospitalView : UserControl
    {
        public HospitalView()
        {
            InitializeComponent();
        }

        private void AddWardButton_Click(object sender, RoutedEventArgs e)
        {
            Ward w = new Ward();
            w.Name = "Name";
            Hospital h = (Hospital)((sender as Button).DataContext as DataStoreItemViewModel).Item;
            w.Hospital = h;
            h.Wards.Add(w);
            WardsGrid.Items.Refresh();
        }
    }
}
