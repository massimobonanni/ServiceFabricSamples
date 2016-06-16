using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Infrastructure;

namespace Core.Actors
{
    [StatePersistence(StatePersistence.Persisted)]
    public abstract class StatefulActor<TState> : Actor
    {
        protected readonly IActorFactory ActorFactory;
        protected readonly IServiceFactory ServiceFactory;

        public StatefulActor() : this(null,null,null)
        {

        }

        public StatefulActor(IActorStateManager stateManager, 
            IActorFactory actorFactory,IServiceFactory serviceFactory) : base()
        {
            _stateManager = stateManager;
            ActorFactory = actorFactory ?? new ReliableFactory();
            ServiceFactory = serviceFactory ?? new ReliableFactory();
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
