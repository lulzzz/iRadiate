using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


using NLog;


using iRadiate.Common.IO;
using iRadiate.Common.Misc;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.RadioPharmacy;

namespace iRadiate.Common.EFDataRetriever
{
    public class EFDataRetriever : IDataRetriever
    {
        public EFDataRetriever()
        {
            db.Configuration.AutoDetectChangesEnabled = false;
        }

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private IRadiateContext db = new IRadiateContext(Properties.Settings.Default.DatabseConnection);
        private object thisLock = new object();
        public DataModel.Common.IDataStoreItem RetrieveItem(int id, Type type)
        {
            //using (IRadiateContext db = new IRadiateContext(Properties.Settings.Default.DatabseConnection))
            //{
                DbSet s = db.Set(type);
                return (IDataStoreItem)s.Find(id);
            //}
        }

        public DataModel.Common.IDataStoreItem RetrieveItem(DataModel.Common.IDataStoreItem item)
        {
            if (item.ID != null)
            {
                int id = item.ID;
                return RetrieveItem(id, item.GetType());
            }
            else
            {
                throw new Exception("Couldn't find that item in the database");
            }
        }

        public bool IsPropertyACollection(PropertyInfo property)
        {
            if (property.PropertyType == typeof(string))
            {
                return false;
            }
            if (property.PropertyType == typeof(Byte[]))
            {
                return false;
            }
            return property.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) != null;
        }

        public void UpdateOriginalRowVersion(IDataStoreItem item)
        {
            //db.db.Entry(item).OriginalValues.SetValues()
        }
        public bool SaveItem(DataModel.Common.IDataStoreItem item)
        {
            
            lock (thisLock)
            {
                
                try
                {
                    
                    if(item == null)
                    {
                        
                    }
                    else
                    {
                        
                    }
                    
                   
                    if (item.ID == 0)
                    {
                        logger.Trace("Item is being added");
                        item.CreationDate = DateTime.Now;

                        db.Entry(item).State = EntityState.Added;
                        db.SaveChanges();

                    }
                    else if (DateTime.Compare(db.Entry(item).GetDatabaseValues().GetValue<DateTime>("LastEditDate"), item.LastEditDate) != 0)
                    {
                        logger.Error("Throw concurrency Error db.LastEditDate  = "  +db.Entry(item).GetDatabaseValues().GetValue<DateTime>("LastEditDate") + " item.LastEditDate = " + item.LastEditDate);
                        db.Entry(item).State = EntityState.Modified;
                        //throw new Exception("Concurrency Error");
                        item.LastEditDate = DateTime.Now;


                        db.SaveChanges();
                    }
                    else
                    {
                        //Here we would implement all the changetracking.
                        //we iterate through all properties i guess....
                        logger.Trace("Item.ID != 0 and lastEditDate checks out...");
                        
                        db.Entry(item).State = EntityState.Modified;
                        item.LastEditDate = DateTime.Now;
                        AddDataStoreItemAlterations(item);
                        UpdateLinkedItems(item);


                        db.SaveChanges();
                        
                    }
                    
                    
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("...Caught Exception " + ex.Message);
                    logger.Error("Error during SaveItem() " + ex.Message);
                }
                return true;
            }
            
            
         
        }

        private void AddDataStoreItemAlterations(IDataStoreItem item)
        {
            foreach (PropertyInfo p in item.ConcreteType.GetProperties())
            {
                if(p.Name == "LastEditDate")
                {
                    continue;
                }
                if (!p.CanWrite)
                {
                    continue;
                }
                if (p.PropertyType.IsPrimitive || p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime))
                {
                    Type t = p.PropertyType;
                    var oldVal = db.Entry(item).OriginalValues.GetValue<object>(p.Name);

                    PropertyInfo pi = item.GetType().GetProperty(p.Name);
                    if (oldVal == null)
                    {
                        DataStoreItemAlteration d = new DataStoreItemAlteration();
                        d.ItemIDNumber = item.ID;
                        d.Workstation = null;
                        d.PropertyName = p.Name;
                        d.OldValue = "";
                        if (pi.GetValue(item, null) == null)
                        {
                            d.NewValue = "";
                        }
                        else
                        {
                            d.NewValue = pi.GetValue(item, null).ToString();
                        }

                        d.DataStoreItemName = item.ConcreteType.Name;
                        db.DataStoreItemAlterations.Add(d);
                    }
                    else if (pi.GetValue(item, null).ToString() != oldVal.ToString())
                    {
                        DataStoreItemAlteration d = new DataStoreItemAlteration();

                        d.Workstation = null;
                        d.PropertyName = p.Name;
                        d.OldValue = oldVal.ToString();
                        d.NewValue = pi.GetValue(item, null).ToString();
                        d.DataStoreItemName = item.ConcreteType.Name;
                        db.DataStoreItemAlterations.Add(d);
                    }

                }



            }
        }

        private void UpdateLinkedItems(IDataStoreItem item)
        {
            if (item.LinkedItems.Any())
            {
                foreach (IDataStoreItem i in item.LinkedItems)
                {
                    db.Entry(i).State = EntityState.Modified;
                    i.LastEditDate = DateTime.Now;
                    AddDataStoreItemAlterations(i);
                    UpdateLinkedItems(i);
                }
                item.LinkedItems.Clear();
            }
        }
        public bool SaveItems(ICollection<DataModel.Common.IDataStoreItem> items)
        {
            foreach (IDataStoreItem i in items)
            {
                SaveItem(i);
            }
            return true;
        }


        public IDataStoreItem RetrieveItem(Type type, ICollection<RetrievalCriteria> criteria)
        {
            lock (thisLock)
            {
                //using (IRadiateContext db = new IRadiateContext(Properties.Settings.Default.DatabseConnection))
                //{
                DbSet s = db.Set(type);
                IEnumerable<IDataStoreItem> s1 = s.Cast<IDataStoreItem>();

                List<IDataStoreItem> result = s1.ToList();
                result = result.Where(x => x.Deleted == false).ToList();
                foreach (RetrievalCriteria rc in criteria)
                {
                    Type propertyType = type.GetProperty(rc.PropertyName).GetType();
                    if (TypeChecker.IsNumericType(type))
                    {
                        switch (rc.CriteriaType)
                        {
                            case CriteraType.Equals:
                                result = result.Where(x => Convert.ToDouble(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) == ((double)rc.FilterValue)).ToList();
                                break;
                            case CriteraType.ExactTextMatch:

                                break;
                            case CriteraType.GreaterThan:
                                result = result.Where(x => Convert.ToDouble(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) > ((double)rc.FilterValue)).ToList();
                                break;
                            case CriteraType.GreaterThanOrEqual:
                                result = result.Where(x => Convert.ToDouble(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) >= ((double)rc.FilterValue)).ToList();
                                break;
                            case CriteraType.LessThan:
                                result = result.Where(x => Convert.ToDouble(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) < ((double)rc.FilterValue)).ToList();
                                break;
                            case CriteraType.LessThanOrEqual:
                                result = result.Where(x => Convert.ToDouble(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) <= ((double)rc.FilterValue)).ToList();
                                break;
                            case CriteraType.TextMatch:

                                break;
                        }
                    }
                    else if (TypeChecker.IsDateTime(type))
                    {
                        switch (rc.CriteriaType)
                        {
                            case CriteraType.Equals:
                                result = result.Where(x => Convert.ToDateTime(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) == ((DateTime)rc.FilterValue)).ToList();
                                break;
                            case CriteraType.ExactTextMatch:

                                break;
                            case CriteraType.GreaterThan:
                                result = result.Where(x => Convert.ToDateTime(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) > ((DateTime)rc.FilterValue)).ToList();
                                break;
                            case CriteraType.GreaterThanOrEqual:
                                result = result.Where(x => Convert.ToDateTime(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) >= ((DateTime)rc.FilterValue)).ToList();
                                break;
                            case CriteraType.LessThan:
                                result = result.Where(x => Convert.ToDateTime(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) < ((DateTime)rc.FilterValue)).ToList();
                                break;
                            case CriteraType.LessThanOrEqual:
                                result = result.Where(x => Convert.ToDateTime(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) <= ((DateTime)rc.FilterValue)).ToList();
                                break;
                            case CriteraType.TextMatch:

                                break;
                        }
                    }
                    else
                    {
                        switch (rc.CriteriaType)
                        {
                            case CriteraType.Equals:
                                result = result.Where(x => x.GetType().GetProperty(rc.PropertyName).GetValue(x, null).ToString().Trim().ToLower() == (rc.FilterValue.ToString().Trim().ToLower())).ToList();
                                break;
                            case CriteraType.ExactTextMatch:
                                result = result.Where(x => x.GetType().GetProperty(rc.PropertyName).GetValue(x, null).ToString() == (rc.FilterValue.ToString())).ToList();
                                break;
                            case CriteraType.GreaterThan:

                                break;
                            case CriteraType.GreaterThanOrEqual:

                                break;
                            case CriteraType.LessThan:

                                break;
                            case CriteraType.LessThanOrEqual:

                                break;
                            case CriteraType.TextMatch:
                                result = result.Where(x => x.GetType().GetProperty(rc.PropertyName).GetValue(x, null).ToString().Trim().ToLower().Contains(rc.FilterValue.ToString().Trim().ToLower())).ToList();
                                break;
                        }
                    }

                }

                return result.First();

            }
            
            //}
        }


        public List<IDataStoreItem> RetrieveItems(Type type, ICollection<RetrievalCriteria> criteria, bool fresh)
        {
            if (fresh)
            {
                return RetrieveItems(type, criteria, new IRadiateContext(Properties.Settings.Default.DatabseConnection));
            }
            return RetrieveItems(type, criteria, db);

            
        }
        

        public List<IDataStoreItem> RetrieveItems(Type type, ICollection<RetrievalCriteria> criteria)
        {

            return RetrieveItems(type, criteria, db);

            
        }

        private List<IDataStoreItem> RetrieveItems(Type type, ICollection<RetrievalCriteria> criteria, IRadiateContext db)
        {
            lock (thisLock)
            {
                logger.Trace("RetrieveItems(" + type.ToString() + ") called");
                db.Configuration.AutoDetectChangesEnabled = false;
                //db = new IRadiateContext(Properties.Settings.Default.DatabseConnection);
                
                {
                    IEnumerable<IDataStoreItem> result;
                    if (type == typeof(User))
                    {

                        result = db.Users.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Patient))
                    {
                        result = db.Patients.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Study))
                    {
                        if (criteria.Any())
                        {
                            if (criteria.First().PropertyName == "Date")
                            {
                                DateTime d1 = Convert.ToDateTime(criteria.First().FilterValue);
                                DateTime d2 = Convert.ToDateTime(criteria.ElementAt(1).FilterValue);
                                string s1 = d1.ToString("yyyyMMdd");
                                string s2 = d2.ToString("yyyyMMdd");
                                if (criteria.Count() > 2)
                                {
                                    int idNum = Convert.ToInt32(criteria.ElementAt(2).FilterValue);
                                    logger.Trace("SELECT * FROM Studies INNER JOIN Appointments on Appointments.Study_ID = Studies.ID WHERE Appointments.ScheduledArrivalTime > '" + s1 + "' AND Appointments.ScheduledArrivalTime < '" + s2 + "' AND Studies.StudyType_ID = " + idNum.ToString());
                                    return db.Studies.SqlQuery("SELECT * FROM Studies INNER JOIN Appointments on Appointments.Study_ID = Studies.ID WHERE Appointments.ScheduledArrivalTime > '" + s1 + "' AND Appointments.ScheduledArrivalTime < '" + s2 + "' AND Studies.StudyType_ID = " + idNum.ToString()).ToList<IDataStoreItem>();
                                }
                                else
                                {
                                    return db.Studies.SqlQuery("SELECT * FROM Studies INNER JOIN Appointments on Appointments.Study_ID = Studies.ID WHERE Appointments.ScheduledArrivalTime > '" + s1 + "' AND Appointments.ScheduledArrivalTime < '" + s2 + "'").ToList<IDataStoreItem>();
                                }



                            }
                            else
                            {

                            }
                        }
                        
                        result = db.Studies.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Appointment))
                    {
                       
                        result = db.Appointments;
                        //result = db.Appointments.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(StaffMemberRole))
                    {
                        result = db.StaffMemberRoles.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Hospital))
                    {
                        result = db.Hospitals.Include(x => x.Wards).ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(NucMedPractice))
                    {
                        result = db.NucMedPractices.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(StandardFiniteTaskType))
                    {
                        result = db.StandardFiniteTaskTypes.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(StandardNonFiniteTaskType))
                    {
                        result = db.StandardNonFiniteTaskTypes.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Ward))
                    {
                        result = db.Wards.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(StudyType))
                    {
                        result = db.StudyTypes.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Radiopharmaceutical))
                    {
                        result = db.Radiopharmaceuticals.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(RoomReservation))
                    {
                        result = db.RoomReservations.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Doctor))
                    {
                        result = db.Doctors.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(StudyReport))
                    {
                        result = db.StudyReports.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Workstation))
                    {
                        result = db.Workstations.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Room))
                    {
                        result = db.Rooms.ToList<IDataStoreItem>();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    //result = result.Where(x => x.Deleted == false).ToList();
                    logger.Trace("....result has returned ");
                    foreach (RetrievalCriteria rc in criteria)
                    {
                        logger.Trace("Processing RetrievalCriteria " + rc.PropertyName + " " + rc.CriteriaType.ToString() + " " + rc.FilterValue.ToString());
                        Type propertyType = type.GetProperty(rc.PropertyName).PropertyType;
                        
                        
                        //logger.Trace("propertyType.Name = " + propertyType.Name + "Type.GetTypeCode(propertyType) = " + Type.GetTypeCode(propertyType));
                        if (TypeChecker.IsNumericType(propertyType))
                        {
                            logger.Trace("isNumericType == true");
                            switch (rc.CriteriaType)
                            {
                                case CriteraType.Equals:
                                    result = result.Where(x => Convert.ToDouble(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) == ((double)rc.FilterValue));
                                    break;
                                case CriteraType.ExactTextMatch:

                                    break;
                                case CriteraType.GreaterThan:
                                    result = result.Where(x => Convert.ToDouble(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) > ((double)rc.FilterValue));
                                    break;
                                case CriteraType.GreaterThanOrEqual:
                                    result = result.Where(x => Convert.ToDouble(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) >= ((double)rc.FilterValue));
                                    break;
                                case CriteraType.LessThan:
                                    result = result.Where(x => Convert.ToDouble(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) < ((double)rc.FilterValue));
                                    break;
                                case CriteraType.LessThanOrEqual:
                                    result = result.Where(x => Convert.ToDouble(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) <= ((double)rc.FilterValue));
                                    break;
                                case CriteraType.IsNull:
                                    result = result.Where(x => x.GetType().GetProperty(rc.PropertyName).GetValue(x, null) == null);
                                    break;
                                case CriteraType.TextMatch:

                                    break;
                            }
                            
                        }
                        else if (TypeChecker.IsDateTime(propertyType))
                        {
                            logger.Trace("isDateTime == true");
                            switch (rc.CriteriaType)
                            {
                                case CriteraType.Equals:
                                    result = result.Where(x => Convert.ToDateTime(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) == ((DateTime)rc.FilterValue));
                                    break;
                                case CriteraType.ExactTextMatch:

                                    break;
                                case CriteraType.GreaterThan:
                                    result = result.Where(x => Convert.ToDateTime(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) > ((DateTime)rc.FilterValue));
                                    break;
                                case CriteraType.GreaterThanOrEqual:
                                    result = result.Where(x => Convert.ToDateTime(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) >= ((DateTime)rc.FilterValue));
                                    break;
                                case CriteraType.LessThan:
                                    result = result.Where(x => Convert.ToDateTime(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) < ((DateTime)rc.FilterValue));
                                    break;
                                case CriteraType.LessThanOrEqual:
                                    result = result.Where(x => Convert.ToDateTime(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) <= ((DateTime)rc.FilterValue));
                                    break;
                                case CriteraType.IsNull:
                                    result = result.Where(x => x.GetType().GetProperty(rc.PropertyName).GetValue(x, null) == null);
                                    break;
                                case CriteraType.TextMatch:

                                    break;
                            }
                        }
                        else
                        {
                            //logger.Trace("isDateTime == False && isNumericType == false");
                            switch (rc.CriteriaType)
                            {
                                case CriteraType.Equals:
                                    result = result.Where(x => x.GetType().GetProperty(rc.PropertyName).GetValue(x, null).ToString() == (rc.FilterValue.ToString()));
                                    break;
                                case CriteraType.ExactTextMatch:
                                    result = result.Where(x => x.GetType().GetProperty(rc.PropertyName).GetValue(x, null).ToString() == (rc.FilterValue.ToString()));
                                    break;
                                case CriteraType.GreaterThan:

                                    break;
                                case CriteraType.GreaterThanOrEqual:

                                    break;
                                case CriteraType.LessThan:

                                    break;
                                case CriteraType.LessThanOrEqual:

                                    break;
                                case CriteraType.IsNull:
                                    result = result.Where(x => x.GetType().GetProperty(rc.PropertyName).GetValue(x, null) == null);
                                    break;
                                case CriteraType.TextMatch:
                                    result = result.Where(x => x.GetType().GetProperty(rc.PropertyName).GetValue(x, null).ToString().Trim().ToLower().Contains(rc.FilterValue.ToString().Trim().ToLower()));
                                    break;
                            }
                        }
                        logger.Trace("Completed processing criterion");
                    }


                    logger.Trace("Result includes " + result.Count().ToString() + " items");
                    db.Configuration.AutoDetectChangesEnabled = false;
                    return result.ToList();
                }
            }
            
        }


        public void DeleteItem(IDataStoreItem item)
        {
            lock (thisLock)
            {
                item.Deleted = true;
                item.DeletionDate = DateTime.Now;
                SaveItem(item);
            }
            
        }


        public void CloseAlternateDataStore()
        {
            throw new NotImplementedException();
        }


        public void UpdateItem(IDataStoreItem item)
        {
            
            db.Entry(item).Reload();
            
        }


        public void UnDeleteItem(IDataStoreItem item)
        {
            item.Deleted = false;
           
            SaveItem(item);
        }

        public void SwitchOnAutoDetect()
        {
            db.Configuration.AutoDetectChangesEnabled = true;
        }

        public void SwitchOffAutoDetect()
        {
            db.Configuration.AutoDetectChangesEnabled = false;
        }
    }
}
