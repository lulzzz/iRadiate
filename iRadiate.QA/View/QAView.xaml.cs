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

using iRadiate.DataModel.Equipment;
using iRadiate.QA.ViewModel;
using iRadiate.DataModel.DataDictionary;
using System.Globalization;

namespace iRadiate.QA.View
{
    /// <summary>
    /// Interaction logic for QAView.xaml
    /// </summary>
    public partial class QAView : UserControl
    {
        public QAView()
        {
            InitializeComponent();
        }

        private void EquipmentTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //System.Diagnostics.Debug.WriteLine("Treeview selected item changed " + e.NewValue.GetType().ToString());
            var mod = this.DataContext as QAModule;
            mod.SelectedEquipment = e.NewValue as EquipmentItem;
            InsertButton.IsEnabled = true;
        }

        private void TextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("GotFocus on TextBlock, DataContext =  " + ((Control)sender).DataContext.GetType().ToString());
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var eq = e.Item as EquipmentItem;
            if (eq.Parent == null)
                e.Accepted = true;
            else
                e.Accepted = false;
        }

        private void AddToListButton_Click(object sender, RoutedEventArgs e)
        {
            //EquipmentTreeView.Items.Refresh();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            EquipmentTreeView.Items.Refresh();
        }

       

        

       
    }

    public class DataContextProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new DataContextProxy();
        }

        #endregion

        public object DataSource
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("DataSource", typeof(object), typeof(DataContextProxy), new UIPropertyMetadata(null));
    }

    public class BooleanEntryVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BooleanDataDictionaryEntry)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MeasurabeEntryVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MeasureableDataDictionaryEntry)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
