using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace ActorModelDemo.Core.Collections
{
    public static class FineGrainQueueManager
    {
        #region Private Methods
        private static string GetQueueHeadIdentityName(string queueName)
        {
            return $"{queueName }_HeadIdentity";
        }

        private static string GetQueueIdentityName(string queueName)
        {
            return $"{queueName }_Identity";
        }

        private static string GetQueueItemKey(string queueName, long itemIdentity)
        {
            return $"{queueName}_{itemIdentity}";
        }

        private static Task SetHeadIdentity(this IActorStateManager state, string queueName,
            long counter, CancellationToken cancellationToken = default(CancellationToken))
        {
            return state.SetStateAsync<long>(GetQueueHeadIdentityName(queueName), counter, cancellationToken);
        }

        private static async Task<long> GetHeadIdentity(this IActorStateManager state, string queueName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var counter = await state.TryGetStateAsync<long>(GetQueueHeadIdentityName(queueName), cancellationToken);
            return counter.HasValue ? counter.Value : 0;
        }

        private static async Task<long> GetIdentity(this IActorStateManager state, string queueName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var identity = await state.TryGetStateAsync<long>(GetQueueIdentityName(queueName), cancellationToken);
            return identity.HasValue ? identity.Value : -1;
        }

        private static Task SetIdentity(this IActorStateManager state, string queueName,
            long identity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return state.SetStateAsync<long>(GetQueueIdentityName(queueName), identity, cancellationToken);
        }

        #endregion

        #region EnqueueAsync
        public static async Task EnqueueAsync<TElement>(this IActorStateManager state, string queueName,
            TElement element, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (state == null)
                throw new NullReferenceException(nameof(state));
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException(nameof(queueName));

            var currentIdentity = await state.GetIdentity(queueName, cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                currentIdentity = await state.AddItemToStateAsync(queueName, element, currentIdentity, cancellationToken);
                await state.SetIdentity(queueName, currentIdentity, cancellationToken);
            }
        }

        /// <summary>
        /// Aggiunge un elemento allo stato gestendo il valore del current identity passato per argomento. Il metodo ritorna il nuovo current identity. .
        /// </summary>
        /// <typeparam name="TElement">Tipo dell'elemento da salvare.</typeparam>
        /// <param name="state">ActorStateManager in cui salvare l'elemento.</param>
        /// <param name="queueName">Nome della coda.</param>
        /// <param name="element">Elemento da salvare.</param>
        /// <param name="currentIdentity">Valore del current identity, cioè l'indice dell'ultimo elemento aggiunto allo stato.</param>
        /// <param name="cancellationToken">CancellationToken utilizzato per l'annullamento delle operazioni asincrone.</param>
        /// <returns>Task&lt;System.Int64&gt;.</returns>
        /// <exception cref="InsufficientMemoryException">Memory overflow in the queue!!!</exception>
        private static async Task<long> AddItemToStateAsync<TElement>(this IActorStateManager state, string queueName,
            TElement element, long currentIdentity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (currentIdentity == long.MaxValue)
            {
                var currentHead = await state.GetHeadIdentity(queueName, cancellationToken);
                if (currentHead == 0)
                    throw new InsufficientMemoryException("Memory overflow in the queue!!!");
                currentIdentity = 0;
            }
            else
            {
                currentIdentity++;
            }
            var itemKey = GetQueueItemKey(queueName, currentIdentity);
            await state.AddOrUpdateStateAsync(itemKey, element, (k, v) => element, cancellationToken);
            return currentIdentity;
        }

        /// <summary>
        /// Accoda un insieme di oggetti alla coda.
        /// </summary>
        /// <typeparam name="TElement">Tipo dell'elemento da salvare.</typeparam>
        /// <param name="state">ActorStateManager in cui salvare l'elemento.</param>
        /// <param name="queueName">Nome della coda.</param>
        /// <param name="elements">Elementi da salvare.</param>
        /// <param name="cancellationToken">CancellationToken utilizzato per l'annullamento delle operazioni asincrone. Se il cancellation token richiede la chiusura dell'operazione, il metodo solleva un'eccezione <see cref="OperationCanceledException"/> in modo da garantire l'atomicità dell'operazione.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NullReferenceException">state</exception>
        /// <exception cref="ArgumentException">queueName</exception>
        /// <exception cref="ArgumentNullException">elements</exception>
        /// <exception cref="OperationCanceledException">Se il cancellation token richiede la chiusura dell'operazione.</exception>
        public static async Task EnqueueAsync<TElement>(this IActorStateManager state, string queueName,
            IEnumerable<TElement> elements, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (state == null)
                throw new NullReferenceException(nameof(state));
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException(nameof(queueName));
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            if (elements.Any())
            {
                var currentIdentity = await state.GetIdentity(queueName, cancellationToken);

                for (int itemIndex = 0; itemIndex < elements.Count(); itemIndex++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var element = elements.ElementAt(itemIndex);
                    currentIdentity = await state.AddItemToStateAsync(queueName, element, currentIdentity, cancellationToken);
                }
                await state.SetIdentity(queueName, currentIdentity, cancellationToken);
            }
        }
        #endregion

        #region DequeueAsync

        public static async Task<TElement> DequeueAsync<TElement>(this IActorStateManager state, string queueName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (state == null)
                throw new NullReferenceException(nameof(state));
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException(nameof(queueName));

            TElement result = default(TElement);

            if (await state.GetQueueLengthAsync(queueName, cancellationToken) > 0)
            {
                var currentHeadIdentity = await state.GetHeadIdentity(queueName, cancellationToken);
                var currentIdentity = await state.GetIdentity(queueName, cancellationToken);
                var headGTIdentity = currentHeadIdentity > currentIdentity; // utile per capire se l'head supera il massimo valore long durante il metodo e se serve di resettare gli indici

                var itemKey = GetQueueItemKey(queueName, currentHeadIdentity);
                var element = await state.TryGetStateAsync<TElement>(itemKey, cancellationToken);

                if (element.HasValue)
                {
                    // elemento in coda esistente e lo rimuovo
                    await state.TryRemoveStateAsync(itemKey, cancellationToken);
                    result = element.Value;

                    // Check su overflow (se l'head arriva al valore massimo di long, debbo resettarlo a zero)
                    if (currentHeadIdentity == long.MaxValue)
                    {
                        currentHeadIdentity = 0;
                    }
                    else
                    {
                        currentHeadIdentity++;
                    }

                    // resetto gli indici se necessario
                    if (currentHeadIdentity > currentIdentity && !headGTIdentity) //in questo caso siamo nella situazione in cui l'head era precedentemente minore dell'identity e lo ha superato, quindi non ci sono elementi in coda 
                    {
                        // in questo caso non ci sono elementi in coda, quindi possiamo resettare l'indice di head e quello dell'identity
                        await state.SetIdentity(queueName, -1, cancellationToken);
                        await state.SetHeadIdentity(queueName, 0, cancellationToken);
                    }
                    else
                    {
                        await state.SetHeadIdentity(queueName, currentHeadIdentity, cancellationToken);
                    }
                }
            }
            return result;
        }

        #endregion 

        #region PurgeQueue
        public static async Task<bool> PurgeQueue(this IActorStateManager state, string queueName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (state == null)
                throw new NullReferenceException(nameof(state));
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException(nameof(queueName));

            var queueItemKeys = (await state.GetStateNamesAsync(cancellationToken))
                .Where(k => k.StartsWith($"{queueName}_")).ToList();
            bool result = true;
            foreach (var queueItemKey in queueItemKeys)
            {
                result = result & await state.TryRemoveStateAsync(queueItemKey, cancellationToken);
            }
            await state.SetIdentity(queueName, -1, cancellationToken);
            await state.SetHeadIdentity(queueName, 0, cancellationToken);
            return result;
        }
        #endregion

        #region GetQueueLengthAsync
        public static async Task<long> GetQueueLengthAsync(this IActorStateManager state, string queueName,
                   CancellationToken cancellationToken = default(CancellationToken))
        {
            if (state == null)
                throw new NullReferenceException(nameof(state));
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException(nameof(queueName));

            long counter = 0;
            var currentIdentity = await state.GetIdentity(queueName, cancellationToken);
            var currentHead = await state.GetHeadIdentity(queueName, cancellationToken);
            if ((currentIdentity == -1 && currentHead == 0) || currentHead <= currentIdentity)
            {
                counter += (currentIdentity - currentHead + 1);
            }
            else
            {
                counter += ((long.MaxValue - currentHead + 1) + (currentIdentity + 1));
            }
            return counter;
        }
        #endregion

        #region PeekQueueAsync
        public static async Task<TElement> PeekQueueAsync<TElement>(this IActorStateManager state, string queueName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (state == null)
                throw new NullReferenceException(nameof(state));
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException(nameof(queueName));

            TElement result = default(TElement);

            if (await state.GetQueueLengthAsync(queueName, cancellationToken) > 0)
            {
                var currentHeadIdentity = await state.GetHeadIdentity(queueName, cancellationToken);
                var itemKey = GetQueueItemKey(queueName, currentHeadIdentity);
                var element = await state.TryGetStateAsync<TElement>(itemKey, cancellationToken);

                if (element.HasValue)
                {
                    result = element.Value;
                }
            }
            return result;
        }

        #endregion
    }
}
