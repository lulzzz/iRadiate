using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.DataModel.NucMed
{
    public class NucMedPractice : DataStoreItem
    {
        private Hospital _hospital;
        private List<Room> _rooms;
        private List<StaffMemberRole> _roles;
        private List<StudyType> _studyTypes;
        private string _name;
        private List<Doctor> _doctors;
        private List<User> _employees;
        public NucMedPractice() : base()
        {

        }
        public virtual List<StaffMemberRole> Roles
        {
            get
            {
                if(_roles == null)
                {
                    _roles = new List<StaffMemberRole>();
                }

                return _roles;
            }
            set
            {
                _roles = value;
            }
        }

        public virtual List<Room> Rooms
        {
            get 
            {
                if (_rooms == null)
                {
                    _rooms = new List<Room>();
                }
                return _rooms; 
            }
            set { _rooms = value; }
        }

        public List<Doctor> Doctors
        {
            get
            {
                if(_doctors == null)
                {
                    _doctors = new List<Doctor>();
                }
                return _doctors;
            }
            set
            {
                _doctors = value;
            }
        }

        public List<User> Employees
        {
            get
            {
                if(_employees == null)
                {
                    _employees = new List<User>();
                }
                return _employees;
            }
            set
            {
                _employees = value;
            }
        }
        public virtual Hospital Hospital
        {
            get { return _hospital; }
            set { _hospital = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual List<StudyType> StudyTypes
        {
            get
            {
                if (_studyTypes == null)
                {
                    _studyTypes = new List<StudyType>();
                }
                return _studyTypes;
            }
            set
            {
                _studyTypes = value;
            }
        }
    }
}
