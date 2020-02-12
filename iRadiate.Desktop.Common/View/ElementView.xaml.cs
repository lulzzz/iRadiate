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

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for ElementView.xaml
    /// </summary>
    public partial class ElementView : UserControl
    {
        public ElementView()
        {
            InitializeComponent();
            var cvs = this.FindResource("AllIsotopes") as CollectionViewSource;
            cvs.Source = DesktopApplication.Librarian.GetItems(typeof(Isotope), new List<RetrievalCriteria>());
            cvs.SortDescriptions.Add(new System.ComponentModel.SortDescription("Weight", System.ComponentModel.ListSortDirection.Ascending));
        }
    }

    public class SecondsToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan ts = new TimeSpan(0, 0, (int)Math.Round((double)value,0));
            return ts;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((TimeSpan)value).TotalSeconds;
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
