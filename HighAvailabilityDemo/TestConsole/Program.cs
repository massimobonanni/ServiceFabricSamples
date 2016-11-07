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
        static void Main(string[] args)
        {
            string apiUrl = "http://localhost:9050/api/sequences/test";
            if (args != null && args.Count() > 0)
            {
                apiUrl = args[0];
            }

            for (int i = 0; i < 10000; i++)
            {
                var client = new RestClient(apiUrl);
                var request = new RestRequest(Method.GET);
                IRestResponse<GetNextSequenceResponse> response = client.Execute<GetNextSequenceResponse>(request);

                Console.WriteLine($"[{response.Data.ActorId }] - Sequence {response.Data.Value } - Node {response.Data.NodeInfo }");
                Task.Delay(100).GetAwaiter().GetResult();

            }
        }
    }
}
