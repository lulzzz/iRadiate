using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight.Command;

using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Radiopharmacy;

namespace iRadiate.Desktop.Common.ViewModel
{
    
    public class DoseAdministrationTaskViewModel : BaseTaskViewModel
    {

        #region constructors
        public DoseAdministrationTaskViewModel()
            : base()
        {
            
        }

        public DoseAdministrationTaskViewModel(DataStoreItem item)
            : base(item)
        {
          
        }
        #endregion

        #region overrides
        public override void NonUIThreadInitialize()
        {
            base.NonUIThreadInitialize();
            if (CalibrationTime < new DateTime(1990, 1, 1))
            {
                CalibrationTime = DateTime.Now;
                //ResidualMeasurementTime = DateTime.Now;
            }
        }

        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            PrintLabelCommand = new RelayCommand(printLabel);
        }
        #endregion

        #region publicProperties
        public double InjectedActivity
        {
            get
            {
                return (Double)((DoseAdministrationTask)Item).UnitDose.NetInjected;
            }
        }

        public double CalibrationActivity
        {
            get
            {
                if (!DoseAssigned)
                {
                    return 0;
                }
                return ((DoseAdministrationTask)Item).UnitDose.CalibrationActivity;
            }
            
        }

        public double CurrentActivity
        {
            get
            {
                if (!DoseAssigned)
                {
                    return 0;
                }
                if (Completed)
                {
                    return InjectedActivity;
                }
                return (Item as DoseAdministrationTask).UnitDose.CurrentActivity;
            }
        }

        
        public Nullable<DateTime> CalibrationTime
        {
            get
            {
                if (!DoseAssigned)
                {
                    return null;
                }
                return ((DoseAdministrationTask)Item).UnitDose.CalibrationDate;
            }
            set
            {
                ((DoseAdministrationTask)Item).UnitDose.CalibrationDate = value.Value;
            }
        }

        public virtual Chemical Radiopharmaceutical
        {
            get
            {
                return ((DoseAdministrationTask)Item).PrescribedRadioPharmaceutical;
            }
            set
            {
                ((DoseAdministrationTask)Item).PrescribedRadioPharmaceutical = value;
                RaisePropertyChanged("Radiopharmaceutical");
            }
        }

        public int PrescribedMinimum
        {
            get
            {
                return ((DoseAdministrationTask)Item).PrescribedMinimum;
            }
            set
            {
                ((DoseAdministrationTask)Item).PrescribedMinimum = value;
                RaisePropertyChanged("PrescribedMinimum");
            }
        }

        public int PrescribedMaximum
        {
            get
            {
                return ((DoseAdministrationTask)Item).PrescribedMaximum;

            }
            set
            {
                ((DoseAdministrationTask)Item).PrescribedMaximum = value;
                RaisePropertyChanged("PrescribedMaximum");
            }
        }

        public Nullable<DateTime> AdministrationTime
        {
            get
            {
                if (((DoseAdministrationTask)Item).UnitDose != null)
                {
                    return ((DoseAdministrationTask)Item).UnitDose.AdministrationDate;
                }
                return null;
            }
            set
            {
                if(((DoseAdministrationTask)Item).UnitDose != null)
                {
                    ((DoseAdministrationTask)Item).UnitDose.AdministrationDate = value.Value;
                }
                RaisePropertyChanged("AdministrationTime");
            }
            
            
        }

        public bool DoseAssigned
        {
            get
            {
                if((Item as DoseAdministrationTask).UnitDose != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<IDataStoreItem> Radiopharmaceuticals
        {
            get
            {
                return DesktopApplication.Librarian.GetItems(typeof(Chemical), new List<RetrievalCriteria>()).ToList();
            }
        }

        /// <summary>
        /// Marks the task as competed
        /// </summary>
        /// <remarks>
        /// This property will also handle a lot of the logic of completing a task.
        /// </remarks>
        public override bool Completed
        {
            get
            {
                return base.Completed;
            }

            set
            {
                base.Completed = value;
                CompletionTime = DateTime.Now;
                //ResidualMeasurementTime = DateTime.Now;
                AdministrationTime = DateTime.Now.AddMinutes(-2);
                User = DesktopApplication.CurrentUser;
                RaisePropertyChanged("Completed");
                RaisePropertyChanged("CompletionTime");
                (Item as DoseAdministrationTask).UnitDose.Disposed = DisposalStatus.Disposed;
                (Item as DoseAdministrationTask).UnitDose.DisposalDate = DateTime.Now;
                
            }
        }

  
        public override bool ChecksPerformed
        {
            get
            {
                if (!(Item as BasicTask).TimeoutPerformed.HasValue)
                {
                    return false;
                }
                if (!(Item as BasicTask).RequestFormCorrect.HasValue)
                {
                    return false;
                }
                if ((Item as BasicTask).TimeoutPerformed.Value && (Item as BasicTask).RequestFormCorrect.Value)
                {
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region publicMethods

        #endregion

        #region privateMethods
        private void printLabel()
        {
            if ((Item as DoseAdministrationTask).UnitDose == null)
                return;
            var vm = new BaseUnitDoseViewModel((Item as DoseAdministrationTask).UnitDose);
            
            DesktopApplication.MakeModalDocument(vm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.UnitDosePrintView");
        }
        #endregion

        #region commands
        public RelayCommand PrintLabelCommand { get; set; }
        #endregion
    }
}
