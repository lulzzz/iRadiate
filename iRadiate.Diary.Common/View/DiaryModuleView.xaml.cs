using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Xceed.Wpf.Toolkit;


using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.Common.IO;
using iRadiate.Desktop.Common;
using iRadiate.Diary.Common.ViewModel;
using System.Windows.Controls;

namespace iRadiate.Diary.Common.View
{
    /// <summary>
    /// Interaction logic for DiaryModuleView.xaml
    /// </summary>
    public partial class DiaryModuleView : UserControl
    {
        private Border selectedPanel;
        private Control parent;
        private Point startingPoint;
        public DiaryModuleView()
        {
            InitializeComponent();
          
        }

        private void UserControl_DragOver(object sender, DragEventArgs e)
        {
            //I dont know
        }

        private void DateTimeUpDown_MouseEnter(object sender, MouseEventArgs e)
        {
            (e.Source as TimePicker).ShowButtonSpinner = true;
            (e.Source as TimePicker).ShowDropDownButton = true;

            selectedPanel = null;
        }

        private void DateTimeUpDown_MouseLeave(object sender, MouseEventArgs e)
        {
            
            (e.Source as TimePicker).ShowButtonSpinner = false;
            (e.Source as TimePicker).ShowDropDownButton = false;
            selectedPanel = null;
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
           //DesktopApplication.ShowDialog("Debug","sender type is " + sender.GetType().Name);
            Border b = sender as Border;
            DiaryEventWrapper dew = b.DataContext as DiaryEventWrapper;
            dew.IsSelected = true;            
                
            
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectedPanel = null;
           
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            
        }
    }

    public class EventStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is EventStatus)
            {
                if ((EventStatus)value == EventStatus.Completed)
                    return Brushes.LightGray;
                else if((EventStatus)value == EventStatus.Commenced)
                    return Brushes.LightGreen;
                else
                    return Brushes.LightBlue;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                    return "yes";
                else
                    return "no";
            }
            return "no";
        }
    }

    public class IsSelectedToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                    return Brushes.Black;
                
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                    return "yes";
                else
                    return "no";
            }
            return "no";
        }
    }
}
