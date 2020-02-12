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
using iRadiate.Common.IO;

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for CapsuleUnitDoseView.xaml
    /// </summary>
    public partial class CapsuleUnitDoseView : UserControl
    {
        public CapsuleUnitDoseView()
        {
            InitializeComponent();
            CollectionViewSource cvs = FindResource("PotentialPatients") as CollectionViewSource;
            cvs.Source = DesktopApplication.Librarian.GetItems(typeof(DoseAdministrationTask), new List<RetrievalCriteria>());
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
            if (printDlg.ShowDialog() == true)
            {


                //now print the visual to printer to fit on the one page.
                printDlg.PrintVisual(this, "First Fit to Page WPF Print");

            }
        }
    }
}
