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

namespace iRadiate.Radiopharmacy.View
{
    /// <summary>
    /// Interaction logic for ReconstitutionView.xaml
    /// </summary>
    public partial class ReconstitutionView : UserControl
    {
        public ReconstitutionView()
        {
            InitializeComponent();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F1)
            {
                object o = this.DataContext;
                if(o is ReconstitutionViewModel)
                {
                    (o as ReconstitutionViewModel).ReadActivityCommand.Execute(null);
                }
            }
        }
    }
}
