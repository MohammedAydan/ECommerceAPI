using ECommerceAPI.Data;
using ECommerceAPI.Helpers;
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
        private readonly UploadImagesHelper _imagesHelper;

        public CategoryRepository(AppDbContext context, UploadImagesHelper imagesHelper)
        {
            _context = context;
            _imagesHelper = imagesHelper;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(int? page = 1, int? limit = 10)
        {
            page = page.HasValue && page.Value > 0 ? page.Value : 1;
            limit = limit.HasValue && limit.Value > 0 ? limit.Value : 10;

            return (await _context.Categories
                .Skip((page.Value - 1) * limit.Value)
                .Take(limit.Value)
                .ToListAsync());
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
            if(category.Image != null)
            {
                category.ImageUrl = await _imagesHelper.UploadImageAsync(category.Image,"categories");
            }
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            if (category.Image is not null)
            {
                if (category.ImageUrl is not null && _imagesHelper.ImageExists(category.ImageUrl))
                {
                    _imagesHelper.DeleteImage(category.ImageUrl);
                }
                category.ImageUrl = await _imagesHelper.UploadImageAsync(category.Image, "categories");
            }
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
                if (category.ImageUrl != null && _imagesHelper.ImageExists(category.ImageUrl))
                {
                    _imagesHelper.DeleteImage(category.ImageUrl);
                }
            }
        }

        public async Task<IEnumerable<Category>> GetTopCategoriesAsync(int? page = 1, int? limit = 10)
        {
            // Validate and normalize input parameters
            int pageNumber = page.GetValueOrDefault(1);
            int pageSize = limit.GetValueOrDefault(10);

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            // Get top category IDs with their scores in a single efficient query
            var topCategories = await _context.Categories
                .Select(c => new
                {
                    Category = c,
                    Score = c.Products.Sum(p => p.CartAddedCount ?? 0),
                    ProductCount = c.Products.Count
                })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.ProductCount)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.Category)
                .ToListAsync();

            return topCategories;
        }

    }
}