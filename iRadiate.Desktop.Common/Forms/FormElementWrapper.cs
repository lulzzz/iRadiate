using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using GalaSoft.MvvmLight;

using iRadiate.DataModel.DataDictionary;
using iRadiate.DataModel.Equipment;
using iRadiate.DataModel.Forms;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace iRadiate.Desktop.Common.Forms
{
    public class FormElementWrapper : ViewModelBase
    {
        private FormElement _element;
        private int _row;
        private int _column;

        public FormElementWrapper(FormElement element)
        {
            _element = element;
           
        }

        [Category("Appearance")]
        [Description("The name of the element within this template")]
        public string Name
        {
            get { return _element.Name; }
            set { _element.Name = value;  }
        }

        [Category("Appearance")]
        [Description("The foreground color of the element")]
        public System.Windows.Media.Color? Foreground
        {
            get
            {
                return (Color)ColorConverter.ConvertFromString(_element.Foreground);
            }
            set
            {
                _element.Foreground = value.Value.ToString();
                
            }
        }

        [Category("Appearance")]
        [Description("The foreground color of the element")]
        public System.Windows.Media.Color? Background
        {
            get
            {
                return (Color)ColorConverter.ConvertFromString(_element.Background);
            }
            set
            {
                _element.Background = value.Value.ToString();
                
            }
        }

        [Category("Font")]
        [Description("The font size of the element in pixels")]
        public int FontSize
        {
            get { return _element.FontSize; }
            set { _element.FontSize = value; }
        }

        [Category("Font")]
        [Description("The font used for this element")]
        public FontFamily FontFamily
        {
            get
            {
                
                return new FontFamily(_element.FontFamily);
            }
            set
            {
                _element.FontFamily = value.Source;
            }
        }
        
        [Category("Font")]
        [Description("The fontweight of the control")]
        public FontWeight FontWeight
        {
            get { return (FontWeight)new FontWeightConverter().ConvertFromString(_element.FontWeight); }
            set
            {
                _element.FontWeight = value.ToString();
            }
        }

        [Category("Font")]
        [Description("The style of font used for the element")]
        public FontStyle FontStyle
        {
            get
            {
                return (FontStyle)new FontStyleConverter().ConvertFromString(_element.FontStyle);
            }
            set
            {
                
                _element.FontStyle = value.ToString();
            }
        }

        [Category("Grid")]
        [Description("The row in the grid where the element will be placed")]
        public int Row
        {
            get { return _element.Row; }
            set { _element.Row = value; }
        }

        [Category("Grid")]
        [Description("The column in the grid where the element will be placed")]
        public int Column
        {
            get { return _element.Column; }
            set { _element.Column = value; }
        }

        [Category("Grid")]
        [Description("The number of rows in the grid that this element spans")]
        public int RowSpan
        {
            get { return _element.RowSpan; }
            set { _element.RowSpan = value; }
        }

        [Category("Grid")]
        [Description("The number of columns in the grid that this element spans")]
        public int ColumnSpan
        {
            get { return _element.ColumnSpan; }
            set { _element.ColumnSpan = value; }
        }

        [Category("Appearance")]
        [Description("The horizontal alignment of the element within the grid cell")]
        public HorizontalAlignment HorizontalAlignment
        {
            get { return _element.HorizontalAlignment; }
            set { _element.HorizontalAlignment = value; }
        }

        [Category("Appearance")]
        [Description("The vertical alignment of the element within the grid cell")]
        public VerticalAlignment VerticalAlignment
        {
            get { return _element.VerticalAlignment; }
            set { _element.VerticalAlignment = value; }
        }

        

    }

    public class LabelElementWrapper : FormElementWrapper
    {
        private LabelFormElement _labelElement;
        public LabelElementWrapper(LabelFormElement element) : base(element)
        {
            _labelElement = element;
        }

        [Category("Data")]
        [Description("The text in the label")]
        public string Text
        {
            get { return _labelElement.LabelText; }
            set { _labelElement.LabelText = value; }
        }
    }

    public class DataFormElementWrapper : FormElementWrapper
    {
        DataFormElement _dataElement;
        public DataFormElementWrapper(DataFormElement element) : base(element)
        {
            _dataElement = element;
        }

        [Category("Data")]
        [Description("The equipment that this measurement is on")]
        [Editor(typeof(EquipmentItemEditor), typeof(EquipmentItemEditor))]
        public EquipmentItem EquipmentItem
        {
            get { return _dataElement.EquipmentItem; }
            set { _dataElement.EquipmentItem = value; }
        }

        [Category("Data")]
        [Description("The entry in the DataDictionary htat this uses")]
        [Editor(typeof(DataDictionaryEditor), typeof(DataDictionaryEditor))]
        public DataDictionaryEntry DataDictionaryEntry
        {
            get { return _dataElement.DataDictionaryEntry; }
            set { _dataElement.DataDictionaryEntry = value; }
        }
    }

    public class NumericElementWrapper : DataFormElementWrapper
    {
        private NumericFormElement _numericElement;
        public NumericElementWrapper(NumericFormElement element) : base(element)
        {
            _numericElement = element;
        }

       

        [Category("Data")]
        [Description("The unit of measurement")]
        [Editor(typeof(UnitEditor), typeof(UnitEditor))]
        public string Unit
        {
            get
            {
                return _numericElement.Unit;
            }
            set
            {
                _numericElement.Unit = value;
            }
        }

        [Category("Data")]
        [Description("The number of decimal places to display and record")]
        public int NumberOfDecimals
        {
            get
            {
                return _numericElement.NumberOfDecimals;
            }
            set
            {
                _numericElement.NumberOfDecimals = value;
            }
        }
    }

    public class DataDictionaryEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            ComboBox comboBox = new ComboBox();
            comboBox.ItemsSource = iRadiate.Common.Platform.Retriever.RetrieveItems(typeof(DataDictionaryEntry)).OrderBy(x=>(x as DataDictionaryEntry).FullName).ToList();
            comboBox.SelectedIndex = 0;
            comboBox.DisplayMemberPath = "FullName";
            comboBox.FontSize = 14;

            var _binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = propertyItem.IsReadOnly ? BindingMode.TwoWay : BindingMode.TwoWay
            };

            BindingOperations.SetBinding(comboBox, ComboBox.SelectedItemProperty, _binding);
            return comboBox;
                
        }

        
    }

    public class EquipmentItemEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            ComboBox comboBox = new ComboBox();
            comboBox.ItemsSource = iRadiate.Common.Platform.Retriever.RetrieveItems(typeof(EquipmentItem)).OrderBy(x => (x as EquipmentItem).FullName).ToList();
            comboBox.SelectedIndex = 0;
            comboBox.DisplayMemberPath = "FullName";
            comboBox.FontSize = 14;

            var _binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = propertyItem.IsReadOnly ? BindingMode.TwoWay : BindingMode.TwoWay
            };

            BindingOperations.SetBinding(comboBox, ComboBox.SelectedItemProperty, _binding);
            return comboBox;
        }
    }

    public class UnitEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            ComboBox comboBox = new ComboBox();
            System.Diagnostics.Debug.WriteLine("ResolveEditor()");
            if (propertyItem.Instance is NumericElementWrapper && ((propertyItem.Instance as NumericElementWrapper).DataDictionaryEntry != null))
            {

                List<string> unitsAvailable = new List<string>();
                var qt = UnitsNet.Quantity.GetInfo(((propertyItem.Instance as NumericElementWrapper).DataDictionaryEntry as MeasureableDataDictionaryEntry).QuantityType.Value);
                System.Diagnostics.Debug.WriteLine("qt.Name = " + qt.Name);
                foreach (UnitsNet.UnitInfo val in qt.UnitInfos)
                {
                 
                    var UnitEnumValueInt = Convert.ToInt32(val.Value);
                    var UnitEnumType = val.Value.GetType();
                    unitsAvailable.Add(UnitsNet.UnitAbbreviationsCache.Default.GetDefaultAbbreviation(UnitEnumType, UnitEnumValueInt));
                }
               
                comboBox.ItemsSource = unitsAvailable;                       
                comboBox.SelectedIndex = 0;                
                comboBox.FontSize = 14;
                
                var _binding = new Binding("Unit")
                {
                    Source = propertyItem.Instance,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true,
                    Mode = propertyItem.IsReadOnly ? BindingMode.TwoWay : BindingMode.TwoWay
                };

                BindingOperations.SetBinding(comboBox, ComboBox.SelectedItemProperty, _binding);
            }
            
            return comboBox;
        }
    }

}
