using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using MyActor.Interfaces;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var actor = ActorProxy.Create<IMyActor>(new ActorId("test"),
                new Uri("fabric:/ActorTimerReminder/MyActorService"));

            var result = actor.GetCountAsync().Result;
          
            Console.ReadLine();
        }
    }
}
