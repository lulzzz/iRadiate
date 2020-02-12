using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common.ViewModel
{
    [PreferredView("iRadiate.Desktop.Common.View.PatientImageView", "iRadiate.Desktop.Common")]
    public class PatientImageViewModel : DataStoreItemViewModel
    {
        private List<ImageProperty> _imageProperties;

        public PatientImageViewModel(DataStoreItem item) : base(item)
        {
            
                Type t = item.GetType();
                foreach(PropertyInfo p in t.GetProperties())
                {
                     
                        
                            ImageProperty ii = new ImageProperty();
                            ii.PropertyName = Regex.Replace(p.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
                            //ii.PropertyName = p.Name;
                            object o = p.GetValue(item);
                            if(o != null)
                            {
                                ii.PropertyValue = o.ToString();
                            }
                            

                            ImageProperties.Add(ii);
                       
                        
                    
                }
           

        }

        public List<ImageProperty> ImageProperties
        {
            get
            {
                if (_imageProperties == null)
                    _imageProperties = new List<ImageProperty>();
                return _imageProperties;
            }
            set
            {
                _imageProperties = value;
                RaisePropertyChanged("ImageProperties");
            }
        }
        public class ImageProperty
        {
            public ImageProperty()
            {

            }
            public string PropertyName { get; set; }
            public string PropertyValue { get; set; }
        }

    }
}
