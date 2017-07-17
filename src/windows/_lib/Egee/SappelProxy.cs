using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IZAR_CSIXLib;

namespace Egee.Proxy
{
    public  class SappelProxy
    {
        public  string getVersion()
        {
            return call("return Environment.getVersion()");
        }

        private  string call(string commande)
        {           
            HyScript api = new HyScript();
            string result = api.call(commande);
            return result;
        }
    }
}
