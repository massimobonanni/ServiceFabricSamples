using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Data.Notifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mocks
{
    public class MockActorStateManager : IActorStateManager
    {
        private Dictionary<string, object> store =
            new Dictionary<string, object>();

        public Task<T> AddOrUpdateStateAsync<T>(string stateName, T addValue,
            Func<string, T, T> updateValueFactory, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (store.ContainsKey(stateName))
                store[stateName] = updateValueFactory.Invoke(stateName, (T)store[stateName]);
            else
                store[stateName] = addValue;

            return Task.FromResult((T)store[stateName]);
        }

        public Task AddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            store[stateName] = value;

            return Task.Delay(0, cancellationToken);
        }

        public Task ClearCacheAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.store.Clear();
            return Task.Delay(0, cancellationToken);
        }

        public Task<bool> ContainsStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(store.ContainsKey(stateName));
        }

        public Task<T> GetOrAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!store.ContainsKey(stateName)) store[stateName] = value; ;

            return Task.FromResult((T)store[stateName]);
        }

        public Task<T> GetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            object result;
            store.TryGetValue(stateName, out result);
            return Task.FromResult((T)result);
        }

        public Task<IEnumerable<string>> GetStateNamesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var keys = store.Keys as IEnumerable<string>;
            return Task.FromResult(keys);
        }

        public Task RemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (store.ContainsKey(stateName)) store.Remove(stateName);
            return Task.Delay(0, cancellationToken);
        }

        public Task SetStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.AddOrUpdateStateAsync(stateName, value, (key, oldvalue) => value, cancellationToken);
        }

        public Task<bool> TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.AddStateAsync(stateName, value, cancellationToken);
            return Task.FromResult(true);
        }

        public Task<ConditionalValue<T>> TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            object item;
            bool result = this.store.TryGetValue(stateName, out item);
            return Task.FromResult(new ConditionalValue<T>(result, (T)item));
        }

        public Task<bool> TryRemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.RemoveStateAsync(stateName, cancellationToken);
            return Task.FromResult(true);
        }
    }
}

