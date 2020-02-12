using System.Reflection;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;

using iRadiate.Desktop.Common.Login;

namespace iRadiate.Desktop.Common.Login
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch
            {

            }
            
        }

       

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            PinBox.Password = PinBox.Password + "7";
        }
        

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            PinBox.Password = PinBox.Password + "8";
        }
        
        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            PinBox.Password = PinBox.Password + "9";
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            PinBox.Password = PinBox.Password + "4";
        }


        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            PinBox.Password = PinBox.Password + "5";
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            PinBox.Password = PinBox.Password + "6";
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            PinBox.Password = PinBox.Password + "1";
        }


        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            PinBox.Password = PinBox.Password + "2";
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            PinBox.Password = PinBox.Password + "3";
        }

        private void Button0_Click(object sender, RoutedEventArgs e)
        {
            PinBox.Password = PinBox.Password + "0";
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((FrameworkElement)e.Source).GetType() == typeof(System.Windows.Controls.TabControl))
            {
                if (PINTabItem.IsSelected)
                {
                    PinBox.Focus();
                    Properties.Settings.Default.LastLoginMethod = 1;
                }
                else
                {
                    Properties.Settings.Default.LastLoginMethod = 0;
                }
                
            }
           
        }

        private void UserNameTab_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                e.Handled = true;
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(LoginButton, new object[] { true });
                var peer = UIElementAutomationPeer.CreatePeerForElement(LoginButton);
                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
              
                invokeProv.Invoke();
                //(this.DataContext as LoginViewModel).LoginCommand.Execute(PasswordBox);
            }
        }

        private void PINTabItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(PinLoginButton, new object[] { true });
                var peer = UIElementAutomationPeer.CreatePeerForElement(PinLoginButton);
                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
               
                invokeProv.Invoke();
                return;
            }
            var isNumber = e.Key >= Key.D0 && e.Key <= Key.D9;
            var isKeyPad = e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9;

            if(isNumber || isKeyPad)
            {
                int value = -1;
                if (isNumber)
                { // numpad
                    value = (int)e.Key - ((int)Key.D0);
                }
                else if (isKeyPad)
                { // regular numbers
                    value = (int)e.Key - ((int)Key.NumPad0);
                }
                PinBox.Password = PinBox.Password + value.ToString();
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
