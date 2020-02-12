using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.Common
{
    public interface IPerson
    {
        string GivenNames { get; set; }

        string Surname { get; set; }

        string Title { get; set; }

        string FullName { get; }

        string FullNameWithTitle { get; }

        Gender Gender { get; set; }

        DateTime DateOfBirth { get; set; }

        int Age { get; }

    }

    /// <summary>
    /// Three genders are enumerated as per the DICOM standard
    /// </summary>
    public enum Gender
    {
        Male,
        Female,
        Other,
    }

    /// <summary>
    /// Represents a real life person.
    /// </summary>
    public abstract class Person :  DataStoreItem, IPerson
    {
        private string _givenNames;
        private string _surname;
        private string _title;
        private Gender _gender;
        private DateTime _dateOfBirth;

        public Person(): base()
        {

        }

        /// <summary>
        /// Gets or sets the person's given names.
        /// </summary>
        [Auditable]
        [Queryable]
        public string GivenNames
        {
            get 
            {
                if (_givenNames != null)
                { return _givenNames; }
                else
                {
                    return "";
                }
            }
            set { _givenNames = value; }
        }

        /// <summary>
        /// Gets or sets the person's surname.
        /// </summary>
        [Auditable]
        [Queryable]
        public string Surname
        {
            get { return _surname; }
            set { _surname = value; }
        }

        /// <summary>
        /// Gets or sets the patient's title.
        /// </summary>
        [Auditable]
        [Queryable]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Gets or sets the person's gender
        /// </summary>
        /// <remarks>
        /// Gender choices are Male, Female, Other.
        /// </remarks>
        [Queryable]
        public Gender Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        /// <summary>
        /// Gets or sets the person's date of birth.
        /// </summary>
        [Auditable]
        [Queryable]
        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set { _dateOfBirth = value; }
        }

        /// <summary>
        /// Gets the patients current age.
        /// </summary>
        [Queryable]
        public int Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - _dateOfBirth.Year;
                if (_dateOfBirth > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>
        /// Gets the person's age at a specific date.
        /// </summary>
        /// <param name="d">The date on which the age is calculated.</param>
        /// <returns>The person's age in years.</returns>
        public int AgeAt(DateTime d)
        {
            
            int age = d.Year - _dateOfBirth.Year;
            if (_dateOfBirth > d.AddYears(-age)) age--;
            return age;
        }

        /// <summary>
        /// Gets the parson's full name.
        /// </summary>
        [Queryable]
        public virtual string FullName
        {
            get
            {
                return GivenNames + " " + Surname;
            }
            
        }

        /// <summary>
        /// Gets the person's full name with title.
        /// </summary>
        public virtual string FullNameWithTitle
        {
            get
            {
                return Title + " " + GivenNames + " " + Surname; ;
            }
            
        }

        public override string ToString()
        {
            return FullNameWithTitle;
        }
    }
}
