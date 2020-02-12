using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace iRadiate.DataModel.Radiopharmacy
{
    public interface IKit
    {
        List<IBulkDose> BulkDoses { get; set; }
        Chemical Radiopharmaceutical { get;  }
        List<IUnitDose> UnitDoses { get;  }
        Chemical RadioactiveIngredient { get;  }
        KitDefinition KitDefinition { get; set; }
        DateTime ExpiryDate { get; set; }
        bool Expired { get;  }

        
    }

    public interface IKitDefinition
    {
        Chemical Product { get; set; }

        Chemical RadioactiveIngredient { get; set; }

        string Name { get; set; }

        bool ColdAdministerable { get; set; }

    }

    public enum KitProductType { [Description("Bulk Dose")]BulkDose,[Description("Unit Dose")]UnitDose};
}