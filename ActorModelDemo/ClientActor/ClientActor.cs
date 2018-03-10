using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ActorDemo.Interfaces;
using ActorModelDemo.Core.Extensions;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using ClientActor.Interfaces;
using ActorReference = ActorModelDemo.Core.ActorReference;

namespace ClientActor
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
    [ActorService(Name = "ClientActor")]
    internal class ClientActor : Actor, IClientActor, ICallbackActor
    {
        /// <summary>
        /// Initializes a new instance of ClientActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ClientActor(ActorService actorService, ActorId actorId)
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

        internal const string StatusStateName = "StatusState";
        internal const string DemoActorUri = "fabric:/ActorModelDemo/ActorDemo";
        public async Task<string> GetStatusAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ActorEventSource.Current.ActorMessage(this, "Retrieve status.");
            var status = await this.StateManager.TryGetStateAsync<string>(StatusStateName, cancellationToken);
            return status.HasValue ? status.Value : null;
        }

        public Task ExecuteOperationAsync(string operationPayload, CancellationToken cancellationToken = default(CancellationToken))
        {
            ActorEventSource.Current.ActorMessage(this, $"ExecuteOperationAsync {operationPayload}.");
            if (operationPayload == null)
                throw new ArgumentNullException(nameof(operationPayload));

            var proxy = ActorProxy.Create<IFireAndForgetActor>(this.Id, new Uri(DemoActorUri));

            return proxy.DoOperationWithCallbackAsync(this.ToActorReference(), operationPayload, cancellationToken);

        }

        public async Task ExecuteBlockedOperationAsync(string operationPayload, CancellationToken cancellationToken = default(CancellationToken))
        {
            ActorEventSource.Current.ActorMessage(this, $"ExecuteBlockedOperationAsync {operationPayload}.");
            if (operationPayload == null)
                throw new ArgumentNullException(nameof(operationPayload));

            var proxy = ActorProxy.Create<IBlockActor>(this.Id, new Uri(DemoActorUri));

            var result = await proxy.DoLongTimeOperationAsync(operationPayload, cancellationToken);

            await this.StateManager.SetStateAsync<string>(StatusStateName, result, cancellationToken);
        }


        #region [ ICallbackActor interface ]
        public Task CallbackAsync(ActorReference caller, string callbackPayload,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.StateManager.SetStateAsync<string>(StatusStateName, callbackPayload, cancellationToken);
        }

        #endregion [ ICallbackActor interface ]
    }
}
