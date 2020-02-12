using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.Radiopharmacy;

namespace iRadiate.DataModel.NucMed
{
    /// <summary>
    /// Represents a scheduled event tht involves administering a unit dose
    /// </summary>
    [Export("TaskType")]
    public class DoseAdministrationTask : BasicTask
    {
        #region privateFields
        private BaseUnitDose _unitDose;       
        private Chemical _radioPharmaceutical;
        private int _prescribedMinimum;
        private int _prescribedMaximum;
        private double _residualActivity;
        private DateTime _residualMeasurementTime;
        private bool _patientVerified;
        private string _injectionSite;
        private AdministrationRoute? _administrationRoute;
        #endregion

        #region constructors
        public DoseAdministrationTask()
            : base()
        {
            
            
        }
        public DoseAdministrationTask(Appointment a)
            : base(a)
        {
            
        }
        #endregion

        #region publicProperties
        
        [Queryable]
        public virtual Chemical PrescribedRadioPharmaceutical
        {
            get
            {
                return _radioPharmaceutical;
            }
            set
            {
                _radioPharmaceutical = value;
            }
        }

        public override bool TaskTypesMatch(BasicTask otherTask)
        {
            if (otherTask is DoseAdministrationTask)
            {
                return true;
            }

            return false;
        }

        public override string TaskName
        {
            get
            {
                if (SequenceNumber > 0)
                {
                    return "Dose Admin " + SequenceNumber.ToString();
                }
                else
                {
                    return "Dose Admin";
                }
            }
        }

        [Auditable]
        [Queryable]
        public int PrescribedMinimum
        {
            get
            {
                return _prescribedMinimum;
            }
            set
            {
                _prescribedMinimum = value;
            }
        }

        [Auditable]
        [Queryable]
        public int PrescribedMaximum
        {
            get
            {
                return _prescribedMaximum;
            }
            set
            {
                _prescribedMaximum = value;
            }
        }

         public override Type ConcreteType
        {
            get
            {
                return typeof(DoseAdministrationTask);
            }
        }

         public override string Status
         {
             get
             {
                 if (Completed)
                 {
                    if(UnitDose != null)
                    {
                        if (UnitDose.Administered)
                        {
                            if (SequenceNumber > 0)
                            {
                                return "Dose " + SequenceNumber.ToString() + " administered " + UnitDose.AdministrationDate.ToShortTimeString();
                            }
                            else
                            {
                                return "Dose administered " + UnitDose.AdministrationDate.ToShortTimeString();
                            }
                        }
                        else
                        {
                            return "Dose administered";
                        }
                    }
                    else
                    {
                        if (SequenceNumber > 0)
                        {
                            return "Dose " + SequenceNumber.ToString() + " administered " + CompletionTime.ToShortTimeString();
                        }
                        else
                        {
                            return "Dose administered " + CompletionTime.ToShortTimeString();
                        }
                    }
                     
                    
                 }
                 else
                 {
                     return TaskName + " Pending";
                 }
             }
         }

        [Queryable]
        public virtual BaseUnitDose UnitDose
        {
            get
            {
                return _unitDose;
            }
            set
            {
                _unitDose = value;
                _unitDose.SetDoseAdministrationTask(this);
            }
        }

        [Queryable]
        public string PrescribedActivityRange
        {
            get { return PrescribedMinimum.ToString() + " - " + PrescribedMaximum.ToString(); }
        }

       
        [Queryable]
        public virtual AdministrationRoute? AdministrationRoute
        {
            get { return _administrationRoute; }
            set { _administrationRoute = value; }
        }

        [NotMapped]
        [Queryable]
        public string InjectionSite
        {
            get { return _injectionSite; }
            set { _injectionSite = value; }
        }

        #endregion

        #region publicMethods
        public void SetUnitDose(BaseUnitDose u)
        {
            if (u == null)
            {
                _unitDose = null;
                return;
            }
            _unitDose = u;
        }

        public override bool CompleteTask()
        {
            return base.CompleteTask();
        }

        
        #endregion

    }

    public enum AdministrationRoute
    {
        Intravenous, Intraarterial, [Description("Oral (liquid)")]OralLiquid, [Description("Oral (solid)")]OralSolid, Subcutaneous, Intrathecal, Sublingual, Rectal, Vaginal, Nasal, Inhalation, IntraArticular, IntraDermal
    }
}
