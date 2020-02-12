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

namespace iRadiate.Radiopharmacy.View
{
    /// <summary>
    /// Interaction logic for RadiopharmacyView.xaml
    /// </summary>
    public partial class RadiopharmacyView : UserControl
    {
        public RadiopharmacyView()
        {
            InitializeComponent();
        }

        private void AddToListButton_Click(object sender, RoutedEventArgs e)
        {
            popup1.IsOpen = true;
        }

        private void popup1_MouseLeave(object sender, MouseEventArgs e)
        {
            popup1.IsOpen = false;
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterPopup.IsOpen = true;
        }

        private void FilterButton_MouseLeave(object sender, MouseEventArgs e)
        {
            FilterPopup.IsOpen = false;
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void popup1_Opened(object sender, EventArgs e)
        {
            InventoryTypeListBox.SelectedIndex = -1;
        }

        private void ItemsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object o = this.DataContext;
            if(o is RadiopharmacyModule)
            {
                var rm = o as RadiopharmacyModule;
                rm.SelectedInventoryItems = ItemsDataGrid.SelectedItems;
            }
        }
    }

    public class BindingProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}
