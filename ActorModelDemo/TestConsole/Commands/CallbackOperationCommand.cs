using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace TestConsole.Commands
{
    class CallbackOperationCommand : ICommand
    {
        public async Task ExuteAsync(string[] args, CancellationToken token = default(CancellationToken))
        {
            Console.WriteLine();
            if (args.Count() >= 4)
            {
                var serviceUri = new Uri(args[1]);
                var actorId = new ActorId(args[2]);
                var operationPayload = args[3];

                var proxy = ActorProxy.Create<IClientActor>(actorId, serviceUri);

                Console.WriteLine("Premere ESC per uscire!");

                Console.WriteLine($"\tActor:{actorId} --> Esecuzione operazione con payload {operationPayload}");
                await proxy.ExecuteOperationAsync(operationPayload, token);

                while (!token.IsCancellationRequested)
                {
                    var status = await proxy.GetStatusAsync(token);
                    Console.WriteLine($"\tActor:{actorId} --> Stato: {status}");
                    await Task.Delay(100, token);
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
