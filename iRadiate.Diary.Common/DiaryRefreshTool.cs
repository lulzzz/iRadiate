using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.IconPacks;

using iRadiate.Desktop.Common;

namespace iRadiate.Diary.Common
{
    [Export(typeof(IDiaryTool))]
    public class DiaryRefreshTool : BaseDiaryTool
    {
        
        public DiaryRefreshTool() : base()
        {
            Available = true;
        }

        #region overrides
        public override string Name
        {
            get
            {
                return "Refresh";
            }
        }

        protected override PackIconBase Icon
        {
            get
            {
                var i = new PackIconModern();
                i.Kind = PackIconModernKind.Refresh;
                i.Width = DesktopApplication.IconWidth;
                i.Height = DesktopApplication.IconHeight;
                return i;
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Refresh the diary";
            }
        }

        protected override void DiarySelectedItemChanged(object sender, EventArgs e)
        {
            
        }

        public override int DiaryPositionIndex
        {
            get
            {
                return Properties.Settings.Default.DiaryRefreshToolPositionIndex;
            }

            set
            {
                Properties.Settings.Default.DiaryRefreshToolPositionIndex = value;
            }
        }

        public override bool DiaryVisible
        {
            get
            {
                return Properties.Settings.Default.DiaryRefreshToolVisible;
            }

            set
            {
                Properties.Settings.Default.DiaryRefreshToolVisible = value;
            }
        }
        #endregion


        #region privateMethods
        protected override void Execute()
        {
            DiaryViewModel.RefreshDiaries();
        }
        public override void Save()
        {
            Properties.Settings.Default.Save();
        }
        #endregion
    }

    
}
