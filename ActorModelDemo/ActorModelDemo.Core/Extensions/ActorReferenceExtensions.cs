using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace ActorModelDemo.Core.Extensions
{
    public static class ActorReferenceExtensions
    {
        public static ActorReference ToActorReference(this Actor actor)
        {
            if (actor == null)
                throw new NullReferenceException(nameof(actor));

            return new ActorReference()
            {
                ActorId = actor.Id.GetStringId(),
                ServiceUri = actor.ServiceUri.AbsoluteUri
            };
        }
    }
}
