using System.Runtime.Serialization;

namespace ActorModelDemo.Core
{
    [DataContract]
    public class ActorReference
    {
        [DataMember]
        public string ServiceUri { get; set; }

        [DataMember]
        public string ActorId { get; set; }
    }
}
