using System;
using System.Collections.Generic;
using AffidoActor;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class AffidoActorTest
    {
        [TestMethod]
        public void TakeInCharge_ReturnFalseIsODLIsNotInAffido()
        {
            var stateManager = new Mocks.MockActorStateManager();

            var idOdl = "333";

            var stato = new AffidoState();
            stato.OdlList = new List<Core.Infos.OdlInfo>()
            {
                new Core.Infos.OdlInfo() {Id = "111"},
                new Core.Infos.OdlInfo() {Id = "222"}
            };

            var target = new AffidoActor.AffidoActor(stateManager);
            var state = target.SetStateAsync(stato);

            var actual = target.TakeInCharge(idOdl).Result ;

            Assert.AreEqual(actual, false);
        }
    }
}
