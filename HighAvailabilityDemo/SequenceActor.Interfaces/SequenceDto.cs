using System.Runtime.Serialization;

namespace SequenceActor.Interfaces
{
    [DataContract]
    public class SequenceDto
    {
        [DataMember]
        public int Value { get; set; }
        [DataMember]
        public string NodeInfo { get; set; }
        [DataMember]
        public string PackageVersion { get; set; }
    }
}