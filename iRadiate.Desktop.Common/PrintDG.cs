using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Printing;

using iRadiate.DataModel.NucMed;

namespace iRadiate.Desktop.Common
{
    public class PrintDG
    {
        public object GetPropertyValue(object obj, string propertyName)
        {
            var _propertyNames = propertyName.Split('.');

            for (var i = 0; i < _propertyNames.Length; i++)
            {
                if (obj != null)
                {
                    var _propertyInfo = obj.GetType().GetProperty(_propertyNames[i]);
                    if (_propertyInfo != null)
                        obj = _propertyInfo.GetValue(obj);
                    else
                        obj = null;
                }
            }

            return obj;
        }
        public void printDG(ICollectionView objectList, string title, string[] columns, string[] columnNames, int[] columnWidths)
        {
            List<Study> studyListPrint = objectList.Cast<Study>().ToList();
            
            
            

            PrintDialog printDialog = new PrintDialog();
           
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
               
                FlowDocument fd = new FlowDocument();

                Paragraph p = new Paragraph(new Run(title));
                p.FontSize = 18;
                p.FontWeight = FontWeights.SemiBold;
                p.FontStyle = FontStyles.Normal;
                p.FontFamily = new FontFamily("Segoe UI");
                //p.FontSize = 14;
                fd.Blocks.Add(p);

                Table table = new Table();
                TableRowGroup tableRowGroup = new TableRowGroup();
                TableRow r = new TableRow();
                
                fd.PageWidth = printDialog.PrintableAreaWidth;
                fd.PageHeight = printDialog.PrintableAreaHeight;
                fd.BringIntoView();

                fd.TextAlignment = TextAlignment.Center;
                fd.ColumnWidth = fd.PageWidth;
                table.CellSpacing = 0;

                //var headerList = dataGrid.Columns.Select(e => e.Header.ToString()).ToList();
               


                for (int j = 0; j < columns.Length; j++)
                {
                    TableColumn tc = new TableColumn();
                    tc.Width = new GridLength(columnWidths[j]);
                    table.Columns.Add(tc);

                    r.Cells.Add(new TableCell(new Paragraph(new Run(columnNames[j]))));
                    //r.Cells[j].ColumnSpan = 4;
                    r.Cells[j].Padding = new Thickness(4);
                    r.Cells[j].BorderBrush = Brushes.Black;
                    r.Cells[j].FontWeight = FontWeights.Bold;
                    r.Cells[j].Background = Brushes.DarkGray;
                    r.Cells[j].Foreground = Brushes.White;
                    r.Cells[j].BorderThickness = new Thickness(1, 1, 1, 1);
                    r.Cells[j].TextAlignment = TextAlignment.Left;
                }
                tableRowGroup.Rows.Add(r);
                table.RowGroups.Add(tableRowGroup);
                
                foreach(object o in studyListPrint)
                {

                    
                    //object o = objectList.ElementAt(i);
                    //if (dataGrid.ItemsSource.ToString().ToLower() == "system.data.linqdataview")
                    //{ row = (Study)dataGrid.Items.GetItemAt(i); }
                    //else
                    //{
                    //    row = (Study)dataGrid.Items.GetItemAt(i);

                    //}

                    table.BorderBrush = Brushes.Gray;
                    table.BorderThickness = new Thickness(1, 1, 0, 0);
                    table.FontStyle = FontStyles.Normal;
                    table.FontFamily = new FontFamily("Segoe UI");
                    table.FontSize = 13;
                    tableRowGroup = new TableRowGroup();
                    r = new TableRow();
                    for (int z = 0; z < columns.Length; z++)
                    {
                        
                            string cellContent = "";

                        ////DesktopApplication.ShowDialog("column", colName);
                        ////if (colName.Contains("."))
                        ////{
                        try
                        {
                            object q = GetPropertyValue(o, columns[z]);



                            if (q == null)
                                cellContent = "";
                            else
                                cellContent = q.ToString();
                        }
                        catch
                        {
                            cellContent = "";
                        }       

                            ////    cellContent = innerObject.ToString();
                            ////}
                            ////else
                            ////{
                              //cellContent = o.GetType().GetProperty(columns[z].ToString()).GetValue(o).ToString();
                            ////}
                            //cellContent = GetPropertyValue(o, columns[z]).ToString();
                            r.Cells.Add(new TableCell(new Paragraph(new Run(cellContent))));
                            //r.Cells[z].ColumnSpan = 4;
                            r.Cells[z].Padding = new Thickness(4);
                            r.Cells[z].BorderBrush = Brushes.DarkGray;
                            r.Cells[z].BorderThickness = new Thickness(0, 0, 1, 1);
                            r.Cells[z].TextAlignment = TextAlignment.Left;

                    }

                    tableRowGroup.Rows.Add(r);
                    table.RowGroups.Add(tableRowGroup);

                }
                fd.Blocks.Add(table);

                printDialog.PrintDocument(((IDocumentPaginatorSource)fd).DocumentPaginator, "");

            }
        }

    }
}
