using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Equipment;
using iRadiate.DataModel.Radiopharmacy;
using iRadiate.DataModel.DataDictionary;

namespace iRadiate.Common.IO
{
    public class IRadiateContext : DbContext
    {
        public IRadiateContext()
            : base()
        {

        }

        public IRadiateContext(string connectionString)
            : base(connectionString)
        {
            
        }

        #region Common
       
        //public DbSet<Country> Countries { get; set; }
        //public DbSet<Province> Provinces { get; set; }
        //public DbSet<Town> Towns { get; set; }
        //public DbSet<Address> Addressess { get; set; }
        //public DbSet<PostalAddress> PostalAddresses { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<DataStoreItemAlteration> DataStoreItemAlterations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Workstation> Workstations { get; set; }
        #endregion

        #region HealthCare
     
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Patient> Patients { get; set; }
        
        #endregion

        #region NucMed
        public DbSet<BasicTask> BasicTasks { get; set; }
        //public DbSet<Scan> Scans { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<NucMedPractice> NucMedPractices { get; set; }
        public DbSet<StaffMemberRole> StaffMemberRoles { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<StandardFiniteTaskType> StandardFiniteTaskTypes { get; set; }
        public DbSet<StandardTaskType> StandardNonFiniteTaskTypes { get; set; }
        public DbSet<Study> Studies { get; set; }
        public DbSet<StudyRequest> StudyRequests { get; set; }
        public DbSet<StudyReport> StudyReports { get; set; }
        public DbSet<StudyType> StudyTypes { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<BaseConstraint> Constraints { get; set; }
        public DbSet<RoomReservation> RoomReservations { get; set; }        
        public DbSet<NMEnergyWindow> NMEnergyWindows { get; set; }
        
        public DbSet<PatientImage> PatientImages { get; set; }
        #endregion

        #region RadioPharmacy
        public DbSet<Element> Elements { get; set; }
        public DbSet<Isotope> Isotopes { get; set; }
        public DbSet<Chemical> Chemicals { get; set; }
        public DbSet<BaseUnitDose> UnitDoses { get; set; }
        public DbSet<BaseBulkDose> BulkDoses { get; set; }
        public DbSet<Generator> Generators { get; set; }
        public DbSet<RadiochemicalPurityAnalysis> RCPurityAnalyses { get; set; }
        public DbSet<RadiochemicalPurityMeasurement> RCPurityMeasurements { get; set; }
        public DbSet<Kit> ColdKits { get; set; }

        public DbSet<KitDefinition> KitDefinitions { get; set; }
        #endregion

        #region QA
        public DbSet<EquipmentItem> EquipmentItems { get; set; }

        public DbSet<EquipmentItemType> EquipmentTypes { get; set; }

        #endregion


        #region DataDictionary
        public DbSet<BaseDataItem> BaseDataItems { get; set; }

        public DbSet<DataDictionaryNamespace> DataDictionaryNameSpaces { get; set; }

        public DbSet<DataDictionaryEntry> DataDictionaryEntries { get; set; }
        #endregion







        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<BaseBulkDose>().HasOptional(s => s.QCAnalysis).WithOptionalPrincipal(x => x.BulkDose);


            modelBuilder.Conventions.Add(new DateTime2Convention());
        } 
    }

    public class DateTime2Convention : Convention
    {
        public DateTime2Convention()
        {
            this.Properties<DateTime>()
                .Configure(c => c.HasColumnType("datetime2"));
        }

        
    }

    

   
}
