using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosteItaliane.Sin.SF.Actors
{
    public static class FineGrainQueueManager
    {
        public static async Task EnqueueAsync<TElement>(this IActorStateManager state, string queueName, TElement element)
        {
            var elementId = Guid.NewGuid();
            var queue = await state.GetOrAddStateAsync<Queue<Guid>>(queueName, new Queue<Guid>());
            queue.Enqueue(elementId);
            await state.SetStateAsync<TElement>(elementId.ToString(), element);
        }

        public static async Task<TElement> DequeueAsync<TElement>(this IActorStateManager state, string queueName)
        {
            TElement result = default(TElement);

            var codaTemp = await state.GetStateAsync<Queue<Guid>>(queueName);
            var queue = await state.GetOrAddStateAsync<Queue<Guid>>(queueName, new Queue<Guid>());

            if (queue.Count == 0)
            {
                return result;
            }

            var elementId = queue.Dequeue().ToString();

            var element = await state.TryGetStateAsync<TElement>(elementId);

            if (element.HasValue)
            {
                await state.TryRemoveStateAsync(elementId);
                result = element.Value;
            }

            await state.SetStateAsync<Queue<Guid>>(queueName, queue);

            return result;
        }

        public static async Task<int> GetQueueLengthAsync(this IActorStateManager state, string queueName)
        {
            var queue = await state.GetOrAddStateAsync<Queue<Guid>>(queueName, new Queue<Guid>());
            return queue.Count;
        }
    }
}
