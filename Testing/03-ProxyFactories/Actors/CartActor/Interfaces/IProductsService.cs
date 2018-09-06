using System;
using System.Collections.Generic;
using System.Fabric.Description;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CartActor.Interfaces
{
    /// <summary>
    /// Define the behaviour for the service that manage products
    /// </summary>
    internal interface IProductsService
    {
        /// <summary>
        /// Gets the product information asynchronous.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;ProductData&gt;.</returns>
        Task<ProductData> GetProductInfoAsync(string productId, double quantity, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <param name="settings">The settings.</param>
        void SetConfiguration(ConfigurationSettings settings);
    }
}
