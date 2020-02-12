using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.DataModel.Common
{

    /// <summary>
    /// Represents a street address
    /// </summary>
    [Obsolete]
    public class Address : DataStoreItem
    {
        private string _streetNumber;
        private string _unitNumber;
        private string _streetName;
        private string _streetType;
        private Town _town;
        private Guid? _streetTypeId;

        public Address()
        {

        }

        public Address(string streetNumber, string unitNumber, string streetName, string streetType, Town town)
        {
            _streetName = streetName;
            _streetNumber = streetNumber;
            _unitNumber = unitNumber;
            _streetType = streetType;
            _town = town;
        }
        
        public string StreetNumber
        {
            get { return _streetNumber; }
            set { _streetNumber = value; }
        }

        public string UnitNumber
        {
            get { return _unitNumber; }
            set { _unitNumber = value; }
        }

        public string StreetName
        {
            get { return _streetName; }
            set { _streetName = value; }
        }

        public Guid? StreetTypeId
        {
            get { return _streetTypeId; }
            set { _streetTypeId = value; }
        }

        public string StreetType
        {
            get { return _streetType; }
            set { _streetType = value; }
        }

        

        public virtual Town Town
        {
            get { return _town; }
            set { _town = value; }
        }

        public virtual String GetFullAddress()
        {
            if (string.IsNullOrWhiteSpace(_unitNumber))
            {
                return _streetNumber + " "+ _streetName + " " + _streetType + Environment.NewLine + Town.Name + Environment.NewLine + Town.Province.Name + " " + Town.PostCode + Environment.NewLine + Town.Province.Country.Name;
            }
            else
            {
                return _unitNumber + " / " +_streetNumber + " " + _streetName + " " + _streetType + Environment.NewLine + Town.Name + Environment.NewLine + Town.Province.Name + " " + Town.PostCode + Environment.NewLine + Town.Province.Country.Name;
            }
        }
    }

    /// <summary>
    /// Represents a postal address.
    /// </summary>
    [Obsolete]
    public class PostalAddress : Address
    {
        private bool _isPOBox;
        private string _pOBoxNumber;

        public bool IsPOBox
        {
            get { return _isPOBox; }
            set { _isPOBox = value; }
        }

        public string POBoxNumber
        {
            get { return _pOBoxNumber; }
            set { _pOBoxNumber = value; }
        }
        public override string GetFullAddress()
        {
            if (_isPOBox)
            {
                return "PO Box " + _pOBoxNumber + Environment.NewLine + Town.Name + Environment.NewLine + Town.Province.Name + " " + Town.PostCode + Environment.NewLine + Town.Province.Country.Name;
            }
            return base.GetFullAddress();
        }
    }

    /// <summary>
    /// Represents a town or suburb.
    /// </summary>
    [Obsolete]
    public class Town : DataStoreItem
    {
        private string _name;
        private string _postCode;
      
        private Province _province;
        private double _longitude;
        private double _latitude;

        /// <summary>
        /// Gets or sets the province the town is in.
        /// </summary>
        public virtual Province Province
        {
            get { return _province; }
            set { _province = value; }
        }

       
        /// <summary>
        /// Gets or sets the postcode of the town.
        /// </summary>
        public string PostCode
        {
            get
            {
                return _postCode;
            }
            set
            {
                _postCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the town.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The longitude of the town
        /// </summary>
        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        /// <summary>
        /// The latitude of the town
        /// </summary>
        public double Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        public string FullName
        {
            get
            {
                return Name + " " + Province.Abbreviation + " " + PostCode + " " + Province.Country.Name;
            }
        }
    }

    /// <summary>
    /// Represents a province or state.
    /// </summary>
    [Obsolete]
    public class Province : DataStoreItem
    {
        private String _name;
        private String _abbreviation;
        private Country _country;
       
        private ICollection<Town> _towns;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public String Abbreviation
        {
            get { return _abbreviation; }
            set { _abbreviation = value; }
        }

        
        public virtual Country Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public virtual ICollection<Town> Towns
        {
            get
            {
                if (_towns == null)
                {
                    _towns = new List<Town>();
                }
                return _towns;
            }
            set
            {
                _towns = value;
            }
        }
    }

    /// <summary>
    /// Represents a country.
    /// </summary>
    [Obsolete]
    public class Country : DataStoreItem
    {
        private String _name;
        private String _abbreviation;
        private ICollection<Province> _provinces;

        public Country()
        {

        }

        public Country(string name, string abbreviation)
        {
            _name = name;
            _abbreviation = abbreviation;
        }
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public String Abbreviation
        {
            get { return _abbreviation; }
            set { _abbreviation = value; }
        }

        public virtual ICollection<Province> Provinces
        {
            get
            {
                if (_provinces == null)
                {
                    _provinces = new List<Province>();
                }
                return _provinces; 
            }
            set { _provinces = value; }
        }
    
    }
}
