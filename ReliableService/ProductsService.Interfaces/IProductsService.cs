using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace ProductsService.Interfaces
{
    public interface IProductsService : IService
    {
        Task<IEnumerable<ProductDto>> SearchProducts(string searchText);

        Task<bool> AddProduct(ProductDto product);

    }
}