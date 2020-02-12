using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

using iRadiate.DataModel.DataDictionary;

namespace iRadiate.Desktop.Common.DataDictionary
{
    /// <summary>
    /// Interaction logic for DataDictionaryView.xaml
    /// </summary>
    public partial class DataDictionaryView : UserControl
    {
        public DataDictionaryView()
        {
            InitializeComponent();
        }

        private void DictionaryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is DataDictionaryEntry)
            {
                var mod = this.DataContext as DataDictionaryModule;
                mod.SelectedEntry = e.NewValue as DataDictionaryEntry;
                InsertRootNamespaceButton.IsEnabled = false;
                InsertEntryButton.IsEnabled = false;
                mod.SelectedNamespace = null;
            }
            else if (e.NewValue is DataDictionaryNamespace)
            {
                var mod = this.DataContext as DataDictionaryModule;
                mod.SelectedEntry = null;
                InsertRootNamespaceButton.IsEnabled = true;
                InsertEntryButton.IsEnabled = true;
                mod.SelectedNamespace = e.NewValue as DataDictionaryNamespace;
            }
        }

        private void DictionaryRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            DictionaryTreeView.Items.Refresh();

        }
        private void CollectionViewSource_Filter_1(object sender, FilterEventArgs e)
        {
            var ns = e.Item as DataDictionaryNamespace;
            if (ns.ParentNamespace == null)
                e.Accepted = true;
            else
                e.Accepted = false;
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
                return Visibility.Collapsed;
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
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
}
