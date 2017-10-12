using SappelWindowsRuntimeComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.ApplicationModel.AppService;
using IZAR_CSIXLib;
using Egee.Proxy.Sappel;
using static Egee.Proxy.Sappel.SappelServiceClient;

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
            HyScript hyScript = new HyScript();
            string initResponse = hyScript.call("init");
            string licenseResponse = hyScript.call("return License.check('EGEEEGEE','I1ARCQI0')");
            string version = hyScript.call("return Environment.getVersion()");
            return version;
            //SappelWrapper wrapper = new SappelWrapper();
            //return wrapper.Test();
        }

        public async static string testWcf()
        {
            //ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
            //Task<string> response = client.GetDataAsync(3);
            //string result = response.Result;
            SappelServiceClient sappelServiceClient = new SappelServiceClient(EndpointConfiguration.BasicHttpBinding_ISappelService);
            InitResponse initResponse = await sappelServiceClient.InitAsync(new InitRequest());

            return "TODO";
        }
    }
}