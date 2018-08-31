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

        private IReliableDictionary<string, ProductDto> products;

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
            await EnsureProductList();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        private async Task EnsureProductList()
        {
            if (products == null)
                products = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, ProductDto>>("products");
        }

        public async Task<IEnumerable<ProductDto>> SearchProducts(string searchText)
        {
            await EnsureProductList();
            List<ProductDto> result = new List<ProductDto>();
            IAsyncEnumerable<KeyValuePair<string, ProductDto>> productList = null;
            using (var tx = this.StateManager.CreateTransaction())
            {
                productList = await products.CreateEnumerableAsync(tx);
                var productsEnumerator = productList.GetAsyncEnumerator();

                while (await productsEnumerator.MoveNextAsync(default(CancellationToken)))
                {
                    var product = productsEnumerator.Current.Value;
                    if (searchText == null)
                    {
                        result.Add(product);
                    }
                    else if (product.Description.ToLower().Contains(searchText.ToLower()))
                    {
                        result.Add(product);
                    }
                }

                await tx.CommitAsync();
            }
            return result;
        }

        private async Task<ProductDto> GetProductAsync(string productCode)
        {
            ProductDto productDto = null;
            using (var tx = this.StateManager.CreateTransaction())
            {
                var product = await products.TryGetValueAsync(tx, productCode);
                if (product.HasValue)
                    productDto = product.Value;
            }
            return productDto;
        }

        public async Task<bool> AddProduct(ProductDto product)
        {
            await EnsureProductList();
            ServiceEventSource.Current.ServiceMessage(this, "AddProduct", product);

            ProductDto productDto = await GetProductAsync(product.Code);
            if (productDto == null)
            {
                using (var tx = this.StateManager.CreateTransaction())
                {
                    await products.AddAsync(tx, product.Code, product);
                    await tx.CommitAsync();
                }
            }
            return true;
        }


        public async Task AddProduct(string key, ProductDto value, CancellationToken cancellationToken)
        {
            bool retry = false;
            do
            {
                try
                {
                    using (ITransaction tx = base.StateManager.CreateTransaction())
                    {
                        await products.AddAsync(tx, key, value, TimeSpan.FromSeconds(5), cancellationToken);

                        await tx.CommitAsync();
                    }
                    retry = true;
                }
                catch (TimeoutException)
                {
                    await Task.Delay(100, cancellationToken);
                    retry = true;
                }
            } while (retry);

        }
    }
}
