using ECommerceAPI.Model.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(CategoryDTO categoryDTO);
        Task UpdateCategoryAsync(CategoryDTO categoryDTO);
        Task DeleteCategoryAsync(int id);
    }
}