
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Egee.Proxy.Test.Sappel;

namespace Egee.Proxy.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SappelGetVresion()
        {
            string version = EgeeProxy.getversion();
            Assert.IsNotNull(version);
        }

        [TestMethod]
        public void WcfConsume()
        {
            //string result = EgeeProxy.testWcf();
            SappelServiceClient sappelServiceClient = new SappelServiceClient(SappelServiceClient.EndpointConfiguration.BasicHttpBinding_ISappelService);
            InitResponse initResponse = sappelServiceClient.InitAsync(new InitRequest()).Result;
            GetVersionResponse getVersionResponse = sappelServiceClient.GetVersionAsync(new GetVersionRequest()).Result;
            Assert.IsNotNull(getVersionResponse.GetVersionResult);
        }
    }
}
