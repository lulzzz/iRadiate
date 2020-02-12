using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

using System.Windows.Controls;
using System.Windows.Data;

namespace iRadiate.Whiteboard.Common
{
    public interface IColumnProvider
    {
        DataGridColumn Column {get;}
        string Name { get;  }

    }

    public abstract class BaseColumn : IColumnProvider
    {
        protected DataGridColumn _column;
        protected string _name;

        public BaseColumn()
        {
            //Put anything fundamental here;
        }
        public DataGridColumn Column
        {
            get
            {
                return _column;
            }
            
        }

        public string Name
        {
            get
            {
                return _name;
            }
            
        }
    }

   [Export(typeof(IColumnProvider))]
    public class SurnameColumn : BaseColumn
    {
        public SurnameColumn()
        {
            _name = "Surname";
           _column  = new DataGridTextColumn();
           _column.Header = "Surname";
           _column.CanUserReorder = true;
           
           _column.SortMemberPath = "Patient.Surname";
           Binding b = new Binding("Patient.Surname");
           ((DataGridTextColumn)_column).Binding = b;

        }
    }

   [Export(typeof(IColumnProvider))]
   public class GivenNamesColumn : BaseColumn
   {
       public GivenNamesColumn()
       {
           _name = "Given Names";
           _column = new DataGridTextColumn();
           _column.Header = "Given Names";
           _column.CanUserReorder = true;
         
           _column.SortMemberPath = "Patient.GivenNames";
           Binding b = new Binding("Patient.GivenNames");
           ((DataGridTextColumn)_column).Binding = b;

       }
   }

   [Export(typeof(IColumnProvider))]
   public class FullNameProvider : BaseColumn
   {
       public FullNameProvider()
       {
           _name = "Full Name";
           _column = new DataGridTextColumn();
           _column.Header = "Full Name";
           _column.CanUserReorder = true;
           
           _column.SortMemberPath = "Patient.FullName";
           Binding b = new Binding("Patient.FullName");
           ((DataGridTextColumn)_column).Binding = b;

       }
   }
}
