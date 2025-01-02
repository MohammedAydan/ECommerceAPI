using ECommerceAPI.Model.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task AddProductAsync(ProductDTO productDTO);
        Task UpdateProductAsync(ProductDTO productDTO);
        Task DeleteProductAsync(int id);
    }
}