using System;
using System.Deployment.Application;
using System.IO;
using System.Windows;
using System.Runtime.InteropServices;
using iRadiate.Common;
using iRadiate.Desktop.Common.ViewModel;
using NLog;

namespace iRadiate.Desktop.Test
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [STAThread]
        static void Main(string[] args)
        {
            logger.Info("               ");
            logger.Info("***************");
            logger.Info("Test program started");
            logger.Trace("Main()...");
            

            var handle = GetConsoleWindow();
            try
            {
                using (StreamReader sr = new StreamReader(ApplicationDeployment.CurrentDeployment.DataDirectory + @"\UserData.xml"))
                {
                    //MessageBox.Show(sr.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "Exception caught in Main(string[] args): " + ex.Message);
                //MessageBox.Show("Could not read file. Error message: " + ex.Message);
            }

            // Hide
            ShowWindow(handle, SW_HIDE);

            // Show
            //ShowWindow(handle, SW_SHOW);

           

            iRadiate.Desktop.Common.Login.LoginViewModel lvm = new Common.Login.LoginViewModel();
            iRadiate.Desktop.Common.Login.LoginWindow loginWindow = new Common.Login.LoginWindow();
            MainViewModel mvm = new MainViewModel();
            iRadiate.Desktop.Common.DesktopApplication.MainViewModel = mvm;

            
            loginWindow.BorderThickness = new System.Windows.Thickness(1);
            loginWindow.DataContext = lvm;

            try
            {
                System.Windows.Application app = new System.Windows.Application();
                
                mvm.WindowsApplication = app;
                //loginWindow.Show();
                app.ShutdownMode = ShutdownMode.OnLastWindowClose;
                
                app.Run(loginWindow);
            }
            catch (Exception ex)
            {
                ShowWindow(handle, SW_SHOW);
                MessageBox.Show("Caught major exception " + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                
            }

            
            
        

            Console.WriteLine("Done.....");
            //Console.ReadLine();
            
            
            logger.Info("Test program complete");
            logger.Info("***************");
            logger.Info("               ");
        }
    }
}
