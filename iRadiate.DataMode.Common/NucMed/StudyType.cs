using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.NucMed
{
    public class StudyType : DataStoreItem
    {
        private string _name;
        private List<WorkflowTemplate> _workflows;
        private NucMedPractice _nucMedPractice;
        private string _shortName;

        public string ShortName
        {
            get
            {
                return _shortName;
            }
            set
            {
                _shortName = value;
            }
        }

        public virtual List<WorkflowTemplate> Workflows
        {
            get
            {
                if (_workflows == null)
                {
                    _workflows = new List<WorkflowTemplate>();
                }
                return _workflows;
            }
            set
            {
                _workflows = value;
            }
        }
        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public NucMedPractice NucMedPractice
        {
            get { return _nucMedPractice; }
            set { _nucMedPractice = value; }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(StudyType);
            }
        }

    }

    public class Workflow : DataStoreItem
    {
        private List<Appointment> _appointments;
        private string _name;
        private NucMedPractice _nucMedPractice;

        public virtual List<Appointment> Appointments
        {
            get
            {
                if (_appointments == null)
                {
                    _appointments = new List<Appointment>();
                }
                return _appointments;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public NucMedPractice NucMedPractice
        {
            get
            {
                return _nucMedPractice;
            }
            set
            {
                _nucMedPractice = value;
            }
        }
    }
}
