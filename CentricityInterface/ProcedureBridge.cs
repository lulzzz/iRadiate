using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.NucMed;

namespace iRadiate.Interfaces.CentricityInterface
{
    public class ProcedureBridge
    {
        private string _foreignKey;
        private int _localID;
        private Study _study;
        private Appointment _appointment;
        private int _appointmentID;

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

        public int LocalID
        {
            get
            {
                return _localID;
            }
            set
            {
                _localID = value;
            }
        }

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

        
    
        
    }
}
