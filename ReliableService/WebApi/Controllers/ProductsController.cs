using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using ProductsService.Interfaces;

namespace WebApi.Controllers
{
    [ServiceRequestActionFilter]
    public class ProductsController : ApiController
    {
        // GET api/values 
        public IEnumerable<ProductDto> Get([FromUri] string searchText)
        {
            var result = ProductsServiceProxy.Instance.SearchProducts(searchText).GetAwaiter().GetResult();
            return result;
        }

        public bool Post(ProductDto product)
        {
            if (product == null)
                this.InternalServerError();
            var result = ProductsServiceProxy.Instance.AddProduct(product).GetAwaiter().GetResult();
            return result;
        }
    }
}
