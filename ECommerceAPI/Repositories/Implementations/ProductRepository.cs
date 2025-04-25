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
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly UploadImagesHelper _imagesHelper;

        public ProductRepository(AppDbContext context, UploadImagesHelper imagesHelper)
        {
            _context = context;
            _imagesHelper = imagesHelper;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(int? page = 1, int? limit = 10)
        {
            return await GetPaginatedProducts(page, limit);
        }

        public async Task<IEnumerable<Product>> GetAllProductsByCategoryIdAsync(string categoryId, int? page = 1, int? limit = 10)
        {
            return await GetPaginatedProducts(page, limit, categoryId);
        }

        private async Task<IEnumerable<Product>> GetPaginatedProducts(int? page, int? limit, string? categoryId = null)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                query = query.Where(p => p.CategoryId.ToString() == categoryId);
            }

            if (page.HasValue && page.Value > 0)
            {
                query = query.Skip((page.Value - 1) * (limit ?? 10)).Take(limit ?? 10);
            }

            return await query
                //.Include(p => p.Category)
                .ToListAsync();
        }


        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                //.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task AddProductAsync(Product product)
        {
            if (product.Image != null)
            {
                var fileName = await _imagesHelper.UploadImageAsync(product.Image, "products");
                product.ImageUrl = fileName;
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // increment a ItemsCount for category
            var category = await _context.Categories.FindAsync(product.CategoryId);
            if (category != null)
            {
                category.ItemsCount++;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.Image != null)
            {
                if (!string.IsNullOrWhiteSpace(product.ImageUrl) && _imagesHelper.ImageExists(product.ImageUrl))
                {
                    _imagesHelper.DeleteImage(product.ImageUrl);
                }

                string fileName = await _imagesHelper.UploadImageAsync(product.Image, "products");
                product.ImageUrl = fileName;
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                // decrement a ItemsCount for category
                var category = await _context.Categories.FindAsync(product.CategoryId);
                if (category != null) {
                    category.ItemsCount--;
                    _context.Categories.Update(category);
                    await _context.SaveChangesAsync();
                }

                if (product.ImageUrl is not null && _imagesHelper.ImageExists(product.ImageUrl))
                {
                    _imagesHelper.DeleteImage(product.ImageUrl);
                }
            }
        }

        public async Task<IEnumerable<Product>> GetTopProductsAsync(int? page = 1, int? limit = 10)
        {
            page = page.HasValue && page.Value > 0 ? page.Value : 1;
            limit = limit.HasValue && limit.Value > 0 ? limit.Value : 10;

            return await _context.Products
                .OrderByDescending(p => p.CartAddedCount)
                .ThenByDescending(p => p.CreatedOrderCount)
                .Skip((page.Value - 1) * limit.Value)
                .Take(limit.Value)
                .ToListAsync();
        }

    }
}