using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
//using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using iRadiate.DataModel.NucMed;

using iRadiate.Desktop.Common.ViewModel;

namespace iRadiate.Desktop.Common.Diary
{
    public delegate void TaskDraggedOutHandler(object sender, EventArgs e);

    /// <summary>
    /// Interaction logic for DiaryEntryHolderView.xaml
    /// </summary>
    public partial class DiaryEntryHolderView : UserControl
    {
        private bool itemClicked = false;
        private BaseTaskViewModel movingModel;
        private double yOffset = 0;
        private bool weAreDragging = false;

        public event TaskDraggedOutHandler DraggedOut;

        protected virtual void OnDraggedOut(EventArgs e)
        {
            if (DraggedOut != null)
            {
                DraggedOut(this, e);
            }
        }
        
        public DiaryEntryHolderView()
        {
            InitializeComponent();
        }

        private void UserControl_DragOver(object sender, DragEventArgs e)
        {
                  
            

        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {

            var taskViewModel = (BaseTaskViewModel)e.Data.GetData("myFormat");
            taskViewModel.GetType().GetProperty((this.DataContext as DiaryEntryHolderViewModel).PropertyName).SetValue(taskViewModel, (this.DataContext as DiaryEntryHolderViewModel).ThisItem.Item, null);
            var obj = e.Data.GetData(e.Data.GetFormats()[0]);
            if (typeof(BaseTaskViewModel).IsAssignableFrom(obj.GetType()))
            {
                if (((DiaryEntryHolderViewModel)this.DataContext).PropertyName == "Role")
                {
                    ((BaseTaskViewModel)obj).Role = (StaffMemberRole)((DiaryEntryHolderViewModel)this.DataContext).ThisItem.Item;
                    ((DiaryEntryHolderViewModel)this.DataContext).TasksView.Refresh();
                }
                else if (((DiaryEntryHolderViewModel)this.DataContext).PropertyName == "Room")
                {
                    ((BaseTaskViewModel)obj).Room = (Room)((DiaryEntryHolderViewModel)this.DataContext).ThisItem.Item;
                    ((DiaryEntryHolderViewModel)this.DataContext).TasksView.Refresh();
                }





            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            //Console.WriteLine("OnDrop");
            
           
            
        }

      

        private void DiaryEntryView_MouseMove(object sender, MouseEventArgs e)
        {
            
           
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if(movingModel != null)
                {
                    DiaryEntryHolderViewModel d = this.DataContext as DiaryEntryHolderViewModel;
                   movingModel.SchedulingTime = d.StartTime.AddMinutes((e.GetPosition(DiaryCanvas).Y-yOffset) * d.MinutesPerPixel);
                   if (d.AlteredTasks.Where(x => x.Item.ID == movingModel.Item.ID).Any() == false)
                   {
                       d.AlteredTasks.Add(movingModel);
                   }
                   ((DiaryEntryView)sender).SchedulingTimeTextBlock.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                if ((this.DataContext as DiaryEntryHolderViewModel).IsEditable)
                {
                    Cursor = Cursors.SizeAll;
                }
                    
                
            }
            
            
           
        }

        private void DiaryEntryView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (Cursor == Cursors.SizeAll)
            {
                //Lets move this bad body
                //Is scheduling time writeable
                yOffset=((e.GetPosition(e.Source as DiaryEntryView).Y));
                
                movingModel = (e.Source as DiaryEntryView).DataContext as BaseTaskViewModel;


            }
           
           
            
        }

        private void DiaryEntryView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            movingModel = null;
            weAreDragging = false;
            ((DiaryEntryView)sender).SchedulingTimeTextBlock.Visibility = System.Windows.Visibility.Hidden;
        }

        private void DiaryEntryView_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || movingModel == null)
            {
                
                Cursor = Cursors.Arrow;
                movingModel = null;
            }
            

            
        }

        private void DiaryEntryViewHolder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (weAreDragging)
            {
                weAreDragging = false;
            }
        }

        private void UserControl_DragLeave(object sender, DragEventArgs e)
        {
            var obj = e.Data.GetData(e.Data.GetFormats()[0]);
            if (typeof(BaseTaskViewModel).IsAssignableFrom(obj.GetType()))
            {
                if(((DiaryEntryHolderViewModel)this.DataContext).PropertyName=="Role")
                {
                    ((BaseTaskViewModel)obj).Role = null;
                    ((DiaryEntryHolderViewModel)this.DataContext).TasksView.Refresh();
                }
                else if (((DiaryEntryHolderViewModel)this.DataContext).PropertyName == "Room")
                {
                    ((BaseTaskViewModel)obj).Room = null;
                    ((DiaryEntryHolderViewModel)this.DataContext).TasksView.Refresh();
                }
                
                



            }
        }

        private void DiaryEntryViewHolder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || movingModel == null)
            {

                Cursor = Cursors.Arrow;
                movingModel = null;
            }

            if (e.LeftButton == MouseButtonState.Pressed || movingModel != null)
            {
                weAreDragging = true;
                movingModel.GetType().GetProperty((this.DataContext as DiaryEntryHolderViewModel).PropertyName).SetValue(movingModel, null, null);
                DragDrop.DoDragDrop(this, new DataObject("myFormat",movingModel), DragDropEffects.Move);
            }
           
        }

        private void DiaryEntryView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //movingModel = (e.Source as DiaryEntryView).DataContext as BaseTaskViewModel;
        }
    }

    public class Ticker : INotifyPropertyChanged
    {
        public Ticker()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 60000; // 1 minute updates
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Now"));
        }

        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
    }


    
}
