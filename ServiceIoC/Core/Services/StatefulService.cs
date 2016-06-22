using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Infrastructure;
using Microsoft.ServiceFabric.Data;


namespace Core.Services
{
    using SF = Microsoft.ServiceFabric.Services.Runtime;

    public abstract class StatefulService : SF.StatefulService
    {
        protected readonly IActorFactory ActorFactory;
        protected readonly IServiceFactory ServiceFactory;

        public StatefulService(StatefulServiceContext serviceContext,
            IActorFactory actorFactory = null,
            IServiceFactory serviceFactory = null) :
            base(serviceContext)
        {
            SetFactories(actorFactory, serviceFactory, out ActorFactory, out ServiceFactory);
        }

        public StatefulService(StatefulServiceContext serviceContext,
            IReliableStateManagerReplica reliableStateManagerReplica,
            IActorFactory actorFactory = null,
            IServiceFactory serviceFactory = null) :
            base(serviceContext, reliableStateManagerReplica)
        {
            SetFactories(actorFactory, serviceFactory, out ActorFactory, out ServiceFactory);
        }

        private void SetFactories(IActorFactory actorFactory, IServiceFactory serviceFactory,
            out IActorFactory destActorFactory, out IServiceFactory destServiceFactory)
        {
            var reliableFactory = actorFactory == null || serviceFactory == null ?
               new ReliableFactory() : null;
            destActorFactory = actorFactory ?? reliableFactory;
            destServiceFactory = serviceFactory ?? reliableFactory;
        }
    }
}
