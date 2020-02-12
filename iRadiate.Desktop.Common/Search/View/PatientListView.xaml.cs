using System.Windows.Controls;
using System.Windows.Input;

using iRadiate.Desktop.Search.ViewModel;

namespace iRadiate.Desktop.Search.View
{
    /// <summary>
    /// Interaction logic for PatientListView.xaml
    /// </summary>
    public partial class PatientListView : UserControl
    {
        public PatientListView()
        {
            InitializeComponent();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                
                (this.DataContext as PatientListViewModel).SearchCommand.Execute(null);
            }
        }
    }
}
