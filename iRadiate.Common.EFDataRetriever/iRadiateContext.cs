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
using iRadiate.DataModel.QA;
using iRadiate.DataModel.RadioPharmacy;


namespace iRadiate.Common.EFDataRetriever
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
        //public DbSet<MedicalSpecialty> MedicalSpecialties { get; set; }
        #endregion

        #region NucMed
        public DbSet<BasicTask> BasicTasks { get; set; }
        //public DbSet<Scan> Scans { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<NucMedPractice> NucMedPractices { get; set; }
        public DbSet<StaffMemberRole> StaffMemberRoles { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<StandardFiniteTaskType> StandardFiniteTaskTypes { get; set; }
        public DbSet<StandardNonFiniteTaskType> StandardNonFiniteTaskTypes { get; set; }
        public DbSet<Study> Studies { get; set; }
        public DbSet<StudyRequest> StudyRequests { get; set; }
        public DbSet<StudyReport> StudyReports { get; set; }
        public DbSet<StudyType> StudyTypes { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<Camera> Camera { get; set; }
        public DbSet<BaseConstraint> Constraints { get; set; }
        public DbSet<RoomReservation> RoomReservations { get; set; }
        public DbSet<Note> Notes { get; set; }
        
        #endregion

        #region RadioPharmacy
        public DbSet<Element> Elements { get; set; }
        public DbSet<Isotope> Isotopes { get; set; }
        public DbSet<Radiopharmaceutical> Radiopharmaceuticals { get; set; }
        public DbSet<PreparedDose> PreparedDoses { get; set; }
        #endregion

        #region QA
        //public DbSet<EquipmentItem> EquipmentItems { get; set; }
        #endregion










        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {



           
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
