using System;
using System.Collections.Generic;
using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Radiopharmacy
{
    public interface IBulkDose
    {
        Chemical Radiopharmaceutical { get; set; }
        string InventoryName { get; }
       
        List<BaseUnitDose> UnitDoses { get; set; }

        event EventHandler<NewDataStoreItemEventArgs> UnitDoseDrawn;
    }
}