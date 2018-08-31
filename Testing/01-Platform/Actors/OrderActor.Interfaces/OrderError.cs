using System.Runtime.Serialization;

namespace OrderActor.Interfaces
{
    [DataContract]
    public enum OrderError
    {
        [EnumMember]
        Ok = 0,

        [EnumMember]
        GenericError = 999
    }
}