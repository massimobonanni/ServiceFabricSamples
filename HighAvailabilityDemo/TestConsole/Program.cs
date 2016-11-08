using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace TestConsole
{
    class Program
    {
        private const string ApiUrlArg = "-apiurl:";
        private const string SequenceNameArg = "-seqname:";
        private const string NumOfIterationArg = "-numiter:";
        private const string HelpArg = "-h";

        private static string ApiUrl;
        private static string SequenceName;
        private static int NumOfIteration;

        static void Main(string[] args)
        {
            if (args.Any(a => a.ToLower() == HelpArg))
            {
                WriteHelp();
                return;
            }

            RetrieveArguments(args);

            string fullApiUrl = $"{ApiUrl}/api/sequences/{SequenceName }";
            var client = new RestClient(fullApiUrl);

            for (int i = 0; i < NumOfIteration; i++)
            {
                var request = new RestRequest(Method.GET);
                IRestResponse<GetNextSequenceResponse> response = client.Execute<GetNextSequenceResponse>(request);

                Console.WriteLine($"[{response.Data.ActorId }] - Sequence {response.Data.Value } - Node {response.Data.NodeInfo }");
                Task.Delay(250).GetAwaiter().GetResult();
            }
        }

        private static void RetrieveArguments(string[] args)
        {
            var argString = args.FirstOrDefault(a => a.ToLower().StartsWith(ApiUrlArg));
            if (argString != null)
                ApiUrl = argString.ToLower().Replace(ApiUrlArg, "");
            else
                ApiUrl = "http://localhost:9050";

            var argSplit = args.FirstOrDefault(a => a.ToLower().StartsWith(SequenceNameArg))?.Split(':');
            if (argSplit != null && argSplit.Count() >= 2)
                SequenceName = argSplit[1];
            else
                SequenceName = "test";

            argSplit = args.FirstOrDefault(a => a.ToLower().StartsWith(NumOfIterationArg))?.Split(':');
            NumOfIteration = 1000;
            if (argSplit != null && argSplit.Count() >= 2)
                int.TryParse(argSplit[1], out NumOfIteration);

        }

        private static void WriteHelp()
        {
            Console.WriteLine($"{ApiUrlArg}<url> indirizzo api (es. http://localhost:9050)");
            Console.WriteLine($"{SequenceNameArg}<name> nome del sequence da testare");
            Console.WriteLine($"{NumOfIterationArg}<num> numero di iterazioni (default 1000)");
            Console.WriteLine();
        }
    }
}
