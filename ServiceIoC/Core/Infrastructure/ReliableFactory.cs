using System;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Core.Infrastructure
{
    public class ReliableFactory : IActorFactory, IServiceFactory
    {
        public TActorInterface CreateActor<TActorInterface>(string actorId,
            Uri serviceUri, string listenerName) where TActorInterface : IActor
        {
            return ActorProxy.Create<TActorInterface>(new ActorId(actorId), serviceUri, listenerName);
        }

        public TActorInterface Create<TActorInterface>(ActorId actorId, Uri serviceUri,
            string listenerName = null) where TActorInterface : IActor
        {
            return ActorProxy.Create<TActorInterface>(actorId, serviceUri, listenerName);
        }

        public TActorInterface Create<TActorInterface>(ActorId actorId, string applicationName = null,
            string serviceName = null,
            string listenerName = null) where TActorInterface : IActor
        {
            return ActorProxy.Create<TActorInterface>(actorId, applicationName, serviceName);
        }

        public TServiceInterface CreateService<TServiceInterface>(Uri serviceUri,
            ServicePartitionKey partitionKey = null,
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, string listenerName = null) where TServiceInterface : IService
        {
            return ServiceProxy.Create<TServiceInterface>(serviceUri, partitionKey, targetReplicaSelector, listenerName);
        }
    }
}
