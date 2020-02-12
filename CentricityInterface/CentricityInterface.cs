using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLog;

using iRadiate.Common;

namespace iRadiate.Interfaces.CentricityInterface
{
    public class CentricityInterface : IExternalDataInterface
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public string Name
        {
            get
            {
                return "Centricity";
            }
        }

        public void Execute(string command)
        {
            CentricityConnector cc;
            try
            {
                cc = new CentricityConnector();
            }
            catch (Exception ex)
            {

                logger.Error(ex, "Caugt exception on the constructor " + ex.Message);
                return;
            }
            logger.Info("Launching Centricity Connector with command: " + command);
            cc.ExecuteCommand(command);
            logger.Info("Centricity Connector completed: " + command);
        }
    }
}
