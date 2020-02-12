using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iRadiate.Common.IO;
using iRadiate.DataModel;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Radiopharmacy;
using iRadiate.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
namespace iRadiate.Desktop.Common.ViewModel
{
    public class BaseUnitDoseViewModel: RadioactiveInventoryItemViewModel
    {
        #region
        private List<IDataStoreItem> _potentialTasks;
        private IDataStoreItem _assignedTask;
        private List<IDataStoreItem> _potentialRadiopharmaceuticals;
        #endregion

        #region constructors
        public BaseUnitDoseViewModel() : base()
        {
            UnassignCommand = new RelayCommand(unAssignTask);
        }

        public BaseUnitDoseViewModel(DataStoreItem item) : base(item)
        {
            fillPotentialTasks();
            UnassignCommand = new RelayCommand(unAssignTask);
        }
        #endregion

        #region publicProperties
        public List<IDataStoreItem> PotentialTasks
        {
            get
            {
                if(_potentialTasks == null)
                {
                    fillPotentialTasks();
                }
                return _potentialTasks;
            }
            set
            {
                _potentialTasks = value;
                RaisePropertyChanged("PotentialTasks");
            }
        }

        public List<IDataStoreItem> PotentialRadiopharmaceuticals
        {
            get
            {
                return _potentialRadiopharmaceuticals;
            }
            set
            {
                _potentialRadiopharmaceuticals = value;
                RaisePropertyChanged("PotentialRadiopharmaceuticals");
            }
        }

        public virtual IDataStoreItem AssignedTask
        {
            get
            {
                if ((Item as BaseUnitDose).DoseAdministrationTask == null)
                    return null;
                return (Item as BaseUnitDose).DoseAdministrationTask;
            }
            set
            {
              
                (Item as BaseUnitDose).DoseAdministrationTask = (value as BasicTask);
               
                RaisePropertyChanged("AssignedTask");
            }
        }

        public virtual bool RadiopharmaceuticalSet
        {
            get
            {
                if (Item == null)
                    return false;

                if((Item as BaseUnitDose).Radiopharmaceutical == null)
                {
                    return false;
                }

                return true;
            }
        }

        public virtual bool RadiopharmaceuticalNotSet
        {
            get
            {
                return !RadiopharmaceuticalSet;
            }
        }

        public Chemical SelectedRadiopharmaceutical
        {
            get
            {
                return (Item as BaseUnitDose).Radiopharmaceutical;
            }
            set
            {
                (Item as BaseUnitDose).Radiopharmaceutical = value;
                RaisePropertyChanged("SelectedRadiopharmaceutical");
                fillPotentialTasks();
                RaisePropertyChanged("PotentialTasks");
            }
        }

        public override Chemical Radiopharmaceutical
        {
            get { return SelectedRadiopharmaceutical; }
            set { }
        }

        public bool IsAssigned
        {
            get
            {
                if (AssignedTask != null)
                {
                    return true;
                }
                return false;
                    
            }
        }
        #endregion

        #region privateMethods
        private void fillPotentialTasks()
        {
            if(Item != null)
            {
                //Need to find all DoseAdministrationTasks where
                //completed == false
                //RadiopharmaceuticalID = item.RadiopharmaceuticalID
                //scheduledCompletionDate == Today
                if((AssignedTask == null) && ((Item as BaseUnitDose).Radiopharmaceutical != null))
                {
                    RetrievalCriteria rc1 = new RetrievalCriteria("ScheduledCompletionTime", CriteraType.GreaterThanOrEqual, DateTime.Today);
                    RetrievalCriteria rc2 = new RetrievalCriteria("Completed", CriteraType.Equals, false);
                    RetrievalCriteria rc3 = new RetrievalCriteria("ScheduledCompletionTime", CriteraType.LessThan, DateTime.Today.AddDays(1));
                    RetrievalCriteria rc4 = new RetrievalCriteria("IsCancelled", CriteraType.Equals, false);
                    List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
                    rcList.Add(rc1);
                    rcList.Add(rc2);
                    rcList.Add(rc3);
                    rcList.Add(rc4);
                    _potentialTasks = DesktopApplication.Librarian.GetItems(typeof(DoseAdministrationTask), rcList).ToList();
                    _potentialTasks = _potentialTasks.Where(y => (y as DoseAdministrationTask).PrescribedRadioPharmaceutical != null && (y as DoseAdministrationTask).UnitDose == null).Where(x => (x as DoseAdministrationTask).PrescribedRadioPharmaceutical.ID == (Item as BaseUnitDose).Radiopharmaceutical.ID).ToList();
                }
                else
                {
                    _potentialTasks = new List<IDataStoreItem>();
                    _potentialTasks.Add(AssignedTask);
                }
                
            }
            else
            {
                _potentialTasks = DesktopApplication.Librarian.GetItems(typeof(DoseAdministrationTask), new List<RetrievalCriteria>()).ToList();
            }

            _potentialRadiopharmaceuticals = DesktopApplication.Librarian.GetItems(typeof(Chemical), new List<RetrievalCriteria>()).OrderBy(x => (x as Chemical).Name).ToList();
        }

        private void unAssignTask()
        {
            AssignedTask = null;
        }
        #endregion

        #region commands
        public RelayCommand UnassignCommand { get; set; }
        #endregion
    }

    public class SyringeUnitDoseViewModel : BaseUnitDoseViewModel
    {
        public SyringeUnitDoseViewModel() : base()
        {
            //SetItem(new SyringeUnitDose());
        }

        public SyringeUnitDoseViewModel(DataStoreItem item) : base(item)
        {

        }
    }

    [PreferredView("iRadiate.Desktop.Common.View.SplitUnitDoseView", "iRadiate.Desktop.Common")]
    public class SplitUnitDoseViewModel : SyringeUnitDoseViewModel
    {
        private AsyncObservableCollection<DoseSplit> _doseSplits;

        public SplitUnitDoseViewModel(DataStoreItem d) : base(d)
        {
            if((Item as SplitUnitDose).SubDoseFractions != null)
            {
                string s = (Item as SplitUnitDose).SubDoseFractions;
                var a = s.Split('\\');
                if (a.Length < 2)
                {
                    NumberOfSubDoses = 1;
                    DoseSplit t = new DoseSplit(this.Item as SyringeUnitDose);
                    t.Fraction = 1;
                   
                    DoseSplits.Clear();
                    DoseSplits.Add(t);
                }
                else
                {
                    for(int i = 0; i < a.Length; i++)
                    {
                        DoseSplit t = new DoseSplit(this.Item as SyringeUnitDose);
                        t.Fraction = Convert.ToDouble(a[i]);
                        DoseSplits.Add(t);
                    }
                }
            }
            else
            {
                NumberOfSubDoses = 1;
                DoseSplit t = new DoseSplit(this.Item as SyringeUnitDose);
                t.Fraction = 1;

                DoseSplits.Clear();
                DoseSplits.Add(t);
            }
            
                
            
        }

        public int? NumberOfSubDoses
        {
            get
            {
                return (Item as SplitUnitDose).NumberOfSubDoses;
            }
            set
            {
                (Item as SplitUnitDose).NumberOfSubDoses = value;
                if(NumberOfSubDoses > DoseSplits.Count)
                {
                    for(int i = DoseSplits.Count; i < NumberOfSubDoses; i++)
                    {
                        var d = new DoseSplit(this.Item as SyringeUnitDose);
                        DoseSplits.Add(d);
                    }
                    foreach(var s in DoseSplits)
                    {
                        s.Fraction = 1.0 / (double)NumberOfSubDoses.Value;
                    }
                }
                else if(NumberOfSubDoses < DoseSplits.Count)
                {
                    DoseSplits = new AsyncObservableCollection<DoseSplit>();
                    for (int i = 0; i < NumberOfSubDoses; i++)
                    {
                        var d = new DoseSplit(this.Item as SyringeUnitDose);
                        d.Fraction = 1.0 / (double)NumberOfSubDoses.Value;
                        DoseSplits.Add(d);
                    }
                    
                }
                
                RaisePropertyChanged("NumberOfSubDoses");
                RaisePropertyChanged("DoseSplits");
            }
        }

        public class DoseSplit: ViewModelBase
        {
            private double _fraction;
            private SyringeUnitDose _parent;

            public DoseSplit(SyringeUnitDose parent)
            {
                _parent = parent;
            }
            public double Fraction {
                get
                {
                    return _fraction;
                }
                set
                {
                    _fraction = value;
                    RaisePropertyChanged("Fraction");
                    RaisePropertyChanged("Activity");
                }
            }

            public double Activity
            {
                get
                {
                    return _fraction * _parent.CalibrationActivity;
                }
            }
        }
        public AsyncObservableCollection<DoseSplit> DoseSplits
        {
            get
            {
                if (_doseSplits == null)
                    _doseSplits = new AsyncObservableCollection<DoseSplit>();

                return _doseSplits;
            }
            set
            {
                _doseSplits = value;
                RaisePropertyChanged("DoseSplits");
            }
        }

        public override void SaveItem()
        {
            string splits = "";
            if(NumberOfSubDoses > 1)
            {
                foreach(var d in DoseSplits)
                {
                    splits = splits + d.Fraction.ToString() + "\\";
                }
                splits = splits.Remove(splits.Length - 1);
            }
            (this.Item as SplitUnitDose).SubDoseFractions = splits;
            if(Item.ID == 0)
            {
                if((Item as SplitUnitDose).BulkDose != null)
                {
                    var parent = (Item as SplitUnitDose).BulkDose;
                    var parentConcentration = parent.CurrentActivity / parent.Volume;
                    (Item as SplitUnitDose).Volume = (Item as SplitUnitDose).CurrentActivity / parentConcentration;
                    parent.Volume = parent.Volume - (Item as SplitUnitDose).Volume;
                }
            }
            base.SaveItem();

            var vm = new SplitUnitDoseViewModel(Item as DataStoreItem);
            vm.PreCalTime = PreCalTime;
            Close();
            DesktopApplication.MakeModalDocument(vm, "iRadiate.Desktop.Common", "iRadiate.Desktop.Common.View.SplitUnitDosePrintView");
        }

        public string ActivityString
        {
            get
            {
                string s = NumberOfSubDoses + " x " + PreCalActivity/((float)NumberOfSubDoses) + " MBq";
                
                return s;
            }
        }

    }
    public class CapsuleUnitDoseViewModel : BaseUnitDoseViewModel
    {

        public CapsuleUnitDoseViewModel() : base()
        {

        }

        public CapsuleUnitDoseViewModel(DataStoreItem item) : base(item)
        {

        }
    }
}
