﻿using System;
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

namespace iRadiate.Settings.Common.View
{
    /// <summary>
    /// Interaction logic for StaffMemberRoleListView.xaml
    /// </summary>
    public partial class StaffMemberRoleListView : UserControl
    {
        public StaffMemberRoleListView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
           
        }

        private void RoleTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}