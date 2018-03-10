using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ActorDemo.Interfaces
{
    public interface IBlockActor : IActor
    {
        Task<string> DoLongTimeOperationAsync(string operationPayload, 
            CancellationToken cancellationToken = default(CancellationToken));

    }
}
