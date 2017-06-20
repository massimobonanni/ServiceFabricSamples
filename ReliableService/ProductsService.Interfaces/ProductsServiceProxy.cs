using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace ProductsService.Interfaces
{
    public class ProductsServiceProxy
    {
        private static Uri serviceUri;
        private static ProductsServiceProxy instance = null;
        private static List<Int64RangePartitionInformation> partitionInfoList = null;

        private static readonly object singletonLock = new object();

        static ProductsServiceProxy()
        {
            serviceUri = new Uri(ServiceNames.ProductsServiceUri);
        }

        public static ProductsServiceProxy Instance
        {
            get
            {
                lock (singletonLock)
                {
                    return instance ?? (instance = new ProductsServiceProxy());
                }
            }
        }

        private async Task EnsurePartitionCount()
        {
            if (partitionInfoList == null || !partitionInfoList.Any())
            {
                using (var client = new FabricClient())
                {
                    var partitionList = await client.QueryManager.GetPartitionListAsync(serviceUri);

                    partitionInfoList = partitionList.Select(p => p.PartitionInformation)
                        .OfType<Int64RangePartitionInformation>().ToList();
                }
            }
        }

        private async Task<ServicePartitionKey> CalculatePartitionKey(ProductDto product)
        {
            await EnsurePartitionCount();
            var categoryValues = Enum.GetValues(typeof(ProductCategory)).Cast<int>();
            ServicePartitionKey partitionKey = null;
            foreach (var partition in partitionInfoList)
            {
                if ((int)product.Category >= partition.LowKey && (int)product.Category <= partition.HighKey)
                {
                    partitionKey = new ServicePartitionKey(partition.LowKey);
                    break;
                }
            }
            return partitionKey;
        }

        private async Task<IProductsService> CreateServiceProxy(ProductDto product)
        {
            var partitionKey = await CalculatePartitionKey(product);
            return ServiceProxy.Create<IProductsService>(serviceUri, partitionKey);
        }

        public async Task<IEnumerable<ProductDto>> SearchProducts(string searchText)
        {
            await EnsurePartitionCount();
            var taskList = new List<Task<IEnumerable<ProductDto>>>();
            foreach (var partition in partitionInfoList)
            {
                var srvPartitionKey = new ServicePartitionKey(partition.LowKey);
                var proxy = ServiceProxy.Create<IProductsService>(serviceUri, srvPartitionKey);
                taskList.Add(proxy.SearchProducts(searchText));
            }
            await Task.WhenAll(taskList);

            var result = taskList.SelectMany(t => t.Result).ToList();
            return result;
        }

        public async Task<bool> AddProduct(ProductDto product)
        {
            var proxy = await CreateServiceProxy(product);
            return await proxy.AddProduct(product); ;
        }
    }
}
