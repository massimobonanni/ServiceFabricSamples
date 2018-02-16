using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyActor
{
    public interface IThrottlingManager
    {
        IThrottlingManager AddThrottlingRule(string methodName, int callsPerSecond, Func<Task> trhottlingLambda);


        Task Check([CallerMemberName] string method = null);
    }
}