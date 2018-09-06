using CartActor.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Fabric.Description;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CartActor.Implementations
{
    /// <summary>
    /// <see cref="IProductsService"/> implementation using SQL Data Access.
    /// </summary>
    /// <seealso cref="CartActor.Interfaces.IProductsService" />
    internal class SQLProductsService : IProductsService
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// get product information as an asynchronous operation.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;ProductData&gt;.</returns>
        public async Task<ProductData> GetProductInfoAsync(string productId, double quantity,
            CancellationToken cancellationToken)
        {
            ProductData result = null;
            // This method implements the data access (e.g. using ADO.NET to retrieve data from the database)
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand("CheckProduct", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ProductId", productId));
                    cmd.Parameters.Add(new SqlParameter("@Quantity", quantity));

                    using (SqlDataReader sqlDataReader = await cmd.ExecuteReaderAsync(cancellationToken))
                    {
                        if (await sqlDataReader.ReadAsync(cancellationToken))
                        {
                            result = new ProductData();
                            result.Id = productId;
                            result.Quantity = quantity;
                            result.UnitCost = sqlDataReader.GetFieldValue<decimal>(0);
                        }
                    }
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException">settings</exception>
        public void SetConfiguration(ConfigurationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.Sections.Contains("SqlDataAccess"))
            {
                var dataAccessSection = settings.Sections["SqlDataAccess"];
                if (dataAccessSection.Parameters.Contains("ConnectionString"))
                {
                    this.ConnectionString = dataAccessSection.Parameters["ConnectionString"].Value;
                }
            }
        }
    }
}
