using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActorDemo.Interfaces;
using ClientActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace TestConsole.Commands
{
    class StateManagementCommand : ICommand
    {
        public async Task ExuteAsync(string[] args, CancellationToken token = default(CancellationToken))
        {
            Console.WriteLine();
            if (args.Count() >= 4)
            {
                var rnd = new Random(DateTime.Now.Millisecond);
                var serviceUri = new Uri(args[1]);
                var actorId = new ActorId(args[2]);
                var stateType = (StateType)Enum.Parse(typeof(StateType),args[3]);
                var numberOfContacts = int.Parse(args[4]);

                var proxy = ActorProxy.Create<IStateActor>(actorId, serviceUri);

                Console.WriteLine("Premere ESC per uscire!");

                Console.WriteLine($"\t[{DateTime.Now:HH:mm:ss.fff}] - Actor:{actorId} --> before InitializeActorAsync {stateType}");
                var sw = Stopwatch.StartNew();
                await proxy.InitializeActorAsync(stateType,numberOfContacts, token);
                sw.Stop();
                Console.WriteLine($"\t[{DateTime.Now:HH:mm:ss.fff}] - Actor:{actorId} --> after InitializeActorAsync {stateType} - {sw.ElapsedMilliseconds} msec");

                while (!token.IsCancellationRequested)
                {
                    var contactIndex = rnd.Next(0, numberOfContacts - 1);
                    var contact= new Contact()
                    {
                        LastName = Faker.Name.First(),
                        FirstName = Faker.Name.Last(),
                        Email = Faker.Internet.Email()
                    };
                    Console.WriteLine($"\t[{DateTime.Now:HH:mm:ss.fff}] - Actor:{actorId} --> before UpdateContactAsync {stateType}");
                    sw = Stopwatch.StartNew();
                    await proxy.UpdateContactAsync(contactIndex,contact, token);
                    sw.Stop();
                    Console.WriteLine($"\t[{DateTime.Now:HH:mm:ss.fff}] - Actor:{actorId} --> after UpdateContactAsync {stateType} - {sw.ElapsedMilliseconds} msec");

                    await Task.Delay(1000, token);
                }
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Parametri errati!");
                Console.WriteLine();
            }
        }
    }
}
