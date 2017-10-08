using System.Runtime.Serialization;

namespace SequenceActor.Interfaces
{
    [DataContract]
    public class SequenceDto
    {
        [DataMember]
        public long Value { get; set; }
        [DataMember]
        public string NodeInfo { get; set; }
        [DataMember]
        public string PackageVersion { get; set; }
    }
}