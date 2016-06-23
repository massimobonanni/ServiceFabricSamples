using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Infrastructure;
using Microsoft.ServiceFabric.Actors;

namespace Core.Actors
{
    [StatePersistence(StatePersistence.Persisted)]
    public abstract class StatefulActor<TState> : Actor
    {
        protected readonly IActorFactory ActorFactory;
        protected readonly IServiceFactory ServiceFactory;

        public StatefulActor() : this(null, null, null)
        {

        }

        public StatefulActor(IActorStateManager stateManager,
            IActorFactory actorFactory, IServiceFactory serviceFactory,
            ActorId actorId = null) : base()
        {
            _stateManager = stateManager;
            _id = actorId;
            var reliableFactory = actorFactory == null || serviceFactory == null ?
                new ReliableFactory() : null;
            ActorFactory = actorFactory ?? reliableFactory;
            ServiceFactory = serviceFactory ?? reliableFactory;
        }

        private readonly IActorStateManager _stateManager;

        public new IActorStateManager StateManager
        {
            get
            {
                if (_stateManager != null) return _stateManager;
                return base.StateManager;
            }
        }


        private readonly ActorId _id;

        public new ActorId Id
        {
            get
            {
                if (_id != null) return _id;
                return base.Id;
            }
        }

        protected virtual string StateName => typeof(TState).Name;

        protected internal async Task<TState> GetStateAsync()
        {
            return await this.StateManager.GetOrAddStateAsync(this.StateName, await this.InitializeState());
        }

        protected internal async Task SetStateAsync(TState state)
        {
            await this.StateManager.AddOrUpdateStateAsync(this.StateName, state, (n, a) => state);
        }

        protected abstract Task<TState> InitializeState();


    }
}
