using System.Runtime.Serialization;

namespace OrderActor
{
    [DataContract]
    internal enum State
    {
        [EnumMember]
        Initial=0,
        [EnumMember]
        Create =1,
        [EnumMember]
        Close =2
    }
}
