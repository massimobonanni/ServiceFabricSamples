using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using CartActor.Interfaces;
using System.Data.SqlClient;
using System.Data;
using System.Fabric.Description;
using OrderActor.Interfaces;
using ProductInfo = CartActor.Interfaces.ProductInfo;
using Core.Interfaces;

namespace CartActor
{
    [StatePersistence(StatePersistence.Persisted)]
    [ActorService(Name = "CartActor")]
    internal class CartActor : Core.Implementations.ActorBase, ICartActor, IRemindable
    {

        public CartActor(ActorService actorService, ActorId actorId,
            IActorFactory actorFactory, IServiceFactory serviceFactory,
            IProductsService productsService)
            : base(actorService, actorId, actorFactory, serviceFactory)
        {
            if (productsService == null)
                throw new ArgumentNullException(nameof(productsService));

            this.productsService = productsService;
        }

        private readonly IProductsService productsService;

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            var config = this.ActorService.Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            ReadSettings(config.Settings);
            this.ActorService.Context.CodePackageActivationContext.ConfigurationPackageModifiedEvent += ConfigurationPackageModifiedEvent;

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

        public async Task<CartError> AddProductAsync(string productId, double quantity, CancellationToken cancellationToken)
        {
            if (productId == null)
                throw new ArgumentNullException(nameof(productId));

            var currentStatus = await GetStateFromStateManagerAsync(cancellationToken);

            if (currentStatus == State.Create)
            {
                var productData = await GetProductFromStorageAsync(productId, quantity, cancellationToken);
                if (productData != null)
                {
                    await AddOrUpdateProductIntoStateManagerAsync(productData, cancellationToken);
                    return CartError.Ok;
                }
            }

            return CartError.GenericError;
        }

        public async Task<CartError> CreateOrderAsync(CancellationToken cancellationToken)
        {
            var currentStatus = await GetStateFromStateManagerAsync(cancellationToken);

            if (currentStatus == State.Create)
            {
                var productList = await CreateProductListForOrderAsync(cancellationToken);
                if (productList != null && productList.Any())
                {
                    OrderError createResult = OrderError.GenericError;

                    var orderProxy = this.actorFactory.Create<IOrderActor>(this.Id,
                        new Uri("fabric:/TestingApp/OrderActor"));

                    try
                    {
                        createResult = await orderProxy.CreateAsync(productList, cancellationToken);
                    }
                    catch
                    {
                        createResult = OrderError.GenericError;
                    }

                    if (createResult == OrderError.Ok)
                    {
                        await SetStateIntoStateManagerAsync(State.Close, cancellationToken);
                        return CartError.Ok;
                    }
                }
            }

            return CartError.GenericError;
        }

        #endregion [ Interface ICartActor ]

        #region [ Interface IRemindable ]
        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == ExpiredReminderName)
            {
                var currentState = await GetStateFromStateManagerAsync();
                if (currentState == State.Initial || currentState == State.Create)
                    await SetStateIntoStateManagerAsync(State.Expire);
            }

        }
        #endregion [ Interface IRemindable ]

        #region [ Private methods ]
        private Task<ProductData> GetProductFromStorageAsync(string productId, double quantity, CancellationToken cancellationToken)
        {
            return this.productsService.GetProductInfoAsync(productId, quantity, cancellationToken);
        }

        private async Task<List<OrderActor.Interfaces.ProductInfo>> CreateProductListForOrderAsync(CancellationToken cancellationToken)
        {
            List<OrderActor.Interfaces.ProductInfo> productList = null;
            var productKeys = await GetProductKeysAsync(cancellationToken);
            if (productKeys.Any())
            {
                productList = new List<OrderActor.Interfaces.ProductInfo>();
                foreach (var productKey in productKeys)
                {
                    var product = await this.StateManager.GetStateAsync<ProductData>(productKey, cancellationToken);
                    productList.Add(new OrderActor.Interfaces.ProductInfo()
                    {
                        Id = product.Id,
                        Quantity = product.Quantity,
                        UnitCost = product.UnitCost
                    });
                }
            }

            return productList;
        }
        #endregion [ Private methods ]

        #region [ Configuration ]
        private void ConfigurationPackageModifiedEvent(object sender, System.Fabric.PackageModifiedEventArgs<System.Fabric.ConfigurationPackage> e)
        {
            ReadSettings(e.NewPackage.Settings);
        }

        private void ReadSettings(ConfigurationSettings settings)
        {
            this.productsService.SetConfiguration(settings);
        }
        #endregion [ Configuration ]
    }
}
