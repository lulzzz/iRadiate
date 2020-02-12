using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

using NLog;

using iRadiate.Common.IO;
using iRadiate.DataModel.Common;


namespace iRadiate.Common.Authentication
{
    public class Authenticator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

       public static User AutheticateUser(string username, string password)
        {
           
            List<RetrievalCriteria> rcList = new List<RetrievalCriteria>();

            RetrievalCriteria rc = new RetrievalCriteria("LoginName", CriteraType.ExactTextMatch, username);
            rcList.Add(rc);

            if (Platform.Retriever.RetrieveItems(typeof(User), rcList).Any())
            {
               
                User u = (User)Platform.Retriever.RetrieveItems(typeof(User), rcList).First();
                string hashed = HashPassword(password);
                if (u.Password == hashed)
                {
                    
                    Properties.Settings.Default.LastLoginName = username;
                    Properties.Settings.Default.Save();
                    
                    logger.Info("User authenticated");
                    return u;

                }
                else
                {
                    return null;
                }
            }
            else
            {
                
                return null;
            }
           
        }

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            //new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
    }


}
