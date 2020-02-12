using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;
using iRadiate.DataModel;
using iRadiate.DataModel.NucMed;
using iRadiate.Interfaces.CentricityInterface;
using iRadiate.Interfaces.DICOM;
using iRadiate.Common;

using NLog;



namespace iRadiate.Interfaces.Test
{

   
    class Program
    {
      
        
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            DicomInterface di = new DicomInterface();
            Task result = di.TestStructuredReport();
            result.Wait();
            Console.ReadLine();
        }

            static void MainX(string[] args)
        {

            DateTime startTime = DateTime.Now;
            Console.WriteLine("Start Time " + startTime);
            logger.Info("******* Interface launcher started: " + startTime + "; Arguments: " + "[{0}]", string.Join(", ", args) + " *******");
            if (args.Length == 0)
            {
                logger.Info("No argument specified");
                Console.WriteLine("Launcher end " + DateTime.Now);
                logger.Info("Launcher end " + DateTime.Now);
                Console.WriteLine((DateTime.Now - startTime).Minutes + " Minutes " + (DateTime.Now - startTime).Seconds);
                //Console.ReadLine();
#if DEBUG
                Console.ReadLine();
#endif
                return;
            }


            switch (args[0])
            {
                case "Centricity":
                    //do code
                    CentricityConnector cc;
                    try
                    {
                        cc = new CentricityConnector();
                    }
                    catch (Exception ex)
                    {

                        logger.Error(ex, "Caugt exception on the constructor " + ex.Message);
                        break;
                    }
                    logger.Info("Launching Centricity Connector with command: " + args[1]);
                    cc.ExecuteCommand(args[1]);
                    logger.Info("Centricity Connector completed: " + args[1]);

                    break;
                case "Dicom":
                    //do code
                    DicomInterface di = new DicomInterface();
                    if (args[1] == "TodaysScans")
                        di.TodaysScans();
                    else if (args[1] == "TodaysScreencaps")
                        di.TodaysScreencaps();
                    else if (args[1] == "Test")
                    {
                        Task t = Task.Run( () => di.Test());
                        t.Wait();
                    }
                       
                    else if (args[1] == "TestWorklist")
                        di.TestWorklist(null);
                    break;
                default:
                    Console.WriteLine("Invalid Argument");
                    logger.Info("Invalid Argument");
                    break;
            }


            //var result = cc.GetAppointments(DateTime.Today, DateTime.Today.AddDays(10));


            Console.WriteLine("Launcher end " + DateTime.Now);
            logger.Info("******* Launcher end " + DateTime.Now + " *******");
            Console.WriteLine((DateTime.Now - startTime).Minutes + " Minutes " + (DateTime.Now - startTime).Seconds);
#if DEBUG
            Console.ReadLine();
#endif 




        }

        static async Task MainAsync(string[] args)
        {
           
        }





    }
}
