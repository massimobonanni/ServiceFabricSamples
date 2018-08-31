using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceFabric.Mocks;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CartActor.Test
{
    public class CartActorTest
    {
        internal static CartActor CreateActor(ActorId id)
        {
            Func<ActorService, ActorId, ActorBase> actorFactory = (service, actorId) => new CartActor(service, id);
            var codePackage = MockCodePackageActivationContext.Default;
            var configSection = MockConfigurationPackage.CreateConfigurationSection("SqlDataAccess");
            var configSettings = MockConfigurationPackage.CreateConfigurationSettings(
                    new MockConfigurationPackage.ConfigurationSectionCollection() { configSection });
            (codePackage as MockCodePackageActivationContext).ConfigurationPackage = MockConfigurationPackage.CreateConfigurationPackage(configSettings, "Config");

            var context = MockStatefulServiceContextFactory.Create(codePackage, "CartActorType", new Uri("fabric:/TestingApp/CartActor"), Guid.NewGuid(), 0);

            var svc = MockActorServiceFactory.CreateActorServiceForActor<CartActor>(actorFactory, null, context);
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
        [Fact]
        public async Task ReceiveReminderAsync_ExpiredReminder_CartStateInitial_CartStateBecameExpired()
        {
            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var actor = CreateActor(id);
            var stateManager = (MockActorStateManager)actor.StateManager;
            await stateManager.SetStateAsync<State>(CartActor.StateKeyName, State.Initial);

            await actor.InvokeOnActivateAsync();

            await actor.ReceiveReminderAsync(CartActor.ExpiredReminderName, null, TimeSpan.Zero, TimeSpan.Zero);

            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, State.Expire);
        }

        [Theory]
        [InlineData(State.Create)]
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
    }
}
