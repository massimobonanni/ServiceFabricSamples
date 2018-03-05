using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using ActorDemo.Interfaces;
using System.Runtime.Serialization;
using ActorModelDemo.Core.Collections;
using ActorReference = ActorModelDemo.Core.ActorReference;
using ActorModelDemo.Core.Extensions;

namespace ActorDemo
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    [ActorService(Name = "ActorDemo")]
    internal class ActorDemo : Actor, IFireAndForgetActor, IRemindable
    {
        /// <summary>
        /// Initializes a new instance of ActorDemo
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ActorDemo(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return Task.CompletedTask;
        }


        #region [ IFireAndForget Interface]
        internal const string FireAndForgetQueueName = "FireAndForgetQueueName";
        internal const string FireAndForgetReminderName = "FireAndForgetReminderName";

        [DataContract]
        private class FireAndForgetQueueItem
        {
            [DataMember]
            public ActorReference ActorReference { get; set; }
            [DataMember]
            public string OperationPayload { get; set; }
        }

        public async Task DoOperationWithCallbackAsync(ActorReference caller, string operationPayload, CancellationToken cancellationToken)
        {
            if (await this.StateManager.GetQueueLengthAsync(FireAndForgetQueueName, cancellationToken) > 0)
            {
                ActorEventSource.Current.ActorMessage(this, "Operation busy!");
                throw new Exception("Operation busy!");
            }

            ActorEventSource.Current.ActorMessage(this, "Enqueue operation data!");

            await this.StateManager.EnqueueAsync<FireAndForgetQueueItem>(FireAndForgetQueueName,
                new FireAndForgetQueueItem()
                {
                    ActorReference = caller,
                    OperationPayload = operationPayload
                }, cancellationToken);

            ActorEventSource.Current.ActorMessage(this, "Start reminder!");
            await RegisterReminderAsync(FireAndForgetReminderName, null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(-1));
        }

        private async Task ExecuteOperationWithCallbackAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"ExecuteOperationWithCallbackAsync");
            var operationItem = await this.StateManager.DequeueAsync<FireAndForgetQueueItem>(FireAndForgetQueueName);
            if (operationItem != null)
            {
                var proxy = ActorProxy.Create<ICallbackActor>(new ActorId(operationItem.ActorReference.ActorId),
                                new Uri(operationItem.ActorReference.ServiceUri));
                try
                {
                    ActorEventSource.Current.ActorMessage(this, $"Callback to {operationItem.ActorReference.ServiceUri}:{operationItem.ActorReference.ActorId}");
                    await proxy.CallbackAsync(this.ToActorReference(), $"Operation : {operationItem.OperationPayload}");
                }
                catch { }
            }
        }

        #endregion [ IFireAndForget Interface]

        #region [ IRemindable Interface ]
        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            ActorEventSource.Current.ActorMessage(this, $"ReceiveReminderAsync {reminderName}");
            if (reminderName == FireAndForgetReminderName)
                return ExecuteOperationWithCallbackAsync();

            return Task.CompletedTask;
        }
        #endregion [ IRemindable Interface ]
    }
}
