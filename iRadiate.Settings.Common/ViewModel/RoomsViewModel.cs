using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace iRadiate.Settings.Common.ViewModel
{
    public class RoomsViewModel : Module
    {
        private AsyncObservableCollection<IDataStoreItem> _rooms;

        public RoomsViewModel()
        {
            
            
            AddRoomCommand = new RelayCommand(AddRoom);
        }

        public AsyncObservableCollection<IDataStoreItem> Rooms
        {
            get
            {
                if (_rooms == null)
                {
                    _rooms = new AsyncObservableCollection<IDataStoreItem>();
                }
                return _rooms;
            }
            set
            {
                _rooms = value;
            }
        }

        public RelayCommand AddRoomCommand
        {
            get;
            private set;
        }

        private void AddRoom()
        {
            Room r = new Room();
            Rooms.Add(r);
            r.NucMedPractice = DesktopApplication.CurrentPratice;
            DataStoreItemViewModel rvm = new DataStoreItemViewModel(r);
            DesktopApplication.MakeModalDocument(rvm, DesktopApplication.DocumentMode.New);
        }

        public override void GetData()
        {
            
            Rooms = Platform.CreateCollection();
            foreach(Room r in DesktopApplication.CurrentPratice.Rooms)
            {
                Rooms.Add(r);
            }
        }
    }
}
