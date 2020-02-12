using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Threading;
using iRadiate.Common;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.View;
using iRadiate.Desktop.Common.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using iRadiate.Common.IO;
using NLog;
using NLog.Config;

namespace iRadiate.Desktop.Common.Login
{
    public class LoginViewModel : ViewModelBase
    {
        #region privateFields
        private string _loginName;
        private string _password;
        private string _pin;
        private string _popupMessage;
        private bool _popupOpen = false;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region constructor
        public LoginViewModel()
        {
           
            LoginCommand = new RelayCommand<object>(x => AttemptLogin(x));
            CloseCommand = new RelayCommand(Close);
            ClosePopupCommand = new RelayCommand(ClosePopup);
            LoginName = Properties.Settings.Default.LastLoginName;
            PinLoginCommand = new RelayCommand<object>(x => AttemptLoginByPin(x));

        }
        #endregion

        #region publicPoperties
        public bool PopupOpen
        {
            get
            {
                return _popupOpen;
            }
            set
            {
                _popupOpen = value;
                RaisePropertyChanged("PopupOpen");
            }
        }

        public string PopupMessage
        {
            get
            {
                return _popupMessage;
            }
            set
            {
                _popupMessage = value;
                RaisePropertyChanged("PopupMessage");
            }
        }

        public string LoginName
        {
            get
            {
                return _loginName;
            }
            set
            {
                _loginName = value;
                RaisePropertyChanged("LoginName");
            }
        }

        public string Password
        {
            get
            {
                return _password;

            }
            set
            {
                _password = value;
                RaisePropertyChanged("Password");
            }
        }

        public string PIN
        {
            get
            {
                return _pin;
            }
            set
            {
                _pin = value;
                RaisePropertyChanged("PIN");
            }
        }

        public int LastLoginMethod
        {
            get { return Properties.Settings.Default.LastLoginMethod; }
            set
            {
                Properties.Settings.Default.LastLoginMethod = value;
                RaisePropertyChanged("LastLoginMethod");
            }
        }
        #endregion

        #region privateMethods
        public User AuthenticateUser(string login, string password, string PIN)
        {
            try
            {
                return iRadiate.Common.Authentication.Authenticator.AutheticateUser(login, password);
            }
            catch (Exception ex)
            {
               
                logger.Error(ex.Message);
            }
            return null;
        }

        public User AuthenticateUserByPin(string PIN)
        {
            try
            {
              
                List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
                RetrievalCriteria rc0 = new RetrievalCriteria("PinNumber", CriteraType.IsNotNull, PIN);
                RetrievalCriteria rc = new RetrievalCriteria("PinNumber", CriteraType.ExactTextMatch, PIN);
                rcList.Add(rc0);
                rcList.Add(rc);

                if (DesktopApplication.Librarian.GetItems(typeof(User), rcList).Any())
                {
                    
                    User u = (User)DesktopApplication.Librarian.GetItems(typeof(User), rcList).First();
                    
                    //Properties.Settings.Default.LastLoginName = login;
                    //Properties.Settings.Default.Save();
                    logger.Info("User authenticated by PIN");
                    return u;
                    
                }
                else
                {
                    logger.Info("Did not find a user to match that PIN.");
                    return null;
                }
            }
            catch (Exception ex)
            {

                logger.Error(ex.Message);
            }
            return null;
        }
        private void AttemptLoginByPin(object obj)
        {
            PasswordBox pwBox = obj as PasswordBox;
            User u = AuthenticateUserByPin(pwBox.Password);
            if (u != null)
            {

                MainWindow mw = new MainWindow();
                mw.WindowStyle = WindowStyle.SingleBorderWindow;
                mw.WindowState = WindowState.Maximized;

                mw.DataContext = DesktopApplication.MainViewModel;
                Platform.CurrentUser = u;
                //Get this workstation
                if (DesktopApplication.Librarian.GetItems(typeof(Workstation), new List<RetrievalCriteria>()).Where(x => (x as Workstation).Name == Environment.MachineName).Any())
                {
                    DesktopApplication.MainViewModel.CurrentWorkstation = (Workstation)DesktopApplication.Librarian.GetItems(typeof(Workstation), new List<RetrievalCriteria>()).Where(x => (x as Workstation).Name == Environment.MachineName).First();
                }
                else
                {
                    Workstation w = new Workstation();
                    w.Name = Environment.MachineName;
                    DesktopApplication.Librarian.SaveItem(w);
                    DesktopApplication.MainViewModel.CurrentWorkstation = w;
                    logger.Info("New workstation " + w.Name + " created and saved");
                }

                if (DesktopApplication.Librarian.GetItems(typeof(NucMedPractice), new List<RetrievalCriteria>()).Any())
                {
                    DesktopApplication.CurrentPratice = (NucMedPractice)DesktopApplication.Librarian.GetItems(typeof(NucMedPractice), new List<RetrievalCriteria>()).First();
                }

                iRadiate.Desktop.Common.DesktopApplication.CurrentUser = u;
                iRadiate.Desktop.Common.DesktopApplication.MainWindow = mw;
                mw.DataContext = DesktopApplication.MainViewModel;
                mw.InitializeComponent();
                mw.Show();
                MetroWindow element = FindParent<MetroWindow>(pwBox);
                element.Close();


            }
            else
            {
                PopupMessage = "Login details not correct - try again";
                PopupOpen = true;
                Password = "";
                PIN = "";
            }
        }
        private void AttemptLogin(object obj)
        {
            PasswordBox pwBox = obj as PasswordBox;
            

            User u = AuthenticateUser(LoginName, pwBox.Password, PIN);
            if (u != null)
            {
                
                MainWindow mw = new MainWindow();
                mw.WindowStyle = WindowStyle.SingleBorderWindow;
                mw.WindowState = WindowState.Maximized;
                
                mw.DataContext = DesktopApplication.MainViewModel;
                Platform.CurrentUser = u;
                //Get this workstation
                if (DesktopApplication.Librarian.GetItems(typeof(Workstation), new List<RetrievalCriteria>()).Where(x => (x as Workstation).Name == Environment.MachineName).Any())
                {
                    DesktopApplication.MainViewModel.CurrentWorkstation = (Workstation)DesktopApplication.Librarian.GetItems(typeof(Workstation), new List<RetrievalCriteria>()).Where(x => (x as Workstation).Name == Environment.MachineName).First();
                }
                else
                {
                    Workstation w = new Workstation();
                    w.Name = Environment.MachineName;
                    DesktopApplication.Librarian.SaveItem(w);
                    DesktopApplication.MainViewModel.CurrentWorkstation = w;
                    logger.Info("New workstation " + w.Name + " created and saved");
                }

                if (DesktopApplication.Librarian.GetItems(typeof(NucMedPractice), new List<RetrievalCriteria>()).Any())
                {
                    DesktopApplication.CurrentPratice = (NucMedPractice)DesktopApplication.Librarian.GetItems(typeof(NucMedPractice), new List<RetrievalCriteria>()).First();
                }
               
                iRadiate.Desktop.Common.DesktopApplication.CurrentUser = u;
                iRadiate.Desktop.Common.DesktopApplication.MainWindow = mw;
                mw.DataContext = DesktopApplication.MainViewModel; 
                mw.InitializeComponent();
                mw.Show();
                Properties.Settings.Default.LastLoginName = u.LoginName;
                Properties.Settings.Default.Save();
                MetroWindow element = FindParent<MetroWindow>(pwBox);
                element.Close();
               
            
            }
            else
            {
                PopupMessage = "Login details not correct - try again";
                PopupOpen = true;
                Password = "";
                PIN = "";
            }
        }

        private void AttemptPINLogin(object obj)
        {
            PasswordBox pwBox = obj as PasswordBox;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        private void Close()
        {

        }

        private void ClosePopup()
        {
            PopupOpen = false;
        }

        #endregion

        #region commands
        public RelayCommand<object> LoginCommand
        {
            get;
            set;
        }

        public RelayCommand<object> PinLoginCommand { get; set; }

        public RelayCommand CloseCommand
        {
            get;
            set;
        }

        public RelayCommand ClosePopupCommand
        {
            get;
            set;
        }
        #endregion
    }
}
