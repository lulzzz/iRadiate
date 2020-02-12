using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NLog;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Radiopharmacy;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Radiopharmacy
{
    [PreferredView("iRadiate.Radiopharmacy.View.RadioactiveGasView", "iRadiate.Radiopharmacy")]
    public class RadioactiveGasModule : DataStoreItemViewModel
    {

        #region privateFields
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Chemical _ingredient;
        private Chemical _product;
        private List<IDataStoreItem> _potentialIngredients;
        private double _ingredientActivity, _productActivity, _ingredientVolume;
        private Patient _patient;
        private DoseAdministrationTask _doseAdministrationTask;
        private IDataStoreItem _selectedIngredient;
        private DateTime _calibrationDate, _administrationDate, _expiryDate;        
        #endregion

        #region constructors
        public RadioactiveGasModule(DoseAdministrationTask dat) : base()
        {
            SaveDoseCommand = new RelayCommand(SaveItem);  
            CalibrationDate = DateTime.Now;
            AdministrationDate = DateTime.Now;
            ExpiryDate = DateTime.Now.AddMinutes(15);
            SetItem(dat);
            DoseAdministrationTask = dat;
            _potentialIngredients = new List<IDataStoreItem>();
            //RetrievalCriteria rc = new RetrievalCriteria("Radiopharmaceutical", CriteraType.Equals, dat.PrescribedRadioPharmaceutical);
            RetrievalCriteria rc1 = new RetrievalCriteria("ExpiryDate", CriteraType.GreaterThan, DateTime.Now);
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            //rcList.Add(rc);
            rcList.Add(rc1);
            
            //get all elutions that match the isotope from  Radioactive Gas Module
            //RetrievalCriteria rc = new RetrievalCriteria("IsExpired", CriteraType.Equals, false);
            //List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc1);
            var result = Platform.Retriever.RetrieveItems(typeof(BaseBulkDose), rcList);
            foreach(IDataStoreItem i  in result)
            {
                if(i is Elution)
                {
                    if((i as Elution).Isotope.ID == dat.PrescribedRadioPharmaceutical.Isotope.ID)
                    {
                        _potentialIngredients.Add(i);
                    }
                }
            }
        }
        #endregion

        #region publicProperties
        /// <summary>
        /// The type of chemical used in the process, i.e. pertechnetate is the ingredient in technegas
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public Chemical Ingredient
        {
            get { return _ingredient; }
            set
            {
                _ingredient = value;
                RaisePropertyChanged("Ingredient");
            }
        }

        public List<IDataStoreItem> PotentialIngredients
        {
            get { return _potentialIngredients; }
            set { _potentialIngredients = value;  RaisePropertyChanged("PotentialIngredients"); }
        }

        public IDataStoreItem SelectedIngredient
        {
            get { return _selectedIngredient; }
            set { _selectedIngredient = value;  RaisePropertyChanged("SelectedIngredient"); }
        }

        /// <summary>
        /// The product chemical that results from this process, e.g. technegas or galligas
        /// </summary>
        public Chemical Product
        {
            get
            {
                if(DoseAdministrationTask != null)
                {
                    if(DoseAdministrationTask.PrescribedRadioPharmaceutical != null)
                    {
                        return DoseAdministrationTask.PrescribedRadioPharmaceutical;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            
        }

       

        /// <summary>
        /// The patient who is being gassed.
        /// </summary>
        public Patient Patient
        {
            get
            {
                return DoseAdministrationTask.Patient;
            }
            
        }

        /// <summary>
        /// The dose administration Task
        /// </summary>
        public DoseAdministrationTask DoseAdministrationTask
        {
            get { return _doseAdministrationTask; }
            set
            {
                _doseAdministrationTask = value;
                RaisePropertyChanged("DoseAdministrationTask");
                RaisePropertyChanged("SaveEnabled");
            }
        }

        /// <summary>
        /// Save is only allowed if all the details are correct
        /// </summary>
        public bool SaveEnabled
        {
            get
            {
                return false;
            }
        }

        public double IngredientActivity
        {
            get { return _ingredientActivity; }
            set { _ingredientActivity = value; RaisePropertyChanged("IngredientActivity"); }
        }

        public double IngredientVolume
        {
            get { return _ingredientVolume; }
            set { _ingredientVolume = value; RaisePropertyChanged("IngredientVolume"); }
        }
        public double ProductActivity
        {
            get { return _productActivity; }
            set { _productActivity = value;  RaisePropertyChanged("ProductActivity"); }
        }
        public DateTime CalibrationDate
        {
            get { return _calibrationDate; }
            set { _calibrationDate = value; RaisePropertyChanged("CalibrationDate"); }
        }

        public DateTime AdministrationDate
        {
            get { return _administrationDate; }
            set { _administrationDate = value; RaisePropertyChanged("AdministrationDate"); }
        }
        public DateTime ExpiryDate
        {
            get { return _expiryDate; }
            set { _expiryDate = value;  RaisePropertyChanged("ExpiryDate"); }
        }

        public RelayCommand SaveDoseCommand
        {
            get;set;
        }
        #endregion

        #region overrides
        public override void SaveItem()
        {
            if (!validateData())
            {
                DesktopApplication.ShowDialog("Error", "Invalid Data - cannot save");
                return;
                
            }

            try
            {
                GaseousUnitDose g = new GaseousUnitDose();
                g.DoseAdministrationTask = DoseAdministrationTask;
                g.CalibrationActivity = ProductActivity;
                g.CalibrationDate = CalibrationDate;
                g.ExpiryDate = ExpiryDate;
                g.BulkDose = SelectedIngredient as BaseBulkDose;
                g.AdministrationDate = AdministrationDate;
                g.Radiopharmaceutical = DoseAdministrationTask.PrescribedRadioPharmaceutical;
                g.Disposed = DisposalStatus.Disposed;
                DesktopApplication.Librarian.SaveItem(g);

                (SelectedIngredient as BaseBulkDose).Volume = (SelectedIngredient as BaseBulkDose).Volume - IngredientVolume;
                (SelectedIngredient as BaseBulkDose).CalibrationActivity = (SelectedIngredient as BaseBulkDose).CurrentActivity - IngredientActivity;
                (SelectedIngredient as BaseBulkDose).CalibrationDate = CalibrationDate;
                DesktopApplication.Librarian.SaveItem(SelectedIngredient as BaseBulkDose);

                DoseAdministrationTask.Completed = true;
                DoseAdministrationTask.Assignee = DesktopApplication.CurrentUser;
                DoseAdministrationTask.CompletionTime = DateTime.Now;
                DesktopApplication.Librarian.SaveItem(DoseAdministrationTask);

                var vm = new BaseUnitDoseViewModel(DoseAdministrationTask.UnitDose);
                vm.PreCalTime = DoseAdministrationTask.UnitDose.AdministrationDate;
                DesktopApplication.MakeModalDocument(vm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.UnitDosePrintView");

                OnClosing();
            }
            catch(Exception e)
            {
                logger.Error(e, e.Message);
                DesktopApplication.ShowDialog("Error", "Exception caught: " + e.Message);
            }
            //base.SaveItem();
            
        }


        #endregion

        #region privateMethods
        private bool validateData()
        {
            if (SelectedIngredient == null)
                return false;
           
            if (ProductActivity == 0)
                return false;
            if (IngredientActivity == 0)
                return false;
            if (IngredientVolume == 0)
                return false;

            return true;
        }
        #endregion
    }
}
