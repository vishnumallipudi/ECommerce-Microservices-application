using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eCommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace ECommerce.ProductCatalog
{
    class ServiceFabricProductRepository : IProductRepository
    {
        private readonly IReliableStateManager _stateManager;
        public ServiceFabricProductRepository(IReliableStateManager reliableStateManager)
        {
            _stateManager = reliableStateManager;
        }

        public async Task AddProduct(Product product)
        {
            var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            using (var tx=_stateManager.CreateTransaction())
            {
                await products.AddOrUpdateAsync(tx,product.Id,product,(id,value)=>product);
                tx.CommitAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            var result =new List<Product>();
            using (var tx=_stateManager.CreateTransaction())
            {
                var allProducts = await products.CreateEnumerableAsync(tx,EnumerationMode.Unordered);
                using (var enumerator=allProducts.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, Product> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }
            return result;
        }
    }
}
