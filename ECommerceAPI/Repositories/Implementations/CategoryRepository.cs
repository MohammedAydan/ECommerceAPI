using ECommerceAPI.Data;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(int? page = 1, int? limit = 10)
        {
            page = page.HasValue && page.Value > 0 ? page.Value : 1;
            limit = limit.HasValue && limit.Value > 0 ? limit.Value : 10;

            return await _context.Categories
                .Skip((page.Value - 1) * limit.Value)
                .Take(limit.Value)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id, bool? getMyProducts = false, int? page = 1, int? limit = 10)
        {
            page = page.HasValue && page.Value > 0 ? page.Value : 1;
            limit = limit.HasValue && limit.Value > 0 ? limit.Value : 10;

            if (getMyProducts == true)
            {
                return await _context.Categories
                    .Include(c => c.SubCategories)
                    .Include(c => c.Products)
                    .Skip((page.Value - 1) * limit.Value)
                    .Take(limit.Value)
                    .FirstOrDefaultAsync(c => c.CategoryId == id);
            }
            else
            {
                return await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryId == id);
            }
        }



        public async Task AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}