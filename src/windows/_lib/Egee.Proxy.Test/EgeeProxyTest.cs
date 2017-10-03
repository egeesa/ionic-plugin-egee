using System;
//using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Xunit;

namespace Egee.Proxy.Test
{
    public class EgeeProxyTest
    {
        [Fact]
        public void TestMethod1()
        {
            string version = EgeeProxy.getversion();
            Assert.NotNull(version);
        }
    }
}
