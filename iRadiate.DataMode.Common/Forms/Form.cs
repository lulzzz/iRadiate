using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Forms
{
    public abstract class Form : DataStoreItem
    {
        private string _name;
        private List<FormTemplate> _versions;

        public Form() : base()
        {

        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public List<FormTemplate> FormTemplates
        {
            get
            {
                if (_versions == null)
                    _versions = new List<FormTemplate>();
                return _versions;
            }
            set
            {
                _versions = value;
            }
        }

        public DateTime? LatestVersionDate
        {
            get
            {
                if (FormTemplates.Any())
                {
                    return FormTemplates.OrderBy(x => x.VersionDate).Last().VersionDate;
                }
                return null;
            }
        }
        
        public int? LatestVersionNumber
        {
            get
            {
                if (FormTemplates.Any())
                {
                    return FormTemplates.OrderBy(x => x.VersionDate).Last().VersionNumber;
                }
                return null;
            }
        } 
    }

    public class QAForm :Form
    {
        private FormFrequency _frequency;

        public QAForm() : base()
        {

        }

        public FormFrequency Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }

        public bool IsFormDue
        {
            get
            {
                return false;
            }
        }

        public List<FormInstance> Instances
        {
            get
            {
                return FormTemplates.SelectMany(x => x.Instances).ToList();
            }
        }

    }

    public enum FormFrequency { Daily,Weekly,Monthly,Quarterly,BiAnnual,Annual,AdHoc}

    
}
