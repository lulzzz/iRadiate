using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.HealthCare
{
    /// <summary>
    /// Represent either a referring or reporting doctor
    /// </summary>
    public class Doctor : Person
    {
       
        private bool _referrer;
        private bool _reporter;
        private string _emailAddress;
        private string _practiceAddress;
        private string _practiceName;
        private string _providerNumber;

        public Doctor() : base()
        {
            _referrer = false;
            _reporter = false;
        }
        /// <summary>
        /// True if this doctor is a referrer
        /// </summary>
        public bool Referrer
        {
            get { return _referrer; }
            set { _referrer = value; }
        }

        /// <summary>
        /// True if this doctor is a rpeorter
        /// </summary>
        public bool Reporter
        {
            get { return _reporter; }
            set { _reporter = value; }
        }

        /// <summary>
        /// Email address used for contacting Dr as either report or referrer.
        /// </summary>
        /// <remarks>
        /// We make no allowance for the possibility of one Dr having multiple contact addresses.
        /// </remarks>
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        /// <summary>
        /// Gets or sets the practice address of this doctor
        /// </summary>
        [Auditable]
        public string PracticeAddress
        {
            get { return _practiceAddress; }
            set { _practiceAddress = value; }
        }

        /// <summary>
        /// The name of the practice
        /// </summary>
        /// <remarks>
        /// When a doctor has multiple practices, they will need to be entered mulitple times.
        /// </remarks>
        [Auditable]
        public string PracticeName
        {
            get { return _practiceName; }
            set { _practiceName = value; }
        }

        /// <summary>
        /// Returns the doctor's ful name with practice name appended
        /// </summary>
        public override string FullName
        {
            get
            {
                return base.FullName + " " + PracticeName;
            }
        }

        /// <summary>
        /// Returns the doctor's full name with Title with practice name appended
        /// </summary>
        public override string FullNameWithTitle
        {
            get
            {
                return base.FullNameWithTitle + " " + PracticeName;
            }
        }

        /// <summary>
        /// Gets or set the doctor's provider number
        /// </summary>
        [Auditable]
        public string ProviderNumber
        {
            get { return _providerNumber; }
            set { _providerNumber = value; }
        }
    }
}
