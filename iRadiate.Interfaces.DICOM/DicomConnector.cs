using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


using iRadiate.DataModel.NucMed;

using Dicom;
using DicomClient = Dicom.Network.Client.DicomClient;
using Dicom.Log;
using Dicom.Network;
using Dicom.Imaging;


namespace iRadiate.Interfaces.DICOM
{
    /// <summary>
    /// A component that can connect to DICOM AEs
    /// </summary>
    public class DicomConnector
    {
        #region privateFields
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();        
        private List<StoredDicomServer> _dicomServers;
        //private DicomClient client;      
        private static List<DicomFile> ReceivedFiles;
        private InterfacePreferences _preferences;
        private IDicomServer server;
        #endregion

        #region staticMethods
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        #endregion

        #region constructors
        public DicomConnector(List<StoredDicomServer> servers)
        {
            logger.Trace("Instantiating Dicom connector..");
            DicomServers = servers;
            DicomTranslator.connector = this;
            _preferences =  DicomModule.GetPreferences();
            server = DicomServer.Create<CStoreSCP>(_preferences.Port);            
            _preferences.HostName = Dns.GetHostName();
            _preferences.IPAddress = GetLocalIPAddress();
            logger.Info("DicomServer AETITLE: {0}; IPAddress: {1}; Port {2}; TimeoutDelay: {3}; HostName: {4}", _preferences.AETitle, _preferences.IPAddress, _preferences.Port, _preferences.TimeoutDelay, _preferences.HostName);
            DicomModule.SavePreferences(_preferences);           
            logger.Info("Server is listening: " + server.IsListening);
            ReceivedFiles = new List<DicomFile>();
            logger.Trace("Instantiating Dicom connector... done.");
        }
        public DicomConnector():this(DicomModule.GetDicomServers())
        {
           
            
            
        }
        #endregion

        #region publicProperties
        /// <summary>
        /// Gets or sets the local AE Title
        /// </summary>
        public string LocalAETitle
        {
            get { return _preferences.AETitle; }
            
        }

        /// <summary>
        /// Gets or sets the local port
        /// </summary>
        public int LocalPort
        {
            get { return _preferences.Port; }
            
        }

        /// <summary>
        /// Gets or sets the list of StoredDicomServers that this class knows about
        /// </summary>
        /// <remarks>
        /// This needs to be stored somewhere, maybe in the settings file
        /// </remarks>
        public List<StoredDicomServer> DicomServers
        {
            get
            {
                if (_dicomServers == null)
                    _dicomServers = new List<StoredDicomServer>();
                return _dicomServers;
            }
            set
            {

                _dicomServers = value;
            }
        }
        #endregion

        #region publicMethods

        public async Task PingServers()
        {
            foreach (StoredDicomServer s in DicomServers.Where(x => x.Enabled))
            {
                DicomClient client = new Dicom.Network.Client.DicomClient(s.IPAddress, s.Port, false, _preferences.HostName, s.AETitle);
                client.AssociationAccepted += Client_AssociationAccepted;
                client.AssociationRejected += Client_AssociationRejected;
                client.NegotiateAsyncOps();
                Task<bool> serverOnline = PingDicomServer(s, client);
                s.Online = await serverOnline;

            }
        }
       
        public List<DicomLink> GetPatientImageByStudy(DateTime studyDate)
        {
            //DateTime start = DateTime.Now;
            //logger.Trace("GetPatientImagesByStudy(" + studyDate.ToShortDateString() + ") commenced at " + start.ToString());
            /////Get all the studies from that date
            //List<TempDicomStudy> Studies = await GetDicomStudies(studyDate);
            //List<DicomLink> Links = new List<DicomLink>();
            //int NumSeries = 0;
            //int NumSeriesTranslated = 0;
            //int NumStudies = 0;
            //int newLinks = 0;
            /////Get all the series which belong to the above studies
            //foreach(TempDicomStudy d in Studies)
            //{
            //    var series = GetDicomSeries(d);
            //    NumSeries = NumSeries + series.Count;
            //    NumStudies++;
            //}

            /////For each series, get the translator to return a patient imag
            ////var server = DicomServer.Create<CStoreSCP>(104);
            //foreach (TempDicomStudy d in Studies)
            //{

            //    logger.Trace(d.DicomSeries.Count + " series within study");
            //    foreach (TempDicomSeries s in d.DicomSeries)
            //    {
            //        logger.Trace("Getting link for " + s.SeriesInstanceUID);
            //        DicomLink link = DicomLink.GetDicomLink(s);
            //        if (link == null)
            //        {
            //            logger.Trace("Series " + s.SeriesInstanceUID + " has not yet been linked");
            //            link = DicomLink.CreateDicomLink(s);

            //            if (link == null)
            //                continue;
            //            DicomMassage.GetScanTaskForImage(link.PatientImage,30);
            //            newLinks++;
            //        }
            //        else
            //            logger.Trace("Series alread Linked to PatientImage " + link.PatientImage.ID);

            //        Links.Add(link);
            //        NumSeriesTranslated++;
            //    }

            //}


            //logger.Info(NumSeries + " found in " + NumStudies + " studies, " + newLinks + " links added " + (NumSeriesTranslated - newLinks) + " already linked");
            //DateTime end = DateTime.Now;
            //int minutes = Convert.ToInt16(Math.Floor((end - start).TotalMinutes));
            //int seconds = Convert.ToInt16(Math.Floor((end - start).TotalSeconds - (minutes * 60)));
            //logger.Info("GetPatientImagesByStudy(" + studyDate.ToShortDateString() + ") finished at " + end.ToString() + " running time " + minutes.ToString() + " minutes and " + seconds + " seconds");
            //return Links;

            return null;
        }

        public async Task<List<TempDicomStudy>> GetDicomStudies(DateTime studyDate)
        {
            logger.Trace("GetDicomStudies(" + studyDate + ")");
            var Studies = new List<TempDicomStudy>();
            var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Study);

            #region studyDataset
            request.Dataset.AddOrUpdate(DicomTag.PatientName, "");
            request.Dataset.AddOrUpdate(DicomTag.PatientID, "");
            request.Dataset.AddOrUpdate(DicomTag.StudyDate, studyDate.ToString("yyyyMMdd"));
            request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, "");
            request.Dataset.AddOrUpdate(DicomTag.AccessionNumber, "");
            request.Dataset.AddOrUpdate(DicomTag.StudyDescription, "");
            request.Dataset.AddOrUpdate(DicomTag.ModalitiesInStudy, "");
            #endregion

            request.OnResponseReceived += (re, response) =>
            {
                logger.Trace(" C-Find response = " + response);
                try
                {
                    if ((response as DicomCFindResponse).HasDataset)
                    {
                        if (Studies.Where(x => x.StudyInstanceUID == (response as DicomCFindResponse).Dataset.GetString(DicomTag.StudyInstanceUID)).Any() == false)
                        {
                            Studies.Add(new TempDicomStudy((response as DicomCFindResponse).Dataset));
                            logger.Trace("Picked up Study Instance UID " + (response as DicomCFindResponse).Dataset.GetString(DicomTag.StudyInstanceUID));
                        }
                        else
                            logger.Trace("Already picked up Study Instance UID " + (response as DicomCFindResponse).Dataset.GetString(DicomTag.StudyInstanceUID));

                    }

                }
                catch (Exception ex)
                {
                    logger.Warn(ex, "Exception in getting studyUID");
                }
                
            };

            foreach (StoredDicomServer s in DicomServers.Where(x => x.Online))
            {
                logger.Trace("Running study level query on " + s.AETitle);
                DicomClient client = new DicomClient(s.IPAddress, s.Port, false, _preferences.AETitle, s.AETitle);
                client.AssociationRejected += Client_AssociationRejected;
                client.AssociationAccepted += Client_AssociationAccepted;
                client.NegotiateAsyncOps();
                await client.AddRequestAsync(request);
                await client.SendAsync();
                //client.Send(s.IPAddress, s.Port, false, LocalAETitle, s.AETitle, 5000);
            }
            return Studies;
        }

        public async Task<List<TempDicomSeries>> GetDicomSeries(TempDicomStudy d)
        {
            var result = new List<TempDicomSeries>();
            var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Series);
            #region seriesDataset
            request.Dataset.AddOrUpdate(DicomTag.Modality, "");
            request.Dataset.AddOrUpdate(DicomTag.SeriesNumber, "");
            request.Dataset.AddOrUpdate(DicomTag.SeriesInstanceUID, "");
            request.Dataset.AddOrUpdate(DicomTag.SeriesDate, "");
            request.Dataset.AddOrUpdate(DicomTag.SeriesTime, "");
            request.Dataset.AddOrUpdate(DicomTag.SeriesDescription, "");
            request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, d.StudyInstanceUID);
            #endregion

            request.OnResponseReceived += (re, response) =>
            {
                if ((response as DicomCFindResponse).Status == DicomStatus.Success);
                    logger.Trace(" C-Find response = " + response);
                try
                {
                    if ((response as DicomCFindResponse).HasDataset)
                    {
                        TempDicomSeries s = new TempDicomSeries((response as DicomCFindResponse).Dataset);
                        if (result.Where(x => x.SeriesInstanceUID == s.SeriesInstanceUID).Any() == false)
                        {
                            s.TempDicomStudy = d;
                            d.DicomSeries.Add(s);
                            result.Add(s);
                            //s.Debug();
                            logger.Trace("Picked up Series Instance UID " + (response as DicomCFindResponse).Dataset.GetString(DicomTag.SeriesInstanceUID));
                        }

                    }

                }
                catch (Exception ex)
                {
                    logger.Warn(ex, "Exception in getting TempDicomSeries");
                }
                
            };

            foreach (StoredDicomServer s in DicomServers.Where(x => x.Online))
            {
                logger.Trace("Running series query on " + s.AETitle);
                DicomClient client = new DicomClient(s.IPAddress, s.Port, false, _preferences.AETitle, s.AETitle);
                client.AssociationRejected += Client_AssociationRejected;
                client.AssociationAccepted += Client_AssociationAccepted;
                client.NegotiateAsyncOps();
                await client.AddRequestAsync(request);
                await client.SendAsync();
            }
            return result;
        }

        public async Task<List<DicomFile>> GetDicomFiles(string studyInstanceUID,string seriesInstanceUID)
        {
           
            ReceivedFiles.Clear();
            DicomCMoveRequest moveRequest = new DicomCMoveRequest(_preferences.AETitle, studyInstanceUID, seriesInstanceUID);
            moveRequest.OnResponseReceived += (DicomCMoveRequest requ, DicomCMoveResponse response) =>
            {
                if (response.Status.State == DicomState.Pending)
                {
                    //logger.Trace("PresentationContext: " + response.PresentationContext.AcceptedTransferSyntax.ToString());
                }
                else if (response.Status.State == DicomState.Success)
                {
                    logger.Trace("Sending successfully finished");
                }
                else if (response.Status.State == DicomState.Failure)
                {
                    logger.Error("Error sending datasets: " + response.Status.Description);
                }
            };
            logger.Debug("Move Request; AE Title: " + _preferences.AETitle + "; Level: " + moveRequest.Level + "; SOP Class UID: " + moveRequest.SOPClassUID);
            foreach (StoredDicomServer s in DicomServers)
            {
                logger.Trace("GetDicomFiles(" + studyInstanceUID + "," + seriesInstanceUID + ") on " + s.AETitle);
               

                var pcs = DicomPresentationContext.GetScpRolePresentationContextsFromStorageUids(
                    DicomStorageCategory.Image,
                    DicomTransferSyntax.ExplicitVRLittleEndian,
                    DicomTransferSyntax.ImplicitVRLittleEndian);
                DicomClient client = new DicomClient(s.IPAddress, s.Port, false, _preferences.AETitle, s.AETitle);
                client.AdditionalPresentationContexts.AddRange(pcs);
                client.AssociationRejected += Client_AssociationRejected;
                client.AssociationAccepted += Client_AssociationAccepted;
                client.NegotiateAsyncOps();
                await client.AddRequestAsync(moveRequest);
                await client.SendAsync();
                

            }

            return ReceivedFiles;
        }

        public async Task<bool> PingDicomServer(StoredDicomServer s, DicomClient client)
        {
            bool result = false;
            logger.Trace("Sending ECHO to " + s.AETitle);
            DicomCEchoRequest r = new DicomCEchoRequest();
            r.OnResponseReceived += (re, response) =>
            {
                logger.Trace(s.AETitle + " C-Echo response = " + response);
                if (((DicomCEchoResponse)response).Status == DicomStatus.Success)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            };
            await client.AddRequestAsync(r);
            try
            {
                //client.Send(s.IPAddress, s.Port, false, LocalAETitle, s.AETitle, 5000);
                Task sendEcho = client.SendAsync();
                await sendEcho;
            }
            catch (Exception ex)
            {
                logger.Error("Exception in echo to " + s.AETitle + " " + ex.Message);
                result = false;
            }

            return result;
        }

        public static string GetStringFromSequence(DicomSequence seq, int recursionDepth)
        {
            string result = "";
            string offset = "";
            for(int a = 0; a < recursionDepth; a++)
            {
                offset = offset + ">";
            }
            foreach(var i in seq.Items)
            {
                foreach(var d in i)
                {
                    if (d.ValueRepresentation != DicomVR.SQ)
                    {
                        string output;
                        if (i.TryGetString(d.Tag, out output))
                        {
                            result = result + offset + d.ToString() + " - " + output + Environment.NewLine;
                        }
                        else
                        {
                            result = result + offset + d.ToString() + " - " + Environment.NewLine;
                        }
                    }
                    else
                    {
                        result = result + offset + "------ begin subsequence -------" + Environment.NewLine + offset + GetStringFromSequence(d as DicomSequence, recursionDepth + 1) + offset + "------ end subsequence -------" + Environment.NewLine;
                    }
                    
                    
                }
            }
            //seq.Items.Each(x => Console.WriteLine(x.ToString()));
            return result;
        }
      
        #endregion

        #region privateMethods
        private void Client_AssociationAccepted(object sender, Dicom.Network.Client.EventArguments.AssociationAcceptedEventArgs e)
        {
            logger.Trace("Association Accepted. CalledAE = " + e.Association.CalledAE + "; CallingAE = " + e.Association.CallingAE + "; Host = " + e.Association.RemoteHost + "; Port = " + e.Association.RemotePort);

        }
        
        private void Client_AssociationRejected(object sender, Dicom.Network.Client.EventArguments.AssociationRejectedEventArgs e)
        {
            logger.Trace("Association Rejected: " + e.Reason);
        }

        #endregion        

        #region publicClassses
        public class CStoreSCP : DicomService, IDicomServiceProvider, IDicomCStoreProvider, IDicomCEchoProvider
        {
            private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            private static readonly DicomTransferSyntax[] AcceptedTransferSyntaxes = new DicomTransferSyntax[]
            {
               DicomTransferSyntax.ExplicitVRLittleEndian,
               DicomTransferSyntax.ExplicitVRBigEndian,
               DicomTransferSyntax.ImplicitVRLittleEndian
            };

            private static readonly DicomTransferSyntax[] AcceptedImageTransferSyntaxes = new DicomTransferSyntax[]
            {
               // Lossless
               DicomTransferSyntax.JPEGLSLossless,
               DicomTransferSyntax.JPEG2000Lossless,
               DicomTransferSyntax.JPEGProcess14SV1,
               DicomTransferSyntax.JPEGProcess14,
               DicomTransferSyntax.RLELossless,
               // Lossy
               DicomTransferSyntax.JPEGLSNearLossless,
               DicomTransferSyntax.JPEG2000Lossy,
               DicomTransferSyntax.JPEGProcess1,
               DicomTransferSyntax.JPEGProcess2_4,
               // Uncompressed
               DicomTransferSyntax.ExplicitVRLittleEndian,
               DicomTransferSyntax.ExplicitVRBigEndian,
               DicomTransferSyntax.ImplicitVRLittleEndian,
               DicomTransferSyntax.ImplicitVRBigEndian
            };

            public CStoreSCP(INetworkStream stream, Encoding fallbackEncoding, Logger log)
                : base(stream, fallbackEncoding, log)
            {
            }

            public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
            {
                if (association.CalledAE == "STORESCP")
                {
                    return SendAssociationRejectAsync(
                        DicomRejectResult.Permanent,
                        DicomRejectSource.ServiceUser,
                        DicomRejectReason.CalledAENotRecognized);
                }

                foreach (var pc in association.PresentationContexts)
                {
                    if (pc.AbstractSyntax == DicomUID.Verification) pc.AcceptTransferSyntaxes(AcceptedTransferSyntaxes);
                    else if (pc.AbstractSyntax.StorageCategory != DicomStorageCategory.None) pc.AcceptTransferSyntaxes(AcceptedImageTransferSyntaxes);
                }

                return SendAssociationAcceptAsync(association);
            }

            public Task OnReceiveAssociationReleaseRequestAsync()
            {
                return SendAssociationReleaseResponseAsync();
            }

            public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
            {
            }

            public void OnConnectionClosed(Exception exception)
            {
            }

            public DicomCStoreResponse OnCStoreRequest(DicomCStoreRequest request)
            {
                logger.Trace("CstoreRequeste Reecieved");
                var studyUid = request.Dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
                var instUid = request.SOPInstanceUID.UID;

                ReceivedFiles.Add(request.File);
               
                return new DicomCStoreResponse(request, DicomStatus.Success);
            }

            public void OnCStoreRequestException(string tempFileName, Exception e)
            {
                // let library handle logging and error response
            }

            public DicomCEchoResponse OnCEchoRequest(DicomCEchoRequest request)
            {
                logger.Trace("C-Echo request received");
                return new DicomCEchoResponse(request, DicomStatus.Success);
            }
        }
        #endregion
    }

    public class TempDicomStudy
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public TempDicomStudy(DicomDataset dataset)
        {
            DicomSeries = new List<TempDicomSeries>();
            string tmp = "";
            if (dataset.TryGetString(DicomTag.StudyDate, out tmp))
                StudyDate = tmp;
            else
                StudyDate = "";

            if (dataset.TryGetString(DicomTag.StudyTime, out tmp))
                StudyTime = tmp;
            else
                StudyTime = "";

            if (dataset.TryGetString(DicomTag.AccessionNumber, out tmp))
                AccesionNumber = tmp;
            else
                AccesionNumber = "";

            if (dataset.TryGetString(DicomTag.PatientName, out tmp))
                PatientName = tmp;
            else
                PatientName = "";

            if (dataset.TryGetString(DicomTag.PatientID, out tmp))
                PatientID = tmp;
            else
                PatientID = "";

            if (dataset.TryGetString(DicomTag.StudyInstanceUID, out tmp))
                StudyInstanceUID = tmp;
            else
                StudyInstanceUID = "";

            if (dataset.TryGetString(DicomTag.StudyDescription, out tmp))
                StudyDescription = tmp;
            else
                StudyDescription = "";
        }

        public void Debug()
        {
            logger.Debug("*********TempDicomStudy Debug********");
            logger.Debug("StudyDate: " + StudyDate);
            logger.Debug("StudyTime: " + StudyTime);
            logger.Debug("Accesion Number: " + AccesionNumber);
            logger.Debug("Patient Name: " + PatientName);
            logger.Debug("Patient ID: " + PatientID);
            logger.Debug("StudyInstanceUID: " + StudyInstanceUID);
            logger.Debug("StudyDescription: " + StudyDescription);
        }
        public List<TempDicomSeries> DicomSeries { get; set; }
        public string StudyDate { get; set; }
        public string StudyTime { get; set; }
        public string AccesionNumber { get; set; }
        public string PatientName { get; set; }
        public string PatientID { get; set; }
        public string StudyInstanceUID { get; set; }
        public string StudyDescription { get; set; }
    }

    public class TempDicomSeries
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public TempDicomSeries(DicomDataset dataset)
        {
            string tmp = "";
            if (dataset.TryGetString(DicomTag.Modality, out tmp))
                Modality = tmp;
            else
                Modality = "";

            if (dataset.TryGetString(DicomTag.SeriesNumber, out tmp))
                SeriesNumber = tmp;
            else
                SeriesNumber = "";

            if (dataset.TryGetString(DicomTag.SeriesInstanceUID, out tmp))
                SeriesInstanceUID = tmp;
            else
                SeriesInstanceUID = "";

            if (dataset.TryGetString(DicomTag.SeriesDate, out tmp))
                SeriesDate = tmp;
            else
                SeriesDate = "";

            if (dataset.TryGetString(DicomTag.SeriesTime, out tmp))
                SeriesTime = tmp;
            else
                SeriesTime = "";

            if (dataset.TryGetString(DicomTag.SeriesDescription, out tmp))
                SeriesDescription = tmp;
            else
                SeriesDescription = "";

        }

        public void Debug()
        {
            logger.Debug("*********TempDicomSeries Debug********");
            logger.Debug("Modality: " + Modality);
            logger.Debug("SeriesNumber: " + SeriesNumber);
            logger.Debug("SeriesInstanceUID: " + SeriesInstanceUID);
            logger.Debug("SeriesDate: " + SeriesDate);
            logger.Debug("SeriesTime: " + SeriesTime);
            logger.Debug("SeriesDescription: " + SeriesDescription);

        }
        public string Modality { get; set; }

        public string SeriesNumber { get; set; }

        public string SeriesInstanceUID { get; set; }

        public string SeriesDate { get; set; }

        public string SeriesTime { get; set; }

        public string SeriesDescription { get; set; }

        public TempDicomStudy TempDicomStudy { get; set; }


    }

    public class TempDicomImage
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public TempDicomImage()
        {

        }
        public string ImageNumber { get; set; }
        public string SOPInstanceUID { get; set; }
        public string ImageID { get; set; }
        public string Rows { get; set; }
        public string Columns { get; set; }
        public string NumberOfFrames { get; set; }
    }
}



