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
    internal class ActorDemo : Actor, IFireAndForgetActor, IBlockActor, IStateActor, IRemindable
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

        #region [ IBlockActor interface ]
        public async Task<string> DoLongTimeOperationAsync(string operationPayload, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Delay(30000, cancellationToken);
            return $"Operation : {operationPayload}";
        }

        #endregion [ IBlockActor interface ]


        #region [ IStateActor interface ]

        internal const string ContactStateName = "ContactState";
        internal const string StateTypeStateName = "StateTypeState";

        public Task InitializeActorAsync(StateType stateType, int numberOfContacts, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (numberOfContacts <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfContacts));

            this.StateManager.SetStateAsync<StateType>(StateTypeStateName, stateType, cancellationToken);

            switch (stateType)
            {
                case StateType.Wrong:
                    return CreateWrongStateAsync(numberOfContacts, cancellationToken);
                case StateType.Right:
                    return CreateRightStateAsync(numberOfContacts, cancellationToken);
                default:
                    return Task.CompletedTask;
            }
        }

        private Task CreateWrongStateAsync(int numberOfContacts, CancellationToken cancellationToken)
        {
            var contacts = Enumerable.Range(0, numberOfContacts)
                .Select(i => ContactHelper.CreateRandomContact())
                .ToList();

            return this.StateManager.SetStateAsync<List<Contact>>(ContactStateName, contacts, cancellationToken);
        }

        private Task CreateRightStateAsync(int numberOfContacts, CancellationToken cancellationToken)
        {
            for (int i = 0; i < numberOfContacts; i++)
            {
                this.StateManager.SetStateAsync<Contact>(GetContactKey(i),
                    ContactHelper.CreateRandomContact(), cancellationToken);
            }
            return Task.CompletedTask;
        }

        public async Task UpdateContactAsync(int contactIndex, Contact contact, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (contactIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(contactIndex));

            var stateType = await this.StateManager.TryGetStateAsync<StateType>(StateTypeStateName, cancellationToken);

            if (!stateType.HasValue)
                throw new Exception();

            switch (stateType.Value)
            {
                case StateType.Wrong:
                    await UpdateWrongStateAsync(contactIndex, contact, cancellationToken);
                    break;
                case StateType.Right:
                    await UpdateRightStateAsync(contactIndex, contact, cancellationToken);
                    break;
            }
        }

        private async Task UpdateRightStateAsync(int contactIndex, Contact contact, CancellationToken cancellationToken)
        {
            if (!await this.StateManager.ContainsStateAsync(GetContactKey(contactIndex), cancellationToken))
                throw new IndexOutOfRangeException();
            await this.StateManager.SetStateAsync<Contact>(GetContactKey(contactIndex), contact, cancellationToken);
        }

        private async Task UpdateWrongStateAsync(int contactIndex, Contact contact, CancellationToken cancellationToken)
        {
            if (!await this.StateManager.ContainsStateAsync(ContactStateName, cancellationToken))
                throw new IndexOutOfRangeException();

            var contactList = await this.StateManager.GetStateAsync<List<Contact>>(ContactStateName, cancellationToken);

            if (contactIndex>= contactList.Count)
                throw new IndexOutOfRangeException();

            contactList[contactIndex] = contact;

            await this.StateManager.SetStateAsync<List<Contact>>(ContactStateName, contactList, cancellationToken);
        }

        private string GetContactKey(int contactIndex)
        {
            return $"{ContactStateName}_{contactIndex}";
        }
        #endregion [ IStateActor interface ]
    }
}
