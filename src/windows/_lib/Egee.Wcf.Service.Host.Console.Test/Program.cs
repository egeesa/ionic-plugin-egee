using Egee.Wcf.Service.Host.Console.Test.Sappel;
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
            SappelServiceClient sappelServiceClient = new SappelServiceClient("BasicHttpBinding_ISappelService");

            InitResponse initResponse = sappelServiceClient.InitAsync(new InitRequest()).Result;
            GetVersionResponse getVersionResponse = sappelServiceClient.GetVersionAsync(new GetVersionRequest()).Result;
            sappelServiceClient.Close();

            System.Console.WriteLine(getVersionResponse.GetVersionResult);
            System.Console.ReadLine();
        }
    }
}
