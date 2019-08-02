using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using ProductsService.Interfaces;

namespace WebApi.Controllers
{
    [ServiceRequestActionFilter]
    public class ProductsController : ApiController
    {
        [Route("api/products")]
        [HttpGet()]
        public IEnumerable<ProductDto> SearchProducts([FromUri] string searchText)
        {
            var result = ProductsServiceProxy.Instance.SearchProducts(searchText).GetAwaiter().GetResult();
            return result;
        }

        [Route("api/products")]
        [HttpPost()]
        public bool AddProduct(ProductDto product)
        {
            if (product == null)
                this.InternalServerError();
            var result = ProductsServiceProxy.Instance.AddProduct(product).GetAwaiter().GetResult();
            return result;
        }

        [Route("api/products/{numItems}")]
        [HttpPost()]
        public bool AddItems(int numItems)
        {
            
            return true;
        }
    }
}
