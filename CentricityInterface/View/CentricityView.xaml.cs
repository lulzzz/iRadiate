using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace iRadiate.Interfaces.CentricityInterface.View
{
    /// <summary>
    /// Interaction logic for CentricityView.xaml
    /// </summary>
    public partial class CentricityView : UserControl
    {
        public CentricityView()
        {
            InitializeComponent();
        }
    }

    public class AdministrationRouteToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            return value.ToString();

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "")
                return null;
            iRadiate.DataModel.NucMed.AdministrationRoute x = iRadiate.DataModel.NucMed.AdministrationRoute.Intravenous;
            if (Enum.TryParse(value.ToString(), out x))
           {
                return x;
            }
            return null;
        }
    }

    public class EnumToStringList : IValueConverter
    {
       

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new List<string>();
            result.Add("");
            result.AddRange(Enum.GetNames(typeof(iRadiate.DataModel.NucMed.AdministrationRoute)));
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
