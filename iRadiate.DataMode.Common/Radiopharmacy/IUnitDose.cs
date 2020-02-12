using System;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;

namespace iRadiate.DataModel.Radiopharmacy
{
    public interface IUnitDose
    {
        /// <summary>
        /// Gets or sets whether the unit dose has been administered
        /// </summary>
        bool Administered { get; set; }

        /// <summary>
        /// Gets or sets the date on which the dose was administered
        /// </summary>
        DateTime AdministrationDate { get; set; }

        /// <summary>
        /// Gets or sets the BulkDose from which the unit dose was created.
        /// </summary>
        BaseBulkDose BulkDose { get; set; }

        /// <summary>
        /// The DoseAdministationTask where this unit dose has been assigned
        /// </summary>
        BasicTask DoseAdministrationTask { get; set; }
        string InventoryName { get; }
        bool IsExpirable { get; }
        Patient Patient { get; }
        bool Prepared { get; set; }
        void SetDoseAdministrationTask(DoseAdministrationTask d);
    }
}