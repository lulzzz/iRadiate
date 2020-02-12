using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.DataModel.Radiopharmacy;

namespace Reporting
{
    [Export(typeof(IQueryableDataItem))]
    public class UnitDoseQueryableDataItem : BaseQueryableDataItem
    {
        public UnitDoseQueryableDataItem() : base()
        {

        }

        public override Type DataStoreItemType
        {
            get
            {
                return typeof(BaseUnitDose);
            }
        }

        public override string Name
        {
            get
            {
                return "Unit Dose";
            }
        }
    }

    [Export(typeof(IQueryableDataItem))]
    public class ReconstitutdedColdKitQueryableDataItem : BaseQueryableDataItem
    {
        public ReconstitutdedColdKitQueryableDataItem() : base()
        {

        }
        public override Type DataStoreItemType
        {
            get
            {
                return typeof(ReconstitutedColdKit);
            }
        }

        public override string Name
        {
            get
            {
                return "Reconstituted cold kit";
            }
        }
    }

    [Export(typeof(IQueryableDataItem))]
    public class ElutionQueryableDataItem : BaseQueryableDataItem
    {
        public ElutionQueryableDataItem() : base()
        {

        }

        public override Type DataStoreItemType
        {
            get
            {
                return typeof(Elution);
            }
        }

        public override string Name
        {
            get
            {
                return "Elution";
            }
        }
    }

    [Export(typeof(IQueryableDataItem))]
    public class GeneratorQueryableDataItem : BaseQueryableDataItem
    {
        public GeneratorQueryableDataItem() : base()
        {

        }

        public override Type DataStoreItemType
        {
            get
            {
                return typeof(Generator);
            }
        }

        public override string Name
        {
            get
            {
                return "Generator";
            }
        }
    }

    [Export(typeof(IQueryableDataItem))]
    public class ColdKitQueryableDataItem : BaseQueryableDataItem
    {
        public ColdKitQueryableDataItem() : base()
        {

        }

        public override string Name
        {
            get
            {
                return "Cold Kit";
            }
        }

        public override Type DataStoreItemType
        {
            get
            {
                return typeof(Kit);
            }
        }
    }
}
