using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyActor
{
    public class ThrottlingManager : IThrottlingManager
    {
        private class ThrottlingRule
        {
            public DateTime? LastCallTime { get; set; }
            public long CallsPerSeconds { get; set; }

            public bool CheckThrottle()
            {
                var now = DateTime.Now;
                if (LastCallTime.HasValue)
                {
                    var delta = now.Subtract(LastCallTime.Value );
                    var rate = 1 / delta;
                    
                }
            }

        }

        private IDictionary<string, ThrottlingRule> Rules = new Dictionary<string, ThrottlingRule>();

        public IThrottlingManager AddThrottlingRule(string methodName, int callsPerSecond, Func<Task> trhottlingLambda)
        {
            throw new NotImplementedException();
        }

        public async Task Check(string method = null)
        {
            throw new NotImplementedException();
        }
    }
}
