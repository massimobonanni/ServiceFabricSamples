using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using CartActor.Implementations;
using CartActor.Interfaces;
using Microsoft.ServiceFabric.Actors.Runtime;
using SimpleInjector;

namespace CartActor
{
    internal static class Program
    {
        static Container container;

        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            ConfigureContainer();

            try
            {
                // This line registers an Actor Service to host your actor class with the Service Fabric runtime.
                // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                // are automatically populated when you build this project.
                // For more information, see https://aka.ms/servicefabricactorsplatform

                ActorRuntime.RegisterActorAsync<CartActor>(
                   (context, actorType) => new ActorService(context, actorType,
                        (actorService, actorId) => new CartActor(actorService, actorId, container.GetInstance<IProductsService>())))
                            .GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static void ConfigureContainer()
        {
            container = new Container();

            container.Register<IProductsService, SQLProductsService>();

            container.Verify();
        }
    }
}
