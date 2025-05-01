using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(
                    int? page = 1,
                    int? limit = 10,
                    string? search = null,
                    string? sortBy = "Id",
                    bool ascending = true,
                    Dictionary<string, string>? filters = null
            );
        Task<Category?> GetCategoryByIdAsync(int id, bool? getMyProducts = false,int? page = 1,int? limit = 10);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);

        Task<IEnumerable<Category>> GetTopCategoriesAsync(int? page = 1, int? limit = 10);
        Task<IEnumerable<SimpleCategory>> GetSimpleCategoriesAsync(int? page = 1, int? limit = 10);
    }
}