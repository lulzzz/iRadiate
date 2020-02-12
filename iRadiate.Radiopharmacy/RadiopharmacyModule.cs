using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;


using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Desktop.Common;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.Radiopharmacy;

namespace iRadiate.Radiopharmacy
{
    [PreferredView("iRadiate.Radiopharmacy.View.RadiopharmacyView", "iRadiate.Radiopharmacy")]
    public class RadiopharmacyModule : Module
    {

        #region privateFields
        private AsyncObservableCollection<IDataStoreItem> _inventoryItems;
        private AsyncObservableCollection<IDataStoreItem> _radiopharmaceuticals;
        private AsyncObservableCollection<IDataStoreItem> _kitDefinitions;

        private IList _selectedInventoryItems;
        private IDataStoreItem _selectedInventoryItem;
        private IDataStoreItem _selectedChemical;
        private IDataStoreItem _selectedKitDefinition;
        private string _selectedInventoryType;
        private ICollectionView _inventoryView;

        private bool _infoButtonEnabled,_chemicalInfoButtonEnabled, _reconstitutionButtonEnabled,_disposeButtonEnabled, _drawDoseButtonEnabled;
        private bool _milkGeneratorButtonEnabled, _printLabelButtonEnabled, _returnItemButtonEnabled, _unassignUnitDoseButtonEnabled;
        private bool _showExpired;
        private bool _showAll, _showGenerators, _showBulkDoses, _showUnitDoses, _showColdKits, _showDisposed;
        private DateTime _generatorDateMinimum, _bulkDoseDateMinimum, _unitDoseDateMinimum, _coldKitDateMinimum;

        private bool _dontFilter = false;
        private double _disposableActivity;
        private  int _elutionsToday;
        private int _coldKitsToday;
        private double _activityOfElutionsToday;
        private double _activityOfColdKitsToday;
        private int _numerOfUnitDosesToday;
        private double _activityOfUnitDosesToday;
        private int _disposableItems;
        private AsyncObservableCollection<IsotopeLevel> _isotopeLevels;
        private int _numberOfExpiredItems;
        private double _activityOfExpiredItems;
        #endregion

        #region constructor
        public RadiopharmacyModule():base()
        {
            //comands
            AddItemCommand = new RelayCommand(addNewInventoryItem);
            ViewInfoCommand = new RelayCommand(ViewItem);
            AddRadiopharmaceuticalCommand = new RelayCommand(addNewRadiopharmaceutical);
            ViewChemicalCommand = new RelayCommand(viewChemical);
            LaunchReconstitutionCommand = new RelayCommand(LaunchReconstitution);
            DisposeItemCommand = new RelayCommand(DisposeItem);
            UnDisposeItemCommand = new RelayCommand(UnDiposeItem);
            MilkGeneratorCommand = new RelayCommand(MilkGenerator);
            DrawDoseCommand = new RelayCommand(DrawDose);
            PrintLabelCommand = new RelayCommand(PrintLabel);
            AddKitDefinitionCommand = new RelayCommand(addKitDefinition);
            ViewKitDefinitionCommand = new RelayCommand(viewKitDefinition);
            ReturnItemCommand = new RelayCommand(ReturnToManufacturer);
            RefreshInventoryCommand = new RelayCommand(reloadData);
            UnassignUnitDoseCommand = new RelayCommand(UnassignUnitDose);

            _showExpired = true;
            _selectedInventoryType = "Nothing";
            _showAll = true;
            _chemicalInfoButtonEnabled = false;
            _reconstitutionButtonEnabled = false;
            _disposeButtonEnabled = false;
            _showDisposed = false;
            _showGenerators = true;
            _showUnitDoses = true;
            _showBulkDoses = true;
            _showColdKits = true;

            
            _generatorDateMinimum = DateTime.Today.AddDays(-14);
            _bulkDoseDateMinimum = DateTime.Today.AddDays(-3);
            _unitDoseDateMinimum = DateTime.Today.AddDays(-1);
            _coldKitDateMinimum = DateTime.Today.AddMonths(-12);

            
        }
        #endregion

        #region overrides
        public override ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Pharmacy;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }

        public override void UIThreadInitialize()
        {
            base.UIThreadInitialize();
            InventoryView = CollectionViewSource.GetDefaultView(InventoryItems);
            InventoryView.Filter = inventoryItemFilter;
            InventoryView.GroupDescriptions.Add(new PropertyGroupDescription("TypeName"));
            resetView();
        }
        public override string IconSource
        {
            get
            {
                return "/iRadiate.Radiopharmacy;component/Images/RadiopharmacyIcon.png";
            }

            set
            {
                base.IconSource = value;
            }
        }

        public override string Name
        {
            get
            {
                return "Radiopharmacy";
            }

            set
            {
                base.Name = value;
            }
        }

        public override void GetData()
        {
            base.GetData();
            KitDefinitions = DesktopApplication.Librarian.GetItems(typeof(KitDefinition), new List<RetrievalCriteria>());
            Radiopharmaceuticals = DesktopApplication.Librarian.GetItems(typeof(Chemical), new List<RetrievalCriteria>());

            RetrievalCriteria rcMin = new RetrievalCriteria("DateAdded", CriteraType.GreaterThan, BulkDoseDateMinimum);
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rcMin);
            InventoryItems = DesktopApplication.Librarian.GetItems(typeof(BaseBulkDose), rcList);
            
           
            
            
            foreach (IDataStoreItem d in InventoryItems)
            {
                (d as BaseBulkDose).UnitDoseDrawn += InventoryItemAdded;
                if (d is Elution && (d as Elution).CalibrationDate.Date == DateTime.Today)
                {
                    //&& (d as Elution).CalibrationDate.Date == DateTime.Today
                    NumberOfElutionsToday++;
                    ActivityOfElutionsToday = ActivityOfElutionsToday + ((d as Elution).CurrentActivity);
                    if((d as IInventory).Expired)
                    {
                        DisposableItems++;
                        DisposableActivity += (d as BaseRadioactiveInventoryItem).CurrentActivity;
                    }
                }
                else if(d is ReconstitutedColdKit)
                {
                    NumberOfColdKitsToday++;
                    ActivityOfColdKitsToday += (d as ReconstitutedColdKit).CurrentActivity;
                    if ((d as IInventory).Expired)
                    {
                        DisposableItems++;
                        DisposableActivity += (d as BaseRadioactiveInventoryItem).CurrentActivity;
                    }
                }
            }

            rcMin = new RetrievalCriteria("DateAdded", CriteraType.GreaterThan, UnitDoseDateMinimum);
            rcList.Clear();
            rcList.Add(rcMin);
            foreach (IDataStoreItem d in DesktopApplication.Librarian.GetItems(typeof(BaseUnitDose), rcList))
            {
                InventoryItems.Add(d);
                NumberOfUnitDosesToday++;
                ActivityOfUnitDosesToday += (d as BaseUnitDose).CurrentActivity;
                if ((d as IInventory).Expired)
                {
                    DisposableItems++;
                    DisposableActivity += (d as BaseRadioactiveInventoryItem).CurrentActivity;
                }
            }
            rcMin = new RetrievalCriteria("DateAdded", CriteraType.GreaterThan, GeneratorDateMinimum);
            rcList.Clear();
            rcList.Add(rcMin);
            foreach (IDataStoreItem d in DesktopApplication.Librarian.GetItems(typeof(Generator), rcList))
            {
                
                InventoryItems.Add(d);
                (d as Generator).GeneratorEluted += InventoryItemAdded;
            }

            rcMin = new RetrievalCriteria("DateAdded", CriteraType.GreaterThan, ColdKitDateMinimum);
            rcList.Clear();
            rcList.Add(rcMin);
            foreach (IDataStoreItem d in DesktopApplication.Librarian.GetItems(typeof(Kit), rcList))
            {
                InventoryItems.Add(d);
                (d as Kit).ColdKitReconstituted += InventoryItemAdded;
            }

            foreach(IDataStoreItem d in InventoryItems)
            {
                (d as IInventory).ItemDisposed += RadiopharmacyModule_ItemDeleted;
                (d as IInventory).ItemUnDisposed += RadiopharmacyModule_ItemUnDisposed;
            }

            
            var isotopes = Platform.Retriever.RetrieveItems(typeof(Isotope),new List<RetrievalCriteria>());
            System.Diagnostics.Debug.WriteLine("Isotopes.count = " + isotopes.Count);
            foreach(var i in isotopes)
            {
                
                IsotopeLevel lvl = new IsotopeLevel();
                lvl.Isotope = (Isotope)i;
                lvl.CurrentActivity = InventoryItems.Where(x => x is BaseRadioactiveInventoryItem).Where(y => (y as BaseRadioactiveInventoryItem).Disposed == DisposalStatus.NotDisposed && (y as BaseRadioactiveInventoryItem).Isotope == i).Sum(z => (z as BaseRadioactiveInventoryItem).CurrentActivity);
                if(lvl.CurrentActivity > 0)
                    IsotopeLevels.Add(lvl);
                
            }
            try
            {
                var a = Platform.Retriever.RetrieveItems(typeof(BaseUnitDose)).Where(x=>(x as BaseUnitDose).Disposed == DisposalStatus.NotDisposed && (x as BaseUnitDose).Expired == true);
                var b = Platform.Retriever.RetrieveItems(typeof(BaseBulkDose)).Where(x => (x as BaseBulkDose).Disposed == DisposalStatus.NotDisposed && (x as BaseBulkDose).Expired == true);
                var c = Platform.Retriever.RetrieveItems(typeof(Generator)).Where(x => (x as BaseBulkDose).Disposed == DisposalStatus.NotDisposed && (x as BaseBulkDose).Expired == true);

                NumberOfExpiredItems = a.Count();
                NumberOfExpiredItems += b.Count();
                NumberOfExpiredItems += c.Count();

                ActivityOfExpiredItems = a.Select(x => (x as BaseRadioactiveInventoryItem).CurrentActivity).Sum();
                ActivityOfExpiredItems += b.Select(x => (x as BaseRadioactiveInventoryItem).CurrentActivity).Sum();
                ActivityOfExpiredItems += c.Select(x => (x as BaseRadioactiveInventoryItem).CurrentActivity).Sum();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception:  " + ex.Message);
            }
        }

        private void RadiopharmacyModule_ItemUnDisposed(object sender, EventArgs e)
        {
            resetView();
        }

        private void RadiopharmacyModule_ItemDeleted(object sender, EventArgs e)
        {
            //Refresh?
            resetView();
        }

        protected override void SetRelayCommands()
        {
           
        }


        #endregion

        #region publicProperties
        public AsyncObservableCollection<IDataStoreItem> InventoryItems
        {
            get
            {
               
                return _inventoryItems;
            }
            set
            {
                _inventoryItems = value;
               
            }
        }

        public IDataStoreItem SelectedInventoryItem
        {
            get
            {
                return _selectedInventoryItem;
            }
            set
            {
                _selectedInventoryItem = value;
                RaisePropertyChanged("SelectedInventoryItem");
                SelectedInventoryItemChanged();
            }
        }

        public IList SelectedInventoryItems
        {
            get
            {
                return _selectedInventoryItems;
            }
            set
            {
                _selectedInventoryItems = value;
                RaisePropertyChanged("SelectedInventoryItems");
            }
        }

        public IDataStoreItem SelectedChemical
        {
            get { return _selectedChemical; }
            set { _selectedChemical = value; RaisePropertyChanged("SelectedChemical"); SelectedChemicalChanged(); }
        }

        public IDataStoreItem SelectedKitDefinition
        {
            get { return _selectedKitDefinition; }
            set { _selectedKitDefinition = value; }
        }

        public AsyncObservableCollection<IDataStoreItem> Radiopharmaceuticals
        {
            get
            {
                return _radiopharmaceuticals;
            }
            set
            {
                _radiopharmaceuticals = value;
            }
        }

        public AsyncObservableCollection<IDataStoreItem> KitDefinitions
        {
            get { return _kitDefinitions; }
            set { _kitDefinitions = value; }
        }

        public List<string> InventoryItemTypes
        {
            get
            {
                return new List<string>() { "Generator", "Bulk Tc99m", "Reconstituted Kit" , "Unit Dose", "Cold Kit", "Capsule", "Split Unit Dose"};
            }
        }

        public string SelectedInventoryType
        {
            get
            {
                return _selectedInventoryType;
            }
            set
            {
                _selectedInventoryType = value;
                RaisePropertyChanged("SelectedInventoryType");
            }
        }

        public bool InfoButtonEnabled
        {
            get
            {
                return _infoButtonEnabled;
            }
            set
            {
                _infoButtonEnabled = value;
                RaisePropertyChanged("InfoButtonEnabled");
            }
        }

        public bool ChemicalInfoButtonEnabled
        {
            get { return _chemicalInfoButtonEnabled; }
            set { _chemicalInfoButtonEnabled = value; RaisePropertyChanged("ChemicalInfoButtonEnabled"); }
        }

        public bool ReconstitutionButtonEnabled
        {
            get
            {
                return _reconstitutionButtonEnabled;
            }
            set
            {
                _reconstitutionButtonEnabled = value;
                RaisePropertyChanged("ReconstitutionButtonEnabled");

            }
        }

        public bool DisposeButtonEnabled
        {
            get { return _disposeButtonEnabled; }
            set { _disposeButtonEnabled = value; RaisePropertyChanged("DisposeButtonEnabled"); RaisePropertyChanged("UndisposeButtonEnabled"); }
        }

        public bool ReturnItemButtonEnabled
        {
            get { return _returnItemButtonEnabled; }
            set { _returnItemButtonEnabled = value;  RaisePropertyChanged("ReturnItemButtonEnabled"); }
        }

        public bool UnDisposeButtonEnabled
        {
            get
            {
                if (SelectedInventoryItem == null)
                {
                    return false;
                }
                return !DisposeButtonEnabled;
            }
        }

        public bool MilkGeneratorButtonEnabled
        {
            get { return _milkGeneratorButtonEnabled; }
            set { _milkGeneratorButtonEnabled = value; RaisePropertyChanged("MilkGeneratorButtonEnabled"); }
        }

        public bool DrawDoseButtonEnabled
        {
            get { return _drawDoseButtonEnabled;}
            set
            {
                _drawDoseButtonEnabled = value;
                RaisePropertyChanged("DrawDoseButtonEnabled");
            }
        }
        
        public bool PrintLabelButtonEnabled
        {
            get { return _printLabelButtonEnabled; }
            set { _printLabelButtonEnabled = value;  RaisePropertyChanged("PrintLabelButtonEnabled"); }
        }

        public bool UnassignUnitDoseButtonEnabled
        {
            get { return _unassignUnitDoseButtonEnabled; }
            set { _unassignUnitDoseButtonEnabled = value;  RaisePropertyChanged("UnassignUnitDoseButtonEnabled"); }
        }

        public bool ShowExpired
        {
            get
            {
                return _showExpired;
            }
            set
            {
                _showExpired = value;
                RaisePropertyChanged("ShowExpired");
                resetView();
            }
        }

        public bool ShowDisposed
        {
            get
            {
                return _showDisposed;
            }
            set
            {
                _showDisposed = value;
                RaisePropertyChanged("ShowDisposed");
                resetView();
            }
        }

        public bool ShowAll
        {
            get
            {
                return _showAll;
            }
            set
            {
                if (value)
                {

                    ShowGenerators = true;
                    ShowBulkDoses = true;
                    ShowUnitDoses = true;
                    ShowColdKits = true;
                }
                _showAll = value;
                RaisePropertyChanged("ShowAll");
                resetView();
            }
        }

        public bool ShowGenerators
        {
            get
            {
                return _showGenerators;
            }
            set
            {
                if(!value && ShowAll)
                {
                    ShowAll = false;
                }
                _showGenerators = value;
                RaisePropertyChanged("ShowGenerators");
                resetView();
            }
        }

        public bool ShowBulkDoses
        {
            get
            {
                return _showBulkDoses;
            }
            set
            {
                if (!value && ShowAll)
                {
                    ShowAll = false;
                }
                _showBulkDoses = value;
                RaisePropertyChanged("ShowBulkDoses");
                resetView();
            }
        }

        public bool ShowUnitDoses
        {
            get
            {
                return _showUnitDoses;
            }
            set
            {
                if (!value && ShowAll)
                {
                    ShowAll = false;
                }
                _showUnitDoses = value;
                RaisePropertyChanged("ShowUnitDoses");
                resetView();
            }
        }

        public bool ShowColdKits
        {
            get
            {
                return _showColdKits;                
            }
            set
            {
                if (!value && ShowAll)
                {
                    ShowAll = false;
                }
                _showColdKits = value;
                RaisePropertyChanged("ShowColdKits");
                resetView();
            }
        }
        public ICollectionView InventoryView
        {
            get
            {
                return _inventoryView;
            }
            set
            {
                _inventoryView = value;
                RaisePropertyChanged("InventoryView");
            }
        }

        public DateTime GeneratorDateMinimum
        {
            get { return _generatorDateMinimum; }
            set { _generatorDateMinimum = value;  RaisePropertyChanged("GeneratorDatemMinimum"); }
        }

        public DateTime BulkDoseDateMinimum
        {
            get { return _bulkDoseDateMinimum; }
            set { _bulkDoseDateMinimum = value;  RaisePropertyChanged("BulkDoseDateMinimum"); }
        }

        public DateTime UnitDoseDateMinimum
        {
            get { return _unitDoseDateMinimum; }
            set { _unitDoseDateMinimum = value; RaisePropertyChanged("UnitDoseDateMinimum"); }
        }

        public DateTime ColdKitDateMinimum
        {
            get { return _coldKitDateMinimum; }
            set { _coldKitDateMinimum = value; RaisePropertyChanged("ColdKitDateMinimum"); }
        }

        public int DisposableItems
        {
            get { return _disposableItems; }
            set { _disposableItems = value; RaisePropertyChanged("DisposableItems"); }
        }
        public double DisposableActivity
        {
            get { return _disposableActivity; }
            set
            {
                _disposableActivity = value;
                RaisePropertyChanged("DisposableActivity");
            }
        }

        public int NumberOfElutionsToday
        {
            get { return _elutionsToday; }
            set { _elutionsToday = value;
                RaisePropertyChanged("ElutionsToday");
            }
        }

        public double ActivityOfElutionsToday
        {
            get { return _activityOfElutionsToday; }
            set { _activityOfElutionsToday = value; RaisePropertyChanged("ActivityOfElutionsToday"); }
        }

        public int NumberOfColdKitsToday
        {
            get { return _coldKitsToday; }
            set { _coldKitsToday = value; RaisePropertyChanged("ColdKitsToday"); }
        }

       public double ActivityOfColdKitsToday
        {
            get { return _activityOfColdKitsToday; }
            set
            {
                _activityOfColdKitsToday = value;
                RaisePropertyChanged("ActivityOfColdKitsToday");
            }
        }

        public int NumberOfUnitDosesToday
        {
            get { return _numerOfUnitDosesToday; }
            set { _numerOfUnitDosesToday = value; RaisePropertyChanged("NumberOfUnitDosesToday"); }
        }
        public double ActivityOfUnitDosesToday
        {
            get { return _activityOfUnitDosesToday; }
            set { _activityOfColdKitsToday = value;  RaisePropertyChanged("ActivityOfUnitDosesToday"); }
        }

        public AsyncObservableCollection<IsotopeLevel> IsotopeLevels
        {
            get
            {
                if (_isotopeLevels == null)
                    _isotopeLevels = new AsyncObservableCollection<IsotopeLevel>();
                return _isotopeLevels;
            }
            set
            {
                _isotopeLevels = value;
                RaisePropertyChanged("IsotopeLevels");
            }
        }

        public int NumberOfExpiredItems
        {
            get { return _numberOfExpiredItems; }
            set { _numberOfExpiredItems = value; RaisePropertyChanged("NumberOfExpiredItems"); }
        }

        public double ActivityOfExpiredItems
        {
            get { return _activityOfExpiredItems; }
            set { _activityOfExpiredItems = value; RaisePropertyChanged("ActivityOfExpiredItems"); }
        }
        #endregion

        #region publicMethods
        public void AddInventoryItem(IDataStoreItem item)
        {
            InventoryItems.Add(item);
        }
        #endregion

        #region privateMethods
        private void addNewInventoryItem()
        {
            //Application.ShowDialog("Debug",SelectedInventoryType);
            if(SelectedInventoryType == "Split Unit Dose")
            {
                SplitUnitDose d = new SplitUnitDose();
                d.CalibrationDate = DateTime.Now;
                d.ExpiryDate = DateTime.Now.AddHours(12);
                d.ItemSaving += InventoryItem_ItemSaved;
                DesktopApplication.MakeModalDocument(new SplitUnitDoseViewModel(d));
            }
            else if(SelectedInventoryType == "Bulk Tc99m")
            {
               
                Elution bd = new Elution();
                bd.CalibrationDate = DateTime.Now;
                bd.ExpiryDate = DateTime.Now.AddDays(1);

                bd.ItemSaving += InventoryItem_ItemSaved;
                System.Diagnostics.Debug.WriteLine("Bulk Dose has been created");
                
                //InventoryItems.Add(bd);
                //System.Diagnostics.Debug.WriteLine("Bulk Dose added to inventory");
                DesktopApplication.MakeModalDocument(new BulkDoseViewModel(bd));
            }
            else if (SelectedInventoryType == "Reconstituted Kit")
            {

                ReconstitutedColdKit bd = new ReconstitutedColdKit();
                bd.CalibrationDate = DateTime.Now;
                bd.ExpiryDate = DateTime.Now.AddDays(1);

                bd.ItemSaving += InventoryItem_ItemSaved;
                

                //InventoryItems.Add(bd);
                //System.Diagnostics.Debug.WriteLine("Bulk Dose added to inventory");
                DesktopApplication.MakeModalDocument(new ReconstitutedColdKitViewModel(bd));
            }
            else if (SelectedInventoryType == "Cold Kit")
            {
               
                Kit ck = new Kit();
                ck.ExpiryDate = DateTime.Now.AddDays(30);
                ck.ItemSaving += InventoryItem_ItemSaved;
                

                //InventoryItems.Add(bd);
                //System.Diagnostics.Debug.WriteLine("Bulk Dose added to inventory");
                DesktopApplication.MakeModalDocument(new DataStoreItemViewModel(ck));
            }
            else if (SelectedInventoryType == "Generator")
            {

                Generator ck = new Generator();
                ck.ItemSaving += InventoryItem_ItemSaved;
                ck.CalibrationDate = DateTime.Now;
                ck.ExpiryDate = DateTime.Now.AddDays(7);

                //InventoryItems.Add(bd);
                //System.Diagnostics.Debug.WriteLine("Bulk Dose added to inventory");
                DesktopApplication.MakeModalDocument(new GeneratorViewModel(ck));
            }
            else if (SelectedInventoryType == "Unit Dose")
            {

                SyringeUnitDose ck = new SyringeUnitDose();
                ck.ItemSaving += InventoryItem_ItemSaved;
                ck.CalibrationDate = DateTime.Now;
                ck.ExpiryDate = DateTime.Now.AddDays(1);

                //InventoryItems.Add(bd);
                //System.Diagnostics.Debug.WriteLine("Bulk Dose added to inventory");
                DesktopApplication.MakeModalDocument(new BaseUnitDoseViewModel(ck));
            }
            else if (SelectedInventoryType == "Capsule")
            {

                CapsuleUnitDose cap = new CapsuleUnitDose();
                cap.ItemSaving += InventoryItem_ItemSaved;
                cap.CalibrationDate = DateTime.Now;
                cap.ExpiryDate = DateTime.Now.AddDays(1);

                //InventoryItems.Add(bd);
                //System.Diagnostics.Debug.WriteLine("Bulk Dose added to inventory");
                DesktopApplication.MakeModalDocument(new CapsuleUnitDoseViewModel(cap));
            }
        }

        //This method ensures that anything which is added to the inventory
        //will actually appear on the list. This method is tightly coupled to the model
        //and if new IInventoryItems are created it will be necessary to rewrite this model.
        
        private void InventoryItem_ItemSaved(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if((sender as IDataStoreItem).ID  == 0)
            {
                InventoryItems.Add(sender as IDataStoreItem);
            }

            if(sender is Generator)
            {
                Generator g = sender as Generator;
                if(g.Elutions.Where(x=>x.ID == 0).Any())
                {
                    foreach(Elution t in g.Elutions.Where(x => x.ID == 0))
                    {
                        InventoryItems.Add(t);
                    }
                }
            }

            if(sender is BaseBulkDose)
            {
                BaseBulkDose bd = sender as BaseBulkDose;
                foreach(BaseUnitDose ud in bd.UnitDoses.Where(x=>x.ID == 0))
                {
                    InventoryItems.Add(ud);
                }
            }

            resetView();
            (sender as IDataStoreItem).ItemSaving -= InventoryItem_ItemSaved;
        }

        private void ViewItem()
        {
            if(SelectedInventoryItem != null)
            {
                SelectedInventoryItem.ItemSaving += InventoryItem_ItemSaved;
                if(SelectedInventoryItem.ConcreteType == typeof(ReconstitutedColdKit))
                {

                    ReconstitutedColdKitViewModel vm = new ReconstitutedColdKitViewModel((DataStoreItem)SelectedInventoryItem);
                   
                    DesktopApplication.MakeModalDocument(vm);
                }
                else if (SelectedInventoryItem.ConcreteType == typeof(Elution))
                {
                    ElutionViewModel dvm = new ElutionViewModel((DataStoreItem)SelectedInventoryItem);
                    DesktopApplication.MakeModalDocument(dvm);
                }
                else if (SelectedInventoryItem.ConcreteType == typeof(SplitUnitDose))
                {
                    SplitUnitDoseViewModel dvm = new SplitUnitDoseViewModel((DataStoreItem)SelectedInventoryItem);
                    DesktopApplication.MakeModalDocument(dvm);
                }
                else if(SelectedInventoryItem.ConcreteType == typeof(SyringeUnitDose))
                {
                    SyringeUnitDoseViewModel dvm = new SyringeUnitDoseViewModel((DataStoreItem)SelectedInventoryItem);
                    DesktopApplication.MakeModalDocument(dvm);
                }
                else if (SelectedInventoryItem.ConcreteType == typeof(Generator))
                {
                    GeneratorViewModel dvm = new GeneratorViewModel((DataStoreItem)SelectedInventoryItem);
                    DesktopApplication.MakeModalDocument(dvm);
                }
                else if (SelectedInventoryItem.ConcreteType == typeof(Kit))
                {
                    DataStoreItemViewModel dvm = new DataStoreItemViewModel((DataStoreItem)SelectedInventoryItem);
                    DesktopApplication.MakeModalDocument(dvm);
                }
                else if (SelectedInventoryItem.ConcreteType == typeof(ReconstitutedColdKit))
                {
                    DataStoreItemViewModel dvm = new DataStoreItemViewModel((DataStoreItem)SelectedInventoryItem);
                    DesktopApplication.MakeModalDocument(dvm);
                }
                else if (SelectedInventoryItem.ConcreteType == typeof(CapsuleUnitDose))
                {
                    CapsuleUnitDoseViewModel dvm = new CapsuleUnitDoseViewModel((DataStoreItem)SelectedInventoryItem);
                    DesktopApplication.MakeModalDocument(dvm);
                }
            }
            else
            {
                DesktopApplication.ShowDialog("Debug", "SelectedInventoryItem is null");
            }
        }

        private void addNewRadiopharmaceutical()
        {
            Chemical rp = new Chemical();
            Radiopharmaceuticals.Add(rp);
            DesktopApplication.MakeModalDocument(new DataStoreItemViewModel(rp));
        }

        private void addKitDefinition()
        {
            KitDefinition kd = new KitDefinition();
            KitDefinitions.Add(kd);
            DesktopApplication.MakeModalDocument(new DataStoreItemViewModel(kd));
        }

        private void viewKitDefinition()
        {
            DesktopApplication.MakeModalDocument(new DataStoreItemViewModel(SelectedKitDefinition as DataStoreItem));
        }

        private void viewChemical()
        {
            DataStoreItemViewModel dvm = new DataStoreItemViewModel((DataStoreItem)SelectedChemical);
            DesktopApplication.MakeModalDocument(dvm);
        }

        private void resetView()
        {
            
            if(InventoryView != null)
            {
                InventoryView.Refresh();
            }
            
        }

        private void reloadData()
        {
            DesktopApplication.MainViewModel.Busy = true;
            GetData();
            DesktopApplication.MainViewModel.Busy = false;
            UIThreadInitialize();
        }
        private bool inventoryItemFilter(object item)
        {
            if (_dontFilter)
                return true;

            IInventory iitem = ((IInventory)item);
            if (iitem.Expired)
            {
                if (!ShowExpired) { return false; }
            }
            if (iitem.Disposed == DisposalStatus.Disposed)
            {
                if (!ShowDisposed) { return false; }
            }
            if (iitem.Disposed == DisposalStatus.Returned)
            {
                if (!ShowDisposed) { return false; }
            }
            if (iitem is BaseBulkDose)
            {
                if (!ShowBulkDoses)
                {
                    return false;
                }
            }
            if (iitem is BaseUnitDose)
            {
                if (!ShowUnitDoses)
                {
                    return false;
                }
            }
            if (iitem is Kit)
            {
                if (!ShowColdKits)
                {
                    return false;
                }
            }
            if (iitem is Generator)
            {
                if (!ShowGenerators)
                {
                    return false;
                }
            }
            return true;
        }

        private void LaunchReconstitution()
        {
            Kit ck = SelectedInventoryItem as Kit;
           ReconstitutedColdKit bs = new ReconstitutedColdKit();

            ReconstitutionViewModel rvm = new ReconstitutionViewModel(ck);
            DesktopApplication.MakeModalDocument(rvm);
        }

        private void InventoryItemAdded(object sender, NewDataStoreItemEventArgs e)
        {
            InventoryItems.Add(e.NewItem);
            if (e.NewItem is Generator)
                NumberOfElutionsToday++;


        }

        private void DisposeItem()
        {
            
            if (SelectedInventoryItems == null)
            {
                return;
            }
            _dontFilter = true;
            foreach(var o in SelectedInventoryItems)
            {
                IInventory item = o as IInventory;
                if (!item.IsDisposed)
                {
                    item.Disposed = DisposalStatus.Disposed;
                    item.Disposer = Platform.CurrentUser;
                    item.DisposalDate = DateTime.Now;
                    DesktopApplication.Librarian.SaveItem(item as DataStoreItem);
                }
            }
            _dontFilter = false;
            resetView();
        }

        private void UnDiposeItem()
        {
            if (SelectedInventoryItem == null)
            {
                return;
            }
            IInventory item = SelectedInventoryItem as IInventory;
            if (item.Disposed == DisposalStatus.Disposed)
            {
                item.Disposed = DisposalStatus.NotDisposed;
               
                DesktopApplication.Librarian.SaveItem(item as DataStoreItem);
            }
        }

        private void MilkGenerator()
        {
            DesktopApplication.MakeModalDocument(new MilkGeneratorViewModel(SelectedInventoryItem as Generator));
        }

        private void DrawDose()
        {
            //Do something here.
            DrawDoseViewModel vm = new DrawDoseViewModel((DataStoreItem)SelectedInventoryItem);

            DesktopApplication.MakeModalDocument(vm,"iRadiate.Radiopharmacy","iRadiate.Radiopharmacy.View.DrawDoseView");
        }

        private void PrintLabel()
        {
            DataStoreItemViewModel vm = new DataStoreItemViewModel((DataStoreItem)SelectedInventoryItem);
            if(SelectedInventoryItem is Generator)
            {
                
                DesktopApplication.MakeModalDocument(vm, "iRadiate.Radiopharmacy", "iRadiate.Radiopharmacy.View.GeneratorPrintView");
            }
            else if (SelectedInventoryItem is SplitUnitDose)
            {
                vm = new SplitUnitDoseViewModel((DataStoreItem)SelectedInventoryItem);
                DesktopApplication.MakeModalDocument(vm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.SplitUnitDosePrintView");
            }
            else if(SelectedInventoryItem is BaseUnitDose)
            {
                vm = new BaseUnitDoseViewModel((DataStoreItem)SelectedInventoryItem);
                DesktopApplication.MakeModalDocument(vm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.UnitDosePrintView");
            }
            else if(SelectedInventoryItem is BaseBulkDose)
            {
                DesktopApplication.MakeModalDocument(vm, "iRadiate.Radiopharmacy", "iRadiate.Radiopharmacy.View.BulkDosePrintView");
            }
            else if(SelectedInventoryItem is Kit)
            {
                DesktopApplication.MakeModalDocument(vm, "iRadiate.Radiopharmacy", "iRadiate.Radiopharmacy.View.ColdKitPrintView");
            }
            
        }

        private void ReturnToManufacturer()
        {
            if(SelectedInventoryItem != null)
            {
                IInventory item = SelectedInventoryItem as IInventory;
                item.Disposed = DisposalStatus.Returned;
                item.Disposer = Platform.CurrentUser;
                item.DisposalDate = DateTime.Now;
                DesktopApplication.Librarian.DataRetriever.SaveItem(item as IDataStoreItem);

            }
        }
        
        #endregion

        #region Commands
        public RelayCommand AddItemCommand { get; set; }

        public RelayCommand ViewInfoCommand { get; set; }

        public RelayCommand AddRadiopharmaceuticalCommand { get; set; }

        public RelayCommand RefreshInventoryCommand { get; set; }

        public RelayCommand ViewChemicalCommand { get; set; }

        public RelayCommand LaunchReconstitutionCommand { get; set; }

        public RelayCommand DisposeItemCommand { get; set; }

        public RelayCommand UnDisposeItemCommand { get; set; }

        public RelayCommand MilkGeneratorCommand { get; set; }

        public RelayCommand DrawDoseCommand { get; set; }

        public RelayCommand PrintLabelCommand { get; set; }

        public RelayCommand AddKitDefinitionCommand { get; set; }

        public RelayCommand ViewKitDefinitionCommand { get; set; }

        public RelayCommand ReturnItemCommand { get; set; }

        public RelayCommand UnassignUnitDoseCommand { get; set; }
        #endregion

        #region events
        protected virtual void SelectedInventoryItemChanged()
        {
            if(SelectedInventoryItem == null)
            {
                InfoButtonEnabled = false;
                ReconstitutionButtonEnabled = false;
                MilkGeneratorButtonEnabled = false;
                DrawDoseButtonEnabled = false;
                PrintLabelButtonEnabled = false;
                UnassignUnitDoseButtonEnabled = false;
            }           
            else
            {
               
                InfoButtonEnabled = true;
                PrintLabelButtonEnabled = true;
                if(SelectedInventoryItem is Kit && !(SelectedInventoryItem as IInventory).Expired)
                {
                    ReconstitutionButtonEnabled = true;
                    
                }
                else
                {
                    ReconstitutionButtonEnabled = false;
                }

                if (SelectedInventoryItem is Generator && !(SelectedInventoryItem as IInventory).Expired)
                {
                    MilkGeneratorButtonEnabled = true;
                }
                else
                {
                    MilkGeneratorButtonEnabled = false;
                }

                if (SelectedInventoryItem is BaseBulkDose && !(SelectedInventoryItem as IInventory).Expired)
                {
                    DrawDoseButtonEnabled = true;

                }
                else
                {
                    DrawDoseButtonEnabled = false;
                }

                if ((SelectedInventoryItem as IInventory).IsDisposed == false)
                {
                    DisposeButtonEnabled = true;
                    ReturnItemButtonEnabled = true;
                }
                else
                {
                    DisposeButtonEnabled = false;
                    MilkGeneratorButtonEnabled = false;
                    ReconstitutionButtonEnabled = false;
                    DrawDoseButtonEnabled = false;
                }
                if(SelectedInventoryItem is BaseUnitDose)
                {
                    if ((SelectedInventoryItem as BaseUnitDose).DoseAdministrationTask != null)
                        UnassignUnitDoseButtonEnabled = true;
                    else
                        UnassignUnitDoseButtonEnabled = false;
                }

            }
        }

        protected virtual void SelectedChemicalChanged()
        {
            if(SelectedChemical == null)
            {
                ChemicalInfoButtonEnabled = false;
            }
            else
            {
                ChemicalInfoButtonEnabled = true;
            }
        }
        #endregion

        #region tools
        private void UnassignUnitDose()
        {
            if (SelectedInventoryItem == null)
            {
                return;
            }
            if (!(SelectedInventoryItem is BaseUnitDose))
                     return;

            BaseUnitDose b = (SelectedInventoryItem as BaseUnitDose);
            if (b.DoseAdministrationTask == null)
                return;

            b.DoseAdministrationTask = null;
            DesktopApplication.Librarian.SaveItem(b);
            resetView();
        }
        #endregion
    }

    public class IsotopeLevel
    {
        public IsotopeLevel()
        {

        }
        public Isotope Isotope { get; set; }
        public double CurrentActivity { get; set; }
    }
}
