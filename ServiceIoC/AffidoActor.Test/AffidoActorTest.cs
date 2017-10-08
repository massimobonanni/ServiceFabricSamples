using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AffidoActor;
using AffidoActor.Interfaces;
using Core.Infrastructure;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdlActor.Interfaces;
using Rhino.Mocks;
using ServiceFabric.Mocks;

namespace Test
{
    [TestClass]
    public class AffidoActorTest
    {
        [TestMethod]
        public void TakeInCharge_ReturnFalseIfODLIsNotInAffido()
        {
            var idOdl = "333";

            var stateManager = new ServiceFabric.Mocks.MockActorStateManager();
            var actorService = ServiceFabric.Mocks.MockActorServiceFactory.CreateActorServiceForActor<AffidoActor.AffidoActor>();

            var stato = new AffidoState();
            stato.OdlList = new List<Core.Infos.OdlInfo>()
            {
                new Core.Infos.OdlInfo() {Id = "111"},
                new Core.Infos.OdlInfo() {Id = "222"}
            };

            var target = new AffidoActor.AffidoActor(actorService, new ActorId("TestActor"), null, null, null, stateManager);
            target.SetStateAsync(stato).GetAwaiter().GetResult();

            var actual = target.TakeInCharge(idOdl).Result;

            Assert.AreEqual(actual, false);
        }

        [TestMethod]
        public void TakeInCharge_ReturnTrueIfODLIsInAffidoAndOdlIsInitialState()
        {
            var idOdl = "333";

            var stateManager = new ServiceFabric.Mocks.MockActorStateManager();
            var actorFactory = MockRepository.GenerateStub<IActorFactory>();
            var actor = MockRepository.GenerateStub<IOdlActor>();

            var actorService = ServiceFabric.Mocks.MockActorServiceFactory.CreateActorServiceForActor<AffidoActor.AffidoActor>();

            actorFactory.Stub(x => x.Create<IOdlActor>(new ActorId(idOdl), serviceName: "fabric:/ServiceIoC/OdlActorService")).Return(actor);
            actor.Stub(x => x.TakeInCharge()).Return(Task.FromResult(true));

            var stato = new AffidoState();
            stato.OdlList = new List<Core.Infos.OdlInfo>()
            {
                new Core.Infos.OdlInfo() {Id = "111"},
                new Core.Infos.OdlInfo() {Id = "222"},
                new Core.Infos.OdlInfo() {Id = "333"}
            };

            var target = new AffidoActor.AffidoActor(actorService,new ActorId("testActor"),
                actorFactory, null,null, stateManager);

            target.SetStateAsync(stato).GetAwaiter().GetResult();

            var actual = target.TakeInCharge(idOdl).Result;

            Assert.AreEqual(actual, true);
        }
    }
}
