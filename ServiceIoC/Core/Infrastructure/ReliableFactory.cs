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
        TActorInterface IActorFactory.Create<TActorInterface>(string actorId,
           Uri serviceUri, string listenerName) 
        {
            return ActorProxy.Create<TActorInterface>(new ActorId(actorId), serviceUri, listenerName);
        }

        TActorInterface IActorFactory.Create<TActorInterface>(ActorId actorId, Uri serviceUri,
           string listenerName = null) 
        {
            return ActorProxy.Create<TActorInterface>(actorId, serviceUri, listenerName);
        }

        TActorInterface IActorFactory.Create<TActorInterface>(ActorId actorId, string applicationName = null,
           string serviceName = null,
           string listenerName = null) 
        {
            return ActorProxy.Create<TActorInterface>(actorId, applicationName, serviceName);
        }

        TServiceInterface IServiceFactory.Create<TServiceInterface>(Uri serviceUri,
           ServicePartitionKey partitionKey = null,
           TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, string listenerName = null) 
        {
            return ServiceProxy.Create<TServiceInterface>(serviceUri, partitionKey, targetReplicaSelector, listenerName);
        }
    }
}
