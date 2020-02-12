using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRadiate.Desktop.Common.Tools;
using iRadiate.Diary.Common.ViewModel;

namespace iRadiate.Diary.Common
{
    
    public class DiarySplitButtonTool : BaseSplitButtonTool, IDiaryTool
    {
        private DiaryViewModel _diaryViewModel;
        public DiarySplitButtonTool() : base() { Available = true; }

        public int DiaryPositionIndex
        {
            get
            {
                return 10;
            }

            set
            {
                
            }
        }

        public DiaryViewModel DiaryViewModel
        {
            get
            {
                return _diaryViewModel;
            }

            set
            {
                _diaryViewModel = value;
                _diaryViewModel.SelectedItemChanged += DiarySelectedItemChanged;
                RaisePropertyChanged("DiaryViewModel");
            }
        }

        protected virtual void DiarySelectedItemChanged(object sender, EventArgs e)
        {

        }

        public bool DiaryVisible
        {
            get
            {
                return true;
            }

            set
            {
               
            }
        }
    }
}
