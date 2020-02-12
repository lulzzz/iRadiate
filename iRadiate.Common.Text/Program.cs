using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRadiate.Common.IO;
using iRadiate.DataModel.HealthCare;
using iRadiate.Common.Authentication;
using iRadiate.DataModel.Common;
using iRadiate.DataModel.NucMed;

namespace iRadiate.Common.Text
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime appStartTime = DateTime.Now;
            Console.WriteLine("Program commenced @ " + appStartTime.ToShortTimeString());
            
            StandardDataLibrarian lib = new StandardDataLibrarian();
            Console.WriteLine("Libarian instantiated");
            printRetriever();
            //List<RetrievalCriteria> rc = new List<RetrievalCriteria>();
            //RetrievalCriteria first = new RetrievalCriteria("CreationDate", CriteraType.GreaterThan, DateTime.Today.Date);
            //rc.Add(first);
            //RetrievalCriteria second = new RetrievalCriteria("CreationDate", CriteraType.LessThan, DateTime.Today.AddDays(1));
            //rc.Add(second);
            //var result = Platform.Retriever.RetrieveItems(typeof(Appointment), rc);
            
            var result = (lib as IDataLibrarian).GetAppointments(DateTime.Today);

            
            printRetriever();
            EFDataRetriever r = (EFDataRetriever)Platform.Retriever;
            r.printAllUnmodified();
            #region ignore
            //RetrievalCriteria rc1 = new RetrievalCriteria("ID", CriteraType.Equals, 1);
            //List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();
            //rcList.Add(rc1);
            //User u = (User)Platform.Retriever.RetrieveItems(typeof(User), rcList).First();
            //Console.WriteLine("User 1 retrieved");
            //printRetriever();
            //string password = "passwod";
            //Console.WriteLine("Password = " + password);
            //string hash = Authenticator.HashPassword(password);
            //Console.WriteLine("Hash = " + hash);
            //hash = Authenticator.HashPassword(password);
            //Console.WriteLine("Hash = " + hash);
            //hash = Authenticator.HashPassword(password);
            //Console.WriteLine("Hash = " + hash);
            #endregion
            DateTime appEndTime = DateTime.Now;
            Console.WriteLine("Program finished @ " + appEndTime.ToShortTimeString() + "; completed in " + (appEndTime - appStartTime).TotalSeconds.ToString("F1") + " seconds");
            Console.ReadLine();
        }

        static void printRetriever()
        {
            Console.WriteLine("Retrieved Items = " + Platform.Retriever.NumberOfItemsRetrieved + "; Tracked items = " + Platform.Retriever.TotalItemsTracked + "; Modified items = " + Platform.Retriever.NumberOfModifiedItems);
        }
    }
}
