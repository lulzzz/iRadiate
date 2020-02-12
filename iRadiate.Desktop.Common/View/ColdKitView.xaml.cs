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
    /// Interaction logic for ColdKitView.xaml
    /// </summary>
    public partial class KitView : UserControl
    {
        public KitView()
        {
            InitializeComponent();
            CollectionViewSource cvs = FindResource("KitDefinitions") as CollectionViewSource;
            cvs.Source = DesktopApplication.Librarian.GetItems(typeof(KitDefinition), new List<RetrievalCriteria>());
        }
    }
}
