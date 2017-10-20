using IZAR_CSIXLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Egee.API.Service.Host
{
    public class SappelController : ApiController
    {
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        public void Init()
        {
            HyScript hyScript = new HyScript();
            string initResponse = hyScript.call("init");
            string licenseResponse = hyScript.call("return License.check('EGEEEGEE','I1ARCQI0')");
        }

        public string GetVersion()
        {
            HyScript hyScript = new HyScript();
            return hyScript.call("return Environment.getVersion()");
        }
    }
}
