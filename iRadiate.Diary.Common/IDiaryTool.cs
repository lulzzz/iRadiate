using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using GalaSoft;
using GalaSoft.MvvmLight;

using iRadiate.Diary.Common.ViewModel;
using iRadiate.Desktop.Common.Tools;

namespace iRadiate.Diary.Common
{
    public interface IDiaryTool : ITool
    {
        bool DiaryVisible { get; set; }

        int DiaryPositionIndex { get; set; }

        DiaryViewModel DiaryViewModel { get; set; }

      
    }

    public abstract class BaseDiaryTool : BaseExecutableTool, IDiaryTool
    { 
        private DiaryViewModel _diaryViewModel;
       
       
        public BaseDiaryTool():base()
        {

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

       

        public virtual int DiaryPositionIndex
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool DiaryVisible
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        protected override void Execute()
        {
            base.Execute();
        }

        public override void Save()
        {
            base.Save();
        }

        
    }

  
}
