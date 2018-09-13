using CartActor.Interfaces;
using Core.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Moq;
using OrderActor.Interfaces;
using ServiceFabric.Mocks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using CartActor;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using System.Threading;

namespace Integration.Test
{
    public class CreateOrderScenarioTest
    {
        internal static CartActor.CartActor CreateCartActor(ActorId id, IActorFactory actorFactory = null,
            IServiceFactory serviceFactory = null, IProductsService productsService = null)
        {
            if (actorFactory == null)
                actorFactory = new Mock<IActorFactory>().Object;
            if (serviceFactory == null)
                serviceFactory = new Mock<IServiceFactory>().Object;
            if (productsService == null)
                productsService = new Mock<IProductsService>().Object;

            Func<ActorService, ActorId, ActorBase> actorInstanceFactory =
                (service, actorId) => new CartActor.CartActor(service, id, actorFactory, serviceFactory, productsService);

            var codePackage = MockCodePackageActivationContext.Default;
            var configSection = MockConfigurationPackage.CreateConfigurationSection("SqlDataAccess");
            var configSettings = MockConfigurationPackage.CreateConfigurationSettings(
                    new MockConfigurationPackage.ConfigurationSectionCollection() { configSection });
            (codePackage as MockCodePackageActivationContext).ConfigurationPackage = MockConfigurationPackage.CreateConfigurationPackage(configSettings, "Config");

            var context = MockStatefulServiceContextFactory.Create(codePackage, "CartActorType", new Uri("fabric:/TestingApp/CartActor"), Guid.NewGuid(), 0);

            var svc = MockActorServiceFactory.CreateActorServiceForActor<CartActor.CartActor>(actorInstanceFactory, null, context);
            var actor = svc.Activate(id);
            return actor;
        }

        internal static OrderActor.OrderActor CreateOrderActor(ActorId id, IActorFactory actorFactory = null,
            IServiceFactory serviceFactory = null)
        {
            if (actorFactory == null)
                actorFactory = new Mock<IActorFactory>().Object;
            if (serviceFactory == null)
                serviceFactory = new Mock<IServiceFactory>().Object;

            Func<ActorService, ActorId, ActorBase> actorInstanceFactory =
                (service, actorId) => new OrderActor.OrderActor(service, id, actorFactory, serviceFactory);

            var svc = MockActorServiceFactory.CreateActorServiceForActor<OrderActor.OrderActor>(actorInstanceFactory);
            var actor = svc.Activate(id);
            return actor;
        }

        [Fact]
        public async Task CreateOrderAsync_CartCreateWithProducts_OrderNotExists_ReturnOK()
        {
            var product1 = new CartActor.ProductData()
            {
                Id = "PRODUCT1",
                Quantity = 10,
                UnitCost = 2
            };
            var product2 = new CartActor.ProductData()
            {
                Id = "PRODUCT2",
                Quantity = 1,
                UnitCost = 5
            };

            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var actorFactory = new Mock<IActorFactory>();
            var orderActor = CreateOrderActor(id);
            var cartActor = CreateCartActor(id, actorFactory.Object, null, null);
            
            actorFactory.Setup(f => f.Create<IOrderActor>(id, new Uri("fabric:/TestingApp/OrderActor"), null))
                .Returns(orderActor);

            var cartStateManager = (MockActorStateManager)cartActor.StateManager;
            var orderStateManager = (MockActorStateManager)orderActor.StateManager;

            await cartActor.InvokeOnActivateAsync();
            await cartStateManager.SetStateAsync(CartActor.CartActor.StateKeyName, State.Create);

            await cartStateManager.SetStateAsync($"{CartActor.CartActor.ProductKeyNamePrefix}{product1.Id}", product1);
            await cartStateManager.SetStateAsync($"{CartActor.CartActor.ProductKeyNamePrefix}{product2.Id}", product2);

            var result = await cartActor.CreateOrderAsync(default(CancellationToken));

            Assert.AreEqual(CartError.Ok, result);
            var cartState = await cartStateManager.GetStateAsync<CartActor.State>(CartActor.CartActor.StateKeyName);
            Assert.AreEqual(cartState, CartActor.State.Close);

            var orderState = await orderStateManager.GetStateAsync<OrderActor.State>(OrderActor.OrderActor.StateKeyName);
            Assert.AreEqual(orderState, OrderActor.State.Create);

            var orderProduct1 = await orderStateManager.GetStateAsync<OrderActor.ProductData>($"{OrderActor.OrderActor.ProductKeyNamePrefix}{product1.Id}");
            Assert.AreEqual(orderProduct1.Quantity, product1.Quantity);
            var orderProduct2 = await orderStateManager.GetStateAsync<OrderActor.ProductData>($"{OrderActor.OrderActor.ProductKeyNamePrefix}{product2.Id}");
            Assert.AreEqual(orderProduct2.Quantity, product2.Quantity);
        }

        [Fact]
        public async Task CreateOrderAsync_CartCreateWithProducts_OrderAlreadyExists_ReturnGenericError()
        {
            var product1 = new CartActor.ProductData()
            {
                Id = "PRODUCT1",
                Quantity = 10,
                UnitCost = 2
            };
            var product2 = new CartActor.ProductData()
            {
                Id = "PRODUCT2",
                Quantity = 1,
                UnitCost = 5
            };

            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var actorFactory = new Mock<IActorFactory>();
            var orderActor = CreateOrderActor(id);
            var cartActor = CreateCartActor(id, actorFactory.Object, null, null);

            actorFactory.Setup(f => f.Create<IOrderActor>(id, new Uri("fabric:/TestingApp/OrderActor"), null))
                .Returns(orderActor);

            var cartStateManager = (MockActorStateManager)cartActor.StateManager;
            var orderStateManager = (MockActorStateManager)orderActor.StateManager;

            await cartActor.InvokeOnActivateAsync();
            await cartStateManager.SetStateAsync(CartActor.CartActor.StateKeyName, CartActor.State.Create);

            await cartStateManager.SetStateAsync($"{CartActor.CartActor.ProductKeyNamePrefix}{product1.Id}", product1);
            await cartStateManager.SetStateAsync($"{CartActor.CartActor.ProductKeyNamePrefix}{product2.Id}", product2);

            await orderStateManager.SetStateAsync(OrderActor.OrderActor.StateKeyName, OrderActor.State.Create);

            var result = await cartActor.CreateOrderAsync(default(CancellationToken));

            Assert.AreEqual(CartError.GenericError, result);
            var cartState = await cartStateManager.GetStateAsync<CartActor.State>(CartActor.CartActor.StateKeyName);
            Assert.AreEqual(cartState, CartActor.State.Create);
         }
    }
}
