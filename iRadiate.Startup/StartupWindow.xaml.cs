using System;
using System.Threading;

using MahApps.Metro;
using MahApps.Metro.Controls;
using NLog;

using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Desktop.Common.Login;



namespace iRadiate.Startup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StartupWindow : MetroWindow
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public StartupWindow()
        {
         
            var config = new NLog.Config.LoggingConfiguration();
           
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "log.log" };
            var coloredConsole = new NLog.Targets.ColoredConsoleTarget("coloredConsole");
            
            logfile.ArchiveAboveSize = 1000000;
            logfile.ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Sequence;
            logfile.MaxArchiveFiles = 1;
            logfile.ReplaceFileContentsOnEachWrite = true;
          
            if(DesktopApplication.TraceDebug)
            {
                config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);
            }
            else
            {
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            }
            config.AddRule(LogLevel.Info, LogLevel.Fatal, coloredConsole);
            //NLog.LogManager.Configuration = config;



            logger.Info("Application launched");
            MainViewModel mvm = new MainViewModel();
            DesktopApplication.MainViewModel = mvm;
            LoginViewModel lvm = new LoginViewModel();
            InitializeComponent();
            string colour = DesktopApplication.ThemeAccent;
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current.Resources, ThemeManager.GetAccent(colour),
                                    ThemeManager.GetAppTheme("BaseLight"));
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            DesktopApplication.MainViewModel.SetSynchronizationContext(SynchronizationContext.Current);
            
        }

        
    }
}
