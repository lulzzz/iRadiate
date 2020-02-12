using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.Interfaces.CentricityInterface
{
    public class DoctorBridge
    {
        private string _foreignKey;
        private string _foreignName;
        private int _localKey;
        private Doctor _doctor;

        public string ForeignKey
        {
            get
            {
                return _foreignKey;
            }
            set
            {
                _foreignKey = value;
            }
        }

        public string ForeignName
        {
            get
            {
                return _foreignName;
            }
            set
            {
                _foreignName = value;
            }
        }

        public int LocalKey
        {
            get
            {
                return _localKey;
            }
            set
            {
                _localKey = value;
            }
        }

        public Doctor Doctor
        {
            get
            {
                return _doctor;
            }
            set
            {
                _doctor = value;
            }
        }
    }
}
