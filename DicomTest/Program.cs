using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.NucMed;
using iRadiate.Interfaces.DICOM;

namespace DicomTest
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            DateTime starttime = DateTime.Now;
            logger.Info("DicomTest start " + DateTime.Now);
            DicomConnector dc = new DicomConnector();
            var links = dc.GetPatientImageByStudy(DateTime.Today.AddDays(0));

            logger.Info("links has returned");
            if (links.Any())
            {
                logger.Info(links.Count + " links found");

                foreach (DicomLink link in links)
                {
                    if (link == null)
                    {
                        logger.Warn("A null link was returned by the DicomConnector");
                        continue;
                    }

                    if (link.PatientImage == null)
                    {
                        logger.Warn("A link was returned by the dataconnector with a null patientIage");
                        continue;
                    }
                    ScanTask t = link.PatientImage.ScanTask;
                    if (t == null)
                    {
                        logger.Error("Null ScanTaskFound for PatientImage " + link.PatientImage.ID);
                        continue;
                    }
                    logger.Info("PatientImage.SeriesDescription = " + link.PatientImage.SeriesDescription);
                    //link.PatientImage.Debug();
                    //DicomMassage.MassageScanTask(link.PatientImage.ScanTask);
                }
            }
            else
            {
                logger.Info("No links returned");
            }


            RetrievalCriteria rc1 = new RetrievalCriteria("ScheduledArrivalTime", CriteraType.GreaterThan, DateTime.Today);
            RetrievalCriteria rc2 = new RetrievalCriteria("ScheduledArrivalTime", CriteraType.LessThan, DateTime.Today.AddDays(1));
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc1);
            rcList.Add(rc2);
            var appointments = Platform.Retriever.RetrieveItems(typeof(Appointment), rcList);
            foreach (Appointment a in appointments)
            {
                foreach(ScanTask st in a.Tasks.Where(x=>x is ScanTask))
                {
                    DicomMassage.MassageScanTask(st);
                }
            }



            logger.Info("DicomTest finish " + DateTime.Now + " - total " + (DateTime.Now-starttime).TotalMinutes + " minutes");
            //Console.ReadLine();
        }
    }
}
