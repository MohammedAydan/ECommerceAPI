using ECommerceAPI.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(
            int? page = 1,
            int? limit = 10,
            string? search = null,
            string? sortBy = "Id",
            bool ascending = true,
            Dictionary<string, string>? filters = null);
        Task<IEnumerable<Product>> GetAllProductsByCategoryIdAsync(string categoryId, int? page = 1, int? limit = 10);
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        Task<IEnumerable<Product>> GetTopProductsAsync(int? page = 1, int? limit = 10);
        Task<IEnumerable<Product>> GetProductsBySearchTermAsync(string searchTerm, int? page = 1, int? limit = 10, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice= null);
    }
}
