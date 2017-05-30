using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egee.Proxy
{
    public sealed class EgeeProxy
    {
        public static string HelloWorld(string message)
        {
            return "Salut " + message;
        }
    }
}