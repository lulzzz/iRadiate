using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

namespace iRadiate.DataModel.HealthCare
{
    public interface IPatientSample
    {
        DateTime CollectionDate { get; set; }

        Patient Patient { get;  }

        Appointment Appointment { get; set; }

        User Collector { get; set; }
    }

    public class BasePatientSample : DataStoreItem, IPatientSample
    {
        private DateTime _collectionDate;
       
        private User _collector;
        public Appointment _appointment;

        public BasePatientSample() : base()
        {

        }

        /// <summary>
        /// Gets or sets the datetime when the sample was collected
        /// </summary>
        public virtual DateTime CollectionDate
        {
            get
            {
                return _collectionDate;
            }

            set
            {
                _collectionDate = value;
            }
        }

        /// <summary>
        /// Gets the patient from whome the sample was taken
        /// </summary>
        public virtual Patient Patient
        {
            get
            {
                if (Appointment == null)
                    return null;

                return Appointment.Patient;
            }

            
        }

        /// <summary>
        /// Gets or sets the user who collected the blood
        /// </summary>
        public User Collector
        {
            get
            {
                return _collector;
            }
            set
            {
                _collector = value;
            }
        }

        /// <summary>
        /// Gets or sets the appointment on which the sample was collected
        /// </summary>
        public Appointment Appointment
        {
            get { return _appointment; }
            set { _appointment = value; }
        }

    }

    /// <summary>
    /// Represents a blood sample which has been taken from a patient
    /// </summary>
    public class BloodSample : BasePatientSample
    {
        private double _volume;
        private VenipunctureSite _vesselType;
        public BloodSample() : base()
        {

        }

        /// <summary>
        /// Gets or sets the volume of blood drawn from the sample
        /// </summary>
        public double Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
            }
        }

        /// <summary>
        /// Gets or set the venipuncture site from which the sample was collected
        /// </summary>
        public VenipunctureSite VenipunctureSite
        {
            get { return _vesselType; }
            set { _vesselType = value; }
        }
        
    }
    public enum VenipunctureSite { [Description("Medical Cubital Vein")]MedicalCubitalVein,
        [Description("CephalicVein")]CephalicVein,
        [Description("Basilic Vein")]BasilicVein,
        [Description("Dorsal Metacarpal Vein")]DorsalMetacarpalVein };
    public enum InjectionMethod { Intravenous,
        Intramuscular,
        Subcutaneous,
        Intradermal,
        [Description("Intra-arterial")]Intraarterial,
        Intrathecal,
        [Description("Intra-articular")]Intraarticular }
}
