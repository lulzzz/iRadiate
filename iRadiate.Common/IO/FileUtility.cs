using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRadiate.Common.IO
{
    public class FileUtility
    {
        public static string DataDirectory
        {
            get
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    return ApplicationDeployment.CurrentDeployment.DataDirectory;
                }
                else
                {
                    return AppDomain.CurrentDomain.BaseDirectory;
                }
                
            }
        }
    }
}
