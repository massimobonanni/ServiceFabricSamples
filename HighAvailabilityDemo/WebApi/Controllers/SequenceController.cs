using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ServiceFabric.Actors.Client;
using SequenceActor.Interfaces;
using WebApi.Response.SequenceController;

namespace WebApi.Controllers
{
    [ServiceRequestActionFilter]
    public class SequenceController : ApiController
    {
        [Route("api/sequences/{sequenceId}")]
        [HttpGet]
        public async Task<GetNextSequenceResponse> GetNextSequence(string sequenceId)
        {

            var proxy = ActorProxy.Create<ISequenceActor>(new Microsoft.ServiceFabric.Actors.ActorId(sequenceId),
                "fabric:/HighAvailabilityDemo", "SequenceActorService");

            var actorResponse = await proxy.GetNextSequenceAsync();

            var response = new GetNextSequenceResponse()
            {
                ActorId = sequenceId,
                NodeInfo = actorResponse.NodeInfo,
                Value = actorResponse.Value,
                PackageVersion = actorResponse.PackageVersion
            };

            return response;
        }


    }
}
