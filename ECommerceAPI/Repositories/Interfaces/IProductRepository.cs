using ECommerceAPI.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(int? page = 1, int? limit = 10);
        Task<IEnumerable<Product>> GetAllProductsByCategoryIdAsync(string categoryId, int? page = 1, int? limit = 10);
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}
