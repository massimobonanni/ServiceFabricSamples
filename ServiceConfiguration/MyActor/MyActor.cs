using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using MyActor.Interfaces;
using Core;
using System.Fabric;

namespace MyActor
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
    internal class MyActor : Actor, IMyActor
    {
        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            ReadConfiguration(this.ActorService.Context.CodePackageActivationContext.GetConfigurationPackageObject("Config"));
            this.ActorService.Context.CodePackageActivationContext.ConfigurationPackageModifiedEvent += ConfigurationPackageModifiedEvent;
            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see http://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }

        private void ConfigurationPackageModifiedEvent(object sender, PackageModifiedEventArgs<ConfigurationPackage> e)
        {
            ReadConfiguration(e.NewPackage);
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <returns></returns>
        Task<int> IMyActor.GetCountAsync()
        {
            return this.StateManager.GetStateAsync<int>("count");
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task IMyActor.SetCountAsync(int count)
        {
            // Requests are not guaranteed to be processed in order nor at most once.
            // The update function here verifies that the incoming count is greater than the current count to preserve order.
            return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value);
        }

        private string ActorSetting;

        private void ReadConfiguration(ConfigurationPackage package)
        {
            var configSection = package.Settings.Sections["MySection"];

            ActorSetting = configSection.Parameters["MyParameter"].Value;
        }

        public Task<string> GetConfiguration()
        {
            return Task.FromResult(ActorSetting);
        }

        public async Task<string> GetFile()
        {
            var dataPkg =
                ActorService.Context.CodePackageActivationContext.GetDataPackageObject("SvcData");

            var customDataFilePath = $@"{dataPkg.Path}\data.json";

            string fileContent;
            using (var reader = File.OpenText(customDataFilePath))
            {
                fileContent = await reader.ReadToEndAsync();
            }

            return fileContent;
        }
    }
}
