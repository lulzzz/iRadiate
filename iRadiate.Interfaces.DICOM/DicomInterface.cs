using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using Dicom;
using Dicom.Network;
using DicomClient = Dicom.Network.Client.DicomClient;
using Dicom.Imaging;
using NLog;
using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Interfaces.DICOM
{
    /// <summary>
    /// An interface to a DICOM AE
    /// </summary>
    /// <remarks>
    /// As far as other components are concered, the interface performs some high level tasks
    /// and returns the result
    /// </remarks>
    public class DicomInterface 
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public DicomInterface()
        {

        }
        public string Name
        {
            get
            {
                return "Dicom";
            }
        }

        public void TodaysScans()
        {
            logger.Info("Retrieve today's images");
            DicomConnector dc = new DicomConnector();
            var links = dc.GetPatientImageByStudy(DateTime.Today.AddDays(0));

            logger.Trace("links has returned");
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
                    logger.Trace("PatientImage.SeriesDescription = " + link.PatientImage.SeriesDescription);
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
                foreach (ScanTask st in a.Tasks.Where(x => x is ScanTask))
                {
                    DicomMassage.MassageScanTask(st);
                }
            }
        }

        public void TodaysScreencaps()
        {

        }

        public async Task Test()
        {
            List<StructuredReport> StructuredReports = new List<StructuredReport>();
            Console.WriteLine("Test!");
            DicomConnector dc = new DicomConnector();
            await dc.PingServers();
            
            foreach(var server in dc.DicomServers)
            {
                Console.WriteLine("Server: " + server.AETitle +" online = " + server.Online);
            }
            //TestWorklist(dc);
           List<TempDicomStudy> Studies = await dc.GetDicomStudies(DateTime.Today);

            foreach (var s in Studies)
            {
                Console.WriteLine("Study: " + s.PatientName + " - " + s.StudyDescription);

                var series = await dc.GetDicomSeries(s);
                foreach (var sr in s.DicomSeries)
                {
                    Console.WriteLine("-----> Series: " + sr.SeriesDescription);
                    if (sr.Modality == "CT")
                    {
                        //Console.WriteLine("CT Series: " + s.PatientName + " - " + s.StudyDescription + " - " + sr.SeriesDescription);
                        if (sr.SeriesDescription.ToLower().Contains("report"))
                        {
                            continue;
                        }
                        else
                        {
                            //List<DicomFile> files = await dc.GetDicomFiles(sr.TempDicomStudy.StudyInstanceUID, sr.SeriesInstanceUID);
                            //Console.WriteLine("Series contains " + files.Count + " files ");
                            //foreach (var f in files.OrderBy(x => Convert.ToDouble(x.Dataset.GetString(DicomTag.SliceLocation))))
                            //{
                            //    //Console.WriteLine("slice location: " + f.Dataset.GetString(DicomTag.SliceLocation) + " - current: " + f.Dataset.GetString(DicomTag.XRayTubeCurrent) + " mA");
                            //}
                        }
                    }
                    else if (sr.Modality == "SR")
                    {
                        //Console.WriteLine("Structured Report");
                        List<DicomFile> files = await dc.GetDicomFiles(sr.TempDicomStudy.StudyInstanceUID, sr.SeriesInstanceUID);
                       if(files.Any())
                        {
                            DicomFile file = files.First();
                            StructuredReport sRep = new StructuredReport(file.Dataset);
                            StructuredReports.Add(sRep);
                           

                        }

                    }
                }


            }
            foreach(StructuredReport sr in StructuredReports)
            {
                sr.Debug();
            }
        }

        public async void TestWorklist(DicomConnector dc)
        {
            Console.WriteLine("Test Worklist!");
            //DicomConnector dc = new DicomConnector();
            foreach (var server in dc.DicomServers.Where(x => x.Worklist))
            {
                Console.WriteLine("Server: " + server.AETitle + " address = " + server.IPAddress + " port = " + server.Port);

                var worklistItems = await GetAllItemsFromWorklistAsync(server.IPAddress, server.Port, server.AETitle, "DESKTOP-UR5U52N");
                Console.WriteLine($"received {worklistItems.Count} worklist items.");
                Console.ReadLine();
            }
        }

        public async Task TestStructuredReport()
        {
            var f = Dicom.DicomFile.Open("StructuredReport.dcm");
            StructuredReport sr = new StructuredReport(f.Dataset);
            sr.Debug();
            XmlSerializer serializer = new XmlSerializer(typeof(StructuredReport));
            TextWriter writer = new StreamWriter("structuredReport.xml");
            serializer.Serialize(writer, sr);
            writer.Close();
        }

        private static async Task<List<DicomDataset>> GetAllItemsFromWorklistAsync(string serverIP, int serverPort, string serverAET, string clientAET)
        {
            var worklistItems = new List<DicomDataset>();
            var cfind = DicomCFindRequest.CreateWorklistQuery(); // no filter, so query all awailable entries
            cfind.OnResponseReceived = (DicomCFindRequest rq, DicomCFindResponse rp) =>
            {
                if (rp.HasDataset)
                {
                    Console.WriteLine("Study UID: {0}", rp.Dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID));
                    string va;
                    foreach(var i in rp.Dataset)
                    {
                        if(i.ValueRepresentation == DicomVR.SQ)
                        {
                            Console.WriteLine(i.ToString());
                            Console.WriteLine(DicomConnector.GetStringFromSequence(i as DicomSequence,1));
                        }
                        else
                        {
                            string val;
                            if (rp.Dataset.TryGetString(i.Tag, out val))
                            {
                                Console.WriteLine(i.ToString() + " " + val);
                            }
                            else
                            {
                                Console.WriteLine(i.ToString());
                            }
                        }
                       
                    }
                    
                    worklistItems.Add(rp.Dataset);
                }
                else
                {
                    Console.WriteLine(rp.Status.ToString());
                }
            };

            var client = new DicomClient(serverIP, serverPort, false, clientAET, serverAET);
            client.AssociationAccepted += Client_AssociationAccepted;
            client.AssociationRejected += Client_AssociationRejected;
            await client.AddRequestAsync(cfind);
            await client.SendAsync();

            return worklistItems;
        }

        private static void Client_AssociationRejected(object sender, Dicom.Network.Client.EventArguments.AssociationRejectedEventArgs e)
        {
            Console.WriteLine("Association rejected: " + e.Reason);
        }

        private static void Client_AssociationAccepted(object sender, Dicom.Network.Client.EventArguments.AssociationAcceptedEventArgs e)
        {
            //throw new NotImplementedException();
        }

       
    }
}
