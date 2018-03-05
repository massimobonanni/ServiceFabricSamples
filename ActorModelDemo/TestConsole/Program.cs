using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace TestConsole
{
    class Program
    {
        private static readonly Dictionary<string, Func<string[], CancellationToken, Task>> commands = new Dictionary<string, Func<string[], CancellationToken, Task>>()
        {
            {"callbackoperation", CallbackOperationAsync}
        };

        static void Main(string[] args)
        {
            if (!args.Any())
            {
                ShowCommands();
            }
            else if (commands.ContainsKey(args[0]))
            {
                var command = commands[args[0]];
                ExecuteCommand(args, command);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Comando '{args[0]}' sconosciuto");
                Console.WriteLine();
            }

            Console.ResetColor();
        }

        private static void ShowCommands()
        {
            Console.WriteLine("Comandi disponibili:");
            foreach (var key in commands.Keys)
            {
                Console.WriteLine($"\t{key}");
            }

            Console.WriteLine();
        }

        private static void ExecuteCommand(string[] args, Func<string[], CancellationToken, Task> command)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            ConsoleKeyInfo key;
            var task = command(args, tokenSource.Token);
            do
            {
                key = Console.ReadKey();
            } while (key.Key != ConsoleKey.Escape && !task.IsCompleted);

            tokenSource.Cancel();
            Console.WriteLine();
        }

        private static async Task CallbackOperationAsync(string[] args, CancellationToken token)
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
