using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using MahApps.Metro;
using MahApps.Metro.Controls;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.Toolkit;

using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly DispatcherTimer _activityTimer;
        private Point _inactiveMousePosition = new Point(0, 0);
        public TimeSpan TimeoutToHide { get; private set; }
        public DateTime LastMouseMove { get; private set; }


        public MainWindow()
        {
            InitializeComponent();
            HomeControl.DataContext = new HomeViewModel();
            TimeoutToHide = TimeSpan.FromSeconds(20);
            InputManager.Current.PreProcessInput += OnActivity;
            _activityTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1), IsEnabled = true };
            _activityTimer.Tick += OnInactivity;
        }

        private void OnInactivity(object sender, EventArgs e)
        {
            _inactiveMousePosition = Mouse.GetPosition(this);
            
            DesktopApplication.FireIdleEvent();
        }
        void OnActivity(object sender, PreProcessInputEventArgs e)
        {
            InputEventArgs inputEventArgs = e.StagingItem.Input;

            if (inputEventArgs is MouseEventArgs || inputEventArgs is KeyboardEventArgs)
            {
                if (e.StagingItem.Input is MouseEventArgs)
                {
                    MouseEventArgs mouseEventArgs = (MouseEventArgs)e.StagingItem.Input;

                    // no button is pressed and the position is still the same as the application became inactive
                    if (mouseEventArgs.LeftButton == MouseButtonState.Released &&
                        mouseEventArgs.RightButton == MouseButtonState.Released &&
                        mouseEventArgs.MiddleButton == MouseButtonState.Released &&
                        mouseEventArgs.XButton1 == MouseButtonState.Released &&
                        mouseEventArgs.XButton2 == MouseButtonState.Released &&
                        _inactiveMousePosition == mouseEventArgs.GetPosition(this))
                        return;
                }

                // set UI on activity
               

                _activityTimer.Stop();
                _activityTimer.Start();
            }
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current.Resources, ThemeManager.GetAccent(Properties.Settings.Default.ColorTheme),
                                    ThemeManager.GetAppTheme("BaseLight"));
        }
        
       

        private void HamburgerMenu_OnOptionsItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as HamburgerMenuItem;            
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ModuleLauncherDockPanel.Width < 100)
            {
                ModuleLauncherDockPanel.Width = 160;
                Properties.Settings.Default.ModuleLauncherCollapsed = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                ModuleLauncherDockPanel.Width = 40;
                Properties.Settings.Default.ModuleLauncherCollapsed = true;
                Properties.Settings.Default.Save();
            }
            
        }

        private void MetroWindow_MouseMove(object sender, MouseEventArgs e)
        {
            
            LastMouseMove = DateTime.Now;
        }

        

       

        
            
            
            
       


    }
}
