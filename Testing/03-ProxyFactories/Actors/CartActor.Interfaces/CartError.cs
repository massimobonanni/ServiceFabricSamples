using System.Runtime.Serialization;

namespace CartActor.Interfaces
{
    [DataContract]
    public enum CartError
    {
        [EnumMember]
        Ok = 0,

        [EnumMember]
        GenericError = 999
    }
}