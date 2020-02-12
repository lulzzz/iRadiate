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

using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Radiopharmacy;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;


namespace iRadiate.Whiteboard.Common.View
{
    /// <summary>
    /// Interaction logic for WhiteboardView.xaml
    /// </summary>
    public partial class WhiteboardView : UserControl
    {
        public WhiteboardView()
        {
            
            InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            iRadiate.Desktop.Common.DesktopApplication.ShowDialog("Update", "Whiteboard layout successfully changed");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UnitDoseContextMenuClick(object sender, RoutedEventArgs e)
        {
            var o = sender as Control;
            if(o.DataContext != null)
            {
                if(o.DataContext is DoseAdministrationTask)
                {
                    if((o.DataContext as DoseAdministrationTask).UnitDose != null)
                    {
                        var vm = new BaseUnitDoseViewModel((o.DataContext as DoseAdministrationTask).UnitDose as DataStoreItem);
                        DesktopApplication.MakeModalDocument(vm);
                    }
                    
                }
            }
            
        }
    }

    public class TasksToScans : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
            {
                return null;
            }
            IEnumerable<IDataStoreItem> res = (IEnumerable<IDataStoreItem>)value;
            if(res == null){
                return null;
            }
            return res.Where(y => y.Deleted == false).Where(x => x is ScanTask).OrderBy(y=>(y as ScanTask).SchedulingTime);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TasksToInjections : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            IEnumerable<IDataStoreItem> res = (IEnumerable<IDataStoreItem>)value;
            if (res == null)
            {
                return null;
            }
            return res.Where(y => y.Deleted == false).Where(x => x is DoseAdministrationTask);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CompletedToRotation : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var b = value as bool?;
           if(b.HasValue)
            {
                if (b.Value)
                {
                    return 0.25;
                }else
                {
                    return 1;
                }
                
            }
           else
            {
                return 1;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TaskStatusToOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var b = value as TaskStatus?;
            if (b.HasValue)
            {
                if (b.Value == TaskStatus.Commenced)
                {
                    return 1;
                }
                else if(b.Value == TaskStatus.Completed)
                {
                    return 0.25;
                }
                else
                {
                    return 1;
                }

            }
            else
            {
                return 1;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TaskStatusToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var b = value as TaskStatus?;
            if (b.HasValue)
            {
                if (b.Value == TaskStatus.Commenced)
                {
                    return Brushes.LightGreen;
                }
                else if (b.Value == TaskStatus.Completed)
                {
                    return Brushes.Transparent;
                }
                else
                {
                    return Brushes.Transparent;
                }

            }
            else
            {
                return Brushes.Transparent;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
