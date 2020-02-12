using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using iRadiate.DataModel.Equipment;
using iRadiate.DataModel.Forms;

namespace iRadiate.Desktop.Common.Forms
{
    /// <summary>
    /// Interaction logic for FormViewer.xaml
    /// </summary>
    public partial class FormViewer : UserControl
    {
        public event EventHandler ControlGotFocus;
        public FormViewer()
        {
            InitializeComponent();
            
            this.DataContextChanged += FormViewer_DataContextChanged;
        }
        public void RaiseDataContextChanged()
        {
            FormViewer_DataContextChanged(null, new DependencyPropertyChangedEventArgs());
        }
        private void FormViewer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           
            if(!(this.DataContext is FormInstance))
            {
                return;
            }
            ///Ok so the datacontex wil be a Forminstance
            FormInstance f = (FormInstance)this.DataContext;
            //does this forminstance already have dataitems
            
            //We clear all the existing elements from the Grid
            FormGrid.Children.Clear();
            //Set up the rows and columns
            FormGrid.RowDefinitions.Clear();
            FormGrid.ColumnDefinitions.Clear();
            var rows = f.FormTemplate.Rows.Split(',');
            for(int i = 0; i < f.FormTemplate.NumberOfRows; i++)
            {

                RowDefinition def = new RowDefinition();
                Binding widthBinding = new Binding("ActualHeight");
                widthBinding.Source = FormGrid;
                widthBinding.Converter = new FractionConverter();
                widthBinding.ConverterParameter = Convert.ToDouble(rows[i]);
                def.SetBinding(RowDefinition.HeightProperty, widthBinding);
                FormGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(Convert.ToDouble(rows[i]),GridUnitType.Star) });
            }
            var cols = f.FormTemplate.Columns.Split(',');
            for (int i = 0; i < f.FormTemplate.NumberOfColumns; i++)
            {
              
                ColumnDefinition def = new ColumnDefinition();
                Binding widthBinding = new Binding("ActualWidth");
                widthBinding.Source = FormGrid;
                widthBinding.Converter = new FractionConverter();
                widthBinding.ConverterParameter = Convert.ToDouble(cols[i]);
                def.SetBinding(ColumnDefinition.WidthProperty, widthBinding);
                //FormGrid.ColumnDefinitions.Add(def);
                FormGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Convert.ToDouble(cols[i]), GridUnitType.Star) });
            }

            ///Now we iterate the elements that make up this template
            ///and add them to the grid
            ///for each form element, we add a control, and we check if the FormInstance already has a
            ///QADataItem that matches in terms of its DataDictionaryEntry and EquipmentItem
            ///if we have match, then we assign that dataitem to the Control we just added
            foreach (FormElement elm in f.FormTemplate.FormElements)
            {
                
                if(elm is LabelFormElement)
                {
                    LabelFormElement l = (LabelFormElement)elm;                    
                    Label X = new Label();
                    X.HorizontalContentAlignment = HorizontalAlignment.Center;
                    X.VerticalContentAlignment = VerticalAlignment.Center;
                    X.Content = l.LabelText;
                   
                    setControlLayout(l, X);
                }
                else if(elm is BooleanFormElement)
                {
                    BooleanFormElement b = (BooleanFormElement)elm;
                    CheckBox X = new CheckBox();
                    QADataItem qaDataItem = null;
                    if(f.DataItems.Where(x=>x.DataDictionaryEntry.FullName == b.DataDictionaryEntry.FullName).Any())
                    {
                        //qaDataItem = (QADataItem)f.DataItems.Where(x => x.DataDictionaryEntry.FullName == b.DataDictionaryEntry.FullName).First();
                    }
                    else
                    {
                        //qaDataItem = new QADataItem();
                        //qaDataItem.EquipmentItem = b.EquipmentItem;
                    }
                    
                    setControlLayout(b, X);
                }
                else if(elm is NumericFormElement)
                {
                    ///create refernce to native type
                    NumericFormElement n = (NumericFormElement)elm;
                    ///Create the control
                    MahApps.Metro.Controls.NumericUpDown t = new MahApps.Metro.Controls.NumericUpDown();
                    t.Maximum = n.Maximum;
                    t.MinHeight = n.Minimum;
                    t.StringFormat = "{}{0:F" + n.NumberOfDecimals.ToString() + "} " + n.Unit;
                    
                    ///We should never have a NumericFormElement without a DataDictionaryEntry
                    if (n.DataDictionaryEntry != null)
                    {
                        ///This object is the underlying QADataItem that the control will bind to.
                        QADataItem d = null;
                        if (f.DataItems.Where(x => x.DataDictionaryEntry.FullName == n.DataDictionaryEntry.FullName && (x as QADataItem).EquipmentItem.FullName == n.EquipmentItem.FullName).Any())
                        {
                            ///This FormInstance already has a QADataItem that matches on DataDictionaryEntry and EquipmentItem, so let's retrieve that one.
                            d = f.DataItems.Where(x => x.DataDictionaryEntry.FullName == n.DataDictionaryEntry.FullName && (x as QADataItem).EquipmentItem.FullName == n.EquipmentItem.FullName).First() as QADataItem;
                            
                        }                       
                        else
                        {
                            d = new QuantifiableQADataItem
                            {
                                EquipmentItem = n.EquipmentItem,
                                DataDictionaryEntry = n.DataDictionaryEntry,
                                QAFormInstance = f as QAFormInstance
                                 
                            };
                          

                        }

                        Binding bd = new Binding("Value");
                        bd.Source = d;                        
                       
                        bd.Mode = BindingMode.TwoWay;
                        BindingOperations.SetBinding(t, MahApps.Metro.Controls.NumericUpDown.ValueProperty, bd);
                    }
                    else
                    {
                        // / we should log this
                    }
                    
                    
                    //t.LostFocus += new RoutedEventHandler((senderX, eX) => FormatNumericElement(senderX, eX, n.Format + n.NumberOfDecimals));
                    setControlLayout(n, t);
                }else if(elm is TextFormElement)
                {
                    TextFormElement tfe = (TextFormElement)elm;
                    TextBox t = new TextBox();
                    setControlLayout(tfe, t);
                }
            }
        }
        

        private void FormatNumericElement(object sender, RoutedEventArgs e, string format)
        {
            MahApps.Metro.Controls.NumericUpDown t = (MahApps.Metro.Controls.NumericUpDown)sender;
            double d = 0;
            //if(Double.TryParse(t.Text,out d))
            //{
            //    if (format.Contains("P"))
            //        d = d / 100;
            //    t.Text = d.ToString(format);
            //    t.BorderThickness = new Thickness(1);
            //    t.BorderBrush = Brushes.LightGray;
            //}
            //else
            //{
                
            //    t.BorderThickness = new Thickness(2);
            //    t.BorderBrush = Brushes.Red;
                
                
            //}
           
            
        }

        private void setControlLayout(FormElement element, Control X)
        {
            X.GotFocus += X_GotFocus;
            X.Margin = new Thickness(2);
            X.VerticalAlignment = VerticalAlignment.Stretch;
            X.HorizontalAlignment = HorizontalAlignment.Stretch;
            X.HorizontalContentAlignment = element.HorizontalAlignment;
            X.SetValue(Grid.ColumnSpanProperty, element.ColumnSpan);
            X.SetValue(Grid.RowSpanProperty, element.RowSpan);
            FormGrid.Children.Add(X);
            X.SetValue(Grid.RowProperty, element.Row);
            X.SetValue(Grid.ColumnProperty, element.Column);
            X.FontFamily = new FontFamily(element.FontFamily);
            
            if (element.Foreground == null)
                X.Foreground = Brushes.Black;
            else
                X.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(element.Foreground);

            if (element.Background == null)
                X.Background = Brushes.White;
            else
                X.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(element.Background);

            X.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(element.FontWeight);

            X.FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(element.FontStyle);

            //if (element.Width > 0)
            //{

            //    Binding widthBinding = new Binding("ActualWidth");
            //    widthBinding.Source = FormGrid;
            //    widthBinding.Converter = new FractionConverter();
            //    widthBinding.ConverterParameter = Convert.ToDouble(element.Width);
            //    X.SetBinding(Control.WidthProperty, widthBinding);

            //}
            //else
            //    X.HorizontalAlignment = HorizontalAlignment.Stretch;

            //if (element.Height > 0)
            //{
            //    Binding heightBinding = new Binding("ActualHeight");
            //    heightBinding.Source = FormGrid;
            //    heightBinding.Converter = new FractionConverter();
            //    heightBinding.ConverterParameter = Convert.ToDouble(element.Height);
            //    X.SetBinding(Control.HeightProperty, heightBinding);
            //}
            //else
            //    X.VerticalAlignment = VerticalAlignment.Stretch;



            if (element.FontSize > 0)
                X.FontSize = element.FontSize;
            else
                X.FontSize = 14;
            if(X is Label || X is TextBox || X is TextBlock)
            {
                Binding fontBinding = new Binding("ActualWidth");
                fontBinding.Source = X;
                fontBinding.Converter = new FontSizeConverter();
                ElementAndControl ec = new ElementAndControl();
                ec.Cont = X;
                ec.Elem = element;
                fontBinding.ConverterParameter = ec;
               
                X.SetBinding(TextBox.FontSizeProperty, fontBinding);
            }
           

            if(element is DataFormElement)
            {
               
                if (((DataFormElement)element).DataDictionaryEntry != null)
                {
                    X.ToolTip = ((DataFormElement)element).DataDictionaryEntry.Description;
                }
                
            }
        }

        private void X_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ControlGotFocus != null)
                ControlGotFocus(sender, e);
        }
    }

    public class ElementAndControl
    {
        public Control Cont { get; set; }
        public FormElement Elem { get; set; }
    }
    public class FractionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double parent = (double)value;
            double fraction = (double)parameter;
            return parent * fraction;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ec = (ElementAndControl)parameter;
            string text = "";
            if (ec.Cont is Label)
                text = ((Label)ec.Cont).Content.ToString();
            if (ec.Cont is TextBox)
                text = ((TextBox)ec.Cont).Text;

            double width = (double)value;

            if ((text.Length * (ec.Elem.FontSize/2)) < width)
                return ec.Elem.FontSize;
            else
                return (int)((width*2) / text.Length);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
