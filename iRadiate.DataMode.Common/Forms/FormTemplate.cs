using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Forms
{
    public class FormTemplate : DataStoreItem
    {
        private DateTime _versionDate;
        private int _versionNumber;
        private Form _form;
        private string _comments;
        private int _fontSize;
        private string _fontFamily;
        private List<FormElement> _formElements;
        private List<FormInstance> _instances;
        private int _numberOfColumns;
        private int _numberOfRows;
        private string _columns;
        private string _rows;

        public FormTemplate() : base()
        {

        }

        public DateTime VersionDate
        {
            get { return _versionDate; }
            set { _versionDate = value; }
        }

        public int VersionNumber
        {
            get { return _versionNumber; }
            set { _versionNumber = value; }
        }

        public Form Form
        {
            get { return _form; }
            set { _form = value; }
        }

        public string Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }

        /// <summary>
        /// The default size of the font for the form
        /// </summary>
        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        /// <summary>
        /// The default font used in the form
        /// </summary>
        public string FontFamily
        {
            get { return _fontFamily; }
            set { _fontFamily = value; }
        }

        /// <summary>
        /// The elements which make up this form version
        /// </summary>
        public virtual List<FormElement> FormElements
        {
            get
            {
                if (_formElements == null)
                    _formElements = new List<FormElement>();
                return _formElements;
            }
            set
            {
                _formElements = value;
            }
        }

        public virtual List<FormInstance> Instances
        {
            get
            {
                if (_instances == null)
                    _instances = new List<FormInstance>();
                return _instances;
            }
            set { _instances = value; }
        }

        /// <summary>
        /// The number of columns in the template
        /// </summary>
        public int NumberOfColumns
        {
            get
            {
                var cols = Columns.Split(',');
                return cols.Length;
            }
            
        }

        /// <summary>
        /// The number of rows in the template
        /// </summary>
        public int NumberOfRows
        {
            get
            {
                var rows = Rows.Split(',');
                return rows.Length;
            }
          
        }

        /// <summary>
        /// The widths of the columns in the grid separate by commas, in fractions of the total grid with
        /// </summary>
        /// <remarks>
        /// 4 uneven columns woud be "0.3,0.2,0.3,0.2" to make 4 columns with widths of 30%,20%,30% and 20% of the total grid width
        /// </remarks>
        public string Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        /// <summary>
        /// The heights of the rowa in the grid separate by commas, in fractions of the total grid height
        /// </summary>
        /// <remarks>
        /// 4 uneven rows would be "0.3,0.2,0.3,0.2" to make 4 rows with heights of 30%,20%,30% and 20% of the total grid height
        /// </remarks>
        public string Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }


    }
}
