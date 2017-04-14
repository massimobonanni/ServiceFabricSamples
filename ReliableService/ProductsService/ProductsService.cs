using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using ProductsService.Interfaces;

namespace ProductsService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductsService : StatefulService, IProductsService
    {
        private const string ProductListName = "products";

        public ProductsService(StatefulServiceContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see http://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(this.CreateServiceRemotingListener) };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        private Int64RangePartitionInformation GetPartitionInfo()
        {
            return this.Partition.PartitionInfo as Int64RangePartitionInformation;
        }

        private string GetProductListStateName()
        {
            var pInfo = GetPartitionInfo();
            return pInfo.LowKey.ToString();
        }

        private async Task<List<ProductDto>> GetProductListAsync(ITransaction tx)
        {
            var productListStateName = GetProductListStateName();
            var state = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, List<ProductDto>>>(ProductListName);
            return await state.GetOrAddAsync(tx, productListStateName, a => new List<ProductDto>());
        }

        private async Task SetProductListAsync(ITransaction tx, List<ProductDto> products)
        {
            var productListStateName = GetProductListStateName();
            var state = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, List<ProductDto>>>(ProductListName);
            await state.AddOrUpdateAsync(tx, productListStateName, a => products, (a, l) => products);
        }

        public async Task<IEnumerable<ProductDto>> SearchProducts(string searchText)
        {
            IEnumerable<ProductDto> result = null;
            using (var tx = this.StateManager.CreateTransaction())
            {
                var productList = await GetProductListAsync(tx);

                result = productList;
                if (searchText != null)
                    result = result.Where(p => p.Description.ToLower().Contains(searchText.ToLower()));

                await tx.CommitAsync();
            }
            return result;
        }

        public async Task<bool> AddProduct(ProductDto product)
        {
            ServiceEventSource.Current.ServiceMessage(this, "AddProduct", product);

            using (var tx = this.StateManager.CreateTransaction())
            {
                var productList = await GetProductListAsync(tx);
                if (productList.Any(p => p.Code == product.Code))
                {
                    var localProduct = productList.First(p => p.Code == product.Code);
                    localProduct.Description = product.Description;
                    localProduct.StoreUnit += product.StoreUnit;
                    localProduct.UnitCost = product.UnitCost;
                }
                else
                {
                    productList.Add(product);
                }

                await SetProductListAsync(tx,productList);
                await tx.CommitAsync();
            }
            return true;
        }
    }
}
