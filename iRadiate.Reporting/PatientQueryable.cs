using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;

namespace Reporting
{
    [Export(typeof(IQueryableDataItem))]
    public class PatientQueryableDataItem:BaseQueryableDataItem
    {
        [ImportMany(typeof(IPatientQueryableProperty))]
        private List<IPatientQueryableProperty> patientProperties;

        public PatientQueryableDataItem() : base()
        {
            ///NumberOfStudiesProperty b = new NumberOfStudiesProperty();

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            foreach(var p in patientProperties)
            {
                p.IsSelectedChanged += S_IsSelectedChanged;
                QueryableProperties.Add(p);
            }
            
            
        }

        
        public override string Name
        {
            get
            {
                return "Patient";
            }
        }

        public override Type DataStoreItemType
        {
            get
            {
                return typeof(Patient);
            }
        }


    }

    public interface IPatientQueryableProperty : IQueryableProperty
    {

    }

    public class BaseQueryablePatientProperty : StandardQueryableProperty, IPatientQueryableProperty
    {
        public BaseQueryablePatientProperty(string name, QueryablePropertyType type, string description) : base(name,type,description)
        {
            
        }
    }
    
    [Export(typeof(IPatientQueryableProperty))]
    public class NumberOfStudiesProperty : BaseQueryablePatientProperty
    {
        public NumberOfStudiesProperty() : base("NumberOfStudies",QueryablePropertyType.Number,"The number of studies the patient has had")
        {

        }
        public override object GetPropertyValue(IDataStoreItem item)
        {
            if(item is Patient)
            {
                var p = (Patient)item;
                return p.Studies.Where(x=>x.IsCancelled==false).Count();
            }
            else
            {
                return 0;
            }
        }
    }

    
    

}
