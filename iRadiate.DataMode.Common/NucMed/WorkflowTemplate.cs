using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.NucMed
{
    public class WorkflowTemplate : DataStoreItem
    {
        private string _name;
        private Appointment _appointment;
        public StudyType _studyType;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                
            }
        }

        public virtual Appointment Appointment
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

        public virtual StudyType StudyType
        {
            get
            {
                return _studyType;
            }
            set
            {
                _studyType = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(WorkflowTemplate);
            }
        }
    }
}
