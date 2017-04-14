using System;
using System.Diagnostics;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ProductsService.Interfaces
{
    public static class ProductExtensions
    {
        public static async Task<ServicePartitionKey> CalculatePartitionKey(this ProductDto product)
        {
            var fabricClient = new FabricClient();

            ServicePartitionList partitionList = await fabricClient.QueryManager.GetPartitionListAsync(new Uri(ServiceNames.ProductsServiceUri));
            
            var categoryValues = Enum.GetValues(typeof(ProductCategory)).Cast<int>();

            var partitionIndex = ((int)product.Category - categoryValues.Min()) * partitionList.Count / categoryValues.Count() + 1;
            Debug.WriteLine("ProductCategory={0} - PartitionIndex={1}", product.Category, partitionIndex);
            return new ServicePartitionKey(partitionIndex);
        }
    }
}