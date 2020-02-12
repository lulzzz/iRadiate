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
using iRadiate.DataModel.Forms;
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Common.Forms
{
    [PreferredView("iRadiate.Desktop.Common.Forms.FormDesigner", "iRadiate.Desktop.Common")]
    public class FormDesignerModule : Module
    {
        #region privateFields
        private FormElementWrapper _selectedFormElement;
        private string _rowString;
        private string _columnString;
        private AsyncObservableCollection<FormElementWrapper> _formElements;        
        private FormTemplate _template;
        private FormInstance _formInstance;
        private List<string> _formElementOptions;
        private string _chosenFormElementOption;
        #endregion

        #region constructor
        public FormDesignerModule() : base()
        {
            _template = new FormTemplate();
            RowString = "0.5,0.5";
            ColumnString = "0.5,0.5";
            generateInstance();
        }
       
        public FormDesignerModule(FormTemplate template) : base()
        {
            foreach(FormElement e in template.FormElements)
            {
                FormElementWrapper w = new FormElementWrapper(e);
                FormElements.Add(w);
            }
            RowString = template.Rows;
            ColumnString = template.Columns;
            _template = template;
           
            generateInstance();
        }
        #endregion

        #region publicProperties
        public FormInstance FormInstance
        {
            get { return _formInstance; }
            set { _formInstance = value; RaisePropertyChanged("FormInstance"); }
        }
        public FormElementWrapper SelectedFormElement
        {
            get { return _selectedFormElement; }
            set
            {
                _selectedFormElement = value;
                RaisePropertyChanged("SelectedFormElement");
            }
        }

        public string RowString
        {
            get { return _rowString; }
            set { _rowString = value;
                RaisePropertyChanged("RowString");
                
                 }
        }

        public string ColumnString
        {
            get { return _columnString; }
            set { _columnString = value;
                
                RaisePropertyChanged("ColumnString"); }
        }

        public AsyncObservableCollection<FormElementWrapper> FormElements
        {
            get
            {
                if (_formElements == null)
                    _formElements = new AsyncObservableCollection<FormElementWrapper>();
                return _formElements;
            }
            set
            {
                _formElements = value;
                RaisePropertyChanged("FormElements");
            }
        }

        public List<String> FormElementOptions
        {
            get
            {
                if(_formElementOptions == null)
                {
                    _formElementOptions = new List<string>();
                    _formElementOptions.Add("Label");
                    _formElementOptions.Add("Text Box");
                    _formElementOptions.Add("Selection Box");
                    _formElementOptions.Add("Numeric Up Down");
                }
                return _formElementOptions;
            }
        }

        public string ChosenFormElementOption
        {
            get
            {
                return _chosenFormElementOption;
            }
            set
            {
                _chosenFormElementOption = value;
            }
        }
        #endregion

        #region overrides
        public override ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconModern icon = new PackIconModern();
                icon.Kind = PackIconModernKind.PageEdit;
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

        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            GenerateInstanceCommand = new RelayCommand(generateInstance);
            AddFormElementCommand = new RelayCommand(addFormElement);
        }

        public override string Name
        {
            get
            {
                return "Form Designer";
            }
        }
        #endregion

        #region privateMethods
        private void generateInstance()
        {
            if (_template == null)
                return;
            RaisePropertyChanged("FormWidth");
            RaisePropertyChanged("FormHeight");
            _template.Rows = RowString;
            _template.Columns = ColumnString;
            FormInstance i = new FormInstance();
            i.FormCommencementDate = DateTime.Now;
            i.FormTemplate = _template;
            FormInstance = i;
        }

        private void addFormElement()
        {
            if((ChosenFormElementOption == null) || (ChosenFormElementOption == String.Empty))
            {
                return;
            }

            if(ChosenFormElementOption == "Label")
            {
                LabelFormElement e = new LabelFormElement();
                e.Name = "New Label";
                e.LabelText = "New Label";
                LabelElementWrapper w = new LabelElementWrapper(e);
                _template.FormElements.Add(e);
                FormElements.Add(w);
                generateInstance();
            }
            else if(ChosenFormElementOption == "Text Box")
            {
                TextFormElement e = new TextFormElement();
                e.Name = "New text box";
                FormElementWrapper w = new FormElementWrapper(e);
                _template.FormElements.Add(e);
                FormElements.Add(w);
                generateInstance();
            }
            else if (ChosenFormElementOption == "Numeric Up Down")
            {
                NumericFormElement e = new NumericFormElement();
                e.Name = "New numeric box";
                NumericElementWrapper w = new NumericElementWrapper(e);
                _template.FormElements.Add(e);
                FormElements.Add(w);
                generateInstance();
            }

        }
        #endregion

        #region commands
        public RelayCommand GenerateInstanceCommand { get; set; }

        public RelayCommand AddFormElementCommand { get; set; }
        #endregion

    }


}
