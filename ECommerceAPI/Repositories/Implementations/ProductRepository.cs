using ECommerceAPI.Data;
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

        public ProductRepository(AppDbContext context)
        {
            _context = context;
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

            return await query.Include(p => p.Category).ToListAsync();
        }


        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
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
            }
        }
    }
}