using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.ApplicationModel.AppService;
using Egee.Proxy.Sappel;
using Windows.Management.Deployment;
//using MetroLog;
//using MetroLog.Targets;

namespace Egee.Proxy
{
    public sealed class EgeeProxy
    {
        public static string helloworld(string message)
        {
            return "Salut " + message;
        }

        public static string getversion()
        {
            //HyScript hyScript = new HyScript();
            //string initResponse = hyScript.call("init");
            //string licenseResponse = hyScript.call("return License.check('EGEEEGEE','I1ARCQI0')");
            //string version = hyScript.call("return Environment.getVersion()");
            //return version;
            //SappelWrapper wrapper = new SappelWrapper();
            //return wrapper.Test();
            return null;
        }

        public static string testwcf()
        {
            //LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new StreamingFileTarget());
            //ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<EgeeProxy>();

            //log.Trace("1 - Création client WCF");

            //ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
            //Task<string> response = client.GetDataAsync(3);
            //string result = response.Result;
            SappelServiceClient sappelServiceClient = new SappelServiceClient(SappelServiceClient.EndpointConfiguration.BasicHttpBinding_ISappelService);

            //log.Trace("2");

            InitResponse initResponse = sappelServiceClient.InitAsync(new InitRequest()).Result;
            //log.Trace("3");

            GetVersionResponse getVersionResponse = sappelServiceClient.GetVersionAsync(new GetVersionRequest()).Result;
            //log.Trace("4");

            return getVersionResponse.GetVersionResult;
        }
    }
}