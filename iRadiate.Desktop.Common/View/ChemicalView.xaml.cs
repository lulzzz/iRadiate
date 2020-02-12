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
    /// Interaction logic for RadiopharmaceuticalView.xaml
    /// </summary>
    public partial class ChemicalView : UserControl
    {
        public ChemicalView()
        {
            InitializeComponent();
            CollectionViewSource cvs = (CollectionViewSource)FindResource("AllIsotopes");
            cvs.Source = DesktopApplication.Librarian.GetItems(typeof(Isotope), new List<RetrievalCriteria>());

        }
    }
}
