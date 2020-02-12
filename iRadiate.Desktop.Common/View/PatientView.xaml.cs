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
using iRadiate.Common.IO;

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for PatientView.xaml
    /// </summary>
    public partial class PatientView : UserControl
    {
        public PatientView()
        {
            InitializeComponent();
            var cvs = this.FindResource("AllWards") as CollectionViewSource;
            cvs.Source = DesktopApplication.Librarian.GetItems(typeof(Ward), new List<RetrievalCriteria>());
            cvs.SortDescriptions.Add(new System.ComponentModel.SortDescription("FullName",System.ComponentModel.ListSortDirection.Ascending));
        }
    }
}
