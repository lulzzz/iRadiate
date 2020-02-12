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
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.DataDictionary;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Equipment;
using iRadiate.DataModel.Radiopharmacy;

namespace iRadiate.Common.IO
{
    public class EFDataRetriever : IDataRetriever
    {
        #region privateFields
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private IRadiateContext db = new IRadiateContext(Properties.Settings.Default.DatabseConnection);
        private object thisLock = new object();
        private int _itemsRetrieved;
        private int _itemsSaved;
        private List<ModifiedDataStoreItem> _modifiedDataStoreItems;
        #endregion

        #region constructor
        public EFDataRetriever()
        {
            db.Configuration.AutoDetectChangesEnabled = true;
            //db.Database.Log = msg => System.Diagnostics.Debug.WriteLine(msg);
            
        }
        #endregion

        #region events
       
        public event EventHandler ItemSaving;
        public event EventHandler ItemSaved;
        public event EventHandler ItemRetrieved;

        protected void OnItemSaving()
        {
            if(ItemSaving != null)
            {
                ItemSaving(this, new EventArgs());
            }
        }

        protected void OnItemSaved()
        {
            if(ItemSaved != null)
            {
                ItemSaved(this, new EventArgs());
            }
        }

        protected void OnItemRetrieved()
        {
            if(ItemRetrieved != null)
            {
                ItemRetrieved(this, new EventArgs());
            }
        }
        #endregion

        #region publicMethods

        #region retrieve
        public DataModel.Common.IDataStoreItem RetrieveItem(int id, Type type)
        {
            
                DbSet s = db.Set(type);
            _itemsRetrieved++;
                return (IDataStoreItem)s.Find(id);
           
        }

        public DataModel.Common.IDataStoreItem RetrieveItem(DataModel.Common.IDataStoreItem item)
        {
            if (item.ID != 0)
            {
                int id = item.ID;
                return RetrieveItem(id, item.GetType());
            }
            else
            {
                throw new Exception("Couldn't find that item in the database");
            }
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

                {
                    IEnumerable<IDataStoreItem> result;
                    if (type == typeof(User))
                    {
                        result = db.Users;
                    }
                    else if (type == typeof(Patient))
                    {
                        result = db.Patients;
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
                                    //logger.Trace("SELECT * FROM Studies INNER JOIN Appointments on Appointments.Study_ID = Studies.ID WHERE Appointments.ScheduledArrivalTime > '" + s1 + "' AND Appointments.ScheduledArrivalTime < '" + s2 + "' AND Studies.StudyType_ID = " + idNum.ToString());
                                    return db.Studies.SqlQuery("SELECT * FROM Studies INNER JOIN Appointments on Appointments.Study_ID = Studies.ID WHERE Appointments.ScheduledArrivalTime > '" + s1 + "' AND Appointments.ScheduledArrivalTime < '" + s2 + "' AND Studies.StudyType_ID = " + idNum.ToString()).ToList<IDataStoreItem>();
                                }
                                else
                                {
                                    _itemsRetrieved = _itemsRetrieved + db.Appointments.Where(x => x.ScheduledArrivalTime > d1 && x.ScheduledArrivalTime < d2).Count();
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
                        if (criteria.Any())
                        {
                            if (criteria.First().PropertyName == "ScheduledArrivalTime")
                            {
                                DateTime d1 = Convert.ToDateTime(criteria.First().FilterValue);
                                DateTime d2 = Convert.ToDateTime(criteria.ElementAt(1).FilterValue);
                                string s1 = d1.ToString("yyyyMMdd");
                                string s2 = d2.ToString("yyyyMMdd");
                                
                                    _itemsRetrieved = _itemsRetrieved + db.Appointments.Where(x => x.ScheduledArrivalTime > d1 && x.ScheduledArrivalTime < d2).Count();
                                    return db.Appointments.SqlQuery("SELECT * FROM Appointments WHERE ScheduledArrivalTime > '" + s1 + "' AND ScheduledArrivalTime < '" + s2 + "'").ToList<IDataStoreItem>();
                            

                            }
                            else
                            {

                            }
                        }
                        result = db.Appointments;
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
                    else if (type == typeof(StandardTaskType))
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
                    else if (type == typeof(Chemical))
                    {
                        result = db.Chemicals.ToList<IDataStoreItem>();
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
                    else if (type == typeof(Element))
                    {
                        result = db.Elements.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Isotope))
                    {
                        result = db.Isotopes.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(BaseBulkDose))
                    {
                        result = db.BulkDoses.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(BaseUnitDose))
                    {
                        result = db.UnitDoses.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Generator))
                    {
                        result = db.Generators.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(DoseAdministrationTask))
                    {
                        result = db.BasicTasks.OfType<DoseAdministrationTask>().ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(ScanTask))
                    {
                        result = db.BasicTasks.OfType<ScanTask>().ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(Kit))
                    {
                            
                        result = db.ColdKits.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(DataStoreItemAlteration))
                    {
                        result = db.DataStoreItemAlterations.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(KitDefinition))
                    {
                        result = db.KitDefinitions.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(EquipmentItem))
                    {
                        result = db.EquipmentItems.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(EquipmentItemType))
                    {
                        result = db.EquipmentTypes.ToList<IDataStoreItem>();
                    }
                    else if (type == typeof(PatientImage))
                    {
                        result = db.PatientImages;
                    }
                    else if (type == typeof(DataDictionaryNamespace))
                    {
                        result = db.DataDictionaryNameSpaces;
                    }
                    else if (type == typeof(DataDictionaryEntry))
                    {
                        result = db.DataDictionaryEntries;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    //result = result.Where(x => x.Deleted == false).ToList();
                    logger.Trace("....result has returned ");
                    foreach (RetrievalCriteria rc in criteria)
                    {
                        if (rc.FilterValue != null)
                            logger.Trace("Processing RetrievalCriteria " + rc.PropertyName + " " + rc.CriteriaType.ToString() + " " + rc.FilterValue.ToString());
                        else
                            logger.Trace("Processing RetrievalCriteria " + rc.PropertyName + " " + rc.CriteriaType.ToString() + " null");
                        Type propertyType = type.GetProperty(rc.PropertyName).PropertyType;


                        //logger.Trace("propertyType.Name = " + propertyType.Name + "Type.GetTypeCode(propertyType) = " + Type.GetTypeCode(propertyType));
                        if (TypeChecker.IsNumericType(propertyType))
                        {
                            logger.Trace("isNumericType == true");
                            switch (rc.CriteriaType)
                            {
                                case CriteraType.Equals:
                                    double tmp = Convert.ToDouble(rc.FilterValue);
                                    result = result.Where(x => Convert.ToDouble(x.GetType().GetProperty(rc.PropertyName).GetValue(x, null)) == tmp);

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
                        else if (propertyType.IsSubclassOf(typeof(DataStoreItem)))
                        {
                            if (rc.CriteriaType == CriteraType.Equals)
                            {
                                result = result.Where(x => (x.GetType().GetProperty(rc.PropertyName).GetValue(x, null) as IDataStoreItem).ID == (rc.FilterValue as IDataStoreItem).ID).ToList();
                            }
                        }
                        else
                        {
                            logger.Trace("isDateTime == False && isNumericType == false");
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
                                case CriteraType.IsNotNull:
                                    result = result.Where(x => x.GetType().GetProperty(rc.PropertyName).GetValue(x, null) != null);
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
                    logger.Trace("All criteria processed");
                    logger.Trace("Result includes " + result.Count().ToString() + " items");

                    _itemsRetrieved = _itemsRetrieved + result.Count();
                    //db.Configuration.AutoDetectChangesEnabled = true;
                    return result.ToList();
                }
            }

        }

        private List<IDataStoreItem> RetrieveAppointments(DateTime d)
        {
            DateTime startDate = d.Date;
            DateTime endDate = startDate.AddDays(1);
            IEnumerable<IDataStoreItem> result = db.Appointments.Where(x => x.ScheduledArrivalTime > startDate && x.ScheduledArrivalTime < endDate);
            _itemsRetrieved = _itemsRetrieved + result.Count();
            return result.ToList();
        }

        public IEnumerable<IDataStoreItem> RetrieveItems(Type type)
        {
            
            IEnumerable<IDataStoreItem> result;
            if (type == typeof(User))
            {
                result = db.Users;
            }
            else if (type == typeof(Patient))
            {
                result = db.Patients;
            }
            else if (type == typeof(Study))
            {
                result = db.Studies;
               
            }
            else if (type == typeof(Appointment))
            {
               
                result = db.Appointments;
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
            else if (type == typeof(StandardTaskType))
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
            else if (type == typeof(Chemical))
            {
                result = db.Chemicals.ToList<IDataStoreItem>();
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
            else if (type == typeof(Element))
            {
                result = db.Elements.ToList<IDataStoreItem>();
            }
            else if (type == typeof(Isotope))
            {
                result = db.Isotopes.ToList<IDataStoreItem>();
            }
            else if (type == typeof(BaseBulkDose))
            {
                result = db.BulkDoses.ToList<IDataStoreItem>();
            }
            else if (type == typeof(BaseUnitDose))
            {
                result = db.UnitDoses.ToList<IDataStoreItem>();
            }
            else if (type == typeof(Generator))
            {
                result = db.Generators.ToList<IDataStoreItem>();
            }
            else if (type == typeof(DoseAdministrationTask))
            {
                result = db.BasicTasks.OfType<DoseAdministrationTask>().ToList<IDataStoreItem>();
            }
            else if (type == typeof(ScanTask))
            {
                result = db.BasicTasks.OfType<ScanTask>().ToList<IDataStoreItem>();
            }
            else if (type == typeof(Kit))
            {

                result = db.ColdKits.ToList<IDataStoreItem>();
            }
            else if (type == typeof(DataStoreItemAlteration))
            {
                result = db.DataStoreItemAlterations.ToList<IDataStoreItem>();
            }
            else if (type == typeof(KitDefinition))
            {
                result = db.KitDefinitions.ToList<IDataStoreItem>();
            }
            else if (type == typeof(PatientImage))
            {
                result = db.PatientImages;
            }
            else if(type == typeof(ReconstitutedColdKit))
            {
                result = db.BulkDoses.Where(x => x is ReconstitutedColdKit);
            }
            else if (type == typeof(Elution))
            {
                result = db.BulkDoses.Where(x => x is Elution);
            }
            else if(type == typeof(BaseRadioactiveInventoryItem))
            {
                var sets =
                    from p in typeof(IRadiateContext).GetProperties()
                    where p.PropertyType.IsGenericType
                    && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
                    let entityType = p.PropertyType.GetGenericArguments().First()
                    where typeof(BaseRadioactiveInventoryItem).IsAssignableFrom(entityType)
                    select p.GetValue(db,null);

                IEnumerable<IDataStoreItem> val = null;
                foreach(var p in sets)
                {
                    System.Diagnostics.Debug.WriteLine("DBSet = " +p.ToString());
                }
                return null;
            }
            else if (type == typeof(EquipmentItemType))
            {
                result = db.EquipmentTypes;
            }
            else if (type == typeof(EquipmentItem))
            {
                result = db.EquipmentItems;
            }
            else if (type == typeof(DataDictionaryNamespace))
            {
                result = db.DataDictionaryNameSpaces;
            }
            else if (type == typeof(DataDictionaryEntry))
            {
                result = db.DataDictionaryEntries;
            }
            else
            {
             
                throw new NotImplementedException();
            }
            return result;
        }

        

        #endregion

        #region save
        public bool SaveItem(DataModel.Common.IDataStoreItem item)
        {
            lock (thisLock)
            {
                try
                {
                    if (item == null)
                    {
                        logger.Error("There was an attempt to save a null item");
                        return false;
                    }
                    if (item.ID == 0)
                    {

                        item.CreationDate = DateTime.Now;
                        item.LastEditDate = item.CreationDate;
                        item.Creator = Platform.CurrentUser;
                        db.Entry(item).State = EntityState.Added;
                        //db.Files.Add(item as File);
                        List<DbEntityEntry> modifiedChanges = db.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).ToList();
                        int numChanges = modifiedChanges.Count;
                        
                            db.SaveChanges();
                            _itemsSaved++;
                            _itemsSaved = _itemsSaved + numChanges;
                       
                        
                    }
                    else if (DateTime.Compare(db.Entry(item).GetDatabaseValues().GetValue<DateTime>("LastEditDate"), item.LastEditDate) != 0)
                    {
                        logger.Error("Throw concurrency Error db.LastEditDate  = " + db.Entry(item).GetDatabaseValues().GetValue<DateTime>("LastEditDate") + " item.LastEditDate = " + item.LastEditDate);
                        db.Entry(item).State = EntityState.Modified;
                        db.Entry(item).Reload();

                        throw new Exception("Concurrency Error, reloaded from database");

                    }
                    else
                    {
                        
                        item.LastEditDate = DateTime.Now;
                        List<DbEntityEntry> modifiedChanges = db.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).ToList();
                        int numChanges = modifiedChanges.Count;
                        //AddDataStoreItemAlterations(item);
                        db.SaveChanges();
                        _itemsSaved = _itemsSaved + numChanges;
                    }


                }
                catch (Exception ex)
                {

                    logger.Error("Error during SaveItem() " + ex.Message);
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        logger.Error("...Inner exception: " + ex.Message);
                    }
                    var myException = new Exception("SaveItem in the dataretriever failed to save: " + ex.Message, ex);

                    throw myException;
                }
                return true;
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
        #endregion

        #region Delete
        public void DeleteItem(IDataStoreItem item)
        {
            lock (thisLock)
            {
                item.Deleted = true;
                item.DeletionDate = DateTime.Now;
                SaveItem(item);
            }
            
        }
        public void UnDeleteItem(IDataStoreItem item)
        {
            item.Deleted = false;

            SaveItem(item);
        }
        #endregion

        #region reload
        public void ReloadAll()
        {
            // Get all objects in statemanager with entityKey 
            // (context.Refresh will throw an exception otherwise) 
            List<DbEntityEntry> modifiedChanges = db.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).ToList();
            foreach (DbEntityEntry entry in modifiedChanges)
            {
                db.Entry(entry.Entity).Reload();
            }
        }

        public void UpdateItem(IDataStoreItem item)
        {
            lock (thisLock)
            {
                db.Entry(item).Reload();
                if (item is Appointment)
                {
                    //System.Diagnostics.Debug.WriteLine("Item being updated is appointment, Tasks will be loaded");
                    //db.Entry(item).Reference(p => (p as Appointment).Tasks).Load();
                    //db.Entry(item).Reference("Tasks").Load();
                    db.Entry(item).Collection(p => (p as Appointment).Tasks).Load();
                }
            }
            

        }
        #endregion

        #region AutoDetect
        public void SwitchOnAutoDetect()
        {
            db.Configuration.AutoDetectChangesEnabled = true;
        }

        public void SwitchOffAutoDetect()
        {
            db.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion

        #endregion

        #region publicProperties
        /// <summary>
        /// The total number of items that were explicitly retrieved by the application
        /// </summary>
        public int NumberOfItemsRetrieved
        {
            get
            {
                return _itemsRetrieved;
            }
        }

        /// <summary>
        /// The total number of items that have been explicitly saved
        /// </summary>
        public int NumberOfItemsSaved
        {
            get
            {
                return _itemsSaved;
            }
        }

        /// <summary>
        /// The total number of items tracked for changes
        /// </summary>
        public int TotalItemsTracked
        {
            get
            {
                var result = db.ChangeTracker.Entries().Count();
                return result;
            }
        }

        /// <summary>
        /// The total number of items that have been modified
        /// </summary>
        public int NumberOfModifiedItems
        {
            get
            {
               var result = db.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).Count();
                return result;
            }
        }

        public List<ModifiedDataStoreItem> ModifiedDataStoreItems
        {
            get
            {
                if (_modifiedDataStoreItems == null)
                    _modifiedDataStoreItems = new List<ModifiedDataStoreItem>();
                refreshModifiedItems();
                return _modifiedDataStoreItems;
            }
        }
        #endregion

        #region privateMethods
        private void AddDataStoreItemAlterations(IDataStoreItem item)
        {
            foreach (PropertyInfo p in item.ConcreteType.GetProperties())
            {
                System.Diagnostics.Debug.WriteLine("p.Name = " + p.Name);
                Attribute[] attrs = Attribute.GetCustomAttributes(p);
                foreach (System.Attribute attr in attrs)
                {
                    System.Diagnostics.Debug.WriteLine("attr.ToString() = " + attr.ToString());
                    if (attr is AuditableAttribute)
                    {
                        Type t = p.PropertyType;
                        var oldVal = db.Entry(item).OriginalValues.GetValue<object>(p.Name);

                        PropertyInfo pi = item.GetType().GetProperty(p.Name);
                        if (oldVal == null)
                        {
                            System.Diagnostics.Debug.WriteLine("oldVal IS null");
                            DataStoreItemAlteration d = new DataStoreItemAlteration();

                            d.ItemIDNumber = item.ID;
                            d.Workstation = Platform.CurrentWorkstation;
                            d.PropertyName = p.Name;
                            d.OldValue = "";
                            if (pi.GetValue(item, null) == null)
                            {
                                d.NewValue = "";
                            }
                            else
                            {
                                d.NewValue = pi.GetValue(item, null).ToString();
                                d.DataStoreItemName = item.ConcreteType.Name;
                                db.DataStoreItemAlterations.Add(d);
                            }


                        }
                        else if (pi.GetValue(item, null).ToString() != oldVal.ToString())
                        {
                            System.Diagnostics.Debug.WriteLine("oldVal is Not null");

                            DataStoreItemAlteration d = new DataStoreItemAlteration();
                            d.ItemIDNumber = item.ID;
                            d.Workstation = Platform.CurrentWorkstation;
                            d.PropertyName = p.Name;
                            d.OldValue = oldVal.ToString();
                            d.NewValue = pi.GetValue(item, null).ToString();
                            d.DataStoreItemName = item.ConcreteType.Name;
                            db.DataStoreItemAlterations.Add(d);
                        }
                    }
                }
            }

        }

        [Obsolete]
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

        private bool IsPropertyACollection(PropertyInfo property)
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

        private void refreshModifiedItems()
        {
            db.ChangeTracker.DetectChanges();
            _modifiedDataStoreItems.Clear();
            foreach (var i in db.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified))
            {
                ModifiedDataStoreItem m = new ModifiedDataStoreItem();
                m.IDNumber = (i.Entity as IDataStoreItem).ID.ToString();
                m.ItemType = (i.Entity as IDataStoreItem).ConcreteType.Name;
                m.Name = (i.Entity as IDataStoreItem).ToString();

                _modifiedDataStoreItems.Add(m);
                var originalValues = i.OriginalValues;
                var currentValues = i.CurrentValues;
                foreach (string propertyName in originalValues.PropertyNames)
                {
                    var original = originalValues[propertyName];
                    var current = currentValues[propertyName];
                    DataStoreItemProperty dip = new DataStoreItemProperty();
                    dip.PropertyName = propertyName;
                    if (original != null)
                        dip.OriginalValue = original.ToString();
                    else
                        dip.OriginalValue = "NULL";
                    if (current != null)
                        dip.CurrentValue = current.ToString();
                    else
                        dip.CurrentValue = "NULL";
                    m.Properties.Add(dip);
                }
            }
        }
        #endregion

        public void printAllModified()
        {
            foreach(var i in db.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified))
            {
                Console.WriteLine((i.Entity as IDataStoreItem).ConcreteType);
                var originalValues = i.OriginalValues;
                var currentValues = i.CurrentValues;

                foreach (string propertyName in originalValues.PropertyNames)
                {
                    var original = originalValues[propertyName];
                    var current = currentValues[propertyName];

                    if (!Equals(original, current))
                    {
                        Console.WriteLine("    Property: " + propertyName + "; original: " + original + "; current = " + current);
                    }
                }
            }
        }

        public void printAllUnmodified()
        {
            foreach (var i in db.ChangeTracker.Entries().Where(x => x.State == EntityState.Unchanged))
            {
                Console.WriteLine((i.Entity as IDataStoreItem).ConcreteType);
                
            }
        }

       
    }
}
