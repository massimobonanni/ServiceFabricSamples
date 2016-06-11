using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AffidoActor.Interfaces;
using Core.Infrastructure;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using WebApi.Core.Requests.Flows;
using WebApi.Core.Responses.Flows;

namespace WebApi.Controllers
{
    [RoutePrefix("")]
    public class FlowsController : ApiControllerBase
    {
        protected readonly IActorFactory ActorFactory;

        public FlowsController(IActorFactory actorFactory)
        {
            this.ActorFactory = actorFactory;
        }


        [Route("api/customers/{idCustomer}/flows/{idFlow}/takeincharge/{idOdl}")]
        [HttpGet]
        public async Task<TakeInChargeResponse> TakeInCharge([FromUri] string idCustomer,
            [FromUri] string idFlow, [FromUri] string idOdl,
            [FromUri] TakeInChargeRequest request)
        {
            if (String.Compare(idCustomer, "EQT", StringComparison.OrdinalIgnoreCase) != 0)
                ThrowHttpResponseException(System.Net.HttpStatusCode.NotFound,
                    "Customer inesistente");

            var actor = ActorFactory.Create<IAffidoActor>(new ActorId(idFlow),
                           new System.Uri("fabric:/ServiceIoC/AffidoActorService"));

            var result = await actor.TakeInCharge(idOdl);

            var response = new TakeInChargeResponse() { IsSuccess = result };

            return response;
        }

        // Implementazioni delle action
    }
}


