using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.DataModel.Radiopharmacy
{
    public abstract class BaseUnitDose : BaseRadioactiveInventoryItem, IUnitDose
    {
        private bool _prepared;
        private Patient _patient;
        private bool _administered;
        private DateTime _administrationDate;

        private BaseBulkDose _bulkDose;
       
        private BasicTask _doseAdministrationTask;
        private Chemical _chemical;

        public BaseUnitDose() : base()
        {

        }

        /// <summary>
        /// Gets or sets whether this dose has been prepared for a patient
        /// </summary>
        public virtual bool Prepared
        {
            get
            {
                return _prepared;
            }
            set
            {
                _prepared = value;
            }
        }

        /// <summary>
        /// Gets or sets whether this dose has been administered.
        /// </summary>
        [NotMapped]
        
        public virtual bool Administered
        {
            get
            {
                if(DoseAdministrationTask != null)
                {
                    return DoseAdministrationTask.Completed;
                }
                return false;
            }
            set
            {
                //_administered = value;
            }
        }

        /// <summary>
        /// Gets or sets the date on which this dose was administed.
        /// </summary>
        [Queryable]
        public DateTime AdministrationDate
        {
            get
            {
                return _administrationDate;
            }
            set
            {
                _administrationDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the patient for whom this dose is prepared.
        /// </summary>
        [NotMapped]
        public virtual Patient Patient
        {
            get
            {
                if(DoseAdministrationTask != null)
                {
                    if(DoseAdministrationTask.Patient != null)
                    {
                        return DoseAdministrationTask.Patient;
                    }
                }
                return null;
            }
            
        }

        [Queryable]
        public virtual BaseBulkDose BulkDose
        {
            get
            {
                return _bulkDose;
            }
            set
            {
                _bulkDose = value;
            }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(BaseUnitDose);
            }
        }

        

        public override bool IsExpirable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the DoseAdministrationTask that this UnitDose is assigned to
        /// </summary>
        [Queryable]
        public virtual BasicTask DoseAdministrationTask
        {
            get
            {
                return _doseAdministrationTask;
            }
            set
            {
                
                if(value == null)
                {
                    (_doseAdministrationTask as DoseAdministrationTask).SetUnitDose(null);
                    _doseAdministrationTask = value;
                }
                else
                {
                    _doseAdministrationTask = value;
                    (_doseAdministrationTask as DoseAdministrationTask).SetUnitDose(this);
                }
                

            }
        }

        public override string InventoryName
        {
            get
            {
                if(Radiopharmaceutical != null)
                {
                    return Radiopharmaceutical.Name;
                }
                return base.InventoryName;
            }
        }

        public void SetDoseAdministrationTask(DoseAdministrationTask d)
        {
            _doseAdministrationTask = d;
        }

        [Queryable]
        public double NetInjected
        {
            get
            {
                if (!Administered)
                {
                    return 0;
                }
                else
                {
                    
                        if (Radiopharmaceutical != null)
                        {
                            double hours = (CalibrationDate - AdministrationDate).TotalHours;
                            double hLife = Radiopharmaceutical.Isotope.HalfLife / 60;
                            double dConst = Math.Log(2) / hLife;
                            return CalibrationActivity * Math.Exp(dConst * hours);
                        }

                    }
                    return CalibrationActivity;
                
            }

        }
    }

    /// <summary>
    /// A unit dose in a syringe. Features properties like volume, residuala activity etc.
    /// </summary>
    public class SyringeUnitDose :BaseUnitDose
    {
        private double _volume;
        private double? _residualActivity;
        private DateTime? _residualMeasurementDate;

        public SyringeUnitDose() : base()
        {

        }

        /// <summary>
        /// Gets or sets the volume in the syringe in ml
        /// </summary>
        [Queryable]
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
        /// Gets or sets the residual activity in the syringe after injection
        /// </summary>
        [Queryable]
        public double? ResidualActivity
        {
            get { return _residualActivity; }
            set { _residualActivity = value; }
        }

        /// <summary>
        /// Gets or sets the date-time when the residual activity was measured.
        /// </summary>
        [Queryable]
        public DateTime? ResidualMeasurementDate
        {
            get { return _residualMeasurementDate; }
            set { _residualMeasurementDate = value; }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(SyringeUnitDose);
            }
        }
    }

    public class CapsuleUnitDose :BaseUnitDose
    {
        public CapsuleUnitDose() : base()
        {

        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(CapsuleUnitDose);
            }
        }
    }

    public class GaseousUnitDose : BaseUnitDose
    {
        public GaseousUnitDose() : base()
        {

        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(GaseousUnitDose);
            }
        }
    }

    public class SplitUnitDose : SyringeUnitDose
    {
        private int? _numberOfSubDoses;
        private string _subDoseFractions;

        public SplitUnitDose() : base()
        {

        }

        /// <summary>
        /// Gets or sets the number of individiual dose that this has bene divided into
        /// </summary>
        public int? NumberOfSubDoses
        {
            get { return _numberOfSubDoses; }
            set { _numberOfSubDoses = value; }
        }

        /// <summary>
        /// Gets or sets a string describing how the dose is split
        /// </summary>
        /// <remarks>
        /// If it is splito into 4 equal doses then this string becomes "0.25\0.25\0.25\0.25"
        /// </remarks>
        public string SubDoseFractions
        {
            get { return _subDoseFractions; }
            set { _subDoseFractions = value; }
        }

        public override Type ConcreteType
        {
            get
            {
                return typeof(SplitUnitDose);
            }
        }
    }
}
