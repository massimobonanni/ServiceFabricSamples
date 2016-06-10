using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Actors
{
    [StatePersistence(StatePersistence.Persisted)]
    public abstract class StatefulActor<TState> : Actor
    {

        protected virtual string StateName
        {
            get
            {
                return typeof(TState).Name;
            }
        }

        protected async Task<TState> GetStateAsync()
        {
            return await this.StateManager.GetOrAddStateAsync(this.StateName, await this.InitializeState());
        }

        internal async Task SetStateAsync(TState state)
        {
            await this.StateManager.AddOrUpdateStateAsync(this.StateName, state, (n, a) => state);
        }

        protected abstract Task<TState> InitializeState();

    }
}
