using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using CartActor.Interfaces;

namespace CartActor
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class CartActor : Actor, ICartActor, IRemindable
    {

        public CartActor(ActorService actorService, ActorId actorId)
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
        internal const string ExpiredReminderName = "ExpiredReminder";

        internal TimeSpan CartExpiredTimeout = TimeSpan.FromMinutes(5);

        #region [ Internal state manager methods ]
        private async Task<State> GetStateFromStateManagerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = await this.StateManager.TryGetStateAsync<State>(StateKeyName, cancellationToken);
            return data.HasValue ? data.Value : State.Initial;
        }
        private Task SetStateIntoStateManagerAsync(State state, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.StateManager.SetStateAsync<State>(StateKeyName, state, cancellationToken);
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

        #region [ Interface ICartActor]
        public async Task<CartError> CreateAsync(CancellationToken cancellationToken)
        {
            var currentStatus = await GetStateFromStateManagerAsync(cancellationToken);

            if (currentStatus == State.Initial)
            {
                await SetStateIntoStateManagerAsync(State.Create, cancellationToken);
                await this.RegisterReminderAsync(ExpiredReminderName, null, CartExpiredTimeout,
                    TimeSpan.FromMilliseconds(-1));

                return CartError.Ok;
            }

            return CartError.GenericError;
        }

        public Task<CartError> AddProductAsync(string productId, double quantity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        

        public Task<CartError> CreateOrderAsync(List<ProductInfo> products, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion [ Interface ICartActor ]

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == ExpiredReminderName)
            {
                var currentState = await GetStateFromStateManagerAsync();
                if (currentState == State.Initial)
                    await SetStateIntoStateManagerAsync(State.Expire);
            }

        }
    }
}
