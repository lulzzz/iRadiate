using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Reporting.View
{
    /// <summary>
    /// Interaction logic for ReportingView.xaml
    /// </summary>
    public partial class ReportingView : UserControl
    {
        public ReportingView()
        {
            InitializeComponent();
        }

        private void QueryExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            QueryResultsGrid.Columns.Clear();
            //var g = (SelectedPropertiesGrid.ItemsSource as ICollectionView).SourceCollection.;
            foreach (var p in SelectedPropertiesGrid.ItemsSource.Cast<IQueryableProperty>().OrderBy(x=>x.ColumnOrder))
            {
                if (((IQueryableProperty)p).IsReturning)
                {
                    DataGridTextColumn t = new DataGridTextColumn();
                    var tmp = ((IQueryableProperty)p).Name.Split('>');
                    if(p.ColumnHeader != string.Empty && p.ColumnHeader != null)
                    {
                        t.Header = p.ColumnHeader;
                    }
                    else
                    {
                        t.Header = Regex.Replace(tmp.Last(), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
                    }
                    
                    t.Binding = new Binding(((IQueryableProperty)p).Name);
                    if (((IQueryableProperty)p).Format != string.Empty)
                    {
                        t.Binding.StringFormat = ((IQueryableProperty)p).Format;
                    }
                    t.SortMemberPath = ((IQueryableProperty)p).Name;
                    QueryResultsGrid.Columns.Add(t);
                }
            }
        }

        private void ExcelButton_Click(object sender, RoutedEventArgs e)
        {
           
            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(iRadiate.Common.IO.FileUtility.DataDirectory);

            string columnHeaders = "";
            foreach (var p in SelectedPropertiesGrid.ItemsSource.Cast<IQueryableProperty>().Where(j => j.IsReturning).OrderBy(x => x.ColumnOrder))
            {
                if(p.ColumnHeader == string.Empty || p.ColumnHeader == null)
                {
                    columnHeaders = columnHeaders + Regex.Replace(p.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ") + ",";
                }
                else
                {
                    columnHeaders = columnHeaders + p.ColumnHeader + ",";
                }
                
            }
            var sb = new StringBuilder();
            columnHeaders = columnHeaders.Remove(columnHeaders.Length - 1);
            sb.AppendLine(columnHeaders);
            System.Diagnostics.Debug.WriteLine(columnHeaders);
            var headers = QueryResultsGrid.Columns;
            
           
            foreach(var dr in QueryResultsGrid.ItemsSource)
            {
                string thisRow = "";
               
                var data = (IDictionary<String, Object>)dr;
               
               


                foreach (var p in SelectedPropertiesGrid.ItemsSource.Cast<IQueryableProperty>().Where(j=>j.IsReturning).OrderBy(x => x.ColumnOrder))
                {
                    if(data[p.Name] == null)
                    {
                        thisRow = thisRow + "" + ",";
                    }
                    else
                    {
                        if(p.PropertyType == QueryablePropertyType.DateTime)
                        {
                            if(Convert.ToDateTime(data[p.Name]) == new DateTime())
                            {
                                thisRow = thisRow + "" + ",";
                            }
                            else
                            {
                                thisRow = thisRow + data[p.Name] + ",";
                            }
                        }else
                        {
                            thisRow = thisRow + data[p.Name] + ",";
                        }
                        
                    }
                    

                }
                thisRow = thisRow.Remove(thisRow.Length - 1);
                sb.AppendLine(thisRow);
                
            }
            System.IO.StreamWriter writer = new System.IO.StreamWriter(System.IO.Path.Combine(iRadiate.Common.IO.FileUtility.DataDirectory, "temp.csv"));

            writer.WriteLine(sb.ToString());
            writer.Dispose();
            System.Diagnostics.Process.Start(System.IO.Path.Combine(iRadiate.Common.IO.FileUtility.DataDirectory, "temp.csv"));
            //var rows = GetDataGridRows(QueryResultsGrid);

            //foreach (DataGridRow row in rows)
            //{
            //    var cells = row.
            //    sb.AppendLine(string.Join(",", cells.Select(cell => "\"" + cell.Value + "\"").ToArray()));
            //}
        }

        public IEnumerable<System.Windows.Controls.DataGridRow> GetDataGridRows(System.Windows.Controls.DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as System.Windows.Controls.DataGridRow;
                if (null != row) yield return row;
            }
        }
    }
}
