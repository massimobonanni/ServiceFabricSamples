using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Core.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using AffidoActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using WebApi.Controllers;

namespace WebApi.Test
{
    [TestClass]
    public class FlowsControllerTest
    {
        [TestMethod]
        public void TakeInCharge_Return404WhenCustomerIsNotEquitalia()
        {
            var actorFactory = MockRepository.GenerateStub<IActorFactory>();
            var actor = MockRepository.GenerateStub<IAffidoActor>();

            var idCustomer = "altro";
            var idFlow = "111";
            var idOdl = "111";

            var target = new FlowsController(actorFactory);

            try
            {
                var response = target.TakeInCharge(idCustomer, idFlow, idOdl, null).GetAwaiter().GetResult();
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.NotFound);
            }

        }

        [TestMethod]
        public void TakeInCharge_ReturnErrorResponseWhenActorReturnFalse()
        {
            // ARRANGE
            var actorFactory = MockRepository.GenerateStub<IActorFactory>();
            var actor = MockRepository.GenerateStub<IAffidoActor>();

            var idCustomer = "eqt";
            var idFlow = "111";
            var idOdl = "111";

            actorFactory.Stub(x => x.Create<IAffidoActor>(new ActorId(idFlow),
                new System.Uri("fabric:/ServiceIoC/AffidoActorService"))).Return(actor);

            actor.Stub(x => x.TakeInCharge(idOdl)).Return(Task.FromResult(false));

            var target = new FlowsController(actorFactory);

            // ACT
            var response = target.TakeInCharge(idCustomer, idFlow, idOdl, null).Result;

            // ASSERT
            Assert.IsFalse(response.IsSuccess);
        }
    }

}
