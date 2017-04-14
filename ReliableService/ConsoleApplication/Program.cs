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
using ProductsService.Interfaces;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateNewProducts(10000);

            var fabricClient = new FabricClient();
            ServicePartitionList partitionList = fabricClient.QueryManager.GetPartitionListAsync(new Uri(ServiceNames.ProductsServiceUri)).GetAwaiter().GetResult();
            var partitionInfos =
                partitionList.Select(p => p.PartitionInformation).OfType<Int64RangePartitionInformation>();

            var searchTasks = new List<Task<IEnumerable<ProductDto>>>();
            foreach (var pInfo in partitionInfos)
            {
                var client = ServiceProxy.Create<IProductsService>(
                    new Uri(ServiceNames.ProductsServiceUri),
                    new ServicePartitionKey(pInfo.LowKey));

                searchTasks.Add(client.SearchProducts("lorem"));
            }
            Task.WhenAll(searchTasks).GetAwaiter().GetResult();

            var resultList = searchTasks.SelectMany(a => a.Result);

            foreach (var item in resultList)
            {
                Console.WriteLine($"{item.Code} - {item.Description } - {item.Category } - {item.CalculatePartitionKey().GetAwaiter().GetResult().Value }");
            }

            Console.ReadLine();
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

            foreach (var product in products)
            {
                Console.WriteLine("[ADD] {0} - {1} - {2}", product.Code, product.Description, product.Category);
                var proxy = ServiceProxy.Create<IProductsService>(
                    new Uri(ServiceNames.ProductsServiceUri),
                    product.CalculatePartitionKey().GetAwaiter().GetResult());

                var result = proxy.AddProduct(product).GetAwaiter().GetResult();
            }
        }
    }
}
