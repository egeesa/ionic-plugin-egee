
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            string result = EgeeProxy.testWcf();
            Assert.IsNotNull(result);
        }
    }
}
