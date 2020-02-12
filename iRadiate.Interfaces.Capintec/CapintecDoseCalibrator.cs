using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.Radiopharmacy;
using iRadiate.Common;

namespace iRadiate.Interfaces.Capintec
{
    public enum CapintecUnit { OverRange,uCi,mCi,Ci,kBq, MBq, GBq, mV,V};

    public enum CapintecErrorCode {
        [Description("Receive error")]
        ReceiveError,
        [Description("Incorrect checksum")]
        IncorrectChecksum,
        [Description("No such command")]
        NoSuchCommand,
        [Description("Length does not agree with command")]
        LengthDoesNotAgreeWithCommand,
        [Description("Chamber does not exist")]
        ChamberDoesNotExist };

    [Export(typeof(IDoseCalibrator))]
    public class CapintecDoseCalibrator :  IDoseCalibrator
    {
        private List<IsotopeChannelNumber> _isotopeChannelNumbers;
        private SerialPort _port;
        private bool _portOpen;

        #region constructor
        public CapintecDoseCalibrator()
        {
           
            LoadIsotopeChannelNumbers();
            //Create a port
            if (OpenPort())
            {
            }
            
        }
        #endregion

        #region publicProperties
        public SerialPort Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public bool PortOpen
        {
            get { return _portOpen; }
            set { _portOpen = value; }
        }
        #endregion

        #region publicMethods
        public bool ChangeIsotope(Isotope isotope)
        {
            throw new NotImplementedException();
        }

        public Isotope GetCurrentIsotope()
        {
            if (Port == null)
                return null;
            string var  = Port.ReadLine();
            return null;
        }
        
        public double ReadActivity(Isotope isotope)
        {
            if(isotope == null)
            {
                return ReadActivity();
                
            }
            if (Port.IsOpen)
            {
                Port.Write("$CR1H#");
                string output = Port.ReadLine();
                string checkSum = output.Substring(output.Length - 1);
                CapintecUnit unit = (CapintecUnit)Convert.ToInt16(output.Substring(output.Length -2,1));
                string channel = output.Substring(5, 8);
                channel = channel.Trim();
                if(IsotopeChannelNumbers.Where(x=>x.IsotopeString == channel).Any() == false)
                {
                    throw new Exception("Incorrect dose calibrator channel");
                }
                else
                {
                    if(IsotopeChannelNumbers.Where(x => x.IsotopeString == channel).First().Isotope.ID != isotope.ID)
                        throw new Exception("Incorrect dose calibrator channel");
                }
                double reading = Convert.ToDouble(output.Substring(13, 6));
                if (unit == CapintecUnit.GBq)
                    reading = reading * 1000;
                return reading;
            }
            else
            {
                throw new Exception("Port is not open");
            }
            
        }

        public double ReadActivity()
        {
            if (Port.IsOpen)
            {
                Port.Write("$CR1H#");
                string output = Port.ReadLine();
                string checkSum = output.Substring(output.Length - 1);
                CapintecUnit unit = (CapintecUnit)Convert.ToInt16(output.Substring(output.Length - 2, 1));

                double reading = Convert.ToDouble(output.Substring(13, 6));
                if (unit == CapintecUnit.GBq)
                    reading = reading * 1000;
                return reading;
            }
            else
            {
                throw new Exception("Port is not open");
            }

        }

        public string ReadLine()
        {
            return Port.ReadLine();
        }

        public List<IsotopeChannelNumber> IsotopeChannelNumbers
        {
            get
            {
                if (_isotopeChannelNumbers == null)
                    _isotopeChannelNumbers = new List<IsotopeChannelNumber>();

                return _isotopeChannelNumbers;
            }
            set
            {
                _isotopeChannelNumbers = value;
                
            }
        }

        public bool IsConnected
        {
            get
            {
                return PortOpen;
            }
        }

        public void SaveIsotopeChamberNumbers()
        {
            using(MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, IsotopeChannelNumbers);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                System.Diagnostics.Debug.WriteLine("buffer = " + Convert.ToBase64String(buffer));
                Properties.Settings.Default.IsotopeChamberNumbers = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }
        }

        public void LoadIsotopeChannelNumbers()
        {
            System.Diagnostics.Debug.WriteLine("Settings variable = " + Properties.Settings.Default.IsotopeChamberNumbers);
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.IsotopeChamberNumbers)))
            {
                if (ms.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Memory stream has zero lenth");
                    return;
                }
                    
                BinaryFormatter bf = new BinaryFormatter();
                IsotopeChannelNumbers = (List<IsotopeChannelNumber>)bf.Deserialize(ms);
                foreach(var l in IsotopeChannelNumbers)
                {
                    l.AvailableIsotopes = Platform.Retriever.RetrieveItems(typeof(Isotope), new List<RetrievalCriteria>());
                }
                
            }
        }

        public void IssueCommand(string command)
        {
            Port.Write(command);
        }

        public string ReadOutput()
        {
            try
            {
                return Port.ReadLine();
            }
            catch(TimeoutException t)
            {
                return "Operation timed out";
            }
            catch(Exception ex)
            {
                return "Readout failed";
            }
            
        }

        public bool OpenPort()
        {
            if (Port != null)
            {
                Port.Close();
            }
            Port = new SerialPort(Properties.Settings.Default.PortName, Properties.Settings.Default.BaudRate, Properties.Settings.Default.Parity, Properties.Settings.Default.DataBits, Properties.Settings.Default.StopBits);
            Port.ReadTimeout = Properties.Settings.Default.ReadTimeout;
            Port.WriteTimeout = Properties.Settings.Default.WriteTimeout;
            Port.NewLine = "#";
            Port.DataReceived += Port_DataReceived;
            try
            {
                Port.Open();
                if (Port.IsOpen)
                {
                    PortOpen = true;
                    return true;

                }
                else
                {
                    PortOpen = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("Unable to open serial port");
                PortOpen = false;
                return false;

            }


        }
        #endregion

        #region privateMethods

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //???
        }
        #endregion

        #region staticMethods
        public static string ConvertHex(String hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;

                }

                return ascii;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return string.Empty;
        }
        #endregion
    }

    [Serializable]
    public class IsotopeChannelNumber
    {
        [NonSerialized]
        private List<IDataStoreItem> _availableIsotopes;

        public IsotopeChannelNumber()
        {
            _availableIsotopes = Platform.Retriever.RetrieveItems(typeof(Isotope), new List<RetrievalCriteria>());
        }

        public int IDNumber { get { return IsotopeID; } }

        public int IsotopeID;

        public string IsotopeString { get; set; }
        public Isotope Isotope
        {
            get
            {
                if (_availableIsotopes != null)
                {
                    if(_availableIsotopes.Where(x => x.ID == IsotopeID).Any())
                    {
                        return _availableIsotopes.Where(x => x.ID == IsotopeID).First() as Isotope;
                    }
                }
                    

                return null;
            }
            set
            {
                IsotopeID = value.ID;
            }
        }

       

      
       
        public List<IDataStoreItem> AvailableIsotopes
        {
            get { return _availableIsotopes; }
            set { _availableIsotopes = value; }
        }
        
      
        
    }

    public class PortNotOpenException : Exception
    {
        public PortNotOpenException() : base()
        {

        }
        public PortNotOpenException(string message):base(message)
        {

        }

        public PortNotOpenException(string message, Exception inner):base(message,inner)
        {
            
        }
    }

    public class InvalidCalibratorUnitException :Exception
    {
        public InvalidCalibratorUnitException() : base()
        {

        }

        public InvalidCalibratorUnitException(string message) : base(message)
        {

        }

        public InvalidCalibratorUnitException(string message, Exception inner) : base(message, inner)
        {

        }
    }

    
}
