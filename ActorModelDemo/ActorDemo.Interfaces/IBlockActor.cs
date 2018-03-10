using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using ActorReference = ActorModelDemo.Core.ActorReference;

namespace ActorDemo.Interfaces
{
    public interface IBlockActor : IActor
    {
        Task<string> DoLongTimeOperationAsync(string operationPayload, 
            CancellationToken cancellationToken = default(CancellationToken));

    }
}
