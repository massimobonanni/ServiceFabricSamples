using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceFabric.Mocks;

namespace CartActor.Test
{
    [TestClass]
    public class CartActorTest
    {
        internal static CartActor CreateActor(ActorId id)
        {
            Func<ActorService, ActorId, ActorBase> actorFactory = (service, actorId) => new CartActor(service, id);
            var svc = MockActorServiceFactory.CreateActorServiceForActor<CartActor>(actorFactory);
            var actor = svc.Activate(id);
            return actor;
        }

        #region [ CreateAsync ]
        [TestMethod]
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

        [TestMethod]
        public async Task CreateAsync_CartStateCreate_Error()
        {
            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var actor = CreateActor(id);
            var stateManager = (MockActorStateManager)actor.StateManager;
            await stateManager.SetStateAsync<State>(CartActor.StateKeyName, State.Create);

            await actor.InvokeOnActivateAsync();

            var actual = await actor.CreateAsync(default(CancellationToken));

            Assert.AreEqual(actual, Interfaces.CartError.GenericError);
            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, State.Create);
        }

        [TestMethod]
        public async Task CreateAsync_CartStateExpire_Error()
        {
            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var actor = CreateActor(id);
            var stateManager = (MockActorStateManager)actor.StateManager;
            await stateManager.SetStateAsync<State>(CartActor.StateKeyName, State.Expire);

            await actor.InvokeOnActivateAsync();

            var actual = await actor.CreateAsync(default(CancellationToken));

            Assert.AreEqual(actual, Interfaces.CartError.GenericError);
            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, State.Expire);
        }

        [TestMethod]
        public async Task CreateAsync_CartStateClose_Error()
        {
            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var actor = CreateActor(id);
            var stateManager = (MockActorStateManager)actor.StateManager;
            await stateManager.SetStateAsync<State>(CartActor.StateKeyName, State.Close);

            await actor.InvokeOnActivateAsync();

            var actual = await actor.CreateAsync(default(CancellationToken));

            Assert.AreEqual(actual, Interfaces.CartError.GenericError);
            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, State.Close);
        }
        #endregion [ CreateAsync ]

        #region [ ReceiveReminderAsync ]
        [TestMethod]
        public async Task ReceiveReminderAsync_ExpiredReminder_CartStateInitial_CarteStateBecameExpired()
        {
            var actorGuid = Guid.NewGuid();
            var id = new ActorId(actorGuid);

            var actor = CreateActor(id);
            var stateManager = (MockActorStateManager)actor.StateManager;
            await stateManager.SetStateAsync<State>(CartActor.StateKeyName, State.Initial);

            await actor.InvokeOnActivateAsync();

            await actor.ReceiveReminderAsync(CartActor.ExpiredReminderName,null,TimeSpan.Zero,TimeSpan.Zero);

            var state = await stateManager.GetStateAsync<State>(CartActor.StateKeyName);
            Assert.AreEqual(state, State.Expire);
        }
        #endregion [ ReceiveReminderAsync ]
    }
}
