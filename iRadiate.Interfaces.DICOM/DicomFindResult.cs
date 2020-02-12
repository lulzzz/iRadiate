using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.Interfaces.DICOM
{
    public class DicomFindResult
    {


        private List<string> seriesUIDs;
        public DicomFindResult(string studyUID)
        {
            seriesUIDs = new List<string>();
            StudyUID = studyUID;
        }
        public string StudyUID { get; set; }

        public List<string> SeriesUIDs
        {
            get
            {
                
                return seriesUIDs;
            }
            set
            {
                seriesUIDs = value;
            }
        }
    }
}
