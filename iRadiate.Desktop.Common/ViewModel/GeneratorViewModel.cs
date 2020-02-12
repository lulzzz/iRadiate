using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight.Command;

using iRadiate.Common.IO;
using iRadiate.Common;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.Radiopharmacy;

namespace iRadiate.Desktop.Common.ViewModel
{
    public class GeneratorViewModel : RadioactiveInventoryItemViewModel
    {
        private List<IDataStoreItem> _potentialProducts;
        private AsyncObservableCollection<IDataStoreItem> _elutions;
        private double _elutedActivity, _elutedVolume, _breakthrough;
        private DateTime _elutionDate,_elutionExpiryDate;
        private string _elutionBatchNumber;

        #region constructors
        public GeneratorViewModel() : base()
        {
            ElutionDate = DateTime.Now;
            ElutionExpiryDate = DateTime.Now.AddDays(1);
            AddElutionCommand = new RelayCommand(MakeElution);
        }
        public GeneratorViewModel(DataStoreItem item) : base(item)
        {
            foreach(Elution l in ((Generator)item).Elutions)
            {
                Elutions.Add(l);
            }
            (item as Generator).GeneratorEluting += GeneratorEluting;
            (item as Generator).GeneratorEluted += GeneratorEluted;
            ElutionDate = DateTime.Now;
            ElutionExpiryDate = DateTime.Now.AddDays(1);
            AddElutionCommand = new RelayCommand(MakeElution);
        }

        private void GeneratorEluted(object sender, NewDataStoreItemEventArgs e)
        {
            Elutions.Add(e.NewItem as Elution);
            RaisePropertyChanged("Elutions");
        }

        private void GeneratorEluting(object sender, NewDataStoreItemEventArgs e)
        {
            
        }
        #endregion

        #region publicProperties
        /// <summary>
        /// Gets or sets the isotope procuced by this nuclide
        /// </summary>
        public Isotope Daughter
        {
            get
            {
                if(((Generator)Item).ParentRadionuclide != null)
                {
                    return ((Generator)Item).ParentRadionuclide.Daugher;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the parent radionuclide of this generator
        /// </summary>
        public Isotope ParentRadionuclide
        {
            get
            {
                return ((Generator)Item).ParentRadionuclide;
            }
            set
            {
                ((Generator)Item).ParentRadionuclide = value;
                RaisePropertyChanged("ParentRadionuclide");
                RaisePropertyChanged("CurrentActivity");
                refreshPotentialProducts();
            }
        }

        /// <summary>
        /// Gets the current activity of the generator in the parent radionuclide
        /// </summary>
        public override double CurrentActivity
        {
            get
            {
                
                    if (ParentRadionuclide == null)
                    {
                        return 0;
                    }
                    return CalibrationActivity * Math.Exp(-(Math.Log(2) / (ParentRadionuclide.HalfLife / 86400)) * (DateTime.Now - CalibrationDate).TotalDays);
                
            }
        }

        /// <summary>
        /// Gets or sets the product which is eluted from the generator
        /// </summary>
        /// <remarks>
        /// It is awkward that this popoerty is a Radiopharmaceutical because that
        /// term usually refers to drugs wereas generators usually give just the plain isotope.
        /// But since what is milked from a generator can be administered to patients it must
        /// be a Radiopharmaceutical. It would have been more accurate to have a larger data model
        /// that included the different chemical form as distinct from a radiopharmaceutical. But there
        /// is no need for that now.
        /// </remarks>
        public Chemical Product
        {
            get
            {
                return ((Generator)Item).Product;
            }
            set
            {
                ((Generator)Item).Product = value;
                RaisePropertyChanged("Product");
            }
        }

        /// <summary>
        /// Gets the list of products that can be generated from this generator
        /// </summary>
        /// <remarks>
        /// This list is any radiopharmaceutical with the isotope that matches the 
        /// daughter for this generator
        /// </remarks>
        public List<IDataStoreItem> PotentialProducts
        {
            get
            {
                if(Daughter == null) { return null; }
                if(_potentialProducts == null)
                {
                    AsyncObservableCollection<IDataStoreItem> temp = DesktopApplication.Librarian.GetItems(typeof(Chemical), new List<RetrievalCriteria>());
                    _potentialProducts = new List<IDataStoreItem>();
                    foreach(IDataStoreItem i in temp)
                    {
                        if ((i as Chemical).Isotope != null)
                        {
                            if ((i as Chemical).Isotope.ID == Daughter.ID)
                            {
                                _potentialProducts.Add(i);
                            }
                        }
                           
                    }
                    
                }
                return _potentialProducts;
            }
            set
            {
                _potentialProducts = value;
                RaisePropertyChanged("PotentialProducts");
            }
        }

        public AsyncObservableCollection<IDataStoreItem> Elutions
        {
            get
            {
                if(_elutions == null)
                {
                    _elutions = new AsyncObservableCollection<IDataStoreItem>();
                    _elutions._synchronizationContext = Platform.SynchronizationContext;
                }
                return _elutions;
            }
            set
            {
                _elutions = value;
            }
        }

        public RelayCommand AddElutionCommand { get; set; }

        /// <summary>
        /// Gets or sets the activity eluted from the generator
        /// </summary>
        public double ElutedActivity
        {
            get { return _elutedActivity; }
            set { _elutedActivity = value; RaisePropertyChanged("ElutedActivity"); }
        }

        /// <summary>
        /// Gets or sets the volume eluted from the generator
        /// </summary>
        public double ElutedVolume
        {
            get { return _elutedVolume; }
            set { _elutedVolume = value; RaisePropertyChanged("ElutedVolume"); }
        }

        /// <summary>
        /// Gets or sets the breakthrough (in %) from the generator
        /// </summary>
        public double Breakthrough
        {
            get { return _breakthrough; }
            set { _breakthrough = value; RaisePropertyChanged("Breakthrough"); }
        }

        /// <summary>
        /// Gets or sets the date onwhich the generator was eluted
        /// </summary>
        public DateTime ElutionDate
        {
            get { return _elutionDate; }
            set { _elutionDate = value; RaisePropertyChanged("ElutionDate"); }
        }

        public DateTime ElutionExpiryDate
        {
            get { return _elutionExpiryDate; }
            set { _elutionExpiryDate = value; RaisePropertyChanged("ElutionExpiryDate"); }
            
        }

        public String ElutionBatchNumber
        {
            get
            {
                return _elutionBatchNumber;
            }
            set
            {
                _elutionBatchNumber = value; RaisePropertyChanged("ElutionBatchNumber");
            }
        }
        #endregion

        #region privateMethods
        private void MakeElution()
        {
            /*Elution bd = new Elution();
            bd.DateAdded = DateTime.Now;
            bd.CalibrationDate = ElutionDate;
            bd.CalibrationActivity = ElutedActivity;
            bd.Volume = ElutedVolume;
            bd.Breakthrough = Breakthrough;
            bd.Manufacturer = "In-house";
            bd.Generator = (Generator)Item;
            bd.ExpiryDate = ElutionExpiryDate;
            bd.Supplier = "In-house";
            bd.Generator = (Generator)Item;
            bd.Radiopharmaceutical = Product;
            ((Generator)Item).Elutions.Add(bd);
            Elutions.Add(bd);
            RaisePropertyChanged("Elutions");*/
            (Item as Generator).Elute(ElutedVolume, ElutedActivity, Breakthrough, ElutionDate, ElutionExpiryDate, ElutionBatchNumber);

        }

        private void refreshPotentialProducts()
        {
            if(_potentialProducts != null)
            {
                _potentialProducts.Clear();
            }
            
            if(Daughter == null)
            {
                return;
            }
            AsyncObservableCollection<IDataStoreItem> temp = DesktopApplication.Librarian.GetItems(typeof(Chemical), new List<RetrievalCriteria>());
            _potentialProducts = new List<IDataStoreItem>();
            foreach (IDataStoreItem i in temp)
            {
                if ((i as Chemical).Isotope != null)
                {
                    if ((i as Chemical).Isotope.ID == Daughter.ID)
                    {
                        _potentialProducts.Add(i);
                    }
                }

            }
            RaisePropertyChanged("PotentialProducts");
        }
        #endregion

        #region overrides
        public override void SaveItem()
        {
            if (((Generator)Item).Elutions.Where(x => x.ID == 0).Any())
            {
                foreach (Elution u in ((Generator)Item).Elutions.Where(x => x.ID == 0))
                {
                    DesktopApplication.Librarian.SaveItem(u);
                }
            }
            base.SaveItem();
        }
        #endregion
    }
}
