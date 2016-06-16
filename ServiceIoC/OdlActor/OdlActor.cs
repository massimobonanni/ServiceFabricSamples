using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Actors;
using Core.Infos;
using Core.Infrastructure;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using OdlActor.Interfaces;

namespace OdlActor
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
    internal class OdlActor : StatefulActor<OdlInfo>, IOdlActor
    {
        public OdlActor() : base()
        {
        }

        public OdlActor(IActorStateManager stateManager,
            IActorFactory actorFactory, IServiceFactory serviceFactory) : base(stateManager, actorFactory, serviceFactory)
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
            // For more information, see http://aka.ms/servicefabricactorsstateserialization
            return Task.Delay(0);
        }

        async Task<bool> IOdlActor.TakeInCharge()
        {
            var state = await this.GetStateAsync();
            if (state.Status == OdlState.Initial)
            {
                state.Status = OdlState.InCharge;
                await this.SetStateAsync(state);
                return true;
            }
            return false;
        }


        protected override Task<OdlInfo> InitializeState()
        {
            return Task.FromResult(new OdlInfo()
            {
                Id = this.Id.ToString(),
                Status = OdlState.Initial
            });
        }
    }
}
