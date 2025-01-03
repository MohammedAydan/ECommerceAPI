using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int? page = 1, int? limit = 10);
        Task<IEnumerable<ProductDTO>> GetAllProductsByCategoryIdAsync(string categoryId, int? page = 1, int? limit = 10);
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task AddProductAsync(ProductDTO productDTO);
        Task UpdateProductAsync(ProductDTO productDTO);
        Task DeleteProductAsync(int id);
    }
}