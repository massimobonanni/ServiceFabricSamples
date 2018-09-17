using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CartActor.Interfaces;
using Core.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OrderActor.Interfaces;
using ServiceFabric.Mocks;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CartActor.Test
{
    public class CartActorTest
    {
        internal static CartActor CreateActor(ActorId id, IActorFactory actorFactory = null,
            IServiceFactory serviceFactory = null, IProductsService productsService = null)
        {
            if (actorFactory == null)
                actorFactory = new Mock<IActorFactory>().Object;
            if (serviceFactory == null)
                serviceFactory = new Mock<IServiceFactory>().Object;
            if (productsService == null)
                productsService = new Mock<IProductsService>().Object;

            Func<ActorService, ActorId, ActorBase> actorInstanceFactory =
                (service, actorId) => new CartActor(service, id, actorFactory, serviceFactory, productsService);

            var codePackage = MockCodePackageActivationContext.Default;
            var configSection = MockConfigurationPackage.CreateConfigurationSection("SqlDataAccess");
            var configSettings = MockConfigurationPackage.CreateConfigurationSettings(
                    new MockConfigurationPackage.ConfigurationSectionCollection() { configSection });
            (codePackage as MockCodePackageActivationContext).ConfigurationPackage = MockConfigurationPackage.CreateConfigurationPackage(configSettings, "Config");

            var context = MockStatefulServiceContextFactory.Create(codePackage, "CartActorType", new Uri("fabric:/TestingApp/CartActor"), Guid.NewGuid(), 0);

            var svc = MockActorServiceFactory.CreateActorServiceForActor<CartActor>(actorInstanceFactory, null, context);
            var actor = svc.Activate(id);
            return actor;
        }

        #region [ CreateAsync ]
        [Fact]
        public async Task CreateAsync_CartStateInitial_Ok()
        {
            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var actor = CreateActor(id);
            var stateManager = (MockActorStateManager)actor.StateManager;

            await actor.InvokeOnActivateAsync();

            var actual = await actor.CreateAsync(default(CancellationToken));

            Assert.AreEqual(actual, Interfaces.CartError.Ok);
            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, State.Create);
            var reminderCollection = actor.GetActorReminders();
            Assert.IsTrue(reminderCollection.Any(r => r.Name == CartActor.ExpiredReminderName));
        }

        [Theory]
        [InlineData(State.Create)]
        [InlineData(State.Expire)]
        [InlineData(State.Close)]
        public async Task CreateAsync_CartStateNotInitial_Error(object testState)
        {
            State initialState = (State)testState;
            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var actor = CreateActor(id);
            var stateManager = (MockActorStateManager)actor.StateManager;
            await stateManager.SetStateAsync<State>(CartActor.StateKeyName, initialState);

            await actor.InvokeOnActivateAsync();

            var actual = await actor.CreateAsync(default(CancellationToken));

            Assert.AreEqual(actual, Interfaces.CartError.GenericError);
            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, initialState);
        }


        #endregion [ CreateAsync ]

        #region [ ReceiveReminderAsync ]
        [Theory]
        [InlineData(State.Initial)]
        [InlineData(State.Create)]
        public async Task ReceiveReminderAsync_ExpiredReminder_CartStateInitial_CartStateBecameExpired(object testState)
        {
            State initialState = (State)testState;

            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var actor = CreateActor(id);
            var stateManager = (MockActorStateManager)actor.StateManager;
            await stateManager.SetStateAsync<State>(CartActor.StateKeyName, initialState);

            await actor.InvokeOnActivateAsync();

            await actor.ReceiveReminderAsync(CartActor.ExpiredReminderName, null, TimeSpan.Zero, TimeSpan.Zero);

            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, State.Expire);
        }

        [Theory]
        [InlineData(State.Expire)]
        [InlineData(State.Close)]
        public async Task ReceiveReminderAsync_ExpiredReminder_CartStateNotInitial_StateNotChange(object testState)
        {
            State initialState = (State)testState;

            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var actor = CreateActor(id);
            var stateManager = (MockActorStateManager)actor.StateManager;
            await stateManager.SetStateAsync<State>(CartActor.StateKeyName, initialState);

            await actor.InvokeOnActivateAsync();

            await actor.ReceiveReminderAsync(CartActor.ExpiredReminderName, null, TimeSpan.Zero, TimeSpan.Zero);

            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, initialState);
        }
        #endregion [ ReceiveReminderAsync ]

        #region [ AddProductAsync ]
        [Fact]
        public async Task AddProductAsync_ProductExists_ReturnOK()
        {
            var productId = "PRODUCT1";
            double quantity = 1;
            decimal unitCost = 10;
            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var productsService = new Mock<IProductsService>();
            productsService.Setup(s => s.GetProductInfoAsync(productId, quantity, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProductData() { Id = productId, Quantity = quantity, UnitCost = unitCost });

            var actor = CreateActor(id, null, null, productsService.Object);
            var stateManager = (MockActorStateManager)actor.StateManager;

            await actor.InvokeOnActivateAsync();
            await stateManager.SetStateAsync<State>(CartActor.StateKeyName, State.Create);

            var result = await actor.AddProductAsync(productId, quantity, default(CancellationToken));

            Assert.AreEqual(CartError.Ok, result);
            var productData = await stateManager.GetStateAsync<ProductData>($"{CartActor.ProductKeyNamePrefix}{productId}");
            Assert.IsNotNull(productData);
            Assert.AreEqual(quantity, productData.Quantity);
            Assert.AreEqual(unitCost, productData.UnitCost);
        }

        [Fact]
        public async Task AddProductAsync_ProductNotExists_ReturnGenericError()
        {
            var productId = "PRODUCT1";
            double quantity = 1;
            decimal unitCost = 10;
            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var productsService = new Mock<IProductsService>();
            productsService.Setup(s => s.GetProductInfoAsync(productId, quantity, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<ProductData>(null));

            var actor = CreateActor(id, null, null, productsService.Object);
            var stateManager = (MockActorStateManager)actor.StateManager;

            await actor.InvokeOnActivateAsync();
            await stateManager.SetStateAsync<State>(CartActor.StateKeyName, State.Create);

            var result = await actor.AddProductAsync(productId, quantity, default(CancellationToken));

            Assert.AreEqual(CartError.GenericError, result);
            var existsProductData = await stateManager.ContainsStateAsync($"{CartActor.ProductKeyNamePrefix}{productId}");
            Assert.IsFalse(existsProductData);
        }

        [Fact]
        public async Task AddProductAsync_ProductAlreadyAdded_ReturnOK()
        {
            var productId = "PRODUCT1";
            double quantity1 = 1;
            decimal unitCost1 = 10;
            double quantity2 = 1;
            decimal unitCost2 = 5;

            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var productsService = new Mock<IProductsService>();
            productsService.Setup(s => s.GetProductInfoAsync(productId, quantity1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProductData() { Id = productId, Quantity = quantity2, UnitCost = unitCost2 });

            var actor = CreateActor(id, null, null, productsService.Object);
            var stateManager = (MockActorStateManager)actor.StateManager;

            await actor.InvokeOnActivateAsync();
            await stateManager.SetStateAsync(CartActor.StateKeyName, State.Create);
            await stateManager.SetStateAsync($"{CartActor.ProductKeyNamePrefix}{productId}", new ProductData()
            {
                Id = productId,
                Quantity = quantity1,
                UnitCost = unitCost1
            });

            var result = await actor.AddProductAsync(productId, quantity2, default(CancellationToken));

            Assert.AreEqual(CartError.Ok, result);
            var productData = await stateManager.GetStateAsync<ProductData>($"{CartActor.ProductKeyNamePrefix}{productId}");
            Assert.IsNotNull(productData);
            Assert.AreEqual(quantity2 + quantity1, productData.Quantity);
            Assert.AreEqual(unitCost2, productData.UnitCost);
        }

        [Fact]
        public async Task AddProductAsync_ProductIdNull_ThrowException()
        {
            string productId = null;
            double quantity = 1;

            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var productsService = new Mock<IProductsService>();


            var actor = CreateActor(id, null, null, productsService.Object);
            var stateManager = (MockActorStateManager)actor.StateManager;

            await actor.InvokeOnActivateAsync();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => actor.AddProductAsync(productId, quantity, default(CancellationToken)));
        }
        #endregion [ AddProductAsync ]

        #region [ CreateOrderAsync ]
        [Fact]
        public async Task CreateOrderAsync_CartCreateWithProducts_ReturnOK()
        {
            var product1 = new ProductData()
            {
                Id = "PRODUCT1",
                Quantity = 10,
                UnitCost = 2
            };
            var product2 = new ProductData()
            {
                Id = "PRODUCT2",
                Quantity = 1,
                UnitCost = 5
            };

            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var orderActor = new Mock<IOrderActor>();

            var actorFactory = new Mock<IActorFactory>();
            actorFactory.Setup(f => f.Create<IOrderActor>(id, new Uri("fabric:/TestingApp/OrderActor"), null))
                .Returns(orderActor.Object);

            orderActor.Setup(a => a.CreateAsync(It.IsAny<List<OrderActor.Interfaces.ProductInfo>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(OrderError.Ok);

            var actor = CreateActor(id, actorFactory.Object, null, null);
            var stateManager = (MockActorStateManager)actor.StateManager;

            await actor.InvokeOnActivateAsync();
            await stateManager.SetStateAsync(CartActor.StateKeyName, State.Create);

            await stateManager.SetStateAsync($"{CartActor.ProductKeyNamePrefix}{product1.Id}", product1);
            await stateManager.SetStateAsync($"{CartActor.ProductKeyNamePrefix}{product2.Id}", product2);

            var result = await actor.CreateOrderAsync(default(CancellationToken));

            Assert.AreEqual(CartError.Ok, result);
            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, State.Close);
        }

        [Fact]
        public async Task CreateOrderAsync_CartCreateWithoutProducts_ReturnError()
        {
            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var orderActor = new Mock<IOrderActor>();

            var actorFactory = new Mock<IActorFactory>();
            actorFactory.Setup(f => f.Create<IOrderActor>(id, new Uri("fabric:/TestingApp/OrderActor"), null))
                .Returns(orderActor.Object);

            var actor = CreateActor(id, actorFactory.Object, null, null);
            var stateManager = (MockActorStateManager)actor.StateManager;

            await actor.InvokeOnActivateAsync();
            await stateManager.SetStateAsync(CartActor.StateKeyName, State.Create);

            var result = await actor.CreateOrderAsync(default(CancellationToken));

            Assert.AreEqual(CartError.GenericError, result);
            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, State.Create);
        }

        [Fact]
        public async Task CreateOrderAsync_OrderAlreadyExists_ReturnError()
        {
            var product1 = new ProductData()
            {
                Id = "PRODUCT1",
                Quantity = 10,
                UnitCost = 2
            };
            var product2 = new ProductData()
            {
                Id = "PRODUCT2",
                Quantity = 1,
                UnitCost = 5
            };

            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var orderActor = new Mock<IOrderActor>();

            var actorFactory = new Mock<IActorFactory>();
            actorFactory.Setup(f => f.Create<IOrderActor>(id, new Uri("fabric:/TestingApp/OrderActor"), null))
                .Returns(orderActor.Object);

            orderActor.Setup(a => a.CreateAsync(It.IsAny<List<OrderActor.Interfaces.ProductInfo>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(OrderError.GenericError);

            var actor = CreateActor(id, actorFactory.Object, null, null);
            var stateManager = (MockActorStateManager)actor.StateManager;

            await stateManager.SetStateAsync($"{CartActor.ProductKeyNamePrefix}{product1.Id}", product1);
            await stateManager.SetStateAsync($"{CartActor.ProductKeyNamePrefix}{product2.Id}", product2);

            await actor.InvokeOnActivateAsync();
            await stateManager.SetStateAsync(CartActor.StateKeyName, State.Create);

            var result = await actor.CreateOrderAsync(default(CancellationToken));

            Assert.AreEqual(CartError.GenericError, result);
            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, State.Create);
        }

        [Fact]
        public async Task CreateOrderAsync_OrderThrowException_ReturnError()
        {
            var product1 = new ProductData()
            {
                Id = "PRODUCT1",
                Quantity = 10,
                UnitCost = 2
            };
            var product2 = new ProductData()
            {
                Id = "PRODUCT2",
                Quantity = 1,
                UnitCost = 5
            };

            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var orderActor = new Mock<IOrderActor>();

            var actorFactory = new Mock<IActorFactory>();
            actorFactory.Setup(f => f.Create<IOrderActor>(id, new Uri("fabric:/TestingApp/OrderActor"), null))
                .Returns(orderActor.Object);

            orderActor.Setup(a => a.CreateAsync(It.IsAny<List<OrderActor.Interfaces.ProductInfo>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            var actor = CreateActor(id, actorFactory.Object, null, null);
            var stateManager = (MockActorStateManager)actor.StateManager;

            await actor.InvokeOnActivateAsync();
            await stateManager.SetStateAsync(CartActor.StateKeyName, State.Create);

            await stateManager.SetStateAsync($"{CartActor.ProductKeyNamePrefix}{product1.Id}", product1);
            await stateManager.SetStateAsync($"{CartActor.ProductKeyNamePrefix}{product2.Id}", product2);

            var result = await actor.CreateOrderAsync(default(CancellationToken));

            Assert.AreEqual(CartError.GenericError, result);
            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, State.Create);
        }
        #endregion [ CreateOrderAsync ]
    }
}
