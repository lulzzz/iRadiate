using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.DataModel.NucMed
{
    public class PatientImage : DataStoreItem, IPatientImage
    {
        #region privateFields
        private Appointment _appointment;
       
        private Modality _modality;
        private string _seriesInstanceUID;
        private int _seriesNumber;
        private Laterality? _laterality;
        private DateTime _seriesDateTime;
        private string _operatorsName;
        private string _seriesDescription;
        private PatientPosition? _patientPosition;
        private string _sOPClassUID;
        private User _user;
        private string _frameOfReferenceUID;
        private string _positionReferenceIndicator;
        private int _rows, _columns;
        private float _pixelSizeX;
        private float _pixelSizeY;
        private int _numberOfFrames;
        private ScanTask _scanTask;
        private double _rangeStart;
        private double _rangeEnd;
        private string _deviceSerialNumber, _manufacturerModelName;
        #endregion

        public PatientImage() : base()
        {

        }

        /// <summary>
        /// The user who acquired the image
        /// </summary>
        public User User
        {
            get { return _user; }
            set { _user = value; }
        }
        /// <summary>
        /// The appointment this image belongs to
        /// </summary>
        public Appointment Appointment
        {
            get
            {
                return _appointment;
            }
            set
            {
                _appointment = value;
            }
        }

        /// <summary>
        /// The study this image belongs to
        /// </summary>
        public Study Study
        {
            get
            {
                if(Appointment != null)
                {
                    return Appointment.Study;
                }
                return null;
            }
        }

        /// <summary>
        /// The patient this image belong sto
        /// </summary>
        public Patient Patient
        {
            get
            {
                if(Study != null)
                {
                    return Study.Patient;
                }
                return null;
            }
        } 

        
        /// <summary>
        /// The modality of this image
        /// </summary>
        public virtual Modality Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

        /// <summary>
        /// Unique identifier of the Series (0020,000E).
        /// </summary>

        public string SeriesInstanceUID
        {
            get { return _seriesInstanceUID; }
            set { _seriesInstanceUID = value; }
        }

        /// <summary>
        /// A number that identifies this Series. 	(0020,0011)
        /// </summary>
        public int SeriesNumber
        {
            get { return _seriesNumber; }
            set { _seriesNumber = value; }
        }

        /// <summary>
        /// Date the Series started, (0008,0021) and Time the Series started (0008,0031).
        /// </summary>
        public DateTime SeriesDateTime
        {
            get { return _seriesDateTime; }
            set { _seriesDateTime = value; }
        }

        /// <summary>
        /// Name(s) of the operator(s) supporting the Series (0008,1070).
        /// </summary>
        public string OperatorsName
        {
            get { return _operatorsName; }
            set { _operatorsName = value; }
        }

        /// <summary>
        /// Description of the Series (0008,103E).
        /// </summary>
        public string SeriesDescription
        {
            get { return _seriesDescription; }
            set { _seriesDescription = value; }
        }

        /// <summary>
        /// Patient position descriptor relative to the equipment (0018,5100).
        /// </summary>
        public PatientPosition? PatientPosition
        {
            get { return _patientPosition; }
            set { _patientPosition = value; }
        }

        /// <summary>
        /// Uniquely identifies the SOP Class (0008,0016).
        /// </summary>
        public string SOPClassUID
        {
            get
            {
                return _sOPClassUID;
            }

            set
            {
                _sOPClassUID = value;
            }
        }

        /// <summary>
        /// Uniquely identifies the frame of reference for a Series (0020,0052).
        /// </summary>
        public string FrameOfReferenceUID
        {
            get { return _frameOfReferenceUID; }
            set { _frameOfReferenceUID = value; }
        }

        /// <summary>
        /// Part of the imaging target used as a reference 	(0020,1040).
        /// </summary>
        public string PositionReferenceIndicator
        {
            get { return _positionReferenceIndicator; }
            set { _positionReferenceIndicator = value; }
        }

        /// <summary>
        /// Number of rows in the image (0028,0010).
        /// </summary>
        public int Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        /// <summary>
        /// Number of columns in the image (0028,0011).
        /// </summary>
        public int Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        /// <summary>
        /// The X size of the pixel in mm
        /// </summary>
        public float PixelSizeX
        {
            get { return _pixelSizeX; }
            set { _pixelSizeX = value; }
        }

        /// <summary>
        /// The Y size of the pixel in mm
        /// </summary>
        public float PixelSizeY
        {
            get { return _pixelSizeY; }
            set { _pixelSizeY = value; }
        }


        /// <summary>
        /// Number of frames in a Multi-frame Image (0028,0008) or the number of slices.
        /// </summary>
        public int NumberOfFrames
        {
            get { return _numberOfFrames; }
            set { _numberOfFrames = value; }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(PatientImage);
            }
        }

        public virtual ScanTask ScanTask
        {
            get
            {
                return _scanTask;
            }

            set
            {
                _scanTask = value;
            }
        }

        public double RangeStart
        {
            get { return _rangeStart; }
            set { _rangeStart = value; }
        }

        public double RangeEnd
        {
            get { return _rangeEnd; }
            set { _rangeEnd = value; }
        }

        public virtual DateTime ScanFinishedDateTime
        {
            get
            {
                return SeriesDateTime.AddMinutes(10);
            }
        }

        public virtual double ScanDuration
        {
            get
            {
                return 10;
            }
        }

        public string DeviceSerialNumber
        {
            get
            {
                return _deviceSerialNumber;
            }

            set
            {
                _deviceSerialNumber = value;
            }
        }

        public string ManufacturerModelName
        {
            get
            {
                return _manufacturerModelName;
            }

            set
            {
                _manufacturerModelName = value;
            }
        }

        public bool IsSupine
        {
            get
            {
                if (PatientPosition.HasValue)
                {
                    if (PatientPosition.Value == NucMed.PatientPosition.FFS || PatientPosition.Value == NucMed.PatientPosition.HFS)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
        }
    }


    public enum PatientOrientation { Erect, Recumbent, Semierect, Unspecified}

    public enum PatientOrientationModifier { Prone,
    SemiProne,
    LateralDecubitus,
    Standing,
    Anatomical,
    Kneeling,
    KneeChest,
    Supine,
    Lithotomy,
    Trendelenburg,
    InverseTrendelenburg,
    Frog,
    StoopedOver,
    Sitting,
    CurledUp,
    RightLateralDecubitus,
    LeftLateralDecubitus,
    Lordotic,
    Unspecified
    }

    public enum Modality {
        AR,
        ASMT,
        AU,
        BDUS,
        BI,
        BMD,
        CR,
        CT,
        CTPROTOCOL,
        DG,
        DOC,
        DX,
        ECG,
        EPS,
        ES,
        FID,
        GM,
        HC,
        HD,
        IO,
        IOL,
        IVOCT,
        IVUS,
        KER,
        KO,
        LEN,
        LS,
        MG,
        MR,
        NM,
        OAM,
        OCT,
        OP,
        OPM,
        OPT,
        OPV,
        OSS,
        OT,
        PLAN,
        PR,
        PT,
        PX,
        REG,
        RESP,
        RF,
        RG,
        RTDOSE,
        RTIMAGE,
        RTPLAN,
        RTRECORD,
        RTSTRUCT,
        RWV,
        SEG,
        SM,
        SMR,
        SR,
        SRF,
        STAIN,
        TG,
        US,
        VA,
        XA,
        XC
    }

    public enum Laterality { Right, Left}

    public enum PatientPosition
    {
        HFP,
        HFS,
        HFDR,
        HFDL,
        FFDR,
        FFDL,
        FFP,
        FFS,
        LFP,
        LFS,
        RFP,
        RFS,
        AFDR,
        AFDL,
        PFDR,
        PFDL,
        Unspecified
    }

    public enum PixelDataCharacteristics { ORIGINAL, DERIVED}

    public enum PixelExaminationCharacteristics { PRIMARY, SECONDARY}

    public enum NMImageType
    {
        Static,
        WholeBody,
        Dynamic,
        Gated,
        Tomo,
        GatedTomo,
        ReconTomo,
        ReconGatedTomo
    }
}
