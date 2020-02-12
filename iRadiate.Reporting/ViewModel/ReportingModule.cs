using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Serialization;

using iRadiate.Common.IO;
using iRadiate.Common;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;

namespace Reporting.ViewModel
{
    [PreferredView("Reporting.View.ReportingView","Reporting")]
    public class ReportingModule : Module
    {
        #region privateFields
        private AsyncObservableCollection<IQueryableDataItem> _queryableDataItems;
        private IQueryableDataItem _selectedDataItem;
      
        private ICollectionView _selectedProperties;
        private AsyncObservableCollection<object> _queryResults;
        private string _serializedQuery;
        private Query _query;
        private AsyncObservableCollection<Query> _queries;
        #endregion

        #region constructors
        public ReportingModule() : base()
        {
            
            
        }
        #endregion

        #region publicProperties
        [ImportMany(typeof(IQueryableDataItem))]
        public AsyncObservableCollection<IQueryableDataItem> QueryableDataItems
        {
            get
            {
                if(_queryableDataItems == null)
                {
                    _queryableDataItems = new AsyncObservableCollection<IQueryableDataItem>();

                }
                return _queryableDataItems;
            }
            set { _queryableDataItems = value; RaisePropertyChanged("QueryableDataItems"); }
        }

        public IQueryableDataItem SelectedDataItem
        {
            get
            {
                return _selectedDataItem;
            }
            set
            {
                _selectedDataItem = value;
                if(_selectedDataItem != null)
                {
                    _selectedDataItem.SelectedPropertiesChanged += _selectedDataItem_SelectedPropertiesChanged;                    
                    _query.DataItem = _selectedDataItem.Name;
                }
                SelectedProperties = new CollectionViewSource { Source = SelectedDataItem.QueryableProperties }.View;
                SelectedProperties.Filter = showSelectedOnly;
                RaisePropertyChanged("SelectedDataItem");
            }
        }

        private void _selectedDataItem_SelectedPropertiesChanged(object sender, EventArgs e)
        {
            SelectedProperties.Refresh();
            System.Diagnostics.Debug.WriteLine("_selectedDataItem_SelectedPropertiesChanged!!!");
        }

        public ICollectionView SelectedProperties
        {
            get
            {
                return _selectedProperties;
            }
            set
            {
                _selectedProperties = value;
                RaisePropertyChanged("SelectedProperties");
            }
        }

        
        public AsyncObservableCollection<Object> QueryResults
        {
            get
            {
                if (_queryResults == null)
                    _queryResults = new AsyncObservableCollection<object>();
                return _queryResults;
            }
            set
            {
                

                _queryResults = value;  RaisePropertyChanged("QueryResults"); }
        }

        public string SerializedQuery
        {
            get { return _serializedQuery; }
            set { _serializedQuery = value; RaisePropertyChanged("SerializedQuery"); }
        }

        public Query Query
        {
            get { return _query; }
            set
            {
                _query = value;
               
                if(_query.DataItem != null && _query.DataItem != string.Empty)
                {
                    SelectedDataItem = QueryableDataItems.Where(x => x.Name == _query.DataItem).First();
                    if (_query.QueryProperties.Any())
                    {
                        foreach(var q in _query.QueryProperties)
                        {
                            var p = SelectedDataItem.QueryableProperties.Where(x => x.Name == q.Name).First();
                            p.IsSelected = q.IsSelected;
                            p.IsReturning = q.IsReturning;
                            p.IsFiltering = q.IsFiltering;
                            p.Format = q.Format;
                            p.FilterOperator = q.FilterOperator;
                            p.ColumnOrder = q.ColumnOrder;
                            p.ColumnHeader = q.ColumnHeader;
                            p.FilterValue = q.FilterValue;
                        }
                    }
                }
                if(_query.Code == null || _query.Code == string.Empty)
                {
                    Random random = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    _query.Code = new string(Enumerable.Repeat(chars, 15).Select(s => s[random.Next(s.Length)]).ToArray());
                }
                RaisePropertyChanged("Query");

            }
        }

        public AsyncObservableCollection<Query> Queries
        {
            get
            {
                if (_queries == null)
                    _queries = new AsyncObservableCollection<Query>();

                return _queries;
            }
            set
            {
                _queries = value;
                RaisePropertyChanged("Queries");
            }
        }
        #endregion

        #region overrides

        public override string Name
        {
            get
            {
                return "Reporting";
            }

            set
            {
                base.Name = value;
            }
        }

        public override void GetData()
        {
            ///Here is where I get the list of all reportDataItems defined in the solution
            ///The user clicks one which then presents all the properties by which it can be defined
            base.GetData();
            //QueryableDataItems.Add(new PatientQueryableDataItem());
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            getAllQueries();
        }

        public override ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.FileChart;
                icon.Height = 18;
                icon.Width = 18;
                cc.Content = icon;
                return cc;
            }
        }

        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            ExecuteQueryCommand = new RelayCommand(executeQuery);
            CreateNewQueryCommand = new RelayCommand(createNewQuery);
            SaveQueryCommand = new RelayCommand(saveQuery);
        }
        #endregion

        #region privateMethods
        private bool showSelectedOnly(object item)
        {
            if(item is IQueryableProperty)
            {
                if (((IQueryableProperty)item).IsSelected)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        
        private void executeQuery()
        {
          
           
            if (!validateQuery(Query))
                return;

            

            var result = Platform.Retriever.RetrieveItems(SelectedDataItem.DataStoreItemType);
            foreach(var qq in SelectedDataItem.QueryableProperties.Where(z => z.IsFiltering))
            {
                
                result = result.Where(a => qq.FilterItem(a));
            }
            QueryResults.Clear();
           
            foreach (var r in result)
            {
                
                dynamic x = new ExpandoObject();
                foreach(var q in SelectedDataItem.QueryableProperties.Where(j => j.IsReturning))
                {
                    //System.Diagnostics.Debug.WriteLine("property Isreturning " + q.Name);
                    var val = q.GetPropertyValue(r);
                    if(q.PropertyType == QueryablePropertyType.Number)
                    {
                        ((IDictionary<string, object>)x)[q.Name] = Convert.ToDouble(val);
                    }
                    else
                    {
                        ((IDictionary<string, object>)x)[q.Name] = q.GetPropertyValue(r);
                    }
                    
                }
                QueryResults.Add(x);
                
            }
            
            
        }

        private bool validateQuery(Query q)
        {
            return true;
        }

        private void getAllQueries()
        {
            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(iRadiate.Common.IO.FileUtility.DataDirectory);
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(iRadiate.Common.IO.FileUtility.DataDirectory, "queries"));
            System.IO.DirectoryInfo queryFolder = new System.IO.DirectoryInfo(System.IO.Path.Combine(iRadiate.Common.IO.FileUtility.DataDirectory, "queries"));
            

            XmlSerializer serializer = new XmlSerializer(typeof(Query));
            var files = queryFolder.GetFiles("*.xml");
            foreach(var f in files)
            {
                System.Diagnostics.Debug.WriteLine("file: " +f.FullName);
                StreamReader reader = new StreamReader(f.FullName);
                var qry = (Query)serializer.Deserialize(reader);
                reader.Close();
                Queries.Add(qry);
            }
        }

        private void createNewQuery()
        {
            Query q = Query = new Query();
            q.Name = "Enter Query Name";
            //RaisePropertyChanged("Query.Name");
            q.Description = "Give a meaningful description";
            //RaisePropertyChanged("Query.Description");
            Queries.Add(q);
            Query = q;
        }

        private void saveQuery()
        {
            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(iRadiate.Common.IO.FileUtility.DataDirectory);
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(iRadiate.Common.IO.FileUtility.DataDirectory, "queries"));
            System.IO.DirectoryInfo queryFolder = new System.IO.DirectoryInfo(System.IO.Path.Combine(iRadiate.Common.IO.FileUtility.DataDirectory, "queries"));
            
            StreamWriter writer = new StreamWriter(System.IO.Path.Combine(queryFolder.FullName, Query.Code + ".xml"));
            writer.Write(serializeQuery());
            writer.Close();
           
        }

        private string serializeQuery()
        {
            Query.SetItem(SelectedDataItem);
            XmlSerializer xsSubmit = new XmlSerializer(typeof(Query));
            //var subReq = new MyObject();

            string result = "";
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww, new XmlWriterSettings { Indent = true }))
                {

                    xsSubmit.Serialize(writer, Query);
                    result = sww.ToString(); // Your XML
                }
            }
            SerializedQuery = result;
            return result;
        }
        #endregion

        #region commands
        public RelayCommand ExecuteQueryCommand { get; set; }

        public RelayCommand SaveQueryCommand { get; set; }

        public RelayCommand CreateNewQueryCommand { get; set; }

        public RelayCommand DeleteQueryCommand { get; set; }
        #endregion

       

        
    }


}
