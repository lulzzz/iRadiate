using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iRadiate.Common.IO
{
    public class TextFileReader
    {
        public static DataTable ReadCSVFile(string path, bool headers)
        {
            DataTable dt = new DataTable();
            
            
            StreamReader st = new StreamReader(path);
            string line;
            int counter = 0;
            while ((line = st.ReadLine()) != null)
            {
                char delimiter = ',';
                string[] cols = Regex.Split(line, ",(?=(?:[^']*'[^']*')*[^']*$)");
                //string[] cols = line.Split(delimiter);
                if (counter == 0)
                {
                    if (headers == true)
                    {
                        for (int x = 0; x < cols.Length; x++)
                        {
                            dt.Columns.Add(cols[x],typeof(string));
                        }
                        counter++;
                        continue;
                    }
                    else
                    {
                        string alphabet = "abcdefghijklmnopqrstuvwxyz";
                        for (int x = 0; x < cols.Length; x++)
                        {
                            dt.Columns.Add(alphabet.Substring(x, 1), typeof(string));
                        }
                        
                    }
                }
                
                //System.Console.WriteLine(line);
                
                DataRow dr = dt.NewRow();
                for (int i = 0; i < cols.Count(); i++)
                {
                    dr[i] = cols[i];
                }

                dt.Rows.Add(dr);

                counter++;

            }

            st.Close();

            return dt;

        }
        public static DataTable ReadTabFile(string path)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("a",typeof(string));
            dt.Columns.Add("b", typeof(string));
            dt.Columns.Add("c", typeof(string));
            dt.Columns.Add("d", typeof(string));
            dt.Columns.Add("e", typeof(string));
            dt.Columns.Add("f", typeof(string));
            dt.Columns.Add("g", typeof(string));
            dt.Columns.Add("h", typeof(string));
            dt.Columns.Add("i", typeof(string));
            dt.Columns.Add("j", typeof(string));
            dt.Columns.Add("k", typeof(string));
            dt.Columns.Add("l", typeof(string));
            StreamReader st = new StreamReader(path);
            string line;
            int counter = 0;
            while ((line = st.ReadLine()) != null)
            {

                                //System.Console.WriteLine(line);
                char delimiter = '\t';
                string[] cols = line.Split(delimiter);
                DataRow dr = dt.NewRow();
                for (int i = 0; i < cols.Count(); i++)
                {
                    dr[i] = cols[i];
                }
                    
                dt.Rows.Add(dr);

                counter++;
                
            }

            st.Close();

            return dt;

        }
    }
}
