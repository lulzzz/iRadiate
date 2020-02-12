using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using MahApps.Metro.IconPacks;
using GalaSoft.MvvmLight.Command;

using iRadiate.Common;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.Equipment;
using iRadiate.DataModel.Forms;
using iRadiate.DataModel.DataDictionary;
using iRadiate.Desktop.Common;

namespace iRadiate.QA.ViewModel
{
    [PreferredView("iRadiate.QA.View.QAView", "iRadiate.QA")]
    public class QAModule : Module
    {
        #region privateFields
        private AsyncObservableCollection<IDataStoreItem> _forms;
        
       
        private IDataStoreItem _selectedForm;
        private FormInstance _selectedFormInstance;
        private AsyncObservableCollection<EquipmentItem> _equipmentItems;
        private EquipmentItem _selectedEquipment;
        private AsyncObservableCollection<EquipmentItemType> _equipmentItemTypes;
        private AsyncObservableCollection<EquipmentItem> _rootEquipmentItems;
        private string _newEquipmentTypeName;
       
        #endregion

        public QAModule() : base()
        {

        }

        #region publicProperties
        public AsyncObservableCollection<IDataStoreItem> Forms
        {
            get
            {
                if (_forms == null)
                    _forms = new AsyncObservableCollection<IDataStoreItem>();
                return _forms;
            }
            set
            {
                _forms = value;
            }
        }

        public IDataStoreItem SelectedForm
        {
            get { return _selectedForm; }
            set { _selectedForm = value;  RaisePropertyChanged("SelectedForm"); }
        }

        public FormInstance SelectedFormInstance
        {
            get
            {
                return _selectedFormInstance;
            }
            set
            {
                _selectedFormInstance = value;
                RaisePropertyChanged("SelectedFormInstance");
            }
        }
        #endregion

        #region overrides
        public override void GetData()
        {
            #region equipment
            var res = Platform.Retriever.RetrieveItems(typeof(EquipmentItemType));
            foreach(var v in res)
            {
                EquipmentItemTypes.Add(v as EquipmentItemType);
            }

            var equipments = Platform.Retriever.RetrieveItems(typeof(EquipmentItem));
            foreach(var q in equipments)
            {
                EquipmentItems.Add(q as EquipmentItem);
            }

            #endregion



            #region Forms

            #region DailyQCForm
            //QAForm dailyForm = new QAForm();
            //dailyForm.Name = "Daily QC";
            //dailyForm.Frequency = FormFrequency.Daily;
            //FormTemplate f1 = new FormTemplate();
            //f1.Columns = "0.35,0.15,0.35,0.15";
            //f1.Rows = "0.1,0.1,0.1,0.1,0.1,0.1,0.1,0.1,0.1,0.1";

            //f1.Form = dailyForm;
            //dailyForm.FormTemplates.Add(f1);
            //f1.VersionDate = DateTime.Now;
            //f1.VersionNumber = 1;

            //LabelFormElement lel = new LabelFormElement();
            //lel.Row = 0;
            //lel.Column = 0;
            //lel.FontSize = 14;
            //lel.LabelText = "Did Air Cal Pass?";
            //lel.Height = 0.05;
            //lel.Width = 0.13;
            //lel.Background = "Gray";
            //lel.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            //lel.Name = "Air cal label";
            //f1.FormElements.Add(lel);

            //BooleanFormElement bel = new BooleanFormElement();
            ////bel.DataDictionaryEntry = CT.Entries.Last();
            //bel.Row = 0;
            //bel.Column = 1;
            //bel.Width = 0.05;
            //bel.Height = 0.05;
            //bel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            //bel.Name = "Air cal pass checkbox";
            //f1.FormElements.Add(bel);

            //LabelFormElement lel2 = new LabelFormElement();
            //lel2.Row = 0;
            //lel2.Column = 2;
            //lel2.FontSize = 14;
            //lel2.LabelText = "Uniformity value:";
            //lel2.Height = 0.05;
            //lel2.Width = 0.13;
            //lel2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            //f1.FormElements.Add(lel2);

            //NumericFormElement nel = new NumericFormElement();
            //nel.DataDictionaryEntry = gammaCamera.Entries.First();
            //nel.Column = 3;
            //nel.Row = 0;
            //nel.Width = 0.13;
            //nel.NumberOfDecimals = 2;
            //nel.Format = "P";
            //nel.Height = 0.05;
            //nel.FontSize = 14;
            //f1.FormElements.Add(nel);

            //LabelFormElement bottomLabel = new LabelFormElement();
            //bottomLabel.LabelText = "Bottom of Page";
            //bottomLabel.FontSize = 24;
            //bottomLabel.Row = 1;
            //bottomLabel.Column = 0;
            //bottomLabel.ColumnSpan = 4;
            //bottomLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //bottomLabel.Foreground = "Orange";
            //bottomLabel.Background = "Blue";
            //f1.FormElements.Add(bottomLabel);
            //Forms.Add(dailyForm);
            #endregion

            #region DoseCalibratorQC
            //QAForm CalibratorQC = new QAForm();
            //CalibratorQC.Name = "Dose Calibrator QC";
            //CalibratorQC.Frequency = FormFrequency.Daily;
            //FormTemplate t = new FormTemplate();
            //t.
            #endregion

            //QAForm w = new QAForm();
            //w.Name = "Weekly QC Form";
            //Forms.Add(w);
            //FormTemplate w1 = new FormTemplate();
            //w1.Form = w;
            //w.FormTemplates.Add(w1);
            //w1.VersionNumber = 1;
            //w1.VersionDate = DateTime.Now.AddDays(-1);

            //FormTemplate w2 = new FormTemplate();
            //w2.Form = w;
            //w.FormTemplates.Add(w2);
            //w2.VersionDate = DateTime.Now;
            //w2.VersionNumber = 2;

            //QAFormInstance wInstance = new QAFormInstance();
            //wInstance.FormTemplate = w2;
            //w2.Instances.Add(wInstance);
            #endregion
        }

        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            CreateFormInstanceCommand = new RelayCommand(createInstance);
            EditFormCommand = new RelayCommand(editForm);
            AddNewEquipmentCommand = new RelayCommand(addNewEquipment);
            InsertNewEquipmentCommand = new RelayCommand(insertNewEquipment);
            AddNewEquipmentTypeCommand = new RelayCommand(addNewEquipmentType);
            SaveEquipmentCommand = new RelayCommand(saveEquipment);
            CreateNewFormCommand = new RelayCommand(createNewForm);
        }

        public override ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.BullseyeArrow;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
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
        #endregion

        #region privateMethods
        private void createInstance()
        {
            FormInstance i = new FormInstance();
            i.FormCommencementDate = DateTime.Now;
            i.FormTemplate = ((Form)SelectedForm).FormTemplates.OrderBy(x => x.VersionDate).Last();
            SelectedFormInstance = i;
        }

        private void editForm()
        {
            
            iRadiate.Desktop.Common.Forms.FormDesignerModule m = new Desktop.Common.Forms.FormDesignerModule(((Form)SelectedForm).FormTemplates.OrderBy(x => x.VersionDate).Last());
            DesktopApplication.MakeDocument(m);
            //DesktopApplication.MainViewModel.LaunchModule(typeof(iRadiate.Desktop.Common.Forms.FormDesignerModule));
        }

        private void insertNewEquipment()
        {
            EquipmentItem newEquipment = new EquipmentItem();           
            SelectedEquipment.SubEquipmentItems.Add(newEquipment);
            newEquipment.Parent = SelectedEquipment;
            SelectedEquipment = newEquipment;
            newEquipment.Functional = true;
            newEquipment.PurchaseDate = newEquipment.Parent.PurchaseDate;
            EquipmentItems.Add(newEquipment);
        }

        private void addNewEquipment()
        {
            EquipmentItem newEquipment = new EquipmentItem();
            EquipmentItems.Add(newEquipment);
            newEquipment.Functional = true;
            newEquipment.PurchaseDate = DateTime.Now;
            SelectedEquipment = newEquipment;
        }

        private void addNewEquipmentType()
        {
            if(NewEquipmentTypeName != string.Empty && NewEquipmentTypeName != null)
            {
                EquipmentItemType t = new EquipmentItemType { Name = NewEquipmentTypeName };
                EquipmentItemTypes.Add(t);
                NewEquipmentTypeName = string.Empty;
            }
        }

        private void saveEquipment()
        {
            foreach(var q in EquipmentItemTypes)
            {
                
                Platform.Retriever.SaveItem(q);
            }
            foreach(var p in EquipmentItems)
            {
                Platform.Retriever.SaveItem(p);
            }
        }

        private void createNewForm()
        {
            //Load the form designer!
            //var designer = new iRadiate.Desktop.Common.Forms.FormDesignerModule();
            DesktopApplication.ShowToastInformation("createNewForm", DesktopApplication.NotificationPosition.TopLeft);
            DesktopApplication.MainViewModel.LaunchModule(typeof(iRadiate.Desktop.Common.Forms.FormDesignerModule));
            //iRadiate.Desktop.Common.Forms.FormDesignerModule m = new Desktop.Common.Forms.FormDesignerModule(((Form)SelectedForm).FormTemplates.OrderBy(x => x.VersionDate).Last());
           // DesktopApplication.MakeDocument(m);
        }
        public override string Name
        {
            get { return "Quality Assurance"; }
        }

        #endregion

        #region commands
        public RelayCommand CreateNewFormCommand { get; set; }

        public RelayCommand CreateFormInstanceCommand { get; set; }

        public RelayCommand EditFormCommand { get; set; }

        public RelayCommand AddNewEquipmentCommand { get; set; }

        public RelayCommand InsertNewEquipmentCommand { get; set; }

        public RelayCommand AddNewEquipmentTypeCommand { get; set; }

        public RelayCommand SaveEquipmentCommand { get; set; }


        #endregion

        #region Equipment
        public virtual AsyncObservableCollection<EquipmentItem> EquipmentItems
        {
            get
            {
                if (_equipmentItems == null)
                    _equipmentItems = new AsyncObservableCollection<EquipmentItem>();
                return _equipmentItems;
            }
            set
            {
                _equipmentItems = value;
                RaisePropertyChanged("EquipmentItems");
            }
        }

       public string NewEquipmentTypeName
        {
            get { return _newEquipmentTypeName; }
            set
            {
                _newEquipmentTypeName = value;
                RaisePropertyChanged("NewEquipmentTypeName");
            }
        }

        public virtual EquipmentItem SelectedEquipment
        {
            get { return _selectedEquipment; }
            set { _selectedEquipment = value; RaisePropertyChanged("SelectedEquipment"); }
        }

        public virtual AsyncObservableCollection<EquipmentItemType> EquipmentItemTypes
        {
            get
            {
                if (_equipmentItemTypes == null)
                    _equipmentItemTypes = new AsyncObservableCollection<EquipmentItemType>();
                return _equipmentItemTypes;
            }
            set
            {
                _equipmentItemTypes = value;
            }
        }
        #endregion
    }
}
