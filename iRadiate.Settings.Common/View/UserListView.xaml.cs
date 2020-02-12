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

using iRadiate.DataModel.Common;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Settings.Common.View
{
    /// <summary>
    /// Interaction logic for UserList.xaml
    /// </summary>
    public partial class UserListView : UserControl
    {

        public UserListView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(UserGrid.SelectedItem != null)
            {
                iRadiate.Desktop.Common.DesktopApplication.MakeModalDocument(new DataStoreItemViewModel(UserGrid.SelectedItem as User), Desktop.Common.DesktopApplication.DocumentMode.Edit);
            }
            //iRadiate.Desktop.Common.Application.ShowDialog("Alert", ((sender as Button).DataContext as IDataStoreItem).ConcreteType.ToString());
            
        }
    }
}
