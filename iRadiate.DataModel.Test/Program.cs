using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Threading;
using iRadiate.Common;
using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.DataDictionary;
using iRadiate.DataModel.Radiopharmacy;
using NLog;
using UnitsNet;
using Newtonsoft.Json;


namespace iRadiate.DataModel.Test
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static EFDataRetriever retriever = new EFDataRetriever();
        
        static void Main(string[] args)
        {
            logger.Info("                  ");
            logger.Info("******************");
            logger.Info("Start Test Application");
            //IQuantity mylength = Irradiance.FromKilowattsPerSquareCentimeter(5);
            //logger.Info(mylength);


            //var splits = mylength.ToString().Split(' ');
            //double value = Convert.ToDouble(splits[0]);
            //string units = splits[1];
            //logger.Info("SLIT: value = " + value + ", units = " + units);
            string input = "0.3 mV";
            var splits = input.Split(' ');
            QuantityType qType = QuantityType.ElectricPotential;            
            QuantityInfo qInfo = UnitsNet.Quantity.GetInfo(qType);
            logger.Info("Input string: " + input);
            logger.Info("qInfo: " + qInfo.ToString());
            var x = Quantity.TryParse(qInfo.ValueType, input, out IQuantity iq2);
            logger.Info("TryParse() result: " + x);
            if (x)
            {
                
                logger.Info(iq2);
                foreach (var u in qInfo.UnitInfos)
                {
                    var converted = UnitConverter.Convert(iq2.Value, iq2.Unit, u.Value);
                    string abbrev = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(qInfo.UnitType, Convert.ToInt32(u.Value));
                    logger.Info(converted + " " + abbrev);
                }
            }
            
            //AddFirstUser();

            //List<DataDictionaryNameSpace> Dictionary = new List<DataDictionaryNameSpace>();

            //DataDictionaryNameSpace Clinical = new DataDictionaryNameSpace();
            //Clinical.Name = "Clinical";
            //DataDictionaryNameSpace Patient = new DataDictionaryNameSpace();
            //Patient.Name = "Patient";
            //Patient.ParentNameSpace = Clinical;
            //Clinical.NameSpaces.Add(Patient);

            //TextDataDictionaryEntry surname = new TextDataDictionaryEntry();
            //surname.Name = "Surname";
            //surname.NameSpace = Patient;
            //Patient.Entries.Add(surname);

            //MeasureableDataDictionaryEntry height = new MeasureableDataDictionaryEntry();
            //height.Name = "Height";
            //height.NameSpace = Patient;
            //height.QuantityType = QuantityType.Length;
            //Patient.Entries.Add(height);

            //logger.Debug("====== Data Dictionary ======");
            //DumpNameSpace(Clinical);

            //Dictionary.Add(Clinical);



            logger.Info("Finished Test Application");            
            logger.Info("******************");
            logger.Info("                  ");
            Console.ReadLine();
        }

        #region dataDictionary
        public static void DumpNameSpace(DataDictionaryNamespace n)
        {
           
            logger.Debug("Namespace: " + n.FullName);
            if (n.Entries.Any())
            {
                logger.Debug("** Entries");
                foreach (DataDictionaryEntry e in n.Entries)
                {
                    logger.Debug(e.ToString());
                }
                logger.Debug("** Finished Entries");
                //logger.Debug();
            }
            else
            {
                logger.Debug("No entries in this namespace");
            }
            if (n.Namespaces.Any())
            {
                logger.Debug("** Sub namespaces");
                foreach (DataDictionaryNamespace ns in n.Namespaces)
                {
                    DumpNameSpace(ns);
                }
                logger.Debug("** Finished Sub namespaces");
            }
            else
            {
                logger.Debug("No sub namespaces");
            }
            //logger.Debug();
        }
        #endregion

        private static void AddFirstUser()
        {
            


                User u = new User();
                u.Surname = "Forwood";
                u.GivenNames = "Nicholas";
                u.LoginName = "nf";
                u.Password = "nf";
                u.PinNumber = "1234";
                u.Title = "Mr";
                retriever.SaveItem(u);



            
        }

        #region User
        private static User GetFirstUser()
        {
            return retriever.RetrieveItem(1, typeof(User)) as User;
                
        }
        #endregion

        #region Workstation
        private static Workstation AddThisWorkstation()
        {
            Workstation w = new Workstation();
            w.Name = System.Environment.MachineName;
            retriever.SaveItem(w);
            return w;
        }

        private static Workstation GetThisWorkstation()
        {
            string name = System.Environment.MachineName;
            RetrievalCriteria rc = new RetrievalCriteria("Name", CriteraType.Equals, name);
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc);
            var result = retriever.RetrieveItems(typeof(Workstation), rcList);
            if (result.Any())
            {
                return result.First() as Workstation;
            }
            else
            {
                return AddThisWorkstation();
            }

        }
        #endregion  

        #region Hospital
        private static void AddFirstHospital()
        {
            

                Hospital h = new Hospital();
                h.Name = "Concord Repatriation General Hospital";
                h.Abbreviation = "CRGH";
            retriever.SaveItem(h);
            Ward w1 = new Ward();
            w1.Name = "Ward 11";
            w1.Abbreviation = "W11";
            w1.Hospital = h;
            retriever.SaveItem(w1);
                
        }
        private static Hospital GetFirstHospital()
        {
            return retriever.RetrieveItems(typeof(Hospital), new List<RetrievalCriteria>()).First() as Hospital;
        }
        #endregion

        #region Practice
        private static void AddNucMedPractice()
        {
            NucMedPractice nmp = new NucMedPractice();
            nmp.Name = "Concord Nuclear Medicine";
            nmp.Hospital = GetFirstHospital();
            retriever.SaveItem(nmp);
            Room r1 = new Room { Name = "8 slice", NucMedPractice = nmp, CameraRoom = true };
            retriever.SaveItem(r1);
            Room r2 = new Room { Name = "16 slice", NucMedPractice = nmp, CameraRoom = true };
            retriever.SaveItem(r2);
            Room r3 = new Room { Name = "Forte", NucMedPractice = nmp, CameraRoom = true };
            retriever.SaveItem(r3);
            Room r4 = new Room { Name = "MPR", NucMedPractice = nmp, CameraRoom = true };
            retriever.SaveItem(r4);
            Room r5 = new Room { Name = "Consult 1", NucMedPractice = nmp, CameraRoom = false };
            retriever.SaveItem(r5);
            Room r6 = new Room { Name = "Consult 2", NucMedPractice = nmp, CameraRoom = false };
            retriever.SaveItem(r6);

        }
        private static NucMedPractice GetFirstNucMedPractice()
        {
            return retriever.RetrieveItems(typeof(NucMedPractice), new List<RetrievalCriteria>()).First() as NucMedPractice;
        }
        #endregion



        private static void AddFirstStudyType()
        {
            StudyType s = new StudyType();
            s.Name = "Bone Scan";
            s.ShortName = "Bone";
            retriever.SaveItem(s);
        }

        private static StudyType GetFirstStudyType()
        {
            return retriever.RetrieveItems(typeof(StudyType), new List<RetrievalCriteria>()).First() as StudyType;
        }

        private static void AddFirstPatient()
        {
            Patient p = new Patient();
            p.GivenNames = "John";
            p.Surname = "Smith";
            p.Title = "Mr";
            p.MobilePhone = "0415 555 555";
            p.Comments = "This patient is a great guy";
            p.DateOfBirth = new DateTime(1970, 5, 17);
            p.EmailAddress = "John.Smith@gmail.com";
            p.Gender = Gender.Male;
            p.HomePhone = "02 9555 5555";
            p.MRN = "XYZ789";
            p.ProvinceName = "NSW";
            p.StreetName = "Majors Bay Road";
            p.StreetNumber = "1/1A";
            p.TownName = "Concord";
            p.TransportType = PatientTransportType.Ambulatory;
            p.WorkPhone = "02 8664 6644";
            retriever.SaveItem(p);
        }

        private static Patient GetFirstPatient()
        {
            return retriever.RetrieveItems(typeof(Patient), new List<RetrievalCriteria>()).First() as Patient;
        }

        private static Study AddAStudy(Patient p, NucMedPractice nmp)
        {
            Study s = new Study();
            s.Practice = nmp;
            s.StudyType = GetFirstStudyType();
            s.Patient = p;
            retriever.SaveItem(s);
            Appointment a = new Appointment();
            a.ScheduledArrivalTime = new DateTime(2017, 10, 18, 9, 0, 0);
            a.Study = s;
            retriever.SaveItem(a);

            ArrivalTask t = new ArrivalTask();
            t.ScheduledCompletionTime = new DateTime(2017, 10, 18, 9, 15, 0);
            t.Appointment = a;
            retriever.SaveItem(t);

            DoseAdministrationTask dat = new DoseAdministrationTask();
            dat.ScheduledCompletionTime = new DateTime(2017, 10, 18, 9, 45, 0);
            dat.Appointment = a;
            retriever.SaveItem(dat);

            ScanTask st = new ScanTask();
            st.ScheduledCommencementTime = new DateTime(2017, 10, 18, 12, 45, 0);
            st.Appointment = a;
            retriever.SaveItem(st);

            return s;
        }

        
        private static void SummariseUser(User u)
        {
            Console.WriteLine("User: " + u.FullName);
            if (u.AlterationsByUser.Any())
            {
                foreach(DataStoreItemAlteration a in u.AlterationsByUser)
                {
                    Console.WriteLine("Alteratoin: " + a.DataStoreItemName + " " + a.PropertyName + " from " + a.OldValue + " to " + a.NewValue);
                }
            }
            else
            {
                Console.WriteLine("No alterations");
            }
        }
        private static void SummariseAppointment(Appointment a)
        {
            Console.WriteLine("Patient: " + a.Patient.FullName);
            Console.WriteLine("Appointment: " + a.Name + " - " + a.ScheduledArrivalTime.ToString());
            foreach(BasicTask bt in a.Tasks)
            {
                foreach (BaseConstraint bc in bt.Constraints)
                {
                    Console.WriteLine("constraint: " + bc.Name);
                }
                if (bt is BasicFiniteTask)
                {
                    BasicFiniteTask bft = bt as BasicFiniteTask;
                    Console.WriteLine("Task: " + bft.TaskName + "; start: " + bft.ScheduledCommencementTime + " finish: " + bft.ScheduledCompletionTime);

                }
                else
                {
                    Console.WriteLine("Task: " + bt.TaskName + ";  completed: " + bt.ScheduledCompletionTime);
                }
            }
        }
    }
}
