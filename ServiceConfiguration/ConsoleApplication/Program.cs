using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using MyActor.Interfaces;
using System.Threading;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var actor = ActorProxy.Create<IMyActor>(new ActorId("Actor1"),
                    new Uri("fabric:/ServiceConfiguration/MyActorService"), null);

                var setting = actor.GetConfiguration().Result;
                Console.WriteLine(setting);
                var fileContent = actor.GetFile().Result;
                Console.WriteLine(fileContent);

                Thread.Sleep(250);
            }
        }
    }
}
