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

using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Common.View
{
    /// <summary>
    /// Interaction logic for NucMedPracticeView.xaml
    /// </summary>
    public partial class NucMedPracticeView : UserControl
    {
        public NucMedPracticeView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ((NucMedPracticeViewModel)this.DataContext).SelectedStaffMemberRole = (StaffMemberRoleViewModel)MyTreeView.SelectedItem;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            MyTreeView.Items.Refresh();
            MyTreeView.UpdateLayout();
        }
    }
}
