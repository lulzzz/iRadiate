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
    /// Interaction logic for GeneratorView.xaml
    /// </summary>
    public partial class GeneratorView : UserControl
    {
        public GeneratorView()
        {
            InitializeComponent();
            var cvs = this.FindResource("AllIsotopes") as CollectionViewSource;
            cvs.Source = DesktopApplication.Librarian.GetItems(typeof(Isotope), new List<RetrievalCriteria>());
            cvs.SortDescriptions.Add(new System.ComponentModel.SortDescription("Weight", System.ComponentModel.ListSortDirection.Ascending));
        }
    }
}
