using System;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Core.Infrastructure
{
    public interface IActorFactory
    {
        TActorInterface Create<TActorInterface>(string actorId,
            Uri serviceUri, string listenerName = null) where TActorInterface : IActor;

        TActorInterface Create<TActorInterface>(ActorId actorId,
            Uri serviceUri, string listenerName = null) where TActorInterface : IActor;

        TActorInterface Create<TActorInterface>(ActorId actorId, string applicationName = null, string serviceName = null,
            string listenerName = null) where TActorInterface : IActor;
    }

    public interface IServiceFactory
    {
        TServiceInterface Create<TServiceInterface>(Uri serviceUri,
            ServicePartitionKey partitionKey, TargetReplicaSelector targetReplicaSelector,
            string listenerName) where TServiceInterface : IService;

        TServiceInterface Create<TServiceInterface>(Uri serviceUri) where TServiceInterface : IService;

        TServiceInterface Create<TServiceInterface>(Uri serviceUri, ServicePartitionKey partitionKey) where TServiceInterface : IService;

        TServiceInterface Create<TServiceInterface>(Uri serviceUri, ServicePartitionKey partitionKey, TargetReplicaSelector targetReplicaSelector) where TServiceInterface : IService;
    }
}
