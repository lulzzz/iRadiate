using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;
using iRadiate.Whiteboard.Common;

namespace iRadiate.Scanbag.Common.ViewModel
{

    [PreferredView("iRadiate.Scanbag.Common.View.NoteScanbagItemView", "iRadiate.Scanbag.Common")]
    public class NoteScanbagItem : ScanBagItem
    {

        private string _noteText;
        private User _author;
        //private Note _note;

        public NoteScanbagItem()
        {
            //_note = n;
        }
        public override string Description
        {
            get
            {
                //return _note.Title; ;
                return "";
            }
            set
            {

            }
        }

        public string NoteText
        {
            get
            {
                //return _note.Text;
                return "";
            }
            set
            {
               
            }
        }

        public User Author
        {
            get
            {
                //return _note.User;
                return null;
            }
            set
            {
                
            }
        }

        public DateTime Date
        {
            get
            {
                //return _note.CreationDate;
                return new DateTime();
            }
        }

        
    }
}
