using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace CartActor.Interfaces
{
    public interface ICartActor : IActor
    {

        Task<CartError> CreateAsync(CancellationToken cancellationToken);

        Task<CartError> AddProductAsync(string productId, double quantity, CancellationToken cancellationToken);

        Task<CartError> CreateOrderAsync(List<ProductInfo> products, CancellationToken cancellationToken);


    }
}
