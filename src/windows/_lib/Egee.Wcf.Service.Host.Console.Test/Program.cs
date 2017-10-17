using Egee.Wcf.Service.Host.Console.Test.Sappel;
using MetroLog;
using MetroLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egee.Wcf.Service.Host.Console.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new StreamingFileTarget());
            ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<Program>();

            log.Trace("1 - Création client WCF");

            SappelServiceClient sappelServiceClient = new SappelServiceClient("BasicHttpBinding_ISappelService");

            InitResponse initResponse = sappelServiceClient.InitAsync(new InitRequest()).Result;
            GetVersionResponse getVersionResponse = sappelServiceClient.GetVersionAsync(new GetVersionRequest()).Result;
            sappelServiceClient.Close();

            System.Console.WriteLine(getVersionResponse.GetVersionResult);
            System.Console.ReadLine();
        }
    }
}
