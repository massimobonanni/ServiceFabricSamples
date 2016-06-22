using System;
using System.Fabric;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocks;

namespace AnagraficaService.Test
{
    [TestClass]
    public class AnagraficaServiceTest
    {
        private StatefulServiceContext CreateServiceContext()
        {
            return new StatefulServiceContext(
                new NodeContext(string.Empty, new NodeId(0, 0), 0, string.Empty, string.Empty),
                new MockCodePackageActivationContext(),
                String.Empty,
                new Uri("fabric:/Mock"),
                null,
                Guid.NewGuid(),
                0);
        }

        [TestMethod]
        public void GetFrazionariAsync_ReturnListOfFrazionari()
        {
            MockReliableStateManager stateManager = new MockReliableStateManager();


            var target = new AnagraficaService(CreateServiceContext(),
                stateManager, null,null);


            var result = target.GetFrazionariAsync().Result;

            Assert.IsTrue(result.Any());
        }
    }
}
