using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsole.Commands
{
    public interface ICommand
    {
        Task ExuteAsync(string[] arguments, CancellationToken token = default(CancellationToken));
    }
}