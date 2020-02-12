using System;
using iRadiate.DataModel.HealthCare;

namespace iRadiate.DataModel.NucMed
{
    public interface IPatientImage
    {
        Appointment Appointment { get; set; }        
        Modality Modality { get; set; }
        string OperatorsName { get; set; }
        Patient Patient { get; }
        PatientPosition? PatientPosition { get; set; }
        DateTime SeriesDateTime { get; set; }
        string SeriesDescription { get; set; }
        string SeriesInstanceUID { get; set; }
        int SeriesNumber { get; set; }
        Study Study { get; }
        string SOPClassUID { get; set; }
        ScanTask ScanTask { get; set; }
        DateTime ScanFinishedDateTime { get; }
        double ScanDuration { get; }
        string DeviceSerialNumber { get; set; }
        string ManufacturerModelName { get; set;}

    }

    
}