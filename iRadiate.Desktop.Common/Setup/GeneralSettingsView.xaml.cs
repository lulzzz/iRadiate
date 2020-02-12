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

using MahApps.Metro;

namespace iRadiate.Desktop.Common.Setup
{
    /// <summary>
    /// Interaction logic for GeneralSettingsView.xaml
    /// </summary>
    public partial class GeneralSettingsView : UserControl
    {
        public GeneralSettingsView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppStyle(DesktopApplication.MainWindow,
                                    ThemeManager.GetAccent("Red"),
                                    ThemeManager.GetAppTheme("BaseLight"));
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ThemeManager.ChangeAppStyle(Application.MainWindow,
                                    //ThemeManager.GetAccent(ColorComboBox.SelectedValue.ToString()),
                                    //ThemeManager.GetAppTheme("BaseLight"));
           //Application.MainWindow.Resources
        }

        private void PasswordBox1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            (this.DataContext as GeneralSettingsProvider).EnteredPassword = PasswordBox1.Password;
        }

        private void PasswordBox2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            (this.DataContext as GeneralSettingsProvider).ConfirmedPasword = PasswordBox2.Password;
        }
    }
}
