using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using MahApps.Metro.IconPacks;
using GalaSoft.MvvmLight.Command;

using iRadiate.DataModel;
using iRadiate.DataModel.DataDictionary;
using iRadiate.Common;
using iRadiate.Desktop.Common;

namespace iRadiate.Desktop.Common.DataDictionary
{
    [PreferredView("iRadiate.Desktop.Common.DataDictionary.DataDictionaryView", "iRadiate.Desktop.Common")]
    public class DataDictionaryModule : Module
    {
        #region privateFields
        private AsyncObservableCollection<DataDictionaryNamespace> _dataDictionary;
        private DataDictionaryEntry _selectedEntry;
        private string _newRootNameSpace;
        private string _newNamespace;
        private DataDictionaryNamespace _selectedNamespace;
        #endregion

        #region constructor
        public DataDictionaryModule() : base()
        {

        }
        #endregion

        #region overrides
        public override void GetData()
        {
            #region DataDictionary

            var res = Platform.Retriever.RetrieveItems(typeof(DataDictionaryNamespace));
            foreach (var i in res)
            {
                DataDictionary.Add(i as DataDictionaryNamespace); 
                foreach(var e in (i as DataDictionaryNamespace).Entries)
                {
                    e.Initialize();
                }
            }

         
            #endregion
        }

        protected override void SetRelayCommands()
        {
            base.SetRelayCommands();
            AddNewRootNamespaceCommand = new RelayCommand(addNewRootNamespace);
            InsertNamespaceCommand = new RelayCommand(insertNamespace);
            InsertBooleanEntryCommand = new RelayCommand(insertBoolean);
            InsertTextEntryCommand = new RelayCommand(insertText);
            InsertQuantifiableEntryCommand = new RelayCommand(insertQuantifiable);
            InsertMeasurableEntryCommand = new RelayCommand(insertMeasurable);
            SaveDictionaryCommand = new RelayCommand(SaveDictionary);
        }

        public override ContentControl IconContent
        {
            get
            {
                ContentControl cc = new ContentControl();
                PackIconMaterial icon = new PackIconMaterial();
                icon.Kind = PackIconMaterialKind.Dictionary;
                icon.Height = 24;
                icon.Width = 24;
                cc.Content = icon;
                return cc;
            }
        }

        public override string Name
        {
            get
            {
                return "Data Dictionary";
            }

            set
            {
                base.Name = value;
            }
        }
        #endregion

        #region privateMethods
        private void addNewRootNamespace()
        {
            if (NewRootNamespace == string.Empty || NewRootNamespace == null)
                return;
            DataDictionaryNamespace newSpace = new DataDictionaryNamespace();
            newSpace.Name = NewRootNamespace;
            DataDictionary.Add(newSpace);
            NewRootNamespace = string.Empty;
        }

        private void insertNamespace()
        {
            if (SelectedNamespace == null)
                return;
            if (NewNamespace == null || NewNamespace == string.Empty)
                return;
            DataDictionaryNamespace ns = new DataDictionaryNamespace { Name = NewNamespace };
            SelectedNamespace.Namespaces.Add(ns);
            ns.ParentNamespace = SelectedNamespace;
            DataDictionary.Add(ns);
            NewNamespace = string.Empty;
        }

        private void insertBoolean()
        {
            if (SelectedNamespace == null)
                return;

            BooleanDataDictionaryEntry b = new BooleanDataDictionaryEntry();
            SelectedNamespace.Entries.Add(b);
            b.Namespace = SelectedNamespace;
            SelectedEntry = b;
        }

        private void insertText()
        {
            if (SelectedNamespace == null)
                return;

            TextDataDictionaryEntry b = new TextDataDictionaryEntry();
            SelectedNamespace.Entries.Add(b);
            b.Namespace = SelectedNamespace;
            SelectedEntry = b;
        }

        private void insertQuantifiable()
        {
            if (SelectedNamespace == null)
                return;

            QuantifableDataDictionaryEntry b = new QuantifableDataDictionaryEntry();
            SelectedNamespace.Entries.Add(b);
            b.Namespace = SelectedNamespace;
            SelectedEntry = b;
        }

        private void insertMeasurable()
        {
            if (SelectedNamespace == null)
                return;

            MeasureableDataDictionaryEntry b = new MeasureableDataDictionaryEntry();
            SelectedNamespace.Entries.Add(b);
            b.Namespace = SelectedNamespace;
            SelectedEntry = b;
        }

        private void SaveDictionary()
        {
            foreach(var d in DataDictionary)
            {
                Platform.Retriever.SaveItem(d);
            }
            DesktopApplication.ShowToastInformation("Dictionary Saved", DesktopApplication.NotificationPosition.BottomLeft);
        }
        #endregion

        #region publicProperties
        public virtual AsyncObservableCollection<DataDictionaryNamespace> DataDictionary
        {
            get
            {
                if (_dataDictionary == null)
                    _dataDictionary = new AsyncObservableCollection<DataDictionaryNamespace>();
                return _dataDictionary;
            }
            set
            {
                _dataDictionary = value;
            }
        }

        public virtual DataDictionaryEntry SelectedEntry
        {
            get { return _selectedEntry; }
            set
            {
                _selectedEntry = value;
                RaisePropertyChanged("SelectedEntry");
            }
        }

        public string NewRootNamespace
        {
            get { return _newRootNameSpace; }
            set { _newRootNameSpace = value; RaisePropertyChanged("NewRootNamespace"); }
        }

        public string NewNamespace
        {
            get { return _newNamespace; }
            set { _newNamespace = value; RaisePropertyChanged("NewNameSpace"); }
        }

        public DataDictionaryNamespace SelectedNamespace
        {
            get { return _selectedNamespace; }
            set { _selectedNamespace = value; RaisePropertyChanged("SelectedNamespace"); }
        }
        #endregion

        #region commands
        public RelayCommand AddNewRootNamespaceCommand { get; set; }

        public RelayCommand InsertNamespaceCommand { get; set; }

        public RelayCommand InsertBooleanEntryCommand { get; set; }

        public RelayCommand InsertTextEntryCommand { get; set; }

        public RelayCommand InsertQuantifiableEntryCommand { get; set; }

        public RelayCommand InsertMeasurableEntryCommand { get; set; }

        public RelayCommand SaveDictionaryCommand { get; set; }
        #endregion
    }

    
}
