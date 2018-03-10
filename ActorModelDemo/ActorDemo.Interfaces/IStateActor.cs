using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using ActorModelDemo.Core;
using ActorReference = ActorModelDemo.Core.ActorReference;

namespace ActorDemo.Interfaces
{
    public interface IStateActor : IActor
    {
        Task InitializeActorAsync(StateType stateType, int numberOfContacts,
            CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateContactAsync(int contactIndex, Contact contact,
            CancellationToken cancellationToken = default(CancellationToken));

    }

    [DataContract]
    public enum StateType
    {
        [EnumMember]
        Right,
        [EnumMember]
        Wrong
    }


    [DataContract]
    public class Contact
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Email { get; set; }
    }
}
