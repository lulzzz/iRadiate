using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

using System.Linq;
using System.Reflection;

using iRadiate.Common.IO;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.HealthCare;
using iRadiate.DataModel.NucMed;
using iRadiate.DataModel.Radiopharmacy;


using System.Text;

using NLog;

using Oracle.ManagedDataAccess.Client;

namespace iRadiate.Interfaces.CentricityInterface
{
    public class CentricityConnector
    {
        #region privateFields
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private OracleConnection oConn; // Connection to the oracle (centricity) database
        
        /// <summary>
        /// Connection the Centricity Interface database
        /// </summary>
        private SqlConnection sConn; // Connection to the sql (Centricity Interface) database
        private EFDataRetriever retriever; // This is the object that interacts with iRadiate
        #endregion

        public CentricityConnector()
        {
            retriever = new EFDataRetriever();
            SetCurrentUser();

            //we need to set the current user to someone called
            //Centricity Interface

            oConn = new OracleConnection();
           
            oConn.ConnectionString = Properties.Settings.Default.CentricityConnString;
           
                oConn.Open();
            
            sConn = new SqlConnection(Properties.Settings.Default.InterfaceConnString);
            
            sConn.Open();
            
            
        }

        #region helperMethods
        //helper methods take the oracle datareader and produce instances of classes in the CentricityModel

        private CentricityReferral GetReferralFromDataReader(OracleDataReader dr)
        {
            logger.Trace("====== GetReferralFromDataReader()");
            CentricityReferral cr = new CentricityReferral();
            PropertyInfo[] properties = typeof(CentricityReferral).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object[] attrs = property.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    OracleColumn oCol = attr as OracleColumn;
                    if (oCol != null)
                    {
                        if (oCol.IsDate)
                        {
                            property.SetValue(cr, GetDateFromDataReader(oCol.ColumnName, dr), null);
                        }
                        else if (oCol.IsDecimal)
                        {
                            
                            property.SetValue(cr, GetDecimalFromDataReader(oCol.ColumnName, dr).ToString(), null);
                        }
                        else
                        {
                            property.SetValue(cr, GetStringFromDataReader(oCol.ColumnName, dr), null);
                        }
                        
                    }
                }
                
            }
            logger.Trace("====== GetReferralFromDataReader()...Done");

            return cr;
        }
        
        private CentricityLocation GetLocationFromDataReader(OracleDataReader dr)
        {
            logger.Trace("======GetLocationFromDataReader()...");
            CentricityLocation cl = new CentricityLocation();
            cl.Code = GetStringFromDataReader("PAT_CURRENT_LOCATION_CODE", dr);
            cl.Name = GetStringFromDataReader("PAT_CURRENT_LOCATION_NAME", dr);
            logger.Trace("Location: " + cl.Name + "; code: " + cl.Code +" ... Done");
            return cl;
        }

        private CentricityStaffMember GetStaffMemberFromDataReader(OracleDataReader dr)
        {
            CentricityStaffMember sm = new CentricityStaffMember();

            PropertyInfo[] properties = typeof(CentricityStaffMember).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object[] attrs = property.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    OracleColumn oCol = attr as OracleColumn;
                    if (oCol != null)
                    {
                        if (oCol.IsDate)
                        {
                            property.SetValue(sm, GetDateFromDataReader(oCol.ColumnName, dr), null);
                        }
                        else if (oCol.IsDecimal)
                        {
                            
                            property.SetValue(sm, GetDecimalFromDataReader(oCol.ColumnName, dr).ToString(), null);
                        }
                        else
                        {
                            property.SetValue(sm, GetStringFromDataReader(oCol.ColumnName, dr), null);
                        }
                        
                    }
                }

            }
            return sm;
        }

        /// <summary>
        /// Gets a CentricityPatient from the OracleDataReader
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private CentricityPatient GetPatientFromDataReader(OracleDataReader dr)
        {
            logger.Trace("====== GetPatientFromDataReader()...");
            CentricityPatient p = new CentricityPatient();

            if (GetStringFromDataReader("PAT_SEX", dr) == "W")
            {
                p.Gender = Gender.Female;
            }
            else if (GetStringFromDataReader("PAT_SEX", dr) == "M")
            {
                p.Gender = Gender.Male;
            }
            else
            {
                p.Gender = Gender.Other;
            }
            p.Town = GetStringFromDataReader("PAT_TOWN", dr);
            p.Province = GetStringFromDataReader("PAT_PROVINCE_NAME", dr);
            p.StreetName = GetStringFromDataReader("PAT_STREET", dr);
            p.LastName = GetStringFromDataReader("PAT_LAST_NAME", dr);
            p.FirstName = GetStringFromDataReader("PAT_FIRST_NAME", dr);
            p.HomePhone = GetStringFromDataReader("PAT_TELEPHONE", dr);
            p.MobilePhone = GetStringFromDataReader("PAT_TELEPHONE_MOBILE", dr); 
            p.MRN = AugmentMRN(GetStringFromDataReader("PATIENT_DOMAIN_ID", dr));
            p.PatientConditionCode = GetConditionCodeFromDataReater("PATIENT_CONDITION_CODE", dr);
            p.Title = GetStringFromDataReader("PAT_TITLE", dr);
            Nullable<DateTime> t = GetDateFromDataReader("PAT_BIRTH_DATE", dr);
            if (t.HasValue)
            {
               p.BirthDate = GetDateFromDataReader("PAT_BIRTH_DATE", dr);
            }
            else
            {
               p.BirthDate = new DateTime();
            }
                
            
            logger.Trace("CentricityPatient - " + p.FirstName + " " + p.LastName + " " + p.MRN + " " + p.BirthDate.Value.ToShortDateString() + "... Done");
           
            return p;
        }
        private CentricityProcedure GetProcedureFromDataReader(OracleDataReader dr)
        {
            logger.Trace("======GetProcedureFromDataReader()...");
            CentricityProcedure cp = new CentricityProcedure();
            DateTime? t = GetDateFromDataReader("APPOINTMENT_START", dr);
            if (t.HasValue)
            {
                
                cp.AppiontmentTime = t.Value;
            }
            
           
            cp.ProcedureKey = GetDecimalFromDataReader("PROCEDURE_KEY", dr).ToString();
            cp.ProcedureCode = GetStringFromDataReader("ACTIVITY_CODE", dr);
            cp.ProcedureName = GetStringFromDataReader("PROCEDURE_NAME", dr);
            cp.PatientMRN = AugmentMRN(GetStringFromDataReader("PATIENT_DOMAIN_ID", dr));
            cp.ClinicalInfo = GetStringFromDataReader("RELEVANT_CLINICAL_INFO", dr);
            cp.RequestRemark = GetStringFromDataReader("PROCEDURE_MED_QUES_CODE", dr);
            cp.PatientHistory = GetStringFromDataReader("PROCEDURE_REMARK", dr);
            cp.ReferralKey = GetStringFromDataReader("REF_SOURCE_KEY", dr);
            cp.ReferrerShortName = GetStringFromDataReader("REF_SOURCE_SHORT_NAME", dr);
            if(!dr.IsDBNull(dr.GetOrdinal("REGISTRATION_ARRIVAL")))
            {
                cp.RegistrationArrival = GetDateFromDataReader("REGISTRATION_ARRIVAL",dr);
            }
            cp.PatientConditionCode = GetConditionCodeFromDataReater("PATIENT_CONDITION_CODE", dr);
            cp.CompletionTime = GetDateFromDataReader("PROCEDURE_END", dr);
            cp.ProcedureRemark = GetStringFromDataReader("PROCEDURE_REMARK", dr);
            cp.PatientRisks = GetStringFromDataReader("PAT_RISKS", dr);
            
            logger.Trace("CentricityProcedure - Code: " + cp.ProcedureKey + ", Code: " + cp.ProcedureCode + ", Name: " + cp.ProcedureName + ", MRN: " + cp.PatientMRN + " ... Done");
            return cp;
        }
        public List<CentricityReport> GetCentricityReports(DateTime startDate, DateTime endDate)
        {
            logger.Trace("GetCentricityReports(" + startDate.ToShortDateString() + "," + endDate.ToShortDateString() + ")");
            CentricityReport cr = new CentricityReport();            
            List<CentricityReport> result = new List<CentricityReport>();           
            retriever.SwitchOnAutoDetect();
            String startDateString = startDate.ToString("ddMMyyyy");
            String endDateString = endDate.ToString("ddMMyyyy");
            OracleCommand cmd = new OracleCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = oConn;
            String text;
            text = "SELECT  rep.*,reptext.* " +
              "FROM A_US_REPORT_ALL rep " +
              "JOIN A_US_REPORT_TEXT reptext ON reptext.report_key = to_char(rep.report_key) " +
              "WHERE rep.WORKPLACE_CODE LIKE 'CRNM%'  AND rep.REPORT_DATE BETWEEN to_date('" + startDateString + "','DDMMYYYY') " +             
              "AND to_date ('" + endDateString + "','DDMMYYYY') ORDER BY rep.procedure_key, rep.report_key, reptext.report_text_sequence";
           
            cmd.CommandText = text;
            cmd.CommandTimeout = 0;
            DateTime before = DateTime.Now;
           
            OracleDataReader dr = cmd.ExecuteReader();
            DateTime after = DateTime.Now;
            
            
            while (dr.Read())
            {
                if (Convert.ToInt32(dr["REPORT_TEXT_SEQUENCE"]) == 1)
                {
                    cr = new CentricityReport();
                    result.Add(cr);
                    Console.WriteLine("ReportKey = " + dr["REPORT_KEY"] + "; ProcedureKey = " + dr["PROCEDURE_KEY"] + "; Sequence = " + dr["REPORT_TEXT_SEQUENCE"] + "; Type = " + dr["REPORT_TYPE"]);
                    
                   
                    #region createCentricityReport
                    
                       
                        PropertyInfo[] properties = typeof(CentricityReport).GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                        //Console.WriteLine("Property = " + property.Name);
                            object[] attrs = property.GetCustomAttributes(true);
                            foreach (object attr in attrs)
                            {
                            //Console.WriteLine("attribute is " + attr.GetType().Name);
                                OracleColumn oCol = attr as OracleColumn;
                                if (oCol != null)
                                {
                                    if (oCol.IsDate)
                                    {
                                        property.SetValue(cr, GetDateFromDataReader(oCol.ColumnName, dr), null);
                                    }
                                    else if (oCol.IsDecimal)
                                    {
                                        property.SetValue(cr, GetDecimalFromDataReader(oCol.ColumnName, dr).ToString(), null);
                                    }
                                    else
                                    {
                                        property.SetValue(cr, GetStringFromDataReader(oCol.ColumnName, dr), null);
                                   
                                    }
                                }
                            }

                       

                      }
                    Console.WriteLine("CentricityReport - Key = " + cr.ReportKey + "; ProcedureKey = " + cr.ProcedureKey + "; Status = " + cr.ReportStatus + "; type = " + cr.ReportType);
                    #endregion
                }

                CentricityReportText crt = new CentricityReportText();
                crt.ReportKey = dr["REPORT_KEY"].ToString();
                crt.Sequence = dr.GetInt16(dr.GetOrdinal("REPORT_TEXT_SEQUENCE"));
                crt.Text = dr["REPORT_TEXT"].ToString();
                cr.ReportTexts.Add(crt);
                
                
            }

            logger.Trace("GetCentricityReports(" + startDate.ToShortDateString() + "," + endDate.ToShortDateString() + ") - Completed");
            Console.WriteLine("Result contains " + result.Count + " items");
            return result;
        }

        private Decimal GetDecimalFromDataReader(String ColumnName, OracleDataReader dr)
        {
            try
            {
                return dr.GetOracleDecimal(dr.GetOrdinal(ColumnName)).Value;
            }
            catch (Exception ex)
            {
                //log.Debug("Exception during GetOracleDecimal: ", ex);

                return 0;
            }
        }
        private String GetStringFromDataReader(String ColumnName, OracleDataReader dr)
        {
            try
            {
                
                return dr.GetOracleString(dr.GetOrdinal(ColumnName)).Value.ToString();

            }
            catch (Exception ex)
            {
                //log.Debug("Exception during GetStringFromDataReader: ",ex);

                return null;
            }

        }       
        private DateTime? GetDateFromDataReader(String ColumnName, OracleDataReader dr)
        {
            if (dr.IsDBNull(dr.GetOrdinal(ColumnName)))
                return null;
            return dr.GetOracleDate(dr.GetOrdinal(ColumnName)).Value;

        }

        private PatientConditionCode GetConditionCodeFromDataReater(string columnName,OracleDataReader dr)
        {
            switch (GetStringFromDataReader(columnName, dr))
            {
                case "WHEEL CHAI":
                    return PatientConditionCode.Wheelchair;
                    
                case "ESCORT":
                    return PatientConditionCode.Escort;
                    
                case "MOBILE PRO":
                    return PatientConditionCode.MobilePro;
                    
                case "OWN BED":
                    return PatientConditionCode.OwnBed;
                    
                case "PERFORM IN":
                    return PatientConditionCode.PerformIn;
                    
                case "TROLLEY":
                    return PatientConditionCode.Trolley;
                   
                case "WALK":
                    return PatientConditionCode.Walk;                    
                case "ICU":
                    return PatientConditionCode.ICU;
                default:
                    return PatientConditionCode.Walk;
            }
        }

        private string ConvertToRTF(string input)
        {
            System.Windows.Forms.RichTextBox rtf = new System.Windows.Forms.RichTextBox();
            rtf.Text = input;
            return rtf.Rtf;
        }

        private string ConvertToText(string rtfInput)
        {
            System.Windows.Forms.RichTextBox rtf = new System.Windows.Forms.RichTextBox();
            rtf.Rtf = rtfInput;
            return rtf.Text;
        }
        #endregion

        #region translationMethods

        //translation methods take an object from the centricity model and return an iRadiate model
        //which has been stored in the database, creating a new record if necessary

        /// <summary>
        /// Translates a CentricityPatient to a Patient, never returns null
        /// </summary>
        /// <param name="cp">The cenricity patient</param>
        /// <returns>An iRadiate patient from the database</returns>
        public Patient GetPatient(CentricityPatient cp)
        {
            
            Patient p;
            RetrievalCriteria rc1 = new RetrievalCriteria("MRN", CriteraType.TextMatch, cp.MRN);
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc1);

            if (retriever.RetrieveItems(typeof(Patient), rcList).Any())
            {
                logger.Trace("Patient is alreadey in iRadiate");
                p = (Patient)retriever.RetrieveItems(typeof(Patient), rcList).First();
                logger.Trace("iRadiate Patient - " + p.GivenNames + " " + p.Surname + " " + p.MRN);
                bool ptChanged = false;
                if (p.Surname != cp.LastName)
                {
                    logger.Info("Surname " + p.Surname + " changed to " + cp.LastName);
                    p.Surname = cp.LastName;
                    ptChanged = true;
                }
                if (p.DateOfBirth != cp.BirthDate.Value)
                {
                    logger.Info("Date of Birth changed from " + p.DateOfBirth + " to " + cp.BirthDate);
                    p.DateOfBirth = cp.BirthDate.Value;
                    ptChanged = true;
                }
                if (p.GivenNames != cp.FirstName)
                {
                    logger.Info("Given Name of patient " + p.Surname + " changed from " + p.GivenNames + " to " + cp.FirstName);
                    p.GivenNames = cp.FirstName;
                    ptChanged = true;
                }

                if (p.HomePhone != cp.HomePhone)
                {
                    logger.Info("Home phone of patient " + p.Surname + " changed from " + p.HomePhone + " to " + cp.HomePhone);
                    p.HomePhone = cp.HomePhone;
                    ptChanged = true;
                }

                if (p.MobilePhone != cp.MobilePhone)
                {
                    logger.Info("Mobile phone of patient " + p.Surname + " changed from " + p.MobilePhone + " to " + cp.MobilePhone);
                    p.MobilePhone = cp.MobilePhone;
                    ptChanged = true;
                }


                if (p.Title != cp.Title)
                {
                    p.Title = cp.Title;
                    ptChanged = true;
                }
                if (p.StreetName != cp.StreetName)
                {
                    p.StreetName = cp.StreetName;
                    ptChanged = true;
                }


                if (p.TownName != cp.Town)
                {
                    p.TownName = cp.Town;
                    ptChanged = true;
                }


                if (ptChanged)
                {
                    retriever.SaveItem(p);
                }
            }
            else
            {
                logger.Info("Patient Not found in iRadiate");
                p = new Patient();
                p.DateOfBirth = cp.BirthDate.Value;
                p.MRN = cp.MRN;
                p.Gender = cp.Gender;
                p.GivenNames = cp.FirstName;
                p.Surname = cp.LastName;
                p.Title = cp.Title;
                p.StreetName = cp.StreetName;
                p.TownName = cp.Town;
                p.HomePhone = cp.HomePhone;
                p.MobilePhone = cp.MobilePhone;
                retriever.SaveItem(p);
                logger.Info("Patient saved!!");
                logger.Info("ID = " + p.ID.ToString());
            }
            logger.Trace("====== GetPatient() ... Done");


            return p;
        }

        /// <summary>
        /// Returns a ward if location is bridged, null otherwise
        /// </summary>
        /// <param name="cl"></param>
        /// <returns></returns>
        public Ward GetWard(CentricityLocation cl)
        {
            logger.Trace("====== GetWard()");
            if (cl.Code == "" || cl.Code == null)
            {
                return null;
            }
            using (SqlConnection tempCon = new SqlConnection(Properties.Settings.Default.InterfaceConnString))
            {
                tempCon.Open();
                Ward w = null;

                SqlCommand command = new SqlCommand("SELECT * FROM dbo.WardBridges WHERE ForeignKey = @ForeignKey", tempCon);

                command.Parameters.AddWithValue("@ForeignKey", cl.Code);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    logger.Trace("Ward has already been bridged");
                    reader.Read();
                    if (reader["LocalKey"] != System.DBNull.Value)
                    {
                        try
                        {
                            int localID = Convert.ToInt32(reader["LocalKey"].ToString());

                            w = (Ward)retriever.RetrieveItem(localID, typeof(Ward));
                            return w;
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Unable to retrieve ward from localkey column, currentLocation = " + cl.Code + "; currentLocationName = " + cl.Name);
                        }
                    }


                }
                else
                {
                    logger.Trace("Ward has not yet been bridged");
                    reader.Close();
                    ///w = new Ward();
                    ///w.Hospital = retriever.RetrieveItems(typeof(Hospital), new List<RetrievalCriteria>()).First() as Hospital;
                    ///w.Name = cl.Name;
                    ///w.Abbreviation = cl.Code;
                    ///retriever.SaveItem(w);
                     SqlCommand insertCommand = new SqlCommand("INSERT INTO dbo.WardBridges (ForeignKey,ForeignName) VALUES (@ForeignKey, @ForeignName)", tempCon);

                    insertCommand.Parameters.AddWithValue("@ForeignKey", cl.Code);
                    insertCommand.Parameters.AddWithValue("@ForeignName", cl.Name);
                    if (insertCommand.ExecuteNonQuery() > 0)
                    {
                        logger.Info("Ward bridge inserted into system.");
                    }
                    else
                    {
                        logger.Error("Error, insert wardbridge did not return > 0 rows.");
                        //Dont bother logging this, it occurs when the ward is not listed in the bridges
                    }

                }
                logger.Trace("====== GetWard() ... Done");
                return w;
            }
           
        }

        /// <summary>
        /// Gets an iRadiate Study based on a CentricityProcedure
        /// </summary>
        /// <param name="cp">The centricity procedure</param>
        /// <returns>An iradiate Study from the data store</returns>
        /// <remarks>
        /// If the CentricityProcedure has already been bridged it will return the existing Study with the date and study type that
        /// match what has come off the RIS.
        /// If the CentricityProcedure has already been bridged but now the relevant CentricityProcedureCode is
        /// a followup then this method will return the first non-deleted, matching Study within the Day range.
        /// It will not create a new iRadiate study if this is a follow up and there are no matching Study records in iRadiate.
        /// If the Centricity Procedure is not yet bridged and is not a followup this method will create a new study but
        /// not set the patient.
        /// </remarks>
        [Obsolete("Use GetAppointment instead")]
        public Study GetStudy(CentricityProcedure cp)
        {
            logger.Trace("GetStudy()...");
            //the big question is whether we are making a new study or adding to an existing study!
            ProcedureTypeBridge studyTypeBridge = GetStudyTypeBridge(cp);
            
            ProcedureBridge studyBridge = GetProcedureBridge(cp);





            return studyBridge.Study;
        }

        /// <summary>
        /// Gets a procedure bridge based on a Centricity Procedure
        /// </summary>
        /// <param name="cp">The CentricityProcedure which has been or will be bridged</param>
        /// <returns>Null if CentricityProcedure is not yet bridged,</returns>
        /// <remarks>Will not create an appointment will just return null</remarks>
        private ProcedureBridge GetProcedureBridge(CentricityProcedure cp)
        {
            logger.Trace("GetProcedureBridge(" + cp.AppiontmentTime.ToString() + ")");
            using (SqlConnection co = new SqlConnection(Properties.Settings.Default.InterfaceConnString))
            {
                co.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ProcedureBridges WHERE ForeignKey = @ForeignKey", co);
                cmd.Parameters.AddWithValue("@ForeignKey", cp.ProcedureKey);
               
                
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    logger.Trace("Procedure has been bridged already");
                    reader.Read();
                    ProcedureBridge pb = new ProcedureBridge();
                    pb.ForeignKey = cp.ProcedureKey;
                    pb.LocalID = reader.GetInt32(reader.GetOrdinal("AppointmentID"));
                    pb.Appointment = (Appointment)retriever.RetrieveItem(pb.LocalID, typeof(Appointment));
                    logger.Trace("GetProcedureBridge(" + cp.AppiontmentTime.ToString() + ") - Completed with value");
                    return pb;
                }
                else
                {
                    logger.Trace("Procedure has not already been bridged");
                }
                logger.Trace("GetProcedureBridge(" + cp.AppiontmentTime.ToString() + ") - returning null");
                return null;
            }
        }

        /// <summary>
        /// Get a ProcedureTypeBridge for a matching CentricityProcedure
        /// </summary>
        /// <param name="cp">The CentricityProcedure extracted from the query</param>
        /// <returns>A ProcedureTypeBridge or null if this StudyType has not yet been bridged.</returns>
        /// <remarks>This method will insert a record in the StudyTypeBridges if the ForeignKey does not match but it will not be bridged automatically</remarks>
        private ProcedureTypeBridge GetStudyTypeBridge(CentricityProcedure cp)
        {
            logger.Trace("====== GetStudyTypeBridge(" + cp.ProcedureName + ")");
            using (SqlConnection co = new SqlConnection(Properties.Settings.Default.InterfaceConnString))
            {
                co.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM StudyTypeBridges WHERE ForeignKey = @ForeignKey", co);
                cmd.Parameters.AddWithValue("@ForeignKey", cp.ProcedureCode);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    
                    reader.Read();
                    ProcedureTypeBridge ptb = new ProcedureTypeBridge();
                    ptb.ForeignKey = reader.GetString(reader.GetOrdinal("ForeignKey"));
                    ptb.ForeignKey = reader.GetString(reader.GetOrdinal("ForeignName"));
                    if (reader.IsDBNull(reader.GetOrdinal("LocalKey")))
                    {
                        logger.Trace("This procedure code is noy yet bridged");
                        //This means an entry is there but it hasn't been bridged yet
                        return null;
                    }
                    else
                    {
                        ptb.LocalKey = Convert.ToInt16(reader["LocalKey"]);
                        logger.Trace("This procedure code is already bridged - LocalKey = " + ptb.LocalKey);
                        ptb.StudyType = (StudyType)retriever.RetrieveItem(Convert.ToInt16(ptb.LocalKey), typeof(StudyType));
                    }
                    if (!reader.IsDBNull(reader.GetOrdinal("IsFollowUp")))
                    {
                        
                        ptb.IsFollowUp = reader["IsFollowUp"] as bool? ?? false;

                    }
                   
                    ptb.NumInjections = reader["NumberOfInjections"] as int? ?? 0;
                    ptb.Range = reader["FollowUpRange"] as int? ?? 0;
                    for (int i = 1; i <= 4; i++)
                    {
                        InjectionDetail id = new InjectionDetail();
                        id.InjectionActivity = reader["Injection" + i.ToString() + "Activity"] as int? ?? 0;
                        id.InjectionDelay = reader["Injection" + i.ToString() + "Delay"] as int? ?? 0;
                        var tmp1 = reader["Injection" + i.ToString() + "Route"] as int? ?? -1;
                        System.Diagnostics.Debug.WriteLine("tmp1 = " + tmp1);
                        if (tmp1 == -1)
                            id.AdministrationRoute = null;
                        else
                        {
                            id.AdministrationRoute = (AdministrationRoute)Enum.ToObject(typeof(AdministrationRoute), tmp1);
                        }
                        
                        int tmp = reader["Injection" + i.ToString() + "RadiopharmaceuticalID"] as int? ?? 0;
                        if (tmp > 0)
                        {
                            id.Radiopharmaceutical = retriever.RetrieveItem(tmp, typeof(Chemical));
                        }
                        ptb.InjectionDetails.Add(id);
                    }

                    ptb.NumScans = reader["NumberOfScans"] as int? ?? 0;
                    for (int i = 1; i <= 4; i++)
                    {
                        ScanDetail sd = new ScanDetail();
                        sd.ScanDelay = reader["Scan" + i.ToString() + "Delay"] as int? ?? 0;
                        sd.ScanDuration = reader["Scan" + i.ToString() + "Duration"] as int? ?? 0;
                        int tmp = reader["Scan" + i.ToString() + "RoomID"] as int? ?? 0;
                        if (tmp > 0)
                        {
                            sd.Room = retriever.RetrieveItem(tmp, typeof(Room));
                        }
                        ptb.ScanDetails.Add(sd);
                    }

                    logger.Trace("GetStudyTypeBridge(" + cp.ProcedureName + ") ... Completed. Returning value");
                    return ptb;
                }
                else
                {
                    logger.Trace("This is a new type of procedure");
                    using (SqlConnection tmpConn = new SqlConnection(Properties.Settings.Default.InterfaceConnString))
                    {
                        tmpConn.Open();
                        SqlCommand insertCommand = new SqlCommand("INSERT INTO dbo.StudyTypeBridges (ForeignKey,ForeignName) VALUES (@ForeignKey, @ForeignName)", tmpConn);
                        insertCommand.Parameters.AddWithValue("@ForeignKey", cp.ProcedureCode);
                        insertCommand.Parameters.AddWithValue("@ForeignName", cp.ProcedureName);
                        if (insertCommand.ExecuteNonQuery() > 0)
                        {
                            logger.Info("Procedure " + cp.ProcedureCode + " bridge inserted into system ");
                        }
                        else
                        {
                            logger.Error("Faied to bridge procedure type" + cp.ProcedureCode);
                            
                        }
                    }
                    return null;
                }
            }
        }

        private DoctorBridge GetDoctorBridge(CentricityReferral cr)
        {
            
            using (SqlConnection co = new SqlConnection(Properties.Settings.Default.InterfaceConnString))
            {
                co.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM DoctorBridges WHERE ForeignKey = @ForeignKey", co);
                cmd.Parameters.AddWithValue("@ForeignKey", cr.ReferralSourceKey);
               
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {

                    reader.Read();
                    DoctorBridge dr = new DoctorBridge();
                   
                    dr.ForeignKey = reader.GetString(reader.GetOrdinal("ForeignKey"));
                    dr.Doctor = retriever.RetrieveItem(reader.GetInt32(reader.GetOrdinal("DoctorID")), typeof(Doctor)) as Doctor;
                    
                    dr.LocalKey = dr.Doctor.ID;
                    return dr;
                    
                }
                else
                {
                    logger.Trace("This is a new Doctor");
                    using (SqlConnection tmpConn = new SqlConnection(Properties.Settings.Default.InterfaceConnString))
                    {
                        //Make a new doctor and save him/her
                        Doctor d = new Doctor();
                        d.Gender = Gender.Other;
                        d.GivenNames = cr.ReferralSourceName.Split(' ')[1];
                        d.Title = cr.ReferralSourceName.Split(' ')[0];
                        d.Surname = cr.ReferralSourceName.Substring(d.Title.Length + d.GivenNames.Length + 1);
                        d.PracticeAddress = cr.ReferralSourcePostalAddress;
                        d.ProviderNumber = cr.ReferralSourceForeignID;
                        d.Referrer = true;
                        string[] tmpArr = cr.RefSourceShortName.Split(' ');
                        d.PracticeName = tmpArr[tmpArr.Length - 2] + " " + tmpArr[tmpArr.Length - 1];

                        retriever.SaveItem(d);
                        tmpConn.Open();
                        SqlCommand insertCommand = new SqlCommand("INSERT INTO dbo.DoctorBridges (ForeignKey,DoctorID) VALUES (@ForeignKey, @DoctorID)", tmpConn);
                        insertCommand.Parameters.AddWithValue("@ForeignKey", cr.ReferralSourceKey);
                        insertCommand.Parameters.AddWithValue("@DoctorID", d.ID);
                        if (insertCommand.ExecuteNonQuery() > 0)
                        {
                            logger.Info("Doctor " + d.FullName + " bridge inserted into system ");
                            DoctorBridge dr = new DoctorBridge();
                            dr.Doctor = d;
                            dr.ForeignKey = cr.ReferralSourceKey;
                            dr.ForeignName = cr.ReferralSourceName;
                            dr.LocalKey = d.ID;
                            return dr;
                        }
                        else
                        {
                            logger.Error("Faied to bridge Doctor " + d.FullName);

                        }
                    }
                    return null;
                }
            }
           
        }

        /// <summary>
        /// Returns an iRadiate appointment based on the CentricityProcedure extracted from a RIS Query
        /// </summary>
        /// <param name="cp">The Centricity Procedure</param>
        /// <returns>An existing Appointment if already bridged, or a new appointment which has been added to iRadiate</returns>
        /// <remarks>
        /// If the procedure has already been bridged, then the method will force the iRadiate database to match the RIS date-time
        /// </remarks>                 
        public Appointment GetAppointment(CentricityProcedure cp)
        {
            logger.Trace("====== GetAppointment()");
            ProcedureBridge pb = GetProcedureBridge(cp);
            if (pb != null)
            {
                if(pb.Appointment != null)
                {
                    logger.Trace("====== GetAppointment().... Done");
                    return pb.Appointment;
                }
                    

                logger.Warn("Centricity Procedure " + cp.ProcedureKey + "returned a null procedure in the procedure bridge");
                return null;
            }
            else
            {
                pb = new ProcedureBridge();
                ProcedureTypeBridge ptb = GetStudyTypeBridge(cp);
                if (ptb == null)
                {
                    logger.Trace("====== GetAppointment().... Done");
                    return null;
                }
                    
                if(ptb.StudyType == null)
                {
                    logger.Trace("====== GetAppointment().... Done");
                    return null;
                }
                else
                {
                    Appointment a = new Appointment();
                    a.ScheduledArrivalTime = cp.AppiontmentTime;
                    logger.Trace("New appointmnt made for " + a.ScheduledArrivalTime.ToString());
                    if (ptb.IsFollowUp)
                    {
                        //Find a recent appointment and add this one to that study
                    }
                    else
                    {
                        Study s = new Study();
                        s.StudyType = ptb.StudyType;
                        a.Study = s;

                    }
                    //now that have created a new appointment, save it then 
                    //create a bridge between the appointment and the centricity procedure
                    retriever.SaveItem(a);
                    logger.Trace("Appointment saved");
                    using (SqlConnection tmpCon = new SqlConnection(Properties.Settings.Default.InterfaceConnString))
                    {
                        tmpCon.Open();
                        SqlCommand insertCommand = new SqlCommand("INSERT INTO dbo.ProcedureBridges (ForeignKey,AppointmentID) VALUES (@ForeignKey, @AppointmentID)", tmpCon);

                        insertCommand.Parameters.AddWithValue("@ForeignKey", cp.ProcedureKey);
                        insertCommand.Parameters.AddWithValue("@AppointmentID", a.ID);
                        if (insertCommand.ExecuteNonQuery() > 0)
                        {
                            logger.Info("Procedure " + cp.ProcedureKey +" bridge inserted into system with appointment " + a.ID);
                        }
                        else
                        {
                            logger.Error("Faied to bridge procedure " + cp.ProcedureKey);
                            //Dont bother logging this, it occurs when the ward is not listed in the bridges
                        }

                    }
                    logger.Trace("====== GetAppointment().... Done");
                    return a;
                }
                
            }
            
        }   

        /// <summary>
        /// Updates the appointment to match the properties in the centricityPatient
        /// </summary>
        /// <param name="a">The appointment being updated</param>
        /// <param name="cp">The centricityprocedur ebeing taken from the RIS</param>
        private void UpdateAppointment(Appointment a, CentricityProcedure cp)
        {
            logger.Trace("====== UpdateAppointment()");
            if(a.ScheduledArrivalTime != cp.AppiontmentTime)
            {
                a.reschedule(cp.AppiontmentTime);
                //a.ScheduledArrivalTime = cp.AppiontmentTime;
                if(a.Patient != null)
                {
                    logger.Info("Appointment for " + a.Patient.FullName + " moved to " + a.ScheduledArrivalTime.ToString());
                }
            }

            if (cp.RegistrationArrival.HasValue)
            {
                if(a.PatientRegistered != true)
                {
                    a.PatientRegistered = true;
                    logger.Info("Appointment number " + a.ID.ToString() + " PatientRegistered set to True");
                    a.PatientRegistrationDate = DateTime.Now;
                    logger.Info("Appointment number " + a.ID.ToString() + " PatientRegistrationDate set to " + a.PatientRegistrationDate.ToString());
                }
               
                if (!a.HasPatientArrived)
                {
                    ArrivalTask at = new ArrivalTask();
                    at.ScheduledCompletionTime = a.ScheduledArrivalTime;
                    at.CompletionTime = DateTime.Now;
                    at.Completed = true;
                    a.Tasks.Add(at);
                    at.Appointment = a;
                    retriever.SaveItem(at);
                    logger.Info("Appointment " + a.ID.ToString() + " has arrival task at " + at.CompletionTime.ToString());
                }
            }
            else
            {
                if (a.PatientRegistered)
                {
                    a.PatientRegistered = false;
                    
                }
            }

            if(cp.PatientConditionCode == PatientConditionCode.OwnBed)
            {
                a.TransportType = PatientTransportType.Bed;
            }
            else if(cp.PatientConditionCode == PatientConditionCode.Trolley)
            {
                a.TransportType = PatientTransportType.Bed;
            }
            else if (cp.PatientConditionCode == PatientConditionCode.Wheelchair)
            {
                a.TransportType = PatientTransportType.Wheelchair;
            }
            else
            {
                a.TransportType = PatientTransportType.Ambulatory;
            }

            if (cp.CompletionTime.HasValue)
            {
                if (a.Completed != true)
                {
                    a.Completed = true;
                    logger.Info("Appointment " + a.ID + " marked as completed " + a.CompletionTime.ToString());
                }
                if(a.CompletionTime != cp.CompletionTime.Value)
                {
                    a.CompletionTime = cp.CompletionTime.Value;
                }
                

                
            }

            if(a.Risks != cp.PatientRisks)
            {
                a.Risks = cp.PatientRisks;
                logger.Info("Risk " + a.Risks + " added to appointment " + a.ID);
            }

            if(a.ReferenceNumber != cp.ProcedureKey)
            {
                a.ReferenceNumber = cp.ProcedureKey;
                logger.Info("Reference number " + a.ReferenceNumber + " added to appointment " + a.ID);
            }
            retriever.SaveItem(a);
            BuildWorkflow(a, GetStudyTypeBridge(cp));
            logger.Trace("====== UpdateAppointment()...Done");
        }

        public StudyRequest GetStudyRequest(CentricityReferral cr)
        {
            StudyRequest sr = new StudyRequest();
            
            sr.ClinicalInfo = cr.ClinicalInformation;
            sr.PatientHistory = cr.MedicalQuestion;
            if(cr.RequestDate.HasValue)
                sr.ReferralDate = cr.RequestDate.Value;

            sr.Referrer = GetReferrer(cr);
            
            return sr;
        }

        public Doctor GetReferrer(CentricityReferral cr)
        {
            //Look in DoctorBridges and see if this key is alrady bridged
            if (cr.ReferralSourceKey == "")
                return null;
            DoctorBridge db = GetDoctorBridge(cr);
            return db.Doctor;
        }
        public void BuildWorkflow(Appointment a, ProcedureTypeBridge ptb)
        {
            logger.Trace("====== BuildWorkflow()");
            if (ptb == null)
            {
                logger.Warn("No worklow to build because ProcedureTypeBridge is null");
                logger.Trace("====== BuildWorkflow()... Done");
                return;
            }
                
            //How many injections should there be in this appointment
            int numInjections = ptb.NumInjections;

            //If there are supposed to be no injections as part of this proceduretypebridge,
            //don't bother trying to delete th old ones, just leave as is
            if (numInjections == 0)
            {
                //logger.Warn("There are supposed to be no injections for this but we found one, just return");
                //logger.Trace("====== BuildWorkflow()... Done");
                //return;
            }
            

            int injectionsInApt = a.Tasks.Where(x => !x.Deleted && x is DoseAdministrationTask).Count();
            if(injectionsInApt < numInjections)
            {
                //how many more should we add?
                int difference = numInjections - injectionsInApt;
                int startNum = numInjections - difference+1;
                for(int x = startNum; x <= numInjections; x++)
                {
                    
                    DoseAdministrationTask inj = new DoseAdministrationTask();
                    inj.ScheduledCompletionTime = a.ScheduledArrivalTime.AddMinutes(ptb.InjectionDetails[x].InjectionDelay);
                    inj.PrescribedRadioPharmaceutical = (Chemical)ptb.InjectionDetails[x].Radiopharmaceutical;
                    inj.PrescribedMinimum = (int)(ptb.InjectionDetails[x].InjectionActivity * 0.9);
                    inj.PrescribedMaximum = (int)(ptb.InjectionDetails[x].InjectionActivity * 1.1);
                    inj.AdministrationRoute = ptb.InjectionDetails[x].AdministrationRoute;
                    logger.Info("Adding a DoseAdministrationTask .. Time: " + inj.ScheduledCompletionTime);
                    a.Tasks.Add(inj);

                }
                retriever.SaveItem(a);
            }
            #region scans
            int numScans = ptb.NumScans;

            //If there are supposed to be no injections as part of this proceduretypebridge,
            //don't bother trying to delete th old ones, just leave as is
            if (numScans == 0)
                return;

            int scansInApt = a.Tasks.Where(x => !x.Deleted && x is ScanTask).Count();
            if (scansInApt < numScans)
            {
                //how many more should we add?
                int difference = numScans - scansInApt;
                int startNum = numScans - difference + 1;
                for (int x = startNum; x <= numScans; x++)
                {
                    /*DoseAdministrationTask inj = new DoseAdministrationTask();
                    inj.ScheduledCompletionTime = a.ScheduledArrivalTime.AddMinutes(ptb.InjectionDetails[x].InjectionDelay);
                    inj.PrescribedRadioPharmaceutical = (Chemical)ptb.InjectionDetails[x].Radiopharmaceutical;
                    inj.PrescribedMinimum = (int)(ptb.InjectionDetails[x].InjectionActivity * 0.9);
                    inj.PrescribedMaximum = (int)(ptb.InjectionDetails[x].InjectionActivity * 1.1);
                    a.Tasks.Add(inj);*/

                    ScanTask st = new ScanTask();
                    st.Room = (Room)ptb.ScanDetails[x].Room;
                    st.ScheduledCommencementTime = a.ScheduledArrivalTime.AddMinutes(ptb.ScanDetails[x].ScanDelay);
                    st.ScheduledCompletionTime = st.ScheduledCommencementTime.AddMinutes(ptb.ScanDetails[x].ScanDuration);
                    logger.Info("Adding a scanTask at " + st.ScheduledCommencementTime);
                    a.Tasks.Add(st); 

                }
                retriever.SaveItem(a);
            }

            logger.Trace("====== BuildWorkflow() ... Done");
            #endregion

        }
        #endregion

        #region privateMethods
        public string AugmentMRN(String mrn)
        {
            if (mrn.Length > 6)
            {
                return mrn;
            }
            String s = new String('0', 7 - mrn.Length);

            return s + mrn;
        }

        /// <summary>
        /// Sets the Platform.CurrentUser to someone called Centricity Interface
        /// </summary>
        /// <returns>True if CurrentUser set correctly</returns>
        private bool SetCurrentUser()
        {
            if (retriever == null)
            {
                throw new Exception("Retriever is null");

            }
            RetrievalCriteria rc1 = new RetrievalCriteria("GivenNames", CriteraType.ExactTextMatch, "Centricity");
            RetrievalCriteria rc2 = new RetrievalCriteria("Surname", CriteraType.ExactTextMatch, "Interface");
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc1);
            rcList.Add(rc2);
            List<IDataStoreItem> result = retriever.RetrieveItems(typeof(User), rcList);
            if (result.Count > 0)
            {
                iRadiate.Common.Platform.CurrentUser = result.First() as User;
            }
            else
            {
                User u = new User();
                u.GivenNames = "Centricity";
                u.Surname = "Interface";
                u.DateOfBirth = new DateTime(1970, 1, 1);
                u.Gender = Gender.Other;
                retriever.SaveItem(u);
                iRadiate.Common.Platform.CurrentUser = u;
            }
            return true;
        }
        #endregion

        #region publicMethods
        public void CloseConnections()
        {
            oConn.Close();
            oConn.Dispose();
        }

        /// <summary>
        /// Gets a collection of iRadiate studies from the centricity database for a given date range
        /// using bridges where available and creating new ones when needed
        /// </summary>
        /// <param name="startDate">The start of the date range</param>
        /// <param name="endDate">The end of the date range</param>
        /// <returns>A list of iRadiate studies, already saved in iRadiate</returns>
        public List<Appointment> GetAppointments(DateTime startDate, DateTime endDate)
        {
            logger.Log(LogLevel.Info, "GetAppointments(" + startDate.ToShortDateString() + "," + endDate.ToShortDateString()+")");
            retriever.SwitchOnAutoDetect();
            List<Appointment> result = new List<Appointment>();
            String startDateString = startDate.ToString("ddMMyyyy");
            String endDateString = endDate.ToString("ddMMyyyy");

            OracleCommand cmd = new OracleCommand();
            cmd.CommandTimeout = 120;
            cmd.Connection = oConn;

            String text;

            text = "SELECT  apt.*,pat.*, dom.*, pro.*,refe.*,wor.*, apt.*, ppro.* " +
              "FROM A_US_APPOINTMENT apt " +
              "JOIN A_US_PATIENT pat ON apt.patient_key = pat.patient_key " +
              "JOIN A_US_PATIENT_DOMAIN_ID dom ON  dom.patient_key = pat.patient_key  " +
              "LEFT JOIN A_US_PERFORMED_PROCEDURES_ALL ppro ON apt.PROCEDURE_KEY = ppro.PROCEDURE_KEY " +
              "LEFT JOIN A_US_BD_PROCEDURE pro ON apt.ACTIVITY_CODE = pro.PROCEDURE_CODE " +
              "LEFT JOIN A_US_BD_REFERRAL_SOURCE refe ON apt.REF_SOURCE_KEY = refe.REF_SOURCE_KEY " +
              "JOIN A_US_BD_WORKPLACE wor ON  wor.WORKPLACE_CODE = apt.APPOINTMENT_BOOK " +
              "WHERE dom.DOMAIN_NAME = 'CRG' AND apt.INSTITUTE_KEY = '2060164' AND apt.APPOINTMENT_START BETWEEN to_date('" + startDateString + "','DDMMYYYY') " +
              "AND to_date ('" + endDateString + "','DDMMYYYY') ";

            cmd.CommandText = text;
            cmd.CommandTimeout = 60;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                logger.Trace("****** Centricity Record ********* Begin...");
                logger.Trace("-> Helper Methods...");
                CentricityPatient cp = GetPatientFromDataReader(dr);
                CentricityLocation cl = GetLocationFromDataReader(dr);
                CentricityProcedure cpr = GetProcedureFromDataReader(dr);
                CentricityReferral cr = GetReferralFromDataReader(dr);
                logger.Trace("-> Helper Methods...DONE!");

                logger.Trace("-> Translation Methods");
                Patient p = GetPatient(cp);
                try
                {
                    Ward w = GetWard(cl);
                    if (w != null)
                    {
                        if (p.Ward != null)
                        {
                            if (p.Ward != w)
                            {
                                p.Ward = w;
                                retriever.SaveItem(p);
                            }
                        }
                        else
                        {
                            p.Ward = w;
                            retriever.SaveItem(w);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Caught exception trying to GetWard() - " + ex.Message);
                }

                Appointment a = null;
                try
                {
                    a = GetAppointment(cpr);
                }
               catch(Exception ex)
                {
                    logger.Error("Exception caught in GetAppointment() " + ex.Message);
                }
                if (a == null) {
                    logger.Warn("GetAppointment return null ???");
                    continue; }
                if (a.Patient == null)
                {
                    a.Study.Patient = p;
                    logger.Info("Assigned Patient " + p.FullName + " to " + a.Name + " on " + a.ScheduledArrivalTime.ToShortDateString());
                    retriever.SaveItem(a);
                }
                try
                {
                    UpdateAppointment(a, cpr);
                }
                catch(Exception ex)
                {
                    logger.Error("Caught exception in UpdateAppointment() for Appointment.ID = " + a.ID + ": " + ex.Message);
                }
                p.TransportType = a.TransportType;
                a.Ward = p.Ward;
                if (a.Ward != null)
                {
                    //a.InPatient = false;
                    
                }
                try
                {
                    if (a.Study.Request == null)
                    {
                        a.Study.Request = GetStudyRequest(cr);
                        retriever.SaveItem(a.Study);
                    }
                }
               catch(Exception ex)
                {
                    logger.Error("Error with getting request for " + a.ID + ": " + ex.Message);
                }

                try
                {
                    retriever.SaveItem(a);
                }
                catch (Exception ex)
                {
                    logger.Error("Error with saving appointment " + a.ID + ": " + ex.Message);
                }

                try
                {
                    result.Add(a);
                }
                catch (Exception ex)
                {
                    logger.Error("Error with adding appointment " + a.ID + " to result set: " + ex.Message);
                }
                logger.Trace("-> Translation Methods...DONE!");
            }
            //we need to get all the appointments in the retriever and cancel any which don't appear in the result
            RetrievalCriteria rc1 = new RetrievalCriteria("ScheduledArrivalTime", CriteraType.GreaterThan, startDate);
            RetrievalCriteria rc2 = new RetrievalCriteria("ScheduledArrivalTime", CriteraType.LessThan, endDate);
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            rcList.Add(rc1);
            rcList.Add(rc2);

            List<IDataStoreItem> iRadiateAppointments = retriever.RetrieveItems(typeof(Appointment), rcList);

            List<int> CentricityIDs = result.Select(x => x.ID).ToList();
            foreach(Appointment a in iRadiateAppointments)
            {
                if (!CentricityIDs.Contains(a.ID))
                {
                    //We need to cancel this
                    if (a.Cancelled != true)
                    {
                        a.Cancelled = true;
                        logger.Info("Cancelled appointment " + a.ID);
                        retriever.SaveItem(a);
                    }
                }
                else
                {
                    if(a.Cancelled == true)
                    {
                        a.Cancelled = false;
                        logger.Info("Uncancelled appointment " + a.ID);
                        retriever.SaveItem(a);
                    }
                }
            }
                
            logger.Info("GetAppointments(" + startDate.ToShortDateString() + "," + endDate.ToShortDateString() + ") - Completed");
            return result;
        }

        public List<StudyReport> GetReports(DateTime startDate, DateTime endDate)
        {
            logger.Log(LogLevel.Info, "GetReports(" + startDate.ToShortDateString() + "," + endDate.ToShortDateString() + ")");
            List<StudyReport> result = new List<StudyReport>();

            List<CentricityReport> CentricityReports = GetCentricityReports(startDate, endDate);
            //go through each report and see if it is is already mapped
            SqlConnection iCon = new SqlConnection(Properties.Settings.Default.InterfaceConnString);
            iCon.Open();
            foreach(CentricityReport cr in CentricityReports)
            {
                //if the report isn't vertified,skip it
                if (cr.ReportStatus != "f")
                    continue;

                //Only bother with main reports, worry about the addenda later
                if (cr.ReportType == "Additional")
                    continue;
             
                Study study; //The Study that this CentricitReport links to
                Appointment appointment; //The Appointment that this CentricityReport links to   
                StudyReport sr;//The StudyReport that this CentricityReport links to             

                /*If the report is for a procedure we haven't bridged, skip it*/

                string qry = "SELECT * FROM ProcedureBridges WHERE ForeignKey = '" + cr.ProcedureKey.ToString() +"'";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = iCon;
                cmd.CommandText = qry;
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    int appointmentID = Convert.ToInt32(dr["AppointmentID"]);
                    appointment = retriever.RetrieveItem(appointmentID, typeof(Appointment)) as Appointment;
                    study = appointment.Study;
                }
                else
                {
                    Console.WriteLine("Procedure key " + cr.ProcedureKey.ToString() + " hasn't been bridged - skip it");
                    dr.Close();
                    continue;
                }
                dr.Close();

                //Now we are ready to get the StudyReport and add it to our result collection
                qry = "SELECT * FROM ReportBridges WHERE ForeignKey = '" + cr.ReportKey + "' AND ProcedureKey = '" + cr.ProcedureKey + "'";
                cmd = new SqlCommand();
                cmd.CommandText = qry;
                cmd.Connection = iCon;
                dr = cmd.ExecuteReader();

                if (dr.HasRows) //This means that this report already has a study report and is linked to this procedure
                {
                    Console.WriteLine("This report is already bridged");
                    dr.Read();
                   
                    StudyReport studyRep = retriever.RetrieveItem(Convert.ToInt32(dr["LocalID"]), typeof(StudyReport)) as StudyReport;
                    result.Add(studyRep);
                    dr.Close();
                    continue;
                }
                else //This means this PROCEDURE-REPORT combination is not bridged, but has this report already been used for a different pocedure?
                {
                    Console.WriteLine("this PROCEDURE-REPORT combination is not bridged, but has this report already been used for a different pocedure?");
                    dr.Close();
                    qry = "SELECT * FROM ReportBridges WHERE ForeignKey = '" + cr.ReportKey + "'";
                    cmd.CommandText = qry;
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows) //this means the report is bridged but for a different procedure
                    {
                        Console.WriteLine("Report found for a different study");
                        dr.Read();
                        int reportID = Convert.ToInt32(dr["LocalID"]);
                        sr = retriever.RetrieveItem(reportID, typeof(StudyReport)) as StudyReport;
                        
                        sr.Studies.Add(study);
                        result.Add(sr);
                        retriever.SaveItem(sr);
                        logger.Info("Linked Study " + study.ID + " to report " + reportID);
                        dr.Close();
                        
                        
                        SqlCommand insertCommand = new SqlCommand("INSERT INTO dbo.ReportBridges (ForeignKey,LocalID, ProcedureKey) VALUES (@ForeignKey, @LocalID, @ProcedureKey)", iCon);

                        insertCommand.Parameters.AddWithValue("@ForeignKey", cr.ReportKey);
                        insertCommand.Parameters.AddWithValue("@LocalID", sr.ID);
                        insertCommand.Parameters.AddWithValue("@ProcedureKey", cr.ProcedureKey);
                        if (insertCommand.ExecuteNonQuery() > 0)
                        {
                            logger.Info("Report bridge inserted into system.");
                        }
                        else
                        {
                            logger.Error("Error, insert report bridge did not return > 0 rows.");
                            
                        }
                    }
                    else
                    {
                        Console.WriteLine("New report has to be created");
                        dr.Close();
                        //Check if we have a study to match theprocedure
                        
                            sr = new StudyReport();
                        sr.VerificationDate = cr.VerificationDate.Value;
                        sr.Studies.Add(study);
                        sr.ReportingDoctor = GetDoctor(cr.ReportingDoctorCode);

                        sr.VerifyingDoctor = GetDoctor(cr.VerifierCode);
                        sr.DictatingDoctor = GetDoctor(cr.DictPersCode);
                        sr.Studies.Add(study);
                        sr.ReportDocument = new DataModel.Common.File();
                        sr.ReportDocument.Description = "Report for Study " + study.ID.ToString();
                        sr.ReportDocument.Extension = ".rtf";
                        string rtf = "";
                        foreach(CentricityReportText crt in cr.ReportTexts.OrderBy(x => x.Sequence))
                        {
                            rtf = rtf + crt.Text;
                        }
                        sr.ReportDocument.Data = System.Text.Encoding.UTF8.GetBytes(rtf);
                        retriever.SaveItem(sr);
                        result.Add(sr);
                        logger.Info("Created Report " + sr.ID);

                        SqlCommand insertCommand = new SqlCommand("INSERT INTO dbo.ReportBridges (ForeignKey,LocalID, ProcedureKey) VALUES (@ForeignKey, @LocalID, @ProcedureKey)", iCon);

                        insertCommand.Parameters.AddWithValue("@ForeignKey", cr.ReportKey);
                        insertCommand.Parameters.AddWithValue("@LocalID", sr.ID);
                        insertCommand.Parameters.AddWithValue("@ProcedureKey", cr.ProcedureKey);
                        if (insertCommand.ExecuteNonQuery() > 0)
                        {
                            logger.Info("Report bridge inserted into system.");
                        }
                        else
                        {
                            logger.Error("Error, insert report bridge did not return > 0 rows.");

                        }




                    }
                    
                    
                }
            }
            
            return result;
        }

        /// <summary>
        /// Returns a Doctor based on a Centricity staff member code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Doctor GetDoctor(string code)
        {
            SqlConnection iCon = new SqlConnection();
            iCon.ConnectionString = Properties.Settings.Default.InterfaceConnString;
            iCon.Open();

            string qry = "SELECT * FROM DoctorBridges WHERE ForeignCode = '" + code +"'";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = qry;
            cmd.Connection = iCon;
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                Doctor doc = retriever.RetrieveItem(Convert.ToInt32(dr["DoctorID"]), typeof(Doctor)) as Doctor;
                return doc;
            }
            else
            {
                dr.Close();
                OracleConnection oCon = new OracleConnection();
                oCon.ConnectionString = Properties.Settings.Default.CentricityConnString;
                oCon.Open();

                qry = "SELECT * FROM A_US_BD_STAFF_MEMBER WHERE STAFF_MEMB_CODE = '" + code + "'";
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandText = qry;
                oCmd.Connection = oCon;
                OracleDataReader odr = oCmd.ExecuteReader();
                if (odr.HasRows)
                {
                    odr.Read();
                    CentricityStaffMember csm = GetStaffMemberFromDataReader(odr);

                    Doctor d = new Doctor();
                    d.Surname = csm.LastName;
                    d.GivenNames = csm.FirstName;
                    d.Title = "Dr";
                    retriever.SaveItem(d);
                    odr.Close();

                    SqlCommand insertCommand = new SqlCommand("INSERT INTO dbo.DoctorBridges (ForeignKey, DoctorID, ForeignCode) VALUES (@ForeignKey, @DoctorID, @ForeignCode)", iCon);

                    insertCommand.Parameters.AddWithValue("@ForeignKey", "x");
                    insertCommand.Parameters.AddWithValue("@DoctorID", d.ID);
                    insertCommand.Parameters.AddWithValue("@ForeignCode", csm.Code);
                    if (insertCommand.ExecuteNonQuery() > 0)
                    {
                        logger.Info("Report bridge inserted into system.");
                    }
                    else
                    {
                        logger.Error("Error, insert report bridge did not return > 0 rows.");
                        //Dont bother logging this, it occurs when the ward is not listed in the bridges
                    }
                   
                    return d;
                }
                else
                {
                    return null;
                }

                
            }
            
        }

        public void ExecuteCommand(string command)
        {
            switch (command)
            {
                case "CurrentBookings":
                    //Get today's reports (also yesterdays)
                    List<Appointment> result = GetAppointments(DateTime.Today, DateTime.Today.AddDays(10));
                    logger.Trace("Get appointments completed");
                    break;
                case "ReportsToday":
                    //Get bookings for today and the next 10 days
                    List<StudyReport> reports = GetReports(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
                    logger.Trace("Get reports completed");
                    break;


            }
        }
       
        #endregion


    }

    
}
