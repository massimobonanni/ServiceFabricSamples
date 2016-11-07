using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SequenceActor.Interfaces;

namespace SequenceActor
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
    internal class SequenceActor : Actor, ISequenceActor
    {
        /// <summary>
        /// Initializes a new instance of SequenceActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public SequenceActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        private const string SequenceStatusKey = "SequenceKey";

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            return this.StateManager.TryAddStateAsync(SequenceStatusKey, 0);
        }

        public async Task<SequenceDto> GetNextSequenceAsync()
        {
            var currentSequence = await this.StateManager.GetStateAsync<int>(SequenceStatusKey);

            var returnData = new SequenceDto()
            {
                Value = currentSequence,
                NodeInfo = this.ActorService.Context.NodeContext.NodeName
            };

            await this.StateManager.SetStateAsync<int>(SequenceStatusKey, ++currentSequence);

            return returnData;
        }
    }
}
