using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using iRadiate.DataModel.Common;

namespace iRadiate.Desktop.Common
{
    public interface IDataStoreItemViewModel : IModule
    {
        IDataStoreItem Item { get; }

        RelayCommand SaveCommand { get; }

        RelayCommand ReloadCommand { get; }

        RelayCommand DeleteCommand { get; }

        RelayCommand CloseCommand { get; }

        RelayCommand ViewMetaDataCommand { get; }
        
    }
}
