using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using AffidoActor.Interfaces;
using Core.Actors;
using Core.Infos;
using Core.Infrastructure;
using OdlActor.Interfaces;

namespace AffidoActor
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
    internal class AffidoActor : StatefulActor<AffidoState>, IAffidoActor
    {
        public AffidoActor() : base()
        {
        }

        public AffidoActor(IActorStateManager stateManager,
            IActorFactory actorFactory, IServiceFactory serviceFactory,
            ActorId actorId = null) : base(stateManager, actorFactory, serviceFactory, actorId)
        {
        }

        public async Task<bool> TakeInCharge(string idOdl)
        {
            ActorEventSource.Current.ActorMessage(this, $"{Id} - TakeInCharge({idOdl})");

            var id = this.Id;
            var state = await this.GetStateAsync();
            var odl = state.OdlList?.FirstOrDefault(o => o.Id == idOdl);
            if (odl == null) return false;

            var actor = ActorFactory.Create<IOdlActor>(new ActorId(idOdl), serviceName: "fabric:/ServiceIoC/AffidoActor");

            return await actor.TakeInCharge();
        }

        public async Task<bool> AddOdl(OdlInfo odl)
        {
            if (odl == null) return false;
            ActorEventSource.Current.ActorMessage(this, $"{Id} - TakeInCharge({odl.Id })");
            var state = await this.GetStateAsync();
            if (state.OdlList == null) state.OdlList = new List<OdlInfo>();
            if (state.OdlList.Any(a => a.Id == odl.Id)) return false;
            state.OdlList.Add(odl);
            await this.SetStateAsync(state);
            return true;
        }

        protected override Task<AffidoState> InitializeState()
        {
            return Task.FromResult(new AffidoState() { });
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


    }
}
