using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using TestConsole.Commands;

namespace TestConsole
{
    class Program
    {
        private static readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>()
        {
            {"callbackoperation", new CallbackOperationCommand()},
            {"blockoperation", new BlockedOperationCommand()},
            {"statemanagement", new StateManagementCommand()}
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

        private static void ExecuteCommand(string[] args, ICommand command)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            ConsoleKeyInfo key;
            var task = command.ExuteAsync(args, tokenSource.Token);
            do
            {
                key = Console.ReadKey();
            } while (key.Key != ConsoleKey.Escape && !task.IsCompleted);

            tokenSource.Cancel();
            Console.WriteLine();
        }

    }
}
