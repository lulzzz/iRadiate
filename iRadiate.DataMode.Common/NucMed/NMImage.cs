using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.NucMed
{
    public abstract class NMImage : PatientImage
    {
        #region privateFields
        private PatientOrientation _patientOrientation;
        private PatientOrientationModifier _patientOrientationModifier;
        
        private int _numberOfDetectors;
        private int? _countsAccumulated;
        private AcquisitionTerminationCondition? _acquisitionTerminationCondition;
       
        private bool _floodCorrected;
        private bool _corCorrected;
        private bool _ncoCorrected;
        private bool _decayCorrected;
        private bool _attenuationCorrected;
        private bool _scatterCorrected;
        private bool _deadTimeCorrected;
        private bool _energyCorrected;
        private bool _linearityCorrected;
        private bool _motionCorrected;
        private bool _countLossCorrected;
        
        
        private List<NMEnergyWindow> _energyWindows;
        private string _collimatorName;
        private CollimatorType _collimatorType;
        private int? _fovXSize;
        private int? _fovYSize;
        private int _focalDistance;
        private float? _xFocusCenter;
        private float? _yFocusCenter;
        private float? _zoomFactor;
        
        private string _imageID;
        private float _detector1StartAngle;
        private float _detector2StartAngle;
        private float _detector1RadialPosition;
        private float _detector2RadialPosition;
        private string _energyWindowName;
        private double _energyWindowUpperLimit;
        private double _energyWindowLowerLimit;
        #endregion

        public NMImage() : base()
        {

        }

        private int _actualFrameDuration;
        private double _tableHeight;
        private double _tableTraverse;

        /// <summary>
        /// Elapsed time for data acquisition in msec (0018,1242).
        /// </summary>
        public int ActualFrameDuration
        {
            get { return _actualFrameDuration; }
            set { _actualFrameDuration = value; }
        }

        /// <summary>
        /// Sequence that describes the orientation of the patient with respect to gravity (0054,0410).
        /// </summary>
        public PatientOrientation PatientOrientation
        {
            get
            {
                return _patientOrientation;
            }
            set
            {
                _patientOrientation = value;
            }
        }

        /// <summary>
        /// Patient Orientation Modifier (0054,0412).
        /// </summary>
        public PatientOrientationModifier PatientOrientationModifier
        {
            get { return _patientOrientationModifier; }
            set { _patientOrientationModifier = value; }
        }

        /// <summary>
        /// Number of detectors (0054,0021).
        /// </summary>
        public int NumberOfDetectors
        {
            get { return _numberOfDetectors; }
            set { _numberOfDetectors = value; }
        }

        /// <summary>
        /// Sum of all gamma events for all frames in the image (0018,0070).
        /// </summary>
        public int? CountsAccumulated
        {
            get { return _countsAccumulated; }
            set { _countsAccumulated = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public AcquisitionTerminationCondition? AcquisitionTerminationCondition
        {
            get { return _acquisitionTerminationCondition; }
            set { _acquisitionTerminationCondition = value; }
        }


        /// <summary>
        /// One or more values that indicate which, if any, corrections have been applied to the image (0028,0051).
        /// </summary>
        #region corrections
        public bool FloodCorrected
        {
            get { return _floodCorrected;}
            set { _floodCorrected = value; }
        }

        public bool CorCorrected
        {
            get { return _corCorrected; }
            set { _corCorrected = value; }
        }

        public bool NcoCorrected
        {
            get { return _ncoCorrected; }
            set { _ncoCorrected = value; }
        }

        public bool DecayCorrected
        {
            get { return _decayCorrected; }
            set { _decayCorrected = value; }
        }

        public bool AttenuationCorrected
        {
            get { return _attenuationCorrected; }
            set { _attenuationCorrected = value; }
        }

        public bool ScatterCorrected
        {
            get { return _scatterCorrected; }
            set { _scatterCorrected = value; }
        }

        public bool DeadTimeCorrected
        {
            get { return _deadTimeCorrected; }
            set { _deadTimeCorrected = value; }
        }

        public bool EnergyCorrected
        {
            get { return _energyCorrected; }
            set { _energyCorrected = value; }
        }

        public bool LinearityCorrected
        {
            get { return _linearityCorrected; }
            set { _linearityCorrected = value; }
        }

        public bool MotionCorrected
        {
            get { return _motionCorrected; }
            set { _motionCorrected = value; }
        }

        public bool CountLossCorrected
        {
            get { return _countLossCorrected; }
            set { _countLossCorrected = value; }
        }
        #endregion

        public string EnergyWindowName
        {
            get { return _energyWindowName; }
            set { _energyWindowName = value; }
        }

        public double EnergyWindowUpperLimit
        {
            get { return _energyWindowUpperLimit; }
            set { _energyWindowUpperLimit = value; }
        }

        public double EnergyWindowLowerLimit
        {
            get { return _energyWindowLowerLimit; }
            set { _energyWindowLowerLimit = value; }
        }
        /// <summary>
        /// Sequence that describes the projection of the anatomic region of interest on the image receptor (0054,0220).
        /// </summary>
        [Obsolete]
        public List<NMEnergyWindow> EnergyWindows
        {
            get
            {
                if(_energyWindows == null)
                {
                    _energyWindows = new List<NMEnergyWindow>();
                }
                return _energyWindows;
            }
            set { _energyWindows = value; }
        }

        /// <summary>
        /// Label describing the collimator used (LEAP, hires, etc.) 	(0018,1180).
        /// </summary>
        public string CollimatorName
        {
            get { return _collimatorName; }
            set { _collimatorName = value; }
        }

        /// <summary>
        /// Collimator type (0018,1181).
        /// </summary>
        public CollimatorType CollimatorType
        {
            get { return _collimatorType; }
            set { _collimatorType = value; }
        }

        
        public float? ZoomFactor
        {
            get { return _zoomFactor; }
            set { _zoomFactor = value; }
        }

       
        /// <summary>
        /// User or equipment generated Image identifier 	(0054,0400).
        /// </summary>
        public string ImageID
        {
            get { return _imageID; }
            set { _imageID = value; }
        }

        /// <summary>
        /// The height of the table in mm   (0018,1130)
        /// </summary>
        public double TableHeight
        {
            get { return _tableHeight; }
            set { _tableHeight = value; }
        }

        /// <summary>
        /// The location of the table in mm   (0018,1131)
        /// </summary>
        public double TableTraverse
        {
            get { return _tableTraverse; }
            set { _tableTraverse = value; }
        }

        #region detectorSequence
        public float Detector1StartAngle
        {
            get { return _detector1StartAngle; }
            set { _detector1StartAngle = value; }
        }

        public float Detector2StartAngle
        {
            get { return _detector2StartAngle; }
            set { _detector2StartAngle = value; }
        }

        public float Detector1RadialPosition
        {
            get { return _detector1RadialPosition; }
            set { _detector1RadialPosition = value; }
        }
        
        public float Detector2RadialPosition
        {
            get { return _detector2RadialPosition; }
            set { _detector2RadialPosition = value; }
        }
        public virtual string DetectorAngleDescription
        {
            get
            {
                string ans = "";
                if(NumberOfDetectors > 0)
                {
                    
                    if(Detector1StartAngle > -15 && Detector1StartAngle < 15)
                    {
                        ans = "POST";
                    }
                    else if(Detector1StartAngle > 15 && Detector1StartAngle < 65)
                    {
                        ans = "LPO";
                    }
                    else if (Detector1StartAngle > 65 && Detector1StartAngle < 85)
                    {
                        ans = "LLAT";
                    }
                    else if (Detector1StartAngle > 85 && Detector1StartAngle < 150)
                    {
                        ans = "LAO";
                    }
                    else if (Detector1StartAngle > 150 && Detector1StartAngle < 205)
                    {
                        ans = "ANT";
                    }
                    else if (Detector1StartAngle > 205 && Detector1StartAngle < 240)
                    {
                        ans = "RAO";
                    }
                    else if (Detector1StartAngle > 240 && Detector1StartAngle < 295)
                    {
                        ans = "RLAT";
                    }
                    else if (Detector1StartAngle > 295 && Detector1StartAngle < 345)
                    {
                        ans = "RPO";
                    }
                }
                if (NumberOfDetectors > 1)
                {

                    if (Detector2StartAngle > -15 && Detector2StartAngle < 15)
                    {
                        ans = ans + " & POST";
                    }
                    else if (Detector2StartAngle > 15 && Detector2StartAngle < 65)
                    {
                        ans = ans + " & LPO";
                    }
                    else if (Detector2StartAngle > 65 && Detector2StartAngle < 85)
                    {
                        ans = ans + " & LLAT";
                    }
                    else if (Detector2StartAngle > 85 && Detector2StartAngle < 150)
                    {
                        ans = ans + " & LAO";
                    }
                    else if (Detector2StartAngle > 150 && Detector2StartAngle < 205)
                    {
                        ans = ans + " & ANT";
                    }
                    else if (Detector2StartAngle > 205 && Detector2StartAngle < 240)
                    {
                        ans = ans + " & RAO";
                    }
                    else if (Detector2StartAngle > 240 && Detector2StartAngle < 295)
                    {
                        ans = ans + " & RLAT";
                    }
                    else if (Detector1StartAngle > 295 && Detector1StartAngle < 345)
                    {
                        ans = ans + " & RPO";
                    }
                }

                return ans;
            }
        }
        #endregion

        public override DateTime ScanFinishedDateTime
        {
            get
            {
                if (ActualFrameDuration > 0)
                    return SeriesDateTime.AddMilliseconds(ActualFrameDuration);
                return base.ScanFinishedDateTime;
            }
        }

        public override double ScanDuration
        {
            get
            {
                if (ActualFrameDuration > 0)
                    return (ActualFrameDuration / 1000) / 60;

                return base.ScanDuration;
            }
        }
        public virtual string BedPositionDescription
        {
            get
            {
                return TableTraverse.ToString() + " mm";
            }
        }

        public virtual string GatingDescription
        {
            get
            {
                return "None";
            }
        }

        public virtual string DynamicDescription
        {
            get
            {
                return "None";
            }
        }

    }

    public class StaticNMImage : NMImage
    {
       

        public StaticNMImage() : base()
        {

        }

       
        public override Type ConcreteType
        {
            get
            {
                return typeof(StaticNMImage);
            }
        }
    }

    public class WholebodyNMImage : NMImage
    {
        
        private float _scanVelocity;
        private int _scanLength;

        public WholebodyNMImage() : base()
        {

        }

        
        public float ScanVelocity
        {
            get { return _scanVelocity; }
            set { _scanVelocity = value; }
        }

        /// <summary>
        /// Size of the imaged area in the direction of scanning motion, in mm	(0018,1302) .
        /// </summary>
        public int ScanLength
        {
            get { return _scanLength; }
            set { _scanLength = value; }
        }

        public override double ScanDuration
        {
            get
            {
                if(ScanLength > 0 && ScanVelocity > 0)
                {
                    return (ScanLength/((float)ScanVelocity))/ 60;
                }
                return base.ScanDuration;
            }
        }

        public override DateTime ScanFinishedDateTime
        {
            get
            {
                return SeriesDateTime.AddMinutes(ScanDuration);
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(WholebodyNMImage);
            }
        }

        public override string BedPositionDescription
        {
            get
            {
                return base.BedPositionDescription + " - " + ScanVelocity + " mm/s";
            }
        }
    }

    public class GatedNMImage : NMImage
    {
        private string _beatRejectionFlag;
        private int _skipBeats;
        private int _heartRate;
        private int _numberOfTimeSlots;
        private double _frameTime;
        private int _lowRrValue;
        private int _highRrValue;
        private int _numIntervalsAcquired;
        private int _numIntervalsRejected;

        public string BeatsRejectionFlag
        {
            get { return _beatRejectionFlag; }
            set { _beatRejectionFlag = value; }
        }

        public int SkipBeats
        {
            get { return _skipBeats; }
            set { _skipBeats = value; }
        }

        public int HeartRate
        {
            get { return _heartRate; }
            set { _heartRate = value; }
        }

        public int NumberOfTimeSlots
        {
            get { return _numberOfTimeSlots; }
            set { _numberOfTimeSlots = value; }
        }

        public double FrameTime
        {
            get { return _frameTime; }
            set { _frameTime = value; }
        }

        public int LowRrValue
        {
            get { return _lowRrValue; }
            set { _lowRrValue = value; }
        }

        public int HighRrValue
        {
            get { return _highRrValue; }
            set { _highRrValue = value; }
        }

        public int NumIntervalsAcquired
        {
            get { return _numIntervalsAcquired; }
            set { _numIntervalsAcquired = value; }
        }

        public int NumIntervalsRejected
        {
            get { return _numIntervalsRejected; }
            set { _numIntervalsRejected = value; }
        }

        public override string GatingDescription
        {
            get
            {
                return "HR " + HeartRate + ", " + SkipBeats + " skipped, " + NumberOfTimeSlots + " slots";
            }
        }
    }

    public class TomoNMImage : NMImage
    {
        private int _numberOfRotations;
        private float _angularStep;
        private RotationDirection _rotationDirection;
        private float? _radialPosition;
        private float _scanArc;

        public TomoNMImage() : base()
        {

        }

        /// <summary>
        /// Number of rotations (0054,0051). 
        /// </summary>
        public int NumberOfRotations
        {
            get { return _numberOfRotations; }
            set { _numberOfRotations = value; }
        }

        /// <summary>
        /// The angular scan arc step between views of the TOMO acquisition, in degrees (0018,1144).
        /// </summary>
        /// <remarks>
        /// Position of the detector about the patient for the start of this rotation, in degrees. 
        /// Zero degrees is referenced to the origin at the patient's back. Viewing from the patient's feet, 
        /// angle increases in a counter-clockwise direction (detector normal rotating from the patient's back towards the patient's left side).
        /// </remarks>
        public float AngularStep
        {
            get { return _angularStep; }
            set { _angularStep = value; }
        }

        /// <summary>
        /// Direction of rotation of the detector about the patient (0018,1140). 
        /// </summary>
        public RotationDirection RotationDirection
        {
            get { return _rotationDirection; }
            set { _rotationDirection = value; }
        }

        /// <summary>
        /// The effective angular range of the scan data in degrees 	(0018,1143).
        /// </summary>
        public float ScanArc
        {
            get { return _scanArc; }
            set { _scanArc = value; }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(TomoNMImage);
            }
        }

        public override DateTime ScanFinishedDateTime
        {
            get
            {
                if(NumberOfDetectors>0 && NumberOfFrames >0 && ActualFrameDuration >0)
                {
                    //int angles = NumberOfFrames / NumberOfDetectors;
                    return SeriesDateTime.AddSeconds((NumberOfFrames/2) * (ActualFrameDuration / 1000)).AddMinutes(1);
                }

                return base.ScanFinishedDateTime;
            }
        }

        public override double ScanDuration
        {
            get
            {
                return (ScanFinishedDateTime - SeriesDateTime).TotalMinutes;
            }
        }

        public override string DetectorAngleDescription
        {
            get
            {
                return ScanArc.ToString() + " deg " + RotationDirection.ToString() + " " + AngularStep + "/step";
            }
        }

       
    }

    public class SPECTNMImage : NMImage
    {
        private int _numberOfSlices;
        private float _spacingBetweenSlices;
        private float _sliceThickness;
        private PatientImage _parentImage;

        public SPECTNMImage() : base()
        {

        }

        public int NumberOfSlices
        {
            get { return _numberOfSlices; }
            set { _numberOfSlices = value; }
        }

        public float SpacingBetweenSlices
        {
            get { return _spacingBetweenSlices; }
            set { _spacingBetweenSlices = value; }
        }

        public float SliceThickness
        {
            get { return _sliceThickness; }
            set { _sliceThickness = value; }
        }

        /// <summary>
        /// The TomoImage from which this image is reconstructed
        /// </summary>
        public PatientImage ParentImage
        {
            get { return _parentImage; }
            set { _parentImage = value; }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(SPECTNMImage);
            }
        }

        public override DateTime ScanFinishedDateTime
        {
            get
            {
                return base.SeriesDateTime;
            }
        }

        public override double ScanDuration
        {
            get
            {
                return 0;
            }
        }
    }
    public class GatedTOMOImage : GatedNMImage
    {
        private int _numberOfRotations;
        private float _angularStep;
        private RotationDirection _rotationDirection;
        private float? _radialPosition;
        private float _scanArc;

        public GatedTOMOImage() : base()
        {

        }

        /// <summary>
        /// Number of rotations (0054,0051). 
        /// </summary>
        public int NumberOfRotations
        {
            get { return _numberOfRotations; }
            set { _numberOfRotations = value; }
        }

        /// <summary>
        /// The angular scan arc step between views of the TOMO acquisition, in degrees (0018,1144).
        /// </summary>
        /// <remarks>
        /// Position of the detector about the patient for the start of this rotation, in degrees. 
        /// Zero degrees is referenced to the origin at the patient's back. Viewing from the patient's feet, 
        /// angle increases in a counter-clockwise direction (detector normal rotating from the patient's back towards the patient's left side).
        /// </remarks>
        public float AngularStep
        {
            get { return _angularStep; }
            set { _angularStep = value; }
        }

        /// <summary>
        /// Direction of rotation of the detector about the patient (0018,1140). 
        /// </summary>
        public RotationDirection RotationDirection
        {
            get { return _rotationDirection; }
            set { _rotationDirection = value; }
        }

        /// <summary>
        /// The effective angular range of the scan data in degrees 	(0018,1143).
        /// </summary>
        public float ScanArc
        {
            get { return _scanArc; }
            set { _scanArc = value; }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(GatedTOMOImage);
            }
        }

        public override DateTime ScanFinishedDateTime
        {
            get
            {


                return SeriesDateTime.AddMinutes(12);
            }
        }

        public override double ScanDuration
        {
            get
            {
                return (ScanFinishedDateTime - SeriesDateTime).TotalMinutes;
            }
        }
    }

    public class DynamicNMImage : NMImage
    {
        private int _numberOfPhases;
        private string _phaseSequence;

        public DynamicNMImage() : base()
        {

        }

        public int NumberOfPhases
        {
            get { return _numberOfPhases; }
            set { _numberOfPhases = value; }
        }

        /// <summary>
        /// The Phase InformationSequence in one string
        /// </summary>
        /// <remarks>
        /// Each Phase is represented as 'PhaseDelay,ActualFrameDuration,PauseBetweenFrames,NumberOfFrames'
        /// Each sequence Item is separated by a ';'
        /// </remarks>
        public string PhaseSequence
        {
            get { return _phaseSequence; }
            set { _phaseSequence = value; }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(DynamicNMImage);
            }
        }

        public override string DynamicDescription
        {
            get
            {
                string ret = "X";
                if(NumberOfPhases == 1)
                {
                    var y = PhaseSequence.Split(',');
                    ret = "1 phase: " + y[3] + " x " + Convert.ToInt16(y[1])/1000 + " sec";
                    return ret;
                }
                else
                {
                    ret = NumberOfPhases + " phases: ";
                }
                var s = PhaseSequence.Split(';');
                for(int i = 0; i < s.Length; i++)
                {
                    if(i > 0)
                    {
                        ret = ret + " & ";
                    }
                    var s1 = s[i].Split(',');
                    ret = ret + s1[3] + " x " + Convert.ToInt16(s1[1]) / 1000 + " sec";
                }
                return ret;
            }
        }
    }
    public class NMEnergyWindow : DataStoreItem
    {
        private string _name;
        private double _upperEnergyLimit;
        private double _lowerEnergyLimit;

        public NMEnergyWindow() : base()
        {

        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double UpperEnergyLimit
        {
            get { return _upperEnergyLimit; }
            set { _upperEnergyLimit = value; }
        }

        public double LowerEnergyLimit
        {
            get { return _lowerEnergyLimit; }
            set { _lowerEnergyLimit = value; }
        }
    }

    #region enums
    public enum AcquisitionTerminationCondition
    {
        CNTS,
        DENS,
        MANU,
        OVFL,
        TIME,
        TRIG,
        Unspecified

    }
    public enum WholeBodyTechnique
    {
        OnePass,TwoPass,PCN,MSP
    }
    public enum CollimatorType
    {
        PARA, PINH, FANB, CONE, SLNT, ASTG, DIVG, NONE, UNKN
    }

    public enum RotationDirection { CW, CC}

    public enum DetectorMotionType { StepAndShoot, Continuous, AcqDuringStep}
    #endregion


}
