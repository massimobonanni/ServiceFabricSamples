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
        internal static CartActor.CartActor CreateActor(ActorId id, IActorFactory actorFactory = null,
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

        [Fact]
        public async Task CreateOrderAsync_CartCreateWithProducts_ReturnOK()
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

            var orderActor = new Mock<IOrderActor>();

            var actorFactory = new Mock<IActorFactory>();
            actorFactory.Setup(f => f.Create<IOrderActor>(id, new Uri("fabric:/TestingApp/OrderActor"), null))
                .Returns(orderActor.Object);

            orderActor.Setup(a => a.CreateAsync(It.IsAny<List<OrderActor.Interfaces.ProductInfo>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(OrderError.Ok);

            var actor = CreateActor(id, actorFactory.Object, null, null);
            var stateManager = (MockActorStateManager)actor.StateManager;

            await actor.InvokeOnActivateAsync();
            await stateManager.SetStateAsync(CartActor.CartActor.StateKeyName, State.Create);

            await stateManager.SetStateAsync($"{CartActor.CartActor.ProductKeyNamePrefix}{product1.Id}", product1);
            await stateManager.SetStateAsync($"{CartActor.CartActor.ProductKeyNamePrefix}{product2.Id}", product2);

            var result = await actor.CreateOrderAsync(default(CancellationToken));

            Assert.AreEqual(CartError.Ok, result);
            var state = await stateManager.GetStateAsync<State>(CartActor.CartActor.StateKeyName);
            Assert.AreEqual(state, State.Close);
        }
    }
}
