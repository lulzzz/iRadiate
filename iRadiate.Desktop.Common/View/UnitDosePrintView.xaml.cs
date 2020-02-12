using System;
using System.Collections.Generic;
using System.Drawing;
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

using iRadiate.Common.Misc;
using iRadiate.DataModel.Radiopharmacy;

using iRadiate.Desktop.Common;

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for UnitDosePrintView.xaml
    /// </summary>
    public partial class UnitDosePrintView : UserControl
    {
        public UnitDosePrintView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PreCalTimeBox.Visibility = Visibility.Visible;
            PreCalTimePicker.Visibility = Visibility.Hidden;

            var printDialog = new PrintDialog();
            if (Properties.Settings.Default.ConfirmPrinterForLabels)
            {
                if (printDialog.ShowDialog() == true)
                {
                    MainDockPanel.Children.Clear();
                    FlowDocument fd = new FlowDocument();
                    fd.DataContext = MainDockPanel.DataContext;
                    fd.Blocks.Add(new BlockUIContainer(LabelGrid));
                    fd.PagePadding = new Thickness(20);
                    fd.PageHeight = printDialog.PrintableAreaHeight;
                    fd.PageWidth = printDialog.PrintableAreaWidth;
                    var paginator = ((IDocumentPaginatorSource)fd).DocumentPaginator;
                    printDialog.PrintDocument(paginator, "Dose Label");
                    DesktopApplication.CloseActiveWindow();
                }
            }
            else
            {
                MainDockPanel.Children.Clear();
                FlowDocument fd = new FlowDocument();
                fd.DataContext = MainDockPanel.DataContext;
                fd.Blocks.Add(new BlockUIContainer(LabelGrid));
                fd.PagePadding = new Thickness(20);
                fd.PageHeight = printDialog.PrintableAreaHeight;
                fd.PageWidth = printDialog.PrintableAreaWidth;
                var paginator = ((IDocumentPaginatorSource)fd).DocumentPaginator;
                printDialog.PrintDocument(paginator, "Dose Label");
                DesktopApplication.CloseActiveWindow();
            }
            
            
            
        }
    }

    public class CompletionDetailsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string notAdministered = "________________________";
            if (value == null)
                return notAdministered;

            BaseUnitDose d = value as BaseUnitDose;
            if (d == null)
                return notAdministered;

            if (!d.Administered)
                return notAdministered;

            return d.AdministrationDate.ToShortTimeString() + " (" + DecayCorrecter.Decay(d.CalibrationDate,d.AdministrationDate,d.Radiopharmaceutical.Isotope.HalfLife,d.CalibrationActivity).ToString("f1") +" MBq)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CompletionUserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string notAdministered = "________________________";
            if (value == null)
                return notAdministered;

            BaseUnitDose d = value as BaseUnitDose;
            if (d == null)
                return notAdministered;

            if (!d.Administered)
                return notAdministered;

            return d.DoseAdministrationTask.Assignee.FullName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
