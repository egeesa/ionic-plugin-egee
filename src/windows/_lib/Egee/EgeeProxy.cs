using SappelWindowsRuntimeComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Wrapper.wInit();
            string version = Wrapper.wGetVersion();
            return version;
        }

    }
}