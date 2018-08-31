﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;
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

        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            await this.StateManager.TryAddStateAsync(SequenceStatusKey, (long)0);
        }

        public async Task<SequenceDto> GetNextSequenceAsync()
        {
            var currentSequence = await this.StateManager.GetOrAddStateAsync<long>(SequenceStatusKey, 0);

            var returnData = new SequenceDto()
            {
                Value = currentSequence,
                NodeInfo = this.ActorService.Context.NodeContext.NodeName,
                PackageVersion = this.ActorService.Context.CodePackageActivationContext.CodePackageVersion
            };
            //CheckHealth(currentSequence);
            await this.StateManager.SetStateAsync<long>(SequenceStatusKey, ++currentSequence);

            return returnData;
        }

        #region Health
        private const string HealtPropertyName = "Sequence";

        private void CheckHealth(long currentSequence)
        {
            if (currentSequence % 1000 == 0)
            {
                ReportHealthInformation(this.Id.ToString(), HealtPropertyName, "Sequence multiplo di 1000",
                    HealthState.Error, 120);
            }
            else if (currentSequence % 500 == 0)
            {
                ReportHealthInformation(this.Id.ToString(), HealtPropertyName, "Sequence multiplo di 500",
                    HealthState.Warning, 60);
            }
            else
            {
                //ReportHealthInformation(this.Id.ToString(), "count", "", HealthState.Ok);
            }
        }

        protected void ReportHealthInformation(string sourceId, string property, string description,
            HealthState state, int secondsToLive)
        {
            HealthInformation healthInformation = new HealthInformation(sourceId, property, state);
            healthInformation.Description = description;
            if (secondsToLive > 0) healthInformation.TimeToLive = TimeSpan.FromSeconds(secondsToLive);
            healthInformation.RemoveWhenExpired = true;
            try
            {
                var activationContext = FabricRuntime.GetActivationContext();
                activationContext.ReportApplicationHealth(healthInformation);
            }
            catch { }
        }
        #endregion

    }
}
