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

using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Common.Diary
{
    /// <summary>
    /// Interaction logic for DiaryEntryView.xaml
    /// </summary>
    public partial class DiaryEntryView : UserControl
    {
        private Point _startPoint;
        private bool IsDragging = false;
        public DiaryEntryView()
        {
            InitializeComponent();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            DiaryEntryView ellipse = sender as DiaryEntryView;
            if (ellipse != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Console.WriteLine("ellipse.GetType().ToString() = " + ellipse.GetType().ToString() + "ellipse.DataContext.GetType().ToString() = " + ellipse.DataContext.GetType().ToString());
                DragDrop.DoDragDrop(ellipse, ellipse.DataContext, DragDropEffects.Move);
            }

        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MenuPopup.IsOpen = true;
        }

        private void MenuPopup_LostFocus(object sender, RoutedEventArgs e)
        {
            MenuPopup.IsOpen = false;
        }

        private void MenuPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuPopup.IsOpen = false;
        }

        private void ActionPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            ActionPopup.IsOpen = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ActionPopup.IsOpen = true;
        }


       
    }
}
    
