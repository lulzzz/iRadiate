using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common
{
    //Part of the scan bag
    public class DataProviderParameter
    {
        public string Description { get; set; }
        public bool IsFormattable { get; set; }
    }

    //This is used by the scanbag somewhere
    public interface IStudyDataProvider
    {
        void SetStudy(Study s);

        object GetData();

        string Name { get; }

        bool IsParamaterized { get; }

        List<DataProviderParameter> Paramaters { get; }

        DataProviderParameter Parameter { get; set; }

        bool IsFormattable { get; }

    }

    [Obsolete("I don't know what this is for")]
    public class BaseStudyDataProvider : IStudyDataProvider
    {
        protected Study _study;
        protected DataProviderParameter _parameter;
        protected List<DataProviderParameter> _parameters;
        
        public virtual void SetStudy(Study s)
        {
            _study = s;
        }

        public virtual object GetData()
        {
            throw new NotImplementedException();
        }

        public virtual string Name
        {
            get { throw new NotImplementedException(); }
        }


        public virtual bool IsParamaterized
        {
            get { throw new NotImplementedException(); }
        }

        public virtual List<DataProviderParameter> Paramaters
        {
            get { throw new NotImplementedException(); }
        }

        public virtual DataProviderParameter Parameter
        {
            get
            {
                return _parameter;
            }
            set
            {
                _parameter = value;
            }
        }

        public bool IsFormattable
        {
            get
            {
                if (_parameter == null)
                {
                    return false;
                }
                return _parameter.IsFormattable;
            }
        }


    }

    [Obsolete("I don't know what this is for")]
    [Export(typeof(IStudyDataProvider))]
    public class PatientDetailsDataProvider : BaseStudyDataProvider
    {
        

        public override string Name
        {
            get
            {
                return "Patient details";
            }
        }

        public override bool IsParamaterized
        {
            get
            {
                return true;
            }
        }

       
       
        public override List<DataProviderParameter> Paramaters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new List<DataProviderParameter>();
                    _parameters.Add(new DataProviderParameter {Description = "Age", IsFormattable=false});
                    _parameters.Add(new DataProviderParameter { Description = "Date of birth", IsFormattable = true });
                    _parameters.Add(new DataProviderParameter { Description = "Title", IsFormattable = false });
                    _parameters.Add(new DataProviderParameter { Description = "Given names", IsFormattable = false });
                    _parameters.Add(new DataProviderParameter { Description = "Surname", IsFormattable = false });
                    _parameters.Add(new DataProviderParameter { Description = "Full name with title", IsFormattable = false });
                    _parameters.Add(new DataProviderParameter { Description = "Age at study date", IsFormattable = false });
                    _parameters.Add(new DataProviderParameter { Description = "Sex", IsFormattable = false });
                    _parameters.Add(new DataProviderParameter { Description = "MRN", IsFormattable = false });
                    

                }
                return _parameters;
            }
        }

        public override object GetData()
        {
            if (_parameter == null)
            {
                return null;
            }
            if (_parameter.Description == "Age")
            {
                return _study.Patient.Age;
            }
            if (_parameter.Description == "Date of birth")
            {
                return _study.Patient.DateOfBirth;
            }
            if (_parameter.Description == "Title")
            {
                return _study.Patient.Title;
            }
            if (_parameter.Description == "Given names")
            {
                return _study.Patient.GivenNames;
            }
            if (_parameter.Description == "Surname")
            {
                return _study.Patient.Surname;
            }
            if (_parameter.Description == "Full name with title")
            {
                return _study.Patient.FullNameWithTitle;
            }
            if (_parameter.Description == "Age at study date")
            {
                return _study.Patient.AgeAt(_study.Date);
            }
            if (_parameter.Description == "Sex")
            {
                return _study.Patient.Gender;
            }
            if (_parameter.Description == "MRN")
            {
                return _study.Patient.MRN;
            }
            return null;
        }

    }

    
}
