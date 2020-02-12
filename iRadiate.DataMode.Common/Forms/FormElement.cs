using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Forms
{
    public abstract class FormElement : DataStoreItem
    {
        private double _top;
        private double _left;
        private double _height;
        private double _width;
        private int _fontSize;
        private string _fontFamily;
        private FormTemplate _formVersion;
        private int _row;
        private int _column;
        private int _rowSpan;
        private int _columnSpan;
        private VerticalAlignment _verticalAlignment;
        private HorizontalAlignment _horizontalAlignment;
        private string _foreground;
        private string _background;
        private string _name;
        private string _fontweight;
        private string _fontStyle;

        public FormElement() : base()
        {
            _verticalAlignment = VerticalAlignment.Center;
            _horizontalAlignment = HorizontalAlignment.Center;
            _rowSpan = 1;
            _columnSpan = 1;
            _fontFamily = "Segoe UI";
            _name = "Unnamed form element";
            _foreground = "Black";
            _background = "White";
            _fontweight = "Normal";
            _fontStyle = "Normal";
        }

        /// <summary>
        /// The FormVersion that this element belongs to
        /// </summary>
        public virtual FormTemplate FormTemplate
        {
            get { return _formVersion; }
            set { _formVersion = value; }
        }

        /// <summary>
        /// The distance (in %) from the top of the page
        /// </summary>
        public double Top
        {
            get { return _top; }
            set { _top = value; }
        }

        /// <summary>
        /// The distance (in %) from the left of the page
        /// </summary>
        public double Left
        {
            get { return _left; }
            set { _left = value; }
        }

        /// <summary>
        /// The height of the element, measured in % of page
        /// </summary>
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// The width of the element, measred in % of page
        /// </summary>
        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// The size of the font for the element
        /// </summary>
        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        /// <summary>
        /// the font used in the element
        /// </summary>
        public string FontFamily
        {
            get { return _fontFamily; }
            set { _fontFamily = value; }
        }

        public string FontWeight
        {
            get { return _fontweight; }
            set { _fontweight = value; }
        }

        public string FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }

        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }

        public int RowSpan
        {
            get { return _rowSpan; }
            set { _rowSpan = value; }
        }

        public int ColumnSpan
        {
            get { return _columnSpan; }
            set { _columnSpan = value; }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set { _verticalAlignment = value; }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set { _horizontalAlignment = value; }
        }

        public string Foreground
        {
            get { return _foreground; }
            set { _foreground = value; }
        }

        public string Background
        {
            get { return _background; }
            set { _background = value; }
        }

        /// <summary>
        /// The name of the element within its template
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
         
    }


}
