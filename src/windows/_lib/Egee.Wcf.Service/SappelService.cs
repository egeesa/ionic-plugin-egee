using Egee.Wcf.Service.Interface;
using IZAR_CSIXLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egee.Wcf.Service
{
    public class SappelService : ISappelService
    {
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
