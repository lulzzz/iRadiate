using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iRadiate.DataModel.NucMed
{
    public class CTImage : PatientImage
    {
        private double _sliceThickness;
        private double _sliceInterval;
        private double _pitch;
        private double _kVp;
        private RotationDirection _rotationDirection;
        private double _revolutionTime;
        private double _tubeCurrent;
        private string _filterType;
        private string _convolutionKernel;
        private double _singleCollimationWidth;
        private double _totalCollimationWidth;
        private double _tableSpeed;
        private double _CTDIvol;

        public CTImage() : base()
        {

        }

        /// <summary>
        /// Gets or sets the thickness of each slice in mm
        /// </summary>
        /// <remarks>
        /// All slices must have the same thickness
        /// </remarks>
        public double SliceThickness
        {
            get { return _sliceThickness; }
            set { _sliceThickness = value; }

        }

        /// <summary>
        /// The interval between each slice in mm
        /// </summary>
        public double SlicerInterval
        {
            get { return _sliceInterval; }
            set { _sliceInterval = value; }
        }

        /// <summary>
        /// Gets or sets the pitch of the CT scan
        /// </summary>
        public double Pitch
        {
            get { return _pitch; }
            set { _pitch = value; }
        }

        /// <summary>
        /// Gets or sets the peak kilovoltage in the image.
        /// </summary>
        public double kVp
        {
            get { return _kVp; }
            set { _kVp = value; }
        }

        public RotationDirection RotationDirection
        {
            get { return _rotationDirection; }
            set { _rotationDirection = value; }
        }

        public double RevolutionTime
        {
            get { return _revolutionTime; }
            set { _revolutionTime = value; }
        }

        public double TubeCurrent
        {
            get { return _tubeCurrent; }
            set { _tubeCurrent = value; }
        }

        /// <summary>
        /// in msec
        /// </summary>
        public double ExposureTime
        {
            get
            {
                if (Pitch > 0)
                    return RevolutionTime / Pitch;
                else
                    return 0;
            }
        }

        public double Exposure
        {
            get { return ExposureTime * TubeCurrent; }
        }

        public string FilterType
        {
            get { return _filterType; }
            set { _filterType = value; }
        }

        public string ConvolutionKernel
        {
            get { return _convolutionKernel; }
            set { _convolutionKernel = value; }
        }

        public double SingleCollimationWidth
        {
            get { return _singleCollimationWidth; }
            set { _singleCollimationWidth = value; }
        }

        public double TotalCollimationWidth
        {
            get { return _totalCollimationWidth; }
            set { _totalCollimationWidth = value; }
        }

        public double TableSpeed
        {
            get { return _tableSpeed; }
            set { _tableSpeed = value; }
        }

        public double CTDIvol
        {
            get { return _CTDIvol; }
            set { _CTDIvol = value; }
        }

        public override DateTime ScanFinishedDateTime
        {
            get
            {
                return SeriesDateTime.AddSeconds(45);
            }
        }

        public override double ScanDuration
        {
            get
            {
                return 45;
            }
        }

        public virtual string CollimationDescription
        {
            get
            {
                double numSlices = TotalCollimationWidth / SingleCollimationWidth;
                return numSlices + " x " + SingleCollimationWidth.ToString("N1") + " mm";
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(CTImage);
            }
        }
    }
}
