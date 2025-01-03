using ECommerceAPI.Model.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync(int? page = 1, int? limit = 10);
        Task<CategoryDTO> GetCategoryByIdAsync(int id, bool? getMyProducts = false, int? page = 1, int? limit = 10);
        Task AddCategoryAsync(CategoryDTO categoryDTO);
        Task UpdateCategoryAsync(CategoryDTO categoryDTO);
        Task DeleteCategoryAsync(int id);
    }
}