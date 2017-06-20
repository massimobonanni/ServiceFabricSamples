using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;
using ProductsService.Interfaces;
using RestSharp;

namespace ConsoleApplication
{
    class Program
    {
        private const string ApiUrlArg = "-apiurl:";
        private const string CommandArg = "-command:";
        private const string NumberOfProductArg = "-numprod:";
        private const string SearchTextArg = "-searchtext:";
        private const string HelpArg = "-h";


        private static string ApiUrl;
        private static string Command;
        private static int NumberOfProduct;
        private static string SearchText;

        static void Main(string[] args)
        {
            if (args.Any(a => a.ToLower() == HelpArg))
            {
                WriteHelp();
                return;
            }

            RetrieveArguments(args);

            if (Command == "addproducts")
            {
                CreateNewProducts(NumberOfProduct);
            }
            else if (Command == "search")
            {
                SearchProducts(SearchText);
            }

            Console.ReadLine();
        }

        private static void SearchProducts(string searchText)
        {
            string fullApiUrl = $"{ApiUrl}/api/products?searchText={searchText}";
            var client = new RestClient(fullApiUrl);
            var request = new RestRequest(Method.GET);
            var response = client.Execute<List<ProductDto>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                foreach (var item in response.Data)
                {
                    Console.WriteLine(
                        $"{item.Code} - {item.Description} - {item.Category} - {item.CalculatePartitionKey().GetAwaiter().GetResult().Value}");
                }
            }
            else
            {
                Console.WriteLine(response.ErrorMessage);
            }
        }

        private static void RetrieveArguments(string[] args)
        {
            var argString = args.FirstOrDefault(a => a.ToLower().StartsWith(ApiUrlArg));
            if (argString != null)
                ApiUrl = argString.ToLower().Replace(ApiUrlArg, "");
            else
                ApiUrl = "http://localhost:8282";

            var argSplit = args.FirstOrDefault(a => a.ToLower().StartsWith(CommandArg))?.Split(':');
            if (argSplit != null && argSplit.Count() >= 2)
                Command = argSplit[1].ToLower();
            else
                Command = "search";

            argSplit = args.FirstOrDefault(a => a.ToLower().StartsWith(SearchTextArg))?.Split(':');
            if (argSplit != null && argSplit.Count() >= 2)
                SearchText = argSplit[1];
            else
                SearchText = null;

            argSplit = args.FirstOrDefault(a => a.ToLower().StartsWith(NumberOfProductArg))?.Split(':');
            NumberOfProduct = 1000;
            if (argSplit != null && argSplit.Count() >= 2)
                int.TryParse(argSplit[1], out NumberOfProduct);

        }

        private static void WriteHelp()
        {
            Console.WriteLine($"{ApiUrlArg}<url> indirizzo api (es. http://localhost:8282)");
            Console.WriteLine($"{CommandArg}<command> comando da eseguire [addproducts, search]");
            Console.WriteLine($"{NumberOfProductArg}<num> numero di prodottida aggiungere (default 1000)");
            Console.WriteLine($"{SearchTextArg}<text> testo da ricercare");
            Console.WriteLine();
        }

        private static void CreateNewProducts(int numProducts)
        {
            var products = Builder<ProductDto>
                .CreateListOfSize(numProducts)
                .All()
                    .With(a => a.Code = Guid.NewGuid().ToString())
                    .With(a => a.Description = Faker.Lorem.Sentence(2))
                    .With(a => a.StoreUnit = Faker.RandomNumber.Next(0, 1000))
                    .With(a => a.UnitCost = Faker.RandomNumber.Next(1, 100))
                .Build();

            string fullApiUrl = $"{ApiUrl}/api/products";
            var client = new RestClient(fullApiUrl);

            foreach (var product in products)
            {
                var jsonString = JsonConvert.SerializeObject(product);
                var request = new RestRequest(Method.POST);
                request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
                IRestResponse<bool> response = client.Execute<bool>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response.Data)
                    {
                        Console.WriteLine("ADDED {0} - {1} - {2}", product.Code, product.Description, product.Category);
                    }
                    else
                    {
                        Console.WriteLine("ERROR ADDING {0} - {1} - {2}", product.Code, product.Description, product.Category);
                    }
                }
                else
                {
                    Console.WriteLine(response.ErrorMessage);
                }
            }
        }
    }
}
