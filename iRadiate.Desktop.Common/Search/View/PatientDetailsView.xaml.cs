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
using iRadiate.Desktop.Common;

using iRadiate.Common.IO;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Search.View
{
    /// <summary>
    /// Interaction logic for PatientDetailsView.xaml
    /// </summary>
    public partial class PatientDetailsView : UserControl
    {
        public PatientDetailsView()
        {
            InitializeComponent();
            var cvs = this.FindResource("AllStudyTypes") as CollectionViewSource;
            cvs.Source = iRadiate.Desktop.Common.DesktopApplication.Librarian.GetItems(typeof(StudyType), new List<RetrievalCriteria>());
            cvs.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddTaskExpander.Visibility = Visibility.Visible;
            AddTaskExpander.IsExpanded = true;
        }
    }
}
