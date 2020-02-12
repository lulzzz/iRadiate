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

using iRadiate.DataModel.Radiopharmacy;
using iRadiate.Common.IO;

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for KitDefinitionView.xaml
    /// </summary>
    public partial class KitDefinitionView : UserControl
    {
        public KitDefinitionView()
        {
            InitializeComponent();
            CollectionViewSource cvs = FindResource("Radiopharmaceuticals") as CollectionViewSource;
            cvs.Source = DesktopApplication.Librarian.GetItems(typeof(Chemical), new List<RetrievalCriteria>());

            CollectionViewSource cvs2 = FindResource("Radiopharmaceuticals2") as CollectionViewSource;
            cvs2.Source = DesktopApplication.Librarian.GetItems(typeof(Chemical), new List<RetrievalCriteria>());
        }
    }
}
