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
            var numActors = 1;
            var timeToReminder = TimeSpan.FromMinutes(3);
            var actorUri = new Uri("fabric:/ActorTimerReminder/MyActorService");
            for (int i = 0; i < numActors; i++)
            {
                var actorId = Guid.NewGuid().ToString();
                var actor = ActorProxy.Create<IMyActor>(new ActorId(actorId), actorUri);
                Console.WriteLine($"Scheduling - Actor {actorId} - TimeToReminder {timeToReminder}");
                actor.ScheduleReminder(timeToReminder);
                Console.WriteLine($"Scheduled - Actor {actorId} - TimeToReminder {timeToReminder}");
            }

            Console.WriteLine("Completed");
            Console.ReadLine();
        }
    }
}
