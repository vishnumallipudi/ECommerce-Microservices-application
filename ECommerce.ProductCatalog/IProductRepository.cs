using eCommerce.ProductCatalog.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.ProductCatalog
{
    /// <summary>
    /// Interface for Service Fabric to add and retrieve products
    /// </summary>
    interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();

        Task AddProduct(Product product);
    }
}
