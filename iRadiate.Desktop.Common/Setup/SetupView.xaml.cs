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

using GalaSoft.MvvmLight;

namespace iRadiate.Desktop.Common.Setup
{
    /// <summary>
    /// Interaction logic for SetupView.xaml
    /// </summary>
    public partial class SetupView : UserControl
    {
        public SetupView()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ISettingsProvider provider = (ISettingsProvider)e.AddedItems[0];
            UserControl uc = DesktopApplication.GetViewVMB(provider as ViewModelBase);
            uc.DataContext = provider;
            SettingsViewModelContainer.Content = null;
            SettingsViewModelContainer.Content = uc;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ISettingsProvider provider = (sender as Button).DataContext as ISettingsProvider;
            UserControl uc = DesktopApplication.GetViewVMB(provider as ViewModelBase);
            uc.DataContext = provider;
            SettingsViewModelContainer.Content = null;
            SettingsViewModelContainer.Content = uc;

        }
    }
}
