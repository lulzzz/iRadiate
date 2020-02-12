using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.DataModel.NucMed
{
    /// <summary>
    /// Represents a role in the workplace which can be filled by an employee.
    /// </summary>
    /// <remarks>
    /// The StaffMemberRole is used to make things more flexible so that tasks can be assigned to a role
    /// and then that role can be dynamically filled by a user. Once a task is completed the user who performed that role
    /// is definitively recorded but before that some internal logic is used to get the user.
    /// </remarks>
    public class StaffMemberRole :DataStoreItem
    {
        private string _name;
        private NucMedPractice _practice;
        private StaffMemberRole _parentRole;
        private List<StaffMemberRole> _childRoles;
        private Room _room;
        
       

        /// <summary>
        /// Gets or sets the description of the staff member role.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual NucMedPractice Practice
        {
            get { return _practice; }
            set { _practice = value; }
        }

        public virtual StaffMemberRole ParentRole
        {
            get { return _parentRole; }
            set { _parentRole = value; }
        }

        public virtual List<StaffMemberRole> ChildRoles
        {
            get {
                if (_childRoles == null)
                {
                    _childRoles = new List<StaffMemberRole>();
                }
                return _childRoles;
            }
            set
            {
                _childRoles = value;
            }
        }

        public virtual Room Room
        {
            get
            {
                return _room;
            }
            set
            {
                _room = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(StaffMemberRole);
            }
        }
        
        
    }

   
}
