using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.Desktop.Common;
using iRadiate.Desktop.Common.ViewModel;

using iRadiate.DataModel;
namespace iRadiate.Scanbag.Common.ViewModel
{
    [PreferredView("iRadiate.Scanbag.Common.View.SummaryScanBagItemView", "iRadiate.Scanbag.Common")]
    public class ScanBagSection : ViewModelBase
    {
        private List<ScanBagItem> _scanBagItems;
        private string _description;
        private DateTime _date;
        private Study _study;
        private string _uploadDescription;
        private string _uploadFileName;
        private bool _showUpload;
        private ScanBagViewModel _scanBag;
        private SummaryScanBagItem _summary;

        public ScanBagSection(Study s)
        {
            _study = s;
            //Well here we should instantiate all the scan bag items
            //starting with the summary and ending with the report.
            Summary = new SummaryScanBagItem(_study);
            //ScanBagItems.Add(sum);
            if (_study.Files.Any())
            {
                foreach (File f in _study.Files)
                {
                    if (f.Extension.ToLower() == "xps")
                    {
                        XPSScanbagItem i = new XPSScanbagItem(f);
                        i.ScanbagSection = this;
                        ScanBagItems.Add(i);
                    }
                    else if (f.Extension.ToLower() == "rtf")
                    {
                        RTFScanbagItem i = new RTFScanbagItem(f);
                        i.ScanbagSection = this;
                        ScanBagItems.Add(i);
                    }
                    else if (f.Extension.ToLower() == "jpg")
                    {
                        ScreencapScanBagItem i = new ScreencapScanBagItem(f);
                        i.ScanbagSection = this;
                        ScanBagItems.Add(i);
                    }
                    else
                    {
                        FileScanBagItem i = new FileScanBagItem(f);
                        i.ScanbagSection = this;
                        ScanBagItems.Add(i);
                    }
                    
                }
            }
            /*if (_study.Notes.Any())
            {
                foreach (Note nt in _study.Notes)
                {
                    NoteScanbagItem n = new NoteScanbagItem(nt);
                    n.ScanbagSection = this;
                    ScanBagItems.Add(n);
                }

            }*/
            if (_study.Report != null)
            {
                ReportScanBagItem rep = new ReportScanBagItem(_study.Report);
                rep.ScanbagSection = this;
                ScanBagItems.Add(rep);
            }

            
            

        }
        
        public ScanBagViewModel ScanBag
        {
            get
            {
                return _scanBag;
            }
            set
            {
                _scanBag = value;
                RaisePropertyChanged("ScanBag");
            }
        }
        
        public SummaryScanBagItem Summary
        {
            get
            {
                return _summary;
            }
            set
            {
                _summary = value;
                RaisePropertyChanged("Summary");
            }
        }
        
        public List<ScanBagItem> ScanBagItems
        {
            get
            {
                if (_scanBagItems == null)
                {
                    _scanBagItems = new List<ScanBagItem>();
                }
                return _scanBagItems;
            }
            set
            {
                _scanBagItems = value;
                RaisePropertyChanged("ScanBagItems");
            }
        }
        
        public string Description
        {
            get
            {
                return _study.Name;
            }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }
        
        public DateTime Date
        {
            get
            {
                return _study.Date;
            }
            
        }

        public Study Study
        {
            get
            {
                return _study;
            }
        }

        public Patient Patient
        {
            get
            {
                return _study.Patient;
            }
        }

        public IEnumerable<DataStoreItem> ScanTasks
        {
            get
            {
                return _study.Appointments.SelectMany(x => x.Tasks).Where(y => y is ScanTask && y.Deleted == false).OrderBy(x => ((ScanTask)x).ValidCommencementTime);
            }
        }
        
        public IEnumerable<DataStoreItem> InjectionTasks
        {
            get
            {
                return _study.Appointments.SelectMany(x => x.Tasks).Where(y => y is DoseAdministrationTask).OrderBy(x => x.ValidCompletionTime);
            }
        }
        
        public bool ShowUpload
        {
            get
            {
                return _showUpload;
            }
            set
            {
                _showUpload = value;
                RaisePropertyChanged("ShowUpload");
            }
        }

        public void UploadFile(string fileName, string description)
        {
           

            File f = new File();
            f.Deleted = false;
            f.Data = System.IO.File.ReadAllBytes(fileName);
            f.Description = description;
            f.Extension = fileName.Split('.').Last();
            f.Study = _study;
           
            iRadiate.Common.Platform.Retriever.SaveItem(f);
            _study.Files.Add(f);

        }
       
    }
}
