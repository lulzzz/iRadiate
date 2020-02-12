using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace iRadiate.Interfaces.DICOM
{
    [Serializable]
    public class StructuredReport 
    {
        public StructuredReport()
        {

        }
        public StructuredReport(Dicom.DicomDataset dataset)
        {
            string val;
            if(dataset.TryGetString(Dicom.DicomTag.Manufacturer,out val))
            {
                Manufacturer = val;
            }
            if (dataset.TryGetString(Dicom.DicomTag.AccessionNumber, out val))
            {
                AccessionNumber = val;
            }
            if (dataset.TryGetString(Dicom.DicomTag.InstitutionName, out val))
            {
                InstitutionName = val;
            }
            if (dataset.TryGetString(Dicom.DicomTag.StationName, out val))
            {
                StationName = val;
            }
            if (dataset.TryGetString(Dicom.DicomTag.StudyDescription, out val))
            {
                StudyDescription = val;
            }
            if (dataset.TryGetString(Dicom.DicomTag.SeriesDescription, out val))
            {
                SeriesDescription = val;
            }
            if (dataset.TryGetString(Dicom.DicomTag.ManufacturerModelName, out val))
            {
                ManufacturerModelName = val;
            }
            if (dataset.TryGetString(Dicom.DicomTag.PatientName, out val))
            {
                PatientName = val;
            }
            if (dataset.TryGetString(Dicom.DicomTag.PatientID, out val))
            {
                PatientID = val;
            }
            if (dataset.TryGetString(Dicom.DicomTag.StudyInstanceUID, out val))
            {
                StudyInstanceUID = val;
            }
            if (dataset.TryGetString(Dicom.DicomTag.SeriesInstanceUID, out val))
            {
                SeriesInstanceUID = val;
            }
            //Dates
            string val2;
            if(dataset.TryGetString(Dicom.DicomTag.StudyDate,out val) && dataset.TryGetString(Dicom.DicomTag.StudyTime,out val2))
            {
                DateTime tmp = new DateTime(Convert.ToInt16(val.Substring(0, 4)), Convert.ToInt16(val.Substring(4, 2)), Convert.ToInt16(val.Substring(6, 2)),Convert.ToInt16(val2.Substring(0,2)),Convert.ToInt16(val2.Substring(2,2)),Convert.ToInt16(val2.Substring(4,2)));
                StudyDateTime = tmp;
            }
            if (dataset.TryGetString(Dicom.DicomTag.ContentDate, out val) && dataset.TryGetString(Dicom.DicomTag.ContentTime, out val2))
            {
                DateTime tmp = new DateTime(Convert.ToInt16(val.Substring(0, 4)), Convert.ToInt16(val.Substring(4, 2)), Convert.ToInt16(val.Substring(6, 2)), Convert.ToInt16(val2.Substring(0, 2)), Convert.ToInt16(val2.Substring(2, 2)), Convert.ToInt16(val2.Substring(4, 2)));
                ContentDateTime = tmp;
            }
            if (dataset.TryGetString(Dicom.DicomTag.ObservationDateTime, out val))
            {
                DateTime tmp = new DateTime(Convert.ToInt16(val.Substring(0, 4)), Convert.ToInt16(val.Substring(4, 2)), Convert.ToInt16(val.Substring(6, 2)), Convert.ToInt16(val.Substring(8, 2)), Convert.ToInt16(val.Substring(10, 2)), Convert.ToInt16(val.Substring(12, 2)));
                ObservationDateTime = tmp;
            }
            if (dataset.TryGetString(Dicom.DicomTag.PatientBirthDate, out val) )
            {
                DateTime tmp = new DateTime(Convert.ToInt16(val.Substring(0, 4)), Convert.ToInt16(val.Substring(4, 2)), Convert.ToInt16(val.Substring(6, 2)));
                PatientBirthDate = tmp;
            }
            //enums
            if (dataset.TryGetString(Dicom.DicomTag.PatientSex, out val))
            {
                if(val == "M")
                {
                    Gender = DataModel.Common.Gender.Male;
                }
                else if(val == "F")
                {
                    Gender = DataModel.Common.Gender.Female;
                }
                else
                {
                    Gender = DataModel.Common.Gender.Other;
                }
            }
            if (dataset.TryGetString(Dicom.DicomTag.ValueType, out val))
            {
                Value typeVal;
                if(Enum.TryParse(val,out typeVal))
                {
                    ValueType = typeVal;
                }
            }
            if (dataset.TryGetString(Dicom.DicomTag.ContinuityOfContent, out val))
            {
                Continuity typeVal;
                if (Enum.TryParse(val, out typeVal))
                {
                    ContinuityOfContent = typeVal;
                }
            }
            if (dataset.TryGetString(Dicom.DicomTag.CompletionFlag, out val))
            {
                Completion typeVal;
                if (Enum.TryParse(val, out typeVal))
                {
                    Completion = typeVal;
                }
            }
            //Concept Name Code Sequence
            Dicom.DicomSequence seq;
            if(dataset.TryGetSequence(Dicom.DicomTag.ConceptNameCodeSequence,out seq))
            {
                Dicom.DicomDataset d = seq.Items.First();
                ConceptNameCodeSequence = new CodeSequence(d);

            }

            //Content Sequence
            if (dataset.TryGetSequence(Dicom.DicomTag.ContentSequence, out seq))
            {
                ContentSequence = new List<ContentItem>();
                foreach (var i in seq.Items)
                {
                    ContentSequence.Add(new ContentItem(i));
                }

            }
        }
        public DateTime StudyDateTime { get; set; }
        public DateTime ContentDateTime { get; set; }
        public string Manufacturer { get; set; }
        public string AccessionNumber { get; set; }
        public string InstitutionName { get; set; }
        public string StationName { get; set; }
        public string StudyDescription { get; set; }
        public string SeriesDescription { get; set; }
        public string ManufacturerModelName { get; set; }
        public string PatientName { get; set; }
        public string PatientID { get; set; }
        public DateTime PatientBirthDate { get; set; }
        public iRadiate.DataModel.Common.Gender Gender {get;set;}
        public string StudyInstanceUID { get; set; }
        public string SeriesInstanceUID { get; set; }
        public DateTime ObservationDateTime { get; set; }
        public Value ValueType { get; set; }
        public CodeSequence ConceptNameCodeSequence { get; set; }
        public Continuity ContinuityOfContent { get; set; }
        public Completion Completion { get; set; }

        public List<ContentItem> ContentSequence { get;set;}
        public void Debug()
        {
            Console.WriteLine("============Debug Study Report=============");
            foreach(var p in typeof(StructuredReport).GetProperties())
            {
                Console.WriteLine(p.Name +": " + p.GetValue(this));
            }
            foreach(var c in ContentSequence)
            {
                Console.WriteLine("=== Content Item: " + c.ToString());
            }
        }
    }

    public enum Value { TEXT,NUM,CODE,DATE,TIME,DATETIME,UIDREF,PNAME,COMPOSITE,IMAGE,WAVEFORM,SCOORD,SCOORD3D,TCOORD,CONTAINER }

    public enum RelationShip { CONTAINS, HAS_OBS_CONTEXT,HAS_CONCEPT_MOD,HAS_PROPERTIES,HAS_ACQ_CONTEXT,INFERRED_FROM,SELECTED_FROM}

    public enum Continuity { SEPARATE, CONTINUOUS}

    public enum Completion { COMPLETE, PARTIAL}

    [Serializable]
    public class CodeSequence
    {
        public CodeSequence()
        {

        }
        public CodeSequence(Dicom.DicomDataset dset)
        {
            string val;
            if(dset.TryGetString(Dicom.DicomTag.CodeValue,out val))
            {
                CodeValue = val;
            }
            if (dset.TryGetString(Dicom.DicomTag.CodingSchemeDesignator, out val))
            {
                CodingSchemeDesignator = val;
            }
            if (dset.TryGetString(Dicom.DicomTag.CodeMeaning, out val))
            {
                CodeMeaning = val;
            }
        }
        public string CodeValue { get; set; }
        public string CodingSchemeDesignator { get; set; }
        public string CodeMeaning { get; set; }

        public override string ToString()
        {
            return Environment.NewLine + "Code: " + CodeValue + Environment.NewLine + "CodingSchemeDesignator: " +CodingSchemeDesignator + Environment.NewLine + "CodeMeaning: " + CodeMeaning;
        }
    }

    [Serializable]
    public class MeasuredValueSequence
    {
        public MeasuredValueSequence()
        {

        }
        public override string ToString()
        {
            string ans;
            ans = "Numerical value: " + NumericValue.ToString() + Environment.NewLine +
                "Floating point value: " + FloatingPointValue.ToString() + Environment.NewLine +
                "Rational value " + RationalNumeratorValue.ToString() + "/" + RationalDenominatorValue;
            if(MeasurmentUnitCodeSequence!= null)
            { ans = ans + Environment.NewLine + " Measurement units sequence --> " + Environment.NewLine + MeasurmentUnitCodeSequence.ToString(); }
            return ans;
        }
        public MeasuredValueSequence(Dicom.DicomDataset dset)
        {
            decimal d;
            if (dset.TryGetValue<decimal>(Dicom.DicomTag.NumericValue,0,out d))
            {
                NumericValue = d;
            }

            float f;
            if (dset.TryGetValue<float>(Dicom.DicomTag.FloatingPointValue, 0, out f))
            {
                FloatingPointValue = f;
            }
            long numerator;
            if (dset.TryGetValue<long>(Dicom.DicomTag.RationalNumeratorValue, 0, out numerator))
            {
                RationalNumeratorValue = numerator;
            }
            long denominator;
            if (dset.TryGetValue<long>(Dicom.DicomTag.RationalDenominatorValue, 0, out denominator))
            {
                RationalDenominatorValue = denominator;
            }

            Dicom.DicomSequence dseq;
            if(dset.TryGetSequence(Dicom.DicomTag.MeasurementUnitsCodeSequence,out dseq))
            {
                MeasurmentUnitCodeSequence = new CodeSequence(dseq.First());
            }
        }
        public decimal NumericValue { get; set; }

        public float FloatingPointValue { get;set;}

        public long RationalNumeratorValue { get; set; }

        public long RationalDenominatorValue { get; set; }

        public CodeSequence MeasurmentUnitCodeSequence { get; set; }
    }

    [Serializable]
    public class ContentItem
    {
        public override string ToString()
        {
            string ans = Environment.NewLine +
                "Relationship Type: " + RelationShipType + Environment.NewLine + 
                "Value Type: " + ValueType + Environment.NewLine +
                "Concept --> " + ConceptNameCodeSequence.ToString();
            if (ConceptCodeSequence != null)
                ans = ans + ConceptCodeSequence.ToString();
            if(ValueType == Value.TEXT)
            {
                ans = ans + Environment.NewLine + "Value: " + TextValue;
            }
            if (DateTimeValue.HasValue)
                ans = ans + Environment.NewLine + "Value: " + DateTimeValue.Value.ToString();
            if(ValueType == Value.NUM)
            {
                ans = ans + Environment.NewLine + MeasuredValueSequence.ToString();
            }
            if (ContentItems.Any())
            {
                foreach(var c in ContentItems)
                {
                    ans = ans + Environment.NewLine + "Content Item-->"  +c.ToString();
                }
            }
            return ans;
        }

        public ContentItem()
        {

        }
        public ContentItem(Dicom.DicomDataset dset)
        {
            ContentItems = new List<ContentItem>();
            string val;
            if (dset.TryGetString(Dicom.DicomTag.RelationshipType, out val))
            {
                switch (val)
                {
                    case "CONTAINS":
                        RelationShipType = RelationShip.CONTAINS;
                        break;
                    case "HAS PROPERTIES":
                        RelationShipType = RelationShip.HAS_PROPERTIES;
                        break;
                    case "HAS OBS CONTEXT":
                        RelationShipType = RelationShip.HAS_OBS_CONTEXT;
                        break;
                    case "HAS ACQ CONTEXT":
                        RelationShipType = RelationShip.HAS_ACQ_CONTEXT;
                        break;
                    case "INFERRED FROM":
                        RelationShipType = RelationShip.INFERRED_FROM;
                        break;
                    case "SELECTED FROM":
                        RelationShipType = RelationShip.SELECTED_FROM;
                        break;
                    case "HAS CONCEPT MOD":
                        RelationShipType = RelationShip.HAS_CONCEPT_MOD;
                        break;
                }
            }

            if (dset.TryGetString(Dicom.DicomTag.ValueType, out val))
            {

                switch (val)
                {
                    case "CODE":
                        ValueType = Value.CODE;
                        Dicom.DicomSequence seq1;
                        if(dset.TryGetSequence(Dicom.DicomTag.ConceptCodeSequence,out seq1))
                        {
                            ConceptCodeSequence = new CodeSequence(seq1.First());
                        }
                        
                        break;
                    case "COMPOSITE":
                        ValueType = Value.COMPOSITE;
                        break;
                    case "CONTAINER":
                        ValueType = Value.CONTAINER;
                        break;
                    case "DATE":
                        ValueType = Value.DATE;
                        break;
                    case "DATETIME":
                        ValueType = Value.DATETIME;
                        DateTime dVal;
                        if(dset.TryGetValue<DateTime>(Dicom.DicomTag.DateTime,0,out dVal))
                        {
                            DateTimeValue = dVal;
                        }
                        break;
                    case "IMAGE":
                        ValueType = Value.IMAGE;
                        break;
                    case "NUM":
                        ValueType = Value.NUM;
                        Dicom.DicomSequence dsZ;
                        if(dset.TryGetSequence(Dicom.DicomTag.MeasuredValueSequence,out dsZ))
                        {
                            MeasuredValueSequence = new MeasuredValueSequence(dsZ.First());
                        }
                        break;
                    case "PNAME":
                        ValueType = Value.PNAME;
                        break;
                    case "SCORD":
                        ValueType = Value.SCOORD;
                        break;
                    case "SCOORD3D":
                        ValueType = Value.SCOORD3D;
                        break;
                    case "TCOORD":
                        ValueType = Value.TCOORD;
                        break;
                    case "TEXT":
                        ValueType = Value.TEXT;
                        if (dset.TryGetString(Dicom.DicomTag.TextValue, out val))
                        {
                            TextValue = val;
                        }
                        break;
                    case "TIME":
                        ValueType = Value.TIME;
                        break;
                    case "UIDREF":
                        ValueType = Value.UIDREF;
                        if (dset.TryGetString(Dicom.DicomTag.UID, out val))
                        {
                            UIDValue = val;
                        }
                        break;
                    case "WAVEFORM":
                        ValueType = Value.WAVEFORM;
                        break;
                }

            }

            Dicom.DicomSequence seq;
            if (dset.TryGetSequence(Dicom.DicomTag.ConceptNameCodeSequence, out seq))
            {
                ConceptNameCodeSequence = new CodeSequence(seq.First());
            }

            if (dset.TryGetSequence(Dicom.DicomTag.ContentSequence, out seq))
            {
                ContentItems = new List<ContentItem>();
                foreach(var t in seq.Items)
                {
                    ContentItems.Add(new ContentItem(t));
                }
            }

        }
        public RelationShip RelationShipType { get; set; }
        public Value ValueType { get; set; }
        public CodeSequence ConceptNameCodeSequence { get; set; }

        /// <summary>
        /// The value of the content item if value type is TEXT
        /// </summary>
        public string TextValue { get; set; }
        
        /// <summary>
        /// The value of the content item if value type is DATE or DATETIME 
        /// </summary>
        public DateTime? DateTimeValue { get; set; }

        /// <summary>
        /// The value of the content item if value type is TIME 
        /// </summary>
        public TimeSpan? TimeValue { get; set; }

        /// <summary>
        /// The value of the content item if value type is PNAME 
        /// </summary>
        public string PersonNameValue { get; set; }

        /// <summary>
        /// The value of the content item if value type is UIDREF 
        /// </summary>
        public string UIDValue { get; set; }

        public double NumberValue { get; set; }

        public CodeSequence ConceptCodeSequence { get; set; }

        public MeasuredValueSequence MeasuredValueSequence { get; set; }

        public List<ContentItem> ContentItems { get; set; }
    }
}
