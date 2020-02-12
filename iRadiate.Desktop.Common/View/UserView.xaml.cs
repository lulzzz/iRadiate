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
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for PersonControl.xaml
    /// </summary>
    public partial class UserView : UserControl
    {
        public UserView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string hashed = iRadiate.Common.Authentication.Authenticator.HashPassword(PasswordBox.Password);
            IDataStoreItem d = (this.DataContext as DataStoreItemViewModel).Item;
            (d as User).Password = hashed;
        }
    }
}
