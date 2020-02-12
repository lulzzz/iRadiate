using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.NucMed;
using NLog;

using Dicom;

namespace iRadiate.Interfaces.DICOM
{
    public class DicomMassage
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static Room GetRoom(PatientImage pi)
        {
            if (pi == null)
            {
                logger.Warn("Attemt to get room for null PatientImage");
                return null;
            }

            var roomBridges = DicomModule.GetRoomBridges();
           
            if(pi.DeviceSerialNumber != null)
            {
                if (roomBridges.Where(x => x.SerialNumber == pi.DeviceSerialNumber).Any())
                    return roomBridges.Where(x => x.SerialNumber == pi.DeviceSerialNumber).First().Room;
            }
            
            if(pi.ManufacturerModelName != null)
            {
                if (roomBridges.Where(x => x.ModelName == pi.ManufacturerModelName).Any())
                    return roomBridges.Where(x => x.ModelName == pi.ManufacturerModelName).First().Room;
            }
            return null; 
            

        }

        public static List<ScanTask> SortIntoTasks(List<PatientImage> images)
        {

            var result = new List<ScanTask>();
            foreach(PatientImage im  in images.OrderBy(x=>x.SeriesDateTime))
            {
                ScanTask st = GetScanTaskForImage(im,30);
                if (st != null)
                {
                    if(st.Patient != null)
                    {
                        if (result.Contains(st) == false)
                        {
                            result.Add(st);
                        }
                    }
                   
                }
                  
               
            }
            return result;
            //var result = new List<ScanTask>();
            //var rooms = new List<Room>();
            //ScanTask task = new ScanTask();
            //int counter = 0;
            //DateTime lastSeriesFinish = new DateTime();
            //foreach (PatientImage img in images.OrderBy(z=>z.ManufacturerModelName).ThenBy(x => x.SeriesDateTime))
            //{
            //    var room = GetRoom(img);
            //    if (room == null)
            //        continue;
            //    if (counter == 0)
            //    {
            //        result.Add(task);
            //        img.ScanTask = task;
            //        task.PatientImages.Add(img);
            //        task.Room = room;
            //        task.Appointment = img.Appointment;
            //        task.Commenced = true;
            //        task.CommencentTime = img.SeriesDateTime.AddMinutes(-3);
            //        task.CompletionTime = img.ScanFinishedDateTime.AddMinutes(3);
            //        lastSeriesFinish = img.ScanFinishedDateTime;
            //        task.Completed = true;


            //    }
            //    else
            //    {
            //        if(room.ID != task.Room.ID)
            //        {
            //            task = new ScanTask();
            //            result.Add(task);
            //            img.ScanTask = task;
            //            task.Room = room;
            //            task.PatientImages.Add(img);
            //            task.Appointment = img.Appointment;
            //            task.Commenced = true;
            //            task.CommencentTime = img.SeriesDateTime.AddMinutes(-3);

            //                task.CompletionTime = img.ScanFinishedDateTime.AddMinutes(3);
            //                lastSeriesFinish = img.ScanFinishedDateTime;

            //            task.Completed = true;
            //        }
            //        else if (room.ID == task.Room.ID)
            //        {
            //            if(img.Patient.MRN != task.Patient.MRN)
            //            {
            //                task = new ScanTask();
            //                result.Add(task);
            //                img.ScanTask = task;
            //                task.PatientImages.Add(img);
            //                task.Room = room;
            //                task.Appointment = img.Appointment;
            //                task.Commenced = true;
            //                task.CommencentTime = img.SeriesDateTime.AddMinutes(-3);

            //                    task.CompletionTime = img.ScanFinishedDateTime.AddMinutes(3);
            //                    lastSeriesFinish = img.ScanFinishedDateTime;


            //                task.Completed = true;
            //            }
            //            else
            //            {
            //               if((img.SeriesDateTime - lastSeriesFinish).TotalMinutes > 10)
            //                {
            //                    task = new ScanTask();
            //                    img.ScanTask = task;
            //                    task.PatientImages.Add(img);
            //                    task.Room = room;
            //                    task.Appointment = img.Appointment;
            //                    task.Commenced = true;
            //                    task.CommencentTime = img.SeriesDateTime.AddMinutes(-3);
            //                    task.CompletionTime = img.ScanFinishedDateTime.AddMinutes(3);

            //                        task.CompletionTime = img.ScanFinishedDateTime.AddMinutes(3);
            //                        lastSeriesFinish = img.ScanFinishedDateTime;


            //                    task.Completed = true;
            //                }
            //                else
            //                {
            //                    if (img.ScanFinishedDateTime > lastSeriesFinish)
            //                    {
            //                        task.CompletionTime = img.ScanFinishedDateTime.AddMinutes(3);
            //                        lastSeriesFinish = img.ScanFinishedDateTime;
            //                    }
            //                    task.PatientImages.Add(img);
            //                    img.ScanTask = task;
            //                }

            //            }

            //        }

            //    }

            //    counter++;
            //}
            //return result;
        }

        public static ScanTask GetScanTaskForImage(PatientImage img, int tolerance)
        {
            logger.Info("GetScanTaskForImage(PatientImage.ID = " + img.ID);
            if(img.ID == 0)
            {
                logger.Error("Cannot assign a scan task to a PatientImage with ID = 0");
                return null;
            }
            if (img.ScanTask != null)
                return img.ScanTask;

            Appointment a = img.Appointment;
            Room room = GetRoom(img);
            if(a == null)
            {
                logger.Error("Cannot assign image to scantask when appointment is null");
                return null;
            }
            if(room == null)
            {
                logger.Error("Cannot assign image to scan task when image room is null");
                return null;
            }

            if(img.SeriesDateTime == null)
            {
                logger.Error("Cannot assign image to scan task when image SeriesDateTime is null");
                return null;
            }

            if(img is SPECTNMImage)
            {
                logger.Error("Scan tasks are not assigned to Recon images");
                return null;
            }

            ///Are there any tasks available in the appointment
            if(a.Tasks.Where(x=>x is ScanTask && x.IsCancelled == false && x.Deleted == false).Any())
            {
                ///Valid tasks are those scan taks which have not been deleted or cancelled;
                var validTasks = a.Tasks.Where(x => x is ScanTask && x.IsCancelled == false && x.Deleted == false).ToList();

                ///Non-empty valid tasks are those which already have at least one patient image in them - THEY CANNOT BE MOVED
                var nonEmptyValidTasks = validTasks.Where(z => (z as ScanTask).PatientImages.Any() && z.Room!= null).ToList();

                ///Empty valid tasks are those which do not have any patient images in them - THEY CAN BE MOVED AND RESCHEDULED
                var emptyValidTasks = validTasks.Where(j => (j as ScanTask).PatientImages.Any() == false && j.Room != null).ToList();

                ///Are there any non-empty valid tasks on the same camera is the img that start before the paitent image and finish no more than 15 minutes before we start 
                if(nonEmptyValidTasks.Where(b=>(b as ScanTask).Room.ID == room.ID && GetImageToTaskDifference((b as ScanTask),img) < tolerance).Any())
                {
                    //grab the first one
                    
                    ScanTask st = nonEmptyValidTasks.Where(b => (b as ScanTask).Room.ID == room.ID && GetImageToTaskDifference((b as ScanTask), img) < tolerance).First() as ScanTask;
                    logger.Info("Image being added to Scan task " + st.ID +" which already has images");
                    st.PatientImages.Add(img);
                    if(img.ScanFinishedDateTime.AddMinutes(3) > st.CompletionTime)
                    {
                        st.CompletionTime = img.ScanFinishedDateTime.AddMinutes(3);
                        if (st.CompletionTime < DateTime.Now.AddMinutes(10))
                        {
                            st.Completed = false;
                            logger.Info("ScanTask " + st.ID + " being mark as incomplete because last image was finished less than 10 minutes ago");
                        }
                        else
                            st.Completed = true;
                    }
                    Platform.Retriever.SaveItem(st);
                    return st;
                }
                else if(emptyValidTasks.Any())
                {
                    logger.Info("Image being added to empty ScanTask");
                    ScanTask st = emptyValidTasks.First() as ScanTask;
                    st.Room = room;
                    st.PatientImages.Add(img);
                    st.Commenced = true;
                    st.CommencentTime = img.SeriesDateTime.AddMinutes(-3);
                    if (img.ScanFinishedDateTime < DateTime.Now.AddMinutes(10))
                    {
                        st.Completed = false;
                        st.CompletionTime = img.ScanFinishedDateTime.AddMinutes(3);
                    }
                    else
                        st.Completed = true;
                    Platform.Retriever.SaveItem(st);
                    return st;
                }
                else
                {
                    logger.Info("New ScanTask created for PatientImage.ID = " + img.ID);
                    ScanTask st = new ScanTask();
                    a.Tasks.Add(st);
                    st.Appointment = a;
                    st.Room = room;
                    st.PatientImages.Add(img);
                    st.Commenced = true;
                    st.CommencentTime = img.SeriesDateTime.AddMinutes(-3);
                    if (img.ScanFinishedDateTime < DateTime.Now.AddMinutes(10))
                    {
                        st.Completed = false;
                        st.CompletionTime = img.ScanFinishedDateTime.AddMinutes(3);
                    }
                    else
                        st.Completed = true;
                    Platform.Retriever.SaveItem(st);
                    return st;
                }
                
                

            }
            else//if not we will make a new task
            {
                logger.Info("New ScanTask created for PatientImage.ID = " + img.ID);
                ScanTask st = new ScanTask();
                a.Tasks.Add(st);
                st.Appointment = a;
                st.Room = room;
                st.PatientImages.Add(img);
                st.Commenced = true;
                st.CommencentTime = img.SeriesDateTime.AddMinutes(-3);
                if (img.ScanFinishedDateTime < DateTime.Now.AddMinutes(10))
                {
                    st.Completed = false;
                    st.CompletionTime = img.ScanFinishedDateTime.AddMinutes(3);
                }
                else
                    st.Completed = true;
                Platform.Retriever.SaveItem(st);
                return st;
            }
        }

        public static void MergeScanTasks(Appointment app, int tolerance)
        {
            logger.Info("Merging scan tasks for " + app.Patient.FullName + " " + app.Name);
            logger.Info("Trimming current scanTasks....");
            foreach(ScanTask st in app.Tasks.Where(x=>x is ScanTask).OrderBy(y => y.SchedulingTime))
            {
                //MassageScanTask(st);
            }
            logger.Info("Trimming current scanTasks....Done");
            logger.Info("Splitting tasks with gaps...");
            List<ScanTask> offshoots = new List<ScanTask>();
            foreach (ScanTask st in app.Tasks.Where(x => x is ScanTask).OrderBy(y => y.SchedulingTime))
            {
                if (ScanTaskHasGaps(st, tolerance))
                {
                    logger.Info("ScanTask " + st.ID + " has gaps");
                    ScanTask offshoot = SplitScanTask(st, tolerance);
                    if (offshoot != null)
                        offshoots.Add(offshoot);

                }
            }
            logger.Info("Splitting tasks with gaps...Done");
            foreach (ScanTask st in offshoots)
            {
                app.Tasks.Add(st);
            }

            int counter = 0;
            DateTime previousFinish = new DateTime();
            ScanTask previousTask = null;
            offshoots.Clear();
            foreach (ScanTask currentTask in app.Tasks.Where(x => x is ScanTask).OrderBy(y => y.SchedulingTime))
            {
                if(counter == 0)
                {
                    previousFinish = currentTask.ValidCompletionTime;
                    previousTask = currentTask;
                }
                else
                {
                    if(((currentTask.ValidCommencementTime - previousTask.ValidCompletionTime).TotalMinutes < tolerance) || currentTask.ValidCommencementTime < previousTask.ValidCompletionTime)
                    {
                        foreach(PatientImage im in currentTask.PatientImages)
                        {
                            im.ScanTask = previousTask;
                        }
                        //st.Deleted = true;
                        //st.DeletionDate = DateTime.Now;
                        
                        //offshoots.Add(st);
                        //MassageScanTask(previousTask);
                    }
                    previousFinish = currentTask.ValidCompletionTime;
                    previousTask = currentTask;
                    counter++;
                }
                
            }
            offshoots.Clear();

            foreach(ScanTask st in app.Tasks.Where(x=>x is ScanTask && (x as ScanTask).PatientImages.Any()==false))
            {
                offshoots.Add(st);
               
            }
            foreach(ScanTask st in offshoots)
            {
                app.Tasks.Remove(st);
                st.Appointment = null;
                logger.Info("Scantask " + st.ID + " assigned to null appointment");
                Platform.Retriever.SaveItem(st);
            }
        }

        //Mark as completed if it is complete!
        public static void MassageScanTask(ScanTask st)
        {
            if (st.PatientImages.Any())
            {
                DateTime firstImageStart;
                firstImageStart = st.PatientImages.OrderBy(x => x.SeriesDateTime).First().SeriesDateTime;
                if(!st.Commenced)
                {
                    st.Commenced = true;
                    logger.Info("Commenced set to true for ScanTask " + st.ID);
                    Platform.Retriever.SaveItem(st);
                }
                if(st.CommencentTime != firstImageStart.AddMinutes(-3))
                {
                    st.CommencentTime = firstImageStart.AddMinutes(-3);
                    logger.Info("Commencement for scantak " + st.ID + " set to " + st.CommencentTime);
                    Platform.Retriever.SaveItem(st);
                }

                try
                {
                    if (st.PatientImages.Where(y => (y is CTImage) == false).OrderBy(x => x.ScanFinishedDateTime).Last().ScanFinishedDateTime < DateTime.Now.AddMinutes(-15))
                    {

                        if (!st.Completed)
                        {
                            st.Completed = true;
                            logger.Info("ScanTask " + st.ID + " marked as completed");
                        }

                        if (st.CompletionTime != st.PatientImages.Where(y => (y is CTImage) == false).OrderBy(x => x.ScanFinishedDateTime).Last().ScanFinishedDateTime.AddMinutes(3))
                        {
                            st.CompletionTime = st.PatientImages.Where(y => (y is CTImage) == false).OrderBy(x => x.ScanFinishedDateTime).Last().ScanFinishedDateTime.AddMinutes(3);
                            logger.Info("ScanTask " + st.ID + " marked as completed on " + st.CompletionTime);
                        }


                        Platform.Retriever.SaveItem(st);
                    }
                    else
                    {
                        st.Completed = false;
                        logger.Info("ScanTask " + st.ID + " marked as not complete because last series finished less than 15 minutes ago");
                        Platform.Retriever.SaveItem(st);
                    }
                }
                catch(Exception ex)
                {
                    logger.Error("Exception in MassageScanTask st.ID = " + st.ID.ToString() + "; " + ex.Message);
                }
               
            }
        }

        public static bool ScanTaskHasGaps(ScanTask st, int tolerance)
        {
            DateTime previous = new DateTime();
            foreach (PatientImage pi in st.PatientImages.OrderBy(x => x.SeriesDateTime))
            {
                if (previous == new DateTime())
                    previous = pi.ScanFinishedDateTime;

                if (pi.SeriesDateTime > previous.AddMinutes(tolerance))
                    return true;
            }

            return false;
        }

        public static ScanTask SplitScanTask(ScanTask st,int tolerance)
        {
            DateTime previous = new DateTime();
            foreach (PatientImage pi in st.PatientImages.OrderBy(x => x.SeriesDateTime))
            {
                if (previous == new DateTime())
                    previous = pi.ScanFinishedDateTime;

                if (pi.SeriesDateTime > previous.AddMinutes(tolerance))
                {
                    st.CompletionTime = previous;
                    st.Completed = true;
                    ScanTask newScan = new ScanTask();
                    //newScan.Appointment = st.Appointment;
                    newScan.Commenced = true;
                    newScan.CommencentTime = st.PatientImages.OrderBy(j => j.SeriesDateTime).Where(y => y.SeriesDateTime > previous).First().SeriesDateTime.AddMinutes(-3);
                    newScan.Room = st.Room;
                    newScan.Completed = true;
                    newScan.CompletionTime = previous.AddMinutes(3);
                    foreach (PatientImage p in st.PatientImages.OrderBy(j=>j.SeriesDateTime).Where(y=>y.SeriesDateTime > previous))
                    {
                        p.ScanTask = newScan;
                    }
                    logger.Info("Scantask split ... " + newScan.ValidCommencementTime.ToShortTimeString() + " - " + newScan.ValidCompletionTime.ToShortTimeString());
                    return newScan;

                }

            }
            return null;
        }
        
        /// <summary>
        /// Returns the time difference between a PatientImage and a ScanTask
        /// </summary>
        /// <param name="st">The ScanTask</param>
        /// <param name="img">the PatientImage</param>
        /// <returns></returns>
        public static double GetImageToTaskDifference(ScanTask st, PatientImage img)
        {
            
            var startStartDiffernece = Math.Abs((st.ValidCommencementTime - img.SeriesDateTime).TotalMinutes);
            var startEndDifference = Math.Abs((st.ValidCommencementTime - img.ScanFinishedDateTime).TotalMinutes);
            var endStartDifference = Math.Abs((st.ValidCompletionTime - img.SeriesDateTime).TotalMinutes);
            var endEndDifference = Math.Abs((st.ValidCompletionTime - img.ScanFinishedDateTime).TotalMinutes);

            return Math.Min(endEndDifference, Math.Min(endStartDifference, Math.Min(startStartDiffernece, startEndDifference)));
        }
        
    }
}
