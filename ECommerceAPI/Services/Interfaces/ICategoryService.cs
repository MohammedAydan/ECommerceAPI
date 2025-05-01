using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync(
                    int? page = 1,
                    int? limit = 10,
                    string? search = null,
                    string? sortBy = "Id",
                    bool ascending = true,
                    Dictionary<string, string>? filters = null
            );
        Task<CategoryDTO> GetCategoryByIdAsync(int id, bool? getMyProducts = false, int? page = 1, int? limit = 10);
        Task AddCategoryAsync(CategoryDTO categoryDTO);
        Task UpdateCategoryAsync(CategoryDTO categoryDTO);
        Task DeleteCategoryAsync(int id);

        Task<IEnumerable<CategoryDTO>> GetTopCategoriesAsync(int? page = 1, int? limit = 10);
        Task<IEnumerable<SimpleCategory>> GetSimpleCategoriesAsync(int? page = 1, int? limit = 10);
    }
}