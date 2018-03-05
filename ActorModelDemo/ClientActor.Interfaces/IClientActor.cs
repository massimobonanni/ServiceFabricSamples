using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;


namespace ClientActor.Interfaces
{

    public interface IClientActor : IActor
    {

        Task<string> GetStatusAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task ExecuteOperationAsync(string operationPayload, CancellationToken cancellationToken = default(CancellationToken));
    }
}
