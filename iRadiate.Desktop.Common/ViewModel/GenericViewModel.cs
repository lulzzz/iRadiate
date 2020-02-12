using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace iRadiate.Desktop.Common.ViewModel
{
    public abstract class GenericViewModel : ViewModelBase
    {

        public event EventHandler ViewModelClosing;
        protected virtual void OnViewModelClosing()
        {
            EventHandler handler = ViewModelClosing;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

       

    }

   
}
