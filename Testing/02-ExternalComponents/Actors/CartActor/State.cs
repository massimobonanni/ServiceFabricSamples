using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CartActor
{
    [DataContract]
    internal enum State
    {
        [EnumMember]
        Initial = 0,
        [EnumMember]
        Create = 1,
        [EnumMember]
        Close = 2,
        [EnumMember]
        Expire = 3
    }
}
