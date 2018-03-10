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
    class BlockedOperationCommand:ICommand
    {
        public async Task ExuteAsync(string[] args, CancellationToken token = default(CancellationToken))
        {
            Console.WriteLine();
            if (args.Count() >= 4)
            {
                var serviceUri = new Uri(args[1]);
                var actorId = new ActorId(args[2]);
                var operationPayload = args[3];

                Console.WriteLine("Premere ESC per uscire!");
                var tasks = new List<Task>
                {
                    BlockedOperationTask(1, serviceUri, actorId, operationPayload, token),
                    GetStatusTask(2, serviceUri, actorId, token),
                    GetStatusTask(3, serviceUri, actorId, token),
                    GetStatusTask(4, serviceUri, actorId, token),
                    GetStatusTask(5, serviceUri, actorId, token)
                };

                await Task.WhenAll(tasks);

                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Parametri errati!");
                Console.WriteLine();
            }
        }

        private async Task BlockedOperationTask(int taskNumber, Uri serviceUri, ActorId actorId,
            string operationPayload, CancellationToken token)
        {
            var proxy = ActorProxy.Create<IClientActor>(actorId, serviceUri);
            await Task.Delay(10000, token);
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine($"\t[{DateTime.Now:HH:mm:ss.fff}] - [{taskNumber}] Actor:{actorId} --> before ExecuteBlockedOperationAsync");
                await proxy.ExecuteBlockedOperationAsync(operationPayload, token);
                Console.WriteLine($"\t[{DateTime.Now:HH:mm:ss.fff}] - [{taskNumber}] Actor:{actorId} --> after ExecuteBlockedOperationAsync");
                await Task.Delay(10000, token);
            }
        }

        private async Task GetStatusTask(int taskNumber, Uri serviceUri, ActorId actorId, CancellationToken token)
        {
            var proxy = ActorProxy.Create<IClientActor>(actorId, serviceUri);
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine($"\t[{DateTime.Now:HH:mm:ss.fff}] - [{taskNumber}] Actor:{actorId} --> before GetStatusAsync");
                var status = await proxy.GetStatusAsync(token);
                Console.WriteLine($"\t[{DateTime.Now:HH:mm:ss.fff}] - [{taskNumber}] Actor:{actorId} --> after GetStatusAsync : status {status}");
                await Task.Delay(1000, token);
            }
        }
    }
}
