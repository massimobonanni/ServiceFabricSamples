using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace MyActor.Interfaces
{
    interface Interface: IActor
    {
        Task DoSomething();
        Task DoSomething(int i);
    }
}
