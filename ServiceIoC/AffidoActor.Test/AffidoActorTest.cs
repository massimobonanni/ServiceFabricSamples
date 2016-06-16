using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AffidoActor;
using Core.Infrastructure;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdlActor.Interfaces;
using Rhino.Mocks;

namespace Test
{
    [TestClass]
    public class AffidoActorTest
    {
        [TestMethod]
        public void TakeInCharge_ReturnFalseIsODLIsNotInAffido()
        {
            var idOdl = "333";

            var stateManager = new Mocks.MockActorStateManager();

            var stato = new AffidoState();
            stato.OdlList = new List<Core.Infos.OdlInfo>()
            {
                new Core.Infos.OdlInfo() {Id = "111"},
                new Core.Infos.OdlInfo() {Id = "222"}
            };

            var target = new AffidoActor.AffidoActor(stateManager, null, null);
            target.SetStateAsync(stato).GetAwaiter().GetResult();

            var actual = target.TakeInCharge(idOdl).Result;

            Assert.AreEqual(actual, false);
        }

        [TestMethod]
        public void TakeInCharge_ReturnTrueIsODLIsInAffidoAndOdlIsInitialState()
        {
            var idOdl = "333";

            var stateManager = new Mocks.MockActorStateManager();
            var actorFactory = MockRepository.GenerateStub<IActorFactory>();
            var actor = MockRepository.GenerateStub<IOdlActor>();

            actorFactory.Stub(x => x.Create<IOdlActor>(new ActorId(idOdl))).Return(actor);
            actor.Stub(x => x.TakeInCharge()).Return(Task.FromResult(true));


            var stato = new AffidoState();
            stato.OdlList = new List<Core.Infos.OdlInfo>()
            {
                new Core.Infos.OdlInfo() {Id = "111"},
                new Core.Infos.OdlInfo() {Id = "222"},
                new Core.Infos.OdlInfo() {Id = "333"}
            };

            var target = new AffidoActor.AffidoActor(stateManager, actorFactory, null);
            target.SetStateAsync(stato).GetAwaiter().GetResult();

            var actual = target.TakeInCharge(idOdl).Result;

            Assert.AreEqual(actual, true);
        }
    }
}
