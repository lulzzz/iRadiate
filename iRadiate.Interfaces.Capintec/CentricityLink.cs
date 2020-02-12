using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Radiopharmacy;

namespace iRadiate.Interfaces.Capintec
{
    public class CapintecLink
    {
        

        public CapintecLink()
        {
            Console.WriteLine("CapintecLink created...");

            
            SerialPort port = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
        

            port.Open();
            if (port.IsOpen)
                Console.WriteLine("POrt open");
            else
                Console.WriteLine("port closed");
            port.ReadTimeout = 1000;
            port.DataReceived += Port_DataReceived;


            port.Write("@");
           
           
            port.NewLine = "#";
            Console.WriteLine("Port has been written to with @");
            Console.WriteLine("readline = " + port.ReadLine());
            
            port.Write("$CR1H#");
            Console.WriteLine("POrt has been written to with $CR1##");
            Console.WriteLine("read line = " + port.ReadLine());
            port.Write("@");
            Console.WriteLine("Port has been written to with @");
            Console.WriteLine("read line = " + port.ReadLine());
            port.Write("$DI13s#");
            Console.WriteLine("Port has been written to with $DI13s#");
            Console.WriteLine("read line = " + port.ReadLine());
            port.Close();
          
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Data recieved " + e.EventType.ToString());
            
            
        }



    }
}
