using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using OrderActor.Interfaces;

namespace OrderActor
{

    [StatePersistence(StatePersistence.Persisted)]
    [ActorService(Name = "OrderActor")]
    internal class OrderActor : Actor, IOrderActor
    {

        public OrderActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }


        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            return Task.CompletedTask;
        }

        internal const string StateKeyName = "State";
        internal const string ProductKeyNamePrefix = "Product_";

        #region [ Internal state manager methods ]
        private async Task<State> GetStateFromStateManagerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = await this.StateManager.TryGetStateAsync<State>(StateKeyName, cancellationToken);
            return data.HasValue ? data.Value : State.Initial;
        }
        private Task SetStateIntoStateManagerAsync(State state, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.StateManager.SetStateAsync(StateKeyName, state, cancellationToken);
        }

        private string GenerateProductKey(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                throw new ArgumentException(nameof(productId));

            return $"{ProductKeyNamePrefix}{productId}";
        }

        private async Task<IEnumerable<string>> GetProductKeysAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await this.StateManager.GetStateNamesAsync(cancellationToken)).Where(k => k.StartsWith(ProductKeyNamePrefix));
        }

        private async Task AddOrUpdateProductIntoStateManagerAsync(ProductData data,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var productKey = GenerateProductKey(data.Id);
            var existingProduct = await this.StateManager.TryGetStateAsync<ProductData>(productKey, cancellationToken);
            if (existingProduct.HasValue)
            {
                data.Quantity += existingProduct.Value.Quantity;
            }

            await this.StateManager.SetStateAsync<ProductData>(productKey, data, cancellationToken);
        }

        #endregion [ Internal state manager methods ]

        #region [ IOrderActor interface ]
        public async Task<OrderError> CreateAsync(List<ProductInfo> products, CancellationToken cancellationToken)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            var currentStatus = await GetStateFromStateManagerAsync(cancellationToken);

            if (currentStatus == State.Initial)
            {
                foreach (var product in products)
                {
                    var prodData = new ProductData(product);
                    await AddOrUpdateProductIntoStateManagerAsync(prodData, cancellationToken);
                }

                await SetStateIntoStateManagerAsync(State.Create, cancellationToken);
            }

            return OrderError.GenericError;
        }
        #endregion [ IOrderActor interface ]


    }
}
