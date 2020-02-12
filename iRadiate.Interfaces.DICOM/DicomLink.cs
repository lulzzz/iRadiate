using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dicom;
using Dicom.Imaging;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Interfaces.DICOM
{
    /// <summary>
    /// The DicomLink class represents a link between a DICOM Dataset) and a PatientImage in iRadiate
    /// </summary>
    public class DicomLink
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private int _imageID;
        private PatientImage _patientImage;
        private string _seriesUID;
        private TempDicomSeries _dicomDataSet;
        public DicomLink()
        {

        }

       

        /// <summary>
        /// The patientImage that is the iRadiate side of the link
        /// </summary>
        public PatientImage PatientImage
        {
            get { return _patientImage; }
            set { _patientImage = value; }
        }

        /// <summary>
        /// The series UID of the DicomDataSet
        /// </summary>
        public string SeriesUID
        {
            get { return _seriesUID; }
            set { _seriesUID = value; }
        }

        /// <summary>
        /// The DicomDataset that represents the Fo-DICOM side of the link
        /// </summary>
        public TempDicomSeries TempDicomSeries
        {
            get { return _dicomDataSet; }
            set { _dicomDataSet = value; }
        }

        /// <summary>
        /// Gets the link based on seriesUID, returns null if link doesnt exist
        /// </summary>
        /// <param name="seriesUID">The series UID retrieved by the Dicom connector</param>
        /// <returns>A DicomLink or null if link doesn't exist</returns>
        public static DicomLink GetDicomLink(TempDicomSeries dicomSeires)
        {
            DicomLink dl = new DicomLink();
            dl.TempDicomSeries = dicomSeires;
            dl.SeriesUID = dicomSeires.SeriesInstanceUID;
            IDataRetriever retriever = Platform.Retriever;
          
            RetrievalCriteria rc1 = new RetrievalCriteria("SeriesInstanceUID", CriteraType.ExactTextMatch, dicomSeires.SeriesInstanceUID);
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc1);
            var images = retriever.RetrieveItems(typeof(PatientImage), rcList);
            if (images.Any())
                dl.PatientImage = images.First() as PatientImage;
            else
                return null;

           

            return dl;
        }

        /// <summary>
        /// Creates a DicomLink for a DicomDataset, returns null if link can't be made.
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>A DicomLinkor null if the link can't be created</returns>
        public static async Task<DicomLink> CreateDicomLink(TempDicomSeries series)
        {
            logger.Trace("CreateDicomLink(" + series.SeriesInstanceUID +")");
            PatientImage img = await DicomTranslator.TranslateSeries(series);
            if(img == null)
            {
                logger.Warn("DicomTranslator returned null");
                return null;
            }
            DicomLink link = new DicomLink();
            link.TempDicomSeries = series;
            link.PatientImage = img;
            link.SeriesUID = series.SeriesInstanceUID;
            Platform.Retriever.SaveItem(img);
            logger.Info("Link created between series " + series.SeriesInstanceUID + " and PatientImage " + img.ID);
            return link;
        }
    }
}
