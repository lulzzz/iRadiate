using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using NLog;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;

using iRadiate.Common.IO;
using iRadiate.Common;

namespace iRadiate.Desktop.Common.ViewModel
{
    [Obsolete]
    public class HomeViewModel : Module
    {
        private static new Logger logger = LogManager.GetCurrentClassLogger();
        private int _unchangedEntities;
        private int _modifiedEntities;
        private AsyncObservableCollection<ModifiedDataStoreItem> _modifiedDataStoreItems;
        private ModifiedDataStoreItem _selectedModifiedDataStoreItem;

        public HomeViewModel()
        {
            
            RefreshCommand = new RelayCommand(RefreshData);
            ReloadLibraryCommand = new RelayCommand(ReloadLibrary);
        }

        public string UserName
        {
            get
            {
                return DesktopApplication.CurrentUser.FullName;
            }
        }

        public string PracticeName
        {
            get
            {
                return DesktopApplication.CurrentPratice.Name;
            }
        }

        public string ComputerName
        {
            get
            {
                return DesktopApplication.MainViewModel.CurrentWorkstation.Name;
            }
        }

        public string MemoryUsage
        {
            get
            {
                return (System.Diagnostics.Process.GetCurrentProcess().WorkingSet64/(1024*1024)).ToString();
            }
        }

        public override string IconSource
        {
            get
            {
                 return "/iRadiate.Desktop.Common;component/Images/HomeIcon.png"; 
            }
            
        }

        public override ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                
                PackIconFontAwesome icon = new PackIconFontAwesome();
                icon.Kind = PackIconFontAwesomeKind.HomeSolid;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
               
                return cc;
            }
        }

        public int UnchangedItems
        {
            get
            {
                return Platform.Retriever.TotalItemsTracked;

            }
        }

        public int ModifiedItems
        {
            get { return Platform.Retriever.NumberOfModifiedItems; }
        }

        public int ItemsRetrieved
        {
            get { return Platform.Retriever.NumberOfItemsRetrieved; }
        }

        public AsyncObservableCollection<ModifiedDataStoreItem> ModifiedDataStoreItems
        {
            get
            {
                if (_modifiedDataStoreItems == null)
                    _modifiedDataStoreItems = new AsyncObservableCollection<ModifiedDataStoreItem>();

                _modifiedDataStoreItems.Clear();
                foreach(var p in Platform.Retriever.ModifiedDataStoreItems)
                {
                    _modifiedDataStoreItems.Add(p);
                }
                return _modifiedDataStoreItems;
               
            }
        }

        public ModifiedDataStoreItem SelectedModifiedDataStoreItem
        {
            get { return _selectedModifiedDataStoreItem; }
            set { _selectedModifiedDataStoreItem = value; RaisePropertyChanged("SelectedModifiedDataStoreItem"); }
        }

        #region commands
        public RelayCommand RefreshCommand { get; set; }

        public RelayCommand ReloadLibraryCommand { get; set; }
        #endregion
        #region privateMethods
        private void RefreshData()
        {
            RaisePropertyChanged("ModifiedItems");
            RaisePropertyChanged("UnchangedItems");
            RaisePropertyChanged("ItemsRetrieved");
            RaisePropertyChanged("ModifiedDataStoreItems");
            
        }

        private void ReloadLibrary()
        {
            Platform.Retriever.ReloadAll();
        }
        #endregion
    }
}
