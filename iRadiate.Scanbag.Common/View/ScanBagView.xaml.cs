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

using iRadiate.Desktop.Common;
using iRadiate.Scanbag.Common.ViewModel;

namespace iRadiate.Scanbag.Common.View
{
    /// <summary>
    /// Interaction logic for ScanBagView.xaml
    /// </summary>
    public partial class ScanBagView : UserControl
    {
        private ScanBagSection _section;
        public ScanBagView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
            if (e.NewValue is ReportScanBagItem)
            {
                ((ScanBagViewModel)this.DataContext).CurrentItem = ((ReportScanBagItem)e.NewValue);
                ((ScanBagViewModel)this.DataContext).CurrentSection = ((ScanBagViewModel)this.DataContext).CurrentItem.ScanbagSection;
                UserControl uc = iRadiate.Desktop.Common.DesktopApplication.GetView((ReportScanBagItem)e.NewValue);
                uc.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                uc.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                uc.DataContext = ((ReportScanBagItem)e.NewValue).Report;
                ContentHoster.Content = uc;
            }
            else if (e.NewValue is ScanBagSection)
            {
                ((ScanBagViewModel)this.DataContext).CurrentStudy = ((ScanBagSection)e.NewValue).Study;
                ((ScanBagViewModel)this.DataContext).CurrentItem = null;
                ((ScanBagViewModel)this.DataContext).CurrentSection = (ScanBagSection)e.NewValue;
                UserControl uc = iRadiate.Desktop.Common.DesktopApplication.GetViewVMB((ScanBagSection)e.NewValue);
                //uc.HorizontalAlignment = HorizontalAlignment.Stretch;
                uc.DataContext = ((ScanBagSection)e.NewValue).Summary;
                ContentHoster.Content = uc;
                

            }
            else if (e.NewValue is ScanBagItem)
            {
                ((ScanBagViewModel)this.DataContext).CurrentItem = ((ScanBagItem)e.NewValue);
                ((ScanBagViewModel)this.DataContext).CurrentSection = ((ScanBagViewModel)this.DataContext).CurrentItem.ScanbagSection;
                UserControl uc = iRadiate.Desktop.Common.DesktopApplication.GetView((ScanBagItem)e.NewValue);
                uc.DataContext = e.NewValue;
                ContentHoster.Content = uc;

            }
            else
            {
                ContentHoster.Content = null;
            }
            
        }

        private void StackPanel_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //iRadiate.Desktop.Common.Application.ShowDialog("Info", ((TextBlock)e.Source).DataContext.GetType().ToString());
            if (((TextBlock)e.Source).DataContext.GetType().ToString() == "iRadiate.Scanbag.Common.ViewModel.ScanBagSection")
            {
                _section = (ScanBagSection)((TextBlock)e.Source).DataContext;
            }
            
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ScanBagTreeView.SelectedItem == null)
            {
                iRadiate.Desktop.Common.DesktopApplication.ShowDialog("Error", "Select an item from the scan bag before upload");
            }
            if (_section != null)
            {
                UploadFileViewModel f = new UploadFileViewModel(_section);
                
                iRadiate.Desktop.Common.DesktopApplication.MakeModalDocument(f);
            }
            
            
        }

        private void ScanBagTreeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }
    }
}
