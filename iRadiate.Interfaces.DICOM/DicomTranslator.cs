using System;
using System.Collections.Generic;

using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Dicom;
using Dicom.Imaging;
using NLog;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;



namespace iRadiate.Interfaces.DICOM
{
   /// <summary>
   /// Comverts a DICOM series into an iRadiate PatientImage
   /// </summary>
    public class DicomTranslator
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static IDataRetriever retriever = Platform.Retriever;

        public static DicomConnector connector;
      
        /// <summary>
        /// Translates a DicomDataset into a PatientImage, saves the image in iRadiate, returns null if untranslateable.
        /// </summary>
        /// <param name="dataset">The DicomDataset to be translated</param>
        /// <returns>A PatientImage or null if it was untranslateable.</returns>
        public static PatientImage TranslateDataset(DicomDataset dataset)
        {
            logger.Trace("TranslateDataset(DicomDataset)...");
            //GetPatientPosition(dataset);
            if (dataset == null)
            {
                logger.Error("Attempt to translate a DicomDataset which is null");
                return null;
            }
            string descr;
            if(dataset.TryGetString(DicomTag.SeriesDescription,out descr))
            {
                if(descr.ToLower().Contains("results") || descr.ToLower().Contains("multiframe") || descr.ToLower().Contains("cardiac spect"))
                {
                    logger.Error("Dataset rejected because of series description '" + descr + "', descritpion cannot include 'results' or 'multiframe' or 'cardiac spect'");
                    return null;
                }
            }

            PatientImage img = null;
            if (dataset.GetValue<DicomUID>(DicomTag.SOPClassUID, 0).UID == "1.2.840.10008.5.1.4.1.1.20")
            {

                #region nucMedImage
                logger.Debug("SOPClassUID is NMImage");
                string[] imageType = dataset.GetString(DicomTag.ImageType).Split('\\');
                logger.Trace("ImageTpe = " + dataset.GetString(DicomTag.ImageType));
                switch (imageType[2])
                {
                    case "STATIC":
                        img = new StaticNMImage();
                        int duration;
                        if (dataset.TryGetValue<int>(DicomTag.ActualFrameDuration, 0, out duration))
                            (img as StaticNMImage).ActualFrameDuration = duration;
                        break;
                    case "TOMO":
                   
                        img = new TomoNMImage();
                        ushort numRotations;
                        if (dataset.TryGetValue<ushort>(DicomTag.NumberOfRotations, 0, out numRotations))
                            (img as TomoNMImage).NumberOfRotations = numRotations;
                        DicomSequence rotationSequence;
                        if(dataset.TryGetSequence(DicomTag.RotationInformationSequence, out rotationSequence))
                        {
                            ///Since we only ever have on rotation we can just take the zeroth item
                            ///and run with that.
                            decimal angularStep;
                            if (rotationSequence.Items[0].TryGetValue<decimal>(DicomTag.AngularStep, 0, out angularStep))
                                (img as TomoNMImage).AngularStep = (float)angularStep;

                            string rotationDirection;
                            if (rotationSequence.Items[0].TryGetString(DicomTag.RotationDirection, out rotationDirection))
                            {
                                switch (rotationDirection)
                                {
                                    case "CW":
                                        (img as TomoNMImage).RotationDirection = RotationDirection.CW;
                                        break;
                                    case "CC":
                                        (img as TomoNMImage).RotationDirection = RotationDirection.CC;
                                        break;
                                }
                            }

                            decimal scanArc;
                            if (rotationSequence.Items[0].TryGetValue<decimal>(DicomTag.ScanArc, 0, out scanArc))
                                (img as TomoNMImage).ScanArc = (float)scanArc;

                            ushort numAngles;
                            if (rotationSequence.Items[0].TryGetValue<ushort>(DicomTag.NumberOfFramesInRotation, 0, out numAngles))
                                (img as TomoNMImage).NumberOfFrames = (int)numAngles;

                            int angleTime;
                            if (rotationSequence.Items[0].TryGetValue<int>(DicomTag.ActualFrameDuration, 0, out angleTime))
                                (img as TomoNMImage).ActualFrameDuration = angleTime;

                        }
                        break;
                    case "GATED TOMO":
                        img = new GatedTOMOImage();
                        
                        if (dataset.TryGetValue<ushort>(DicomTag.NumberOfRotations, 0, out numRotations))
                            (img as GatedTOMOImage).NumberOfRotations = numRotations;
                        
                        if (dataset.TryGetSequence(DicomTag.RotationInformationSequence, out rotationSequence))
                        {
                            ///Since we only ever have on rotation we can just take the zeroth item
                            ///and run with that.
                            decimal angularStep;
                            if (rotationSequence.Items[0].TryGetValue<decimal>(DicomTag.AngularStep, 0, out angularStep))
                                (img as GatedTOMOImage).AngularStep = (float)angularStep;

                            string rotationDirection;
                            if (rotationSequence.Items[0].TryGetString(DicomTag.RotationDirection, out rotationDirection))
                            {
                                switch (rotationDirection)
                                {
                                    case "CW":
                                        (img as GatedTOMOImage).RotationDirection = RotationDirection.CW;
                                        break;
                                    case "CC":
                                        (img as GatedTOMOImage).RotationDirection = RotationDirection.CC;
                                        break;
                                }
                            }

                            decimal scanArc;
                            if (rotationSequence.Items[0].TryGetValue<decimal>(DicomTag.ScanArc, 0, out scanArc))
                                (img as GatedTOMOImage).ScanArc = (float)scanArc;

                            int numAngles;
                            if (rotationSequence.Items[0].TryGetValue<int>(DicomTag.NumberOfFramesInRotation, 0, out numAngles))
                                (img as GatedTOMOImage).NumberOfFrames = (int)numAngles;

                            int angleTime;
                            if (rotationSequence.Items[0].TryGetValue<int>(DicomTag.ActualFrameDuration, 0, out angleTime))
                                (img as GatedTOMOImage).ActualFrameDuration = angleTime;



                        }
                        break;
                    case "WHOLE BODY":
                        img = new WholebodyNMImage();        
                        ///Scan duration                
                        if (dataset.TryGetValue<int>(DicomTag.ActualFrameDuration, 0, out duration))
                            (img as WholebodyNMImage).ActualFrameDuration = duration;
                        ///Scan Velocity
                        decimal velocity;
                        if (dataset.TryGetValue<decimal>(DicomTag.ScanVelocity, 0, out velocity))
                            (img as WholebodyNMImage).ScanVelocity = (float)velocity;
                        ///Scan Length
                        int scanLength;
                        if (dataset.TryGetValue<int>(DicomTag.ScanLength, 0, out scanLength))
                            (img as WholebodyNMImage).ScanLength = scanLength;

                        
                        break;
                    case "RECON TOMO":
                        img = new SPECTNMImage();
                        ///Number of slices
                        int numSlices;
                        if (dataset.TryGetValue<int>(DicomTag.NumberOfSlices, 0, out numSlices))
                            (img as SPECTNMImage).NumberOfSlices = numSlices;
                        ///Space between slices
                        decimal spaceBetweenSlices;
                        if (dataset.TryGetValue<decimal>(DicomTag.SpacingBetweenSlices, 0, out spaceBetweenSlices))
                            (img as SPECTNMImage).SpacingBetweenSlices = (float)spaceBetweenSlices;
                        ///Slice thickness
                        decimal sliceThickness;
                        if (dataset.TryGetValue<decimal>(DicomTag.SliceThickness, 0, out sliceThickness))
                            (img as SPECTNMImage).SliceThickness = (float)sliceThickness;
                        break;
                    case "GATED":
                        img = new GatedNMImage();

                        
                        break;
                    case "DYNAMIC":
                        img = new DynamicNMImage();
                        ///Number of Phases
                        int numPhases;
                        if (dataset.TryGetValue<int>(DicomTag.NumberOfPhases, 0, out numPhases))
                            (img as DynamicNMImage).NumberOfPhases = numPhases;

                        #region PhaseInformationSequence
                        DicomSequence phaseSequence;
                        string phaseString = "";
                        if(dataset.TryGetSequence(DicomTag.PhaseInformationSequence,out phaseSequence))
                        {
                            foreach(DicomDataset ds in phaseSequence.Items)
                            {
                                if (phaseString != "")
                                    phaseString = phaseString + ";";

                                int phaseDelay, numFrames, pauseBetweenFrames, frameDuration;
                                if (ds.TryGetValue<int>(DicomTag.PhaseDelay, 0, out phaseDelay))
                                    phaseString = phaseString + phaseDelay.ToString();
                                else
                                    phaseString = phaseString + "0";

                                if (ds.TryGetValue<int>(DicomTag.ActualFrameDuration, 0, out frameDuration))
                                    phaseString = phaseString + "," + frameDuration.ToString();
                                else
                                    phaseString = phaseString + ",0";

                                if (ds.TryGetValue<int>(DicomTag.PauseBetweenFrames, 0, out pauseBetweenFrames))
                                    phaseString = phaseString + "," + pauseBetweenFrames.ToString();
                                else
                                    phaseString = phaseString + ",0";

                                if (ds.TryGetValue<int>(DicomTag.NumberOfFrames, 0, out numFrames))
                                    phaseString = phaseString + "," + numFrames.ToString();
                                else
                                    phaseString = phaseString + ",0";

                            }
                        }

                        (img as DynamicNMImage).PhaseSequence = phaseString;
                        #endregion
                        break;
                    }

                #region gated
                if (imageType[2].Contains("GATED"))
                {
                    GatedNMImage gate = img as GatedNMImage;
                    ///BeatsRejectionFlag
                    string beatsRejectionFlag;
                    if (dataset.TryGetString(DicomTag.BeatRejectionFlag, out beatsRejectionFlag))
                        gate.BeatsRejectionFlag = beatsRejectionFlag;

                    ///SkipBeats
                    int skipBeats;
                    if (dataset.TryGetValue<int>(DicomTag.SkipBeats, 0, out skipBeats))
                        gate.SkipBeats = skipBeats;

                    ///HeartRate
                    int heartRate;
                    if (dataset.TryGetValue<int>(DicomTag.HeartRate, 0, out heartRate))
                        gate.HeartRate = heartRate;

                    ///NumberOfTimeSlots
                    int numTimeSlots;
                    if (dataset.TryGetValue<int>(DicomTag.NumberOfTimeSlots, 0, out numTimeSlots))
                        gate.NumberOfTimeSlots = numTimeSlots;

                    ///FrameTime
                    double frameTime;
                    if (dataset.TryGetValue<double>(DicomTag.FrameTime, 0, out frameTime))
                        gate.FrameTime = frameTime;

                    ///Low R-R Value
                    int lowRrValue;
                    if (dataset.TryGetValue<int>(DicomTag.LowRRValue, 0, out lowRrValue))
                        gate.LowRrValue = lowRrValue;

                    ///HighR-R Value
                    int highRrValue;
                    if (dataset.TryGetValue<int>(DicomTag.HighRRValue, 0, out highRrValue))
                        gate.HighRrValue = highRrValue;

                    ///Number of Intervals Acquired
                    int numAcquired;
                    if (dataset.TryGetValue<int>(DicomTag.IntervalsAcquired, 0, out numAcquired))
                        gate.NumIntervalsAcquired = numAcquired;

                    ///Number of Intervals Rejected
                    int numRejected;
                    if (dataset.TryGetValue<int>(DicomTag.IntervalsRejected, 0, out numRejected))
                        gate.NumIntervalsRejected = numRejected;

                }
                #endregion

                #region commonNMImage
                if (img != null)
                {

                    ///Patient Orientation
                    (img as NMImage).PatientOrientation = GetPatientOrientation(dataset);
                    ///Patient Orientation Modifier
                    (img as NMImage).PatientOrientationModifier = GetPatientOrientationModifier(dataset);
                    ///Number of Detectors
                    ushort tmp;
                    if (dataset.TryGetValue<ushort>(DicomTag.NumberOfDetectors, 0, out tmp))
                        (img as NMImage).NumberOfDetectors = tmp;

                    ///Counts accumulated
                    int a;
                    if (dataset.TryGetValue<int>(DicomTag.CountsAccumulated, 0, out a))
                        (img as NMImage).CountsAccumulated = a;

                    ///Zoom Factor
                    int zoom;
                    if (dataset.TryGetValue<int>(DicomTag.ZoomFactor, 0, out zoom))
                        (img as NMImage).ZoomFactor = zoom;

                    ///Number of Frames
                    if(!(img is TomoNMImage))
                    {
                        int numFrames;
                        if (dataset.TryGetValue<int>(DicomTag.NumberOfFrames, 0, out numFrames))
                            (img as NMImage).NumberOfFrames = numFrames;
                    }

                    ///Table Height
                    double tableHeight;
                    if (dataset.TryGetValue<double>(DicomTag.TableHeight, 0, out tableHeight))
                        (img as NMImage).TableHeight = tableHeight;

                    ///Table Traverse
                    double tableTravers;
                    if (dataset.TryGetValue<double>(DicomTag.TableTraverse, 0, out tableTravers))
                        (img as NMImage).TableTraverse = tableTravers;

                    #region  AcquisitionTerminationCondition
                    string term = "";
                    if (dataset.TryGetString(DicomTag.AcquisitionTerminationCondition, out term))
                    {
                        switch (term)
                        {
                            case "CNTS":
                                (img as NMImage).AcquisitionTerminationCondition = AcquisitionTerminationCondition.CNTS;
                                break;
                            case "DENS":
                                (img as NMImage).AcquisitionTerminationCondition = AcquisitionTerminationCondition.DENS;
                                break;
                            case "MANU":
                                (img as NMImage).AcquisitionTerminationCondition = AcquisitionTerminationCondition.MANU;
                                break;
                            case "OVFL":
                                (img as NMImage).AcquisitionTerminationCondition = AcquisitionTerminationCondition.OVFL;
                                break;
                            case "TIME":
                                (img as NMImage).AcquisitionTerminationCondition = AcquisitionTerminationCondition.TIME;
                                break;
                            case "TRIG":
                                (img as NMImage).AcquisitionTerminationCondition = AcquisitionTerminationCondition.TRIG;
                                break;
                        }
                    }
                    #endregion

                    #region correctedImage
                    if (dataset.TryGetString(DicomTag.CorrectedImage, out term))
                    {
                        if (term.Contains("UNIF"))
                            (img as NMImage).FloodCorrected = true;

                        if (term.Contains("COR"))
                            (img as NMImage).CorCorrected = true;

                        if (term.Contains("NCO"))
                            (img as NMImage).NcoCorrected = true;

                        if (term.Contains("DECY"))
                            (img as NMImage).DecayCorrected = true;

                        if 
                            (term.Contains("ATTN"))
                            (img as NMImage).AttenuationCorrected = true;

                        if (term.Contains("SCAT"))
                            (img as NMImage).ScatterCorrected = true;

                        if (term.Contains("DTIM"))
                            (img as NMImage).DeadTimeCorrected = true;

                        if (term.Contains("NRGY"))
                            (img as NMImage).EnergyCorrected = true;

                        if (term.Contains("LIN"))
                            (img as NMImage).LinearityCorrected = true;

                        if (term.Contains("MOTN"))
                            (img as NMImage).MotionCorrected = true;

                        if (term.Contains("CLN"))
                            (img as NMImage).CountLossCorrected = true;
                    }
                    #endregion

                    #region DetectorInformationSequence
                    DicomSequence DetectorSequence;
                    if (dataset.TryGetSequence(DicomTag.DetectorInformationSequence, out DetectorSequence))
                    {
                        for (int i = 0; i < (img as NMImage).NumberOfDetectors; i++)
                        {
                            #region collimatorNameType
                            string colName, colType;
                            if (DetectorSequence.Items[i].TryGetString(DicomTag.CollimatorGridName, out colName))
                            {
                                (img as NMImage).CollimatorName = colName;
                            }
                            if (DetectorSequence.Items[i].TryGetString(DicomTag.CollimatorType, out colType))
                            {
                                switch (colType)
                                {
                                    case "PARA":
                                        (img as NMImage).CollimatorType = CollimatorType.PARA;
                                        break;
                                    case "PINH":
                                        (img as NMImage).CollimatorType = CollimatorType.PINH;
                                        break;
                                    case "FANB":
                                        (img as NMImage).CollimatorType = CollimatorType.FANB;
                                        break;
                                    case "CONE":
                                        (img as NMImage).CollimatorType = CollimatorType.CONE;
                                        break;
                                    case "SLNT":
                                        (img as NMImage).CollimatorType = CollimatorType.SLNT;
                                        break;
                                    case "ASTG":
                                        (img as NMImage).CollimatorType = CollimatorType.ASTG;
                                        break;
                                    case "DIVG":
                                        (img as NMImage).CollimatorType = CollimatorType.DIVG;
                                        break;
                                    case "NONE":
                                        (img as NMImage).CollimatorType = CollimatorType.NONE;
                                        break;
                                    case "UNKN":
                                        (img as NMImage).CollimatorType = CollimatorType.UNKN;
                                        break;
                                }
                            }
                            #endregion

                            #region StartAngle
                            Decimal angle;
                            if (DetectorSequence.Items[i].TryGetSingleValue<decimal>(DicomTag.StartAngle, out angle))
                            {
                                if (i == 0)
                                    (img as NMImage).Detector1StartAngle = (float)angle;

                                if (i == 1)
                                    (img as NMImage).Detector2StartAngle = (float)angle;
                            }
                            #endregion

                            #region RadialPosition
                            ///Radial position seems to be rarely used
                            ///so I'll skip this one?
                            #endregion
                        }

                    }

                    #endregion

                    #region energyWindow
                    DicomSequence WindowSequence;
                    if(dataset.TryGetSequence(DicomTag.EnergyWindowInformationSequence, out WindowSequence))
                    {
                        ///We wil only bother with the first items in the sequence,
                        string windowName;
                        if(WindowSequence.Items[0].TryGetString(DicomTag.EnergyWindowName,out windowName))
                        {
                            (img as NMImage).EnergyWindowName = windowName;
                            DicomSequence energyRange;
                            if(WindowSequence.Items[0].TryGetSequence(DicomTag.EnergyWindowRangeSequence, out energyRange))
                            {
                                double lowerLimit;
                                if (energyRange.Items[0].TryGetValue<double>(DicomTag.EnergyWindowLowerLimit, 0, out lowerLimit))
                                {
                                    (img as NMImage).EnergyWindowLowerLimit = lowerLimit;
                                }
                                double upperLimit;
                                if (energyRange.Items[0].TryGetValue<double>(DicomTag.EnergyWindowUpperLimit, 0, out upperLimit))
                                {
                                    (img as NMImage).EnergyWindowUpperLimit = upperLimit;
                                }
                            }
                           }
                    }
                    #endregion


                }
                #endregion

                #endregion
            }
            else if(dataset.GetValue<DicomUID>(DicomTag.SOPClassUID,0).UID == "1.2.840.10008.5.1.4.1.1.2")
            {
                #region CTImageStorage
                logger.Info("SOPClassUID is CT Image");
                img = new CTImage();
                CTImage ct = img as CTImage;
                ct.Modality = Modality.CT;

                ///sliceThickness
                double sliceThickness;
                if (dataset.TryGetValue<double>(DicomTag.SliceThickness, 0, out sliceThickness))
                    ct.SliceThickness = sliceThickness;

                ///kVp
                double kvp;
                if (dataset.TryGetValue<double>(DicomTag.KVP, 0, out kvp))
                    ct.kVp = kvp;

                #region rotationDirection
                string rotationDirection;
                if(dataset.TryGetString(DicomTag.RotationDirection,out rotationDirection))
                {
                    switch (rotationDirection)
                    {
                        case "CW":
                            ct.RotationDirection = RotationDirection.CW;
                            break;
                        case "CC":
                            ct.RotationDirection = RotationDirection.CC;
                            break;
                    }
                }
                #endregion

                /////ExposureTie
                //double exposureTime;
                //if (dataset.TryGetValue<double>(DicomTag.ExposureTime, 0, out exposureTime))
                //    ct.ExposureTime = exposureTime;

                ///TubeCurrent
                double tubeCurrent;
                if (dataset.TryGetValue<double>(DicomTag.XRayTubeCurrent, 0, out tubeCurrent))
                    ct.TubeCurrent = tubeCurrent;

                ///FilerType
                string filterType;
                if (dataset.TryGetString(DicomTag.FilterType, out filterType))
                    ct.FilterType = filterType;

                ///ConvolutionKernel
                string kernel;
                if (dataset.TryGetString(DicomTag.ConvolutionKernel, out kernel))
                    ct.ConvolutionKernel = kernel;

                ///RevolutionTime
                double revolutionTime;
                if (dataset.TryGetValue<double>(DicomTag.RevolutionTime, 0, out revolutionTime))
                    ct.RevolutionTime = revolutionTime;

                ///SingleCollimationWidth
                double singleWidth;
                if (dataset.TryGetValue<double>(DicomTag.SingleCollimationWidth, 0, out singleWidth))
                    ct.SingleCollimationWidth = singleWidth;

                ///TotalCollimationWidth
                double totalWidth;
                if (dataset.TryGetValue<double>(DicomTag.TotalCollimationWidth, 0, out totalWidth))
                    ct.TotalCollimationWidth = totalWidth;

                ///TableSpeed
                double tableSpeed;
                if(dataset.TryGetValue<double>(DicomTag.TableSpeed,0,out tableSpeed))                
                    ct.TableSpeed = tableSpeed;

                ///Pitch
                double pitch;
                if (dataset.TryGetValue<double>(DicomTag.SpiralPitchFactor, 0, out pitch))
                    ct.Pitch = pitch;

                double ctdi;
                if (dataset.TryGetValue<double>(DicomTag.CTDIvol, 0, out ctdi))
                    ct.CTDIvol = ctdi;
               
                #endregion
            }

            if (img != null)
            {
                ///SereisDateTime
                img.SeriesDateTime = dataset.GetDateTime(DicomTag.SeriesDate, DicomTag.SeriesTime);
                ///Frame of reference
                string tmp = "";
                if (dataset.TryGetString(DicomTag.FrameOfReferenceUID, out tmp))
                    img.FrameOfReferenceUID = tmp;
                img.Columns = dataset.GetValue<ushort>(DicomTag.Columns, 0);
                img.Rows = dataset.GetValue<ushort>(DicomTag.Rows, 0);
                ///SeriesDescription
                if (dataset.TryGetString(DicomTag.SeriesDescription, out tmp))
                    img.SeriesDescription = tmp;
                ///OperatorsName
                if (dataset.TryGetString(DicomTag.OperatorsName, out tmp))
                    img.OperatorsName = tmp;
                ///SOPClassUID
                img.SOPClassUID = dataset.GetString(DicomTag.SOPClassUID);
                ///SeriesInstanceUID
                img.SeriesInstanceUID = dataset.GetString(DicomTag.SeriesInstanceUID);
                
                ///PixelSpacing
                string pixelSpacing;
                if(dataset.TryGetString(DicomTag.PixelSpacing,out pixelSpacing))
                {
                    (img as PatientImage).PixelSizeX = Convert.ToSingle(pixelSpacing.Split('\\')[0]);
                    (img as PatientImage).PixelSizeY = Convert.ToSingle(pixelSpacing.Split('\\')[1]);
                }
                ///Modality
                string modality;
                if (dataset.TryGetString(DicomTag.Modality, out modality))
                {
                    Modality m;
                    if (Enum.TryParse<Modality>(modality, out m))
                        (img as PatientImage).Modality = m;
                }

                ///Patient Position
                (img as PatientImage).PatientPosition = GetPatientPosition(dataset);

                ///DeviceSerialnumber
                string deviceSerial;
                if (dataset.TryGetString(DicomTag.DeviceSerialNumber, out deviceSerial))
                    img.DeviceSerialNumber = deviceSerial;

                ///ManufacturerModelName
                string modelName;
                if (dataset.TryGetString(DicomTag.ManufacturerModelName, out modelName))
                    img.ManufacturerModelName = modelName;

                ///Appointment
                string procedureID;
                if(dataset.TryGetString(DicomTag.AccessionNumber,out procedureID))
                {
                    logger.Trace("Matching accession number " + procedureID);
                    RetrievalCriteria rc1 = new RetrievalCriteria("ReferenceNumber", CriteraType.Equals, procedureID);
                    RetrievalCriteria rc2 = new RetrievalCriteria("ReferenceNumber", CriteraType.IsNotNull, null);
                    List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
                    rcList.Add(rc2);
                    rcList.Add(rc1);
                    
                    var items = retriever.RetrieveItems(typeof(Appointment), rcList);
                   
                    if (items.Any())
                        img.Appointment = items.First() as Appointment;

                }
                

                
            }



            if (img != null)
            {
               
                Platform.Retriever.SaveItem(img);
                logger.Debug("Series has been translated into PatientImage - " + img.ConcreteType.ToString() + " ID = " + img.ID);
            }
            else
                logger.Warn("SOPClass not recognized by translator " + dataset.GetString(DicomTag.SOPClassUID));
            return img;
        }

        public static async Task<PatientImage> TranslateSeries(TempDicomSeries series)
        {

            if(IsSeriesTranslated(series.SeriesInstanceUID))
            {
                RetrievalCriteria rc1 = new RetrievalCriteria("SeriesInstanceUID", CriteraType.Equals, series.SeriesInstanceUID);
                List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
                rcList.Add(rc1);
                var img = retriever.RetrieveItems(typeof(PatientImage), rcList);
                if (img.Any())
                    return img.First() as PatientImage;
                else
                {
                    logger.Error("Could not retrieve Patient Image for series " + series.SeriesInstanceUID);
                    return null;
                }
            }
            else
            {
                logger.Trace("This series has yet to be translated");
                logger.Debug("Series description = " + series.SeriesDescription);
                if (series.SeriesDescription.ToLower().Contains("results") || series.SeriesDescription.ToLower().Contains("multiframe") || series.SeriesDescription.ToLower().Contains("cardiac spect") || series.SeriesDescription.ToLower().Contains("dose report"))
                {
                    logger.Error("Dataset rejected because of series description '" + series.SeriesDescription + "', descritpion cannot include 'results' or 'multiframe' or 'cardiac spect' or 'dose report'");
                    return null;
                }
                
                ///Retrieve the images and convert to
                List<DicomFile> files = await connector.GetDicomFiles(series.TempDicomStudy.StudyInstanceUID, series.SeriesInstanceUID);
                ///How many files do we have
                if (files.Count == 1)
                {
                    if(files.First().Dataset.GetString(DicomTag.SOPClassUID) == "1.2.840.10008.5.1.4.1.1.7")
                    {
                        logger.Debug("file recognized as secondary capture");
                        var f = TranslateScreencap(series);
                        return null;
                    }
                    return TranslateDataset(files.First().Dataset);
                }
                    
                else if(files.Count > 1)
                {
                    PatientImage pi = TranslateDataset(files.First().Dataset);
                    if(pi is StaticNMImage || pi is WholebodyNMImage)
                    {
                        ///If there is another file for the other detecotr we should look that up
                       
                    }
                    if(pi is CTImage)
                    {
                        ///We need to go through the other files to get the full scan range
                        (pi as CTImage).NumberOfFrames = files.Count;
                    }
                    Platform.Retriever.SaveItem(pi);
                    return pi;
                }
                else
                    logger.Trace("There are " + files.Count + " files in the series");
                    return null;
            }
        }

        public static PatientImage TranslateCTFiles(List<DicomFile> files)
        {
            return null;
        }

        public static async Task<iRadiate.DataModel.Common.File> TranslateScreencap(TempDicomSeries series)
        {
            if (DicomModule.IsScreencapStored(series.SeriesInstanceUID))
                return null;

            iRadiate.DataModel.Common.File f = new iRadiate.DataModel.Common.File();
            f.Description = series.SeriesDescription;
            f.Extension = "jpg";
            var accNumber = series.TempDicomStudy.AccesionNumber;
            RetrievalCriteria rc0 = new RetrievalCriteria("ReferenceNumber", CriteraType.IsNotNull, accNumber);
            RetrievalCriteria rc1 = new RetrievalCriteria("ReferenceNumber", CriteraType.Equals, accNumber);
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc0);
            rcList.Add(rc1);
            var images = retriever.RetrieveItems(typeof(Appointment), rcList);

            if (images.Any())
            {
                List<DicomFile> files = await connector.GetDicomFiles(series.TempDicomStudy.StudyInstanceUID, series.SeriesInstanceUID);
                if (files.Any())
                {
                    MemoryStream ms = new MemoryStream();
                    //files.First().Save(ms);
                    DicomImage image = new DicomImage(files.First().Dataset);
                    image.RenderImage().AsSharedBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] buf = ms.GetBuffer();
                    f.Data = buf;
                    f.Study = (images.First() as Appointment).Study;
                    retriever.SaveItem(f);
                    DicomModule.SaveScreencapLink(series.SeriesInstanceUID, f);
                    logger.Info("Saved screencap " + series.SeriesInstanceUID);
                    return f;
                }
                else
                {
                    logger.Error("No files were returned for the screencap");
                }
               
            }
          
            return null;
        }
        private static PatientOrientation GetPatientOrientation(DicomDataset dataset)
        {
            DicomSequence seq;
            PatientOrientation result = PatientOrientation.Unspecified;
            if(dataset.TryGetSequence(DicomTag.PatientOrientationCodeSequence,out seq))
            {
                string tmp = "";
                if(seq.Items.Where(x=>x.TryGetString(DicomTag.CodeValue, out tmp)).Any())
                {
                    switch (tmp)
                    {
                        case "F-10440":
                            result = PatientOrientation.Erect;
                            break;
                        case "F-10450":
                            result = PatientOrientation.Recumbent;
                            break;
                        case "F-10460":
                            result = PatientOrientation.Semierect;
                            break;


                    }
                }
            }
            return result;
        }

        private static PatientOrientationModifier GetPatientOrientationModifier(DicomDataset dataset)
        {
            PatientOrientationModifier result = PatientOrientationModifier.Unspecified;
            DicomSequence seq;
            if (dataset.TryGetSequence(DicomTag.PatientOrientationCodeSequence, out seq))
            {
                DicomSequence innerSeq = null;
                if(seq.Items.Where(y=>y.TryGetSequence(DicomTag.PatientOrientationModifierCodeSequence, out innerSeq)).Any())
                {
                    string tmp = "";
                    if (innerSeq.Items.Where(x => x.TryGetString(DicomTag.CodeValue, out tmp)).Any())
                    {
                        switch (tmp)
                        {
                            case "F-10310":
                                result = PatientOrientationModifier.Prone;
                                break;
                            case "F-10316":
                                result = PatientOrientationModifier.SemiProne;
                                break;
                            case "F-10318":
                                result = PatientOrientationModifier.LateralDecubitus;
                                break;
                            case "F-10320":
                                result = PatientOrientationModifier.Standing;
                                break;
                            case "F-10330":
                                result = PatientOrientationModifier.Kneeling;
                                break;
                            case "F-10336":
                                result = PatientOrientationModifier.KneeChest;
                                break;
                            case "F-10340":
                                result = PatientOrientationModifier.Supine;
                                break;
                            case "F-10346":
                                result = PatientOrientationModifier.Lithotomy;
                                break;
                            case "F-10348":
                                result = PatientOrientationModifier.Trendelenburg;
                                break;
                            case "F-10349":
                                result = PatientOrientationModifier.InverseTrendelenburg;
                                break;
                            case "F-10380":
                                result = PatientOrientationModifier.Frog;
                                break;
                            case "F-10390":
                                result = PatientOrientationModifier.StoopedOver;
                                break;
                            case "F-103A0":
                                result = PatientOrientationModifier.Sitting;
                                break;
                            case "F-10410":
                                result = PatientOrientationModifier.CurledUp;
                                break;
                            case "F-10317":
                                result = PatientOrientationModifier.RightLateralDecubitus;
                                break;
                            case "F-10319":
                                result = PatientOrientationModifier.LeftLateralDecubitus;
                                break;
                            case "R-40799":
                                result = PatientOrientationModifier.Lordotic;
                                break;
                        }
                    }
                }
            }
            return result;
        }
        private static PatientPosition GetPatientPosition(DicomDataset dataset)
        {
            string tmp = "";
            DicomSequence pgrcds;
            PatientPosition result = PatientPosition.Unspecified;
            if(dataset.TryGetString(DicomTag.PatientPosition,out tmp))
            {
                
                if(tmp != "")
                {
                    if (Enum.TryParse(tmp, out result))
                        logger.Trace("PatientPosition parsed from value " + tmp);
                    
                }
                
            }
            else if(dataset.TryGetSequence(DicomTag.PatientGantryRelationshipCodeSequence,out pgrcds))
            {
                string code = pgrcds.Items[0].GetString(DicomTag.CodeValue);
                PatientOrientationModifier m = GetPatientOrientationModifier(dataset);
                if(m != PatientOrientationModifier.Unspecified)
                {
                    switch (code)
                    {
                        case "F-10480":
                            if (m == PatientOrientationModifier.Supine)
                                result = PatientPosition.FFS;
                            else if (m == PatientOrientationModifier.Prone)
                                result = PatientPosition.FFP;
                            break;
                        case "F-10470":
                            if (m == PatientOrientationModifier.Supine)
                                result = PatientPosition.HFS;
                            else if (m == PatientOrientationModifier.Prone)
                                result = PatientPosition.HFP;
                            break;
                    }
                }
                
            }
            
                

            
            return result;
        }

        private static bool IsSeriesTranslated(string seriesUID)
        {
            RetrievalCriteria rc1 = new RetrievalCriteria("SeriesInstanceUID", CriteraType.Equals, seriesUID);
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc1);
            var images = retriever.RetrieveItems(typeof(PatientImage), rcList);
            if (images.Any())
                return true;
            return false;
        }

       
    }
}
