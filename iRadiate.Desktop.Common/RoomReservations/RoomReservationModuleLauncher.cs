using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

namespace iRadiate.Desktop.Common.RoomReservations
{
    
    public class RoomReservationModuleLauncher :ModuleLauncher
    {
        public override string Name
        {
            get
            {

                return "Reservations";
            }
        }

        public string IconSource
        {
            get { return "/iRadiate.Desktop.Common;component/Images/ReservationsIconWhite.png"; }
        }

        public override void Launch()
        {
            //logger.Trace("SettingsLauncher.Launch()");
            DesktopApplication.MainViewModel.LaunchModule(typeof(RoomReservationsModule));

        }

        public override int Order
        {
            get
            {
                return 6;
            }
        }
    }
}
