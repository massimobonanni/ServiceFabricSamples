using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace SequenceActor.Interfaces
{
    public interface ISequenceActor : IActor
    {
       Task<SequenceDto> GetNextSequenceAsync();
    }
}
