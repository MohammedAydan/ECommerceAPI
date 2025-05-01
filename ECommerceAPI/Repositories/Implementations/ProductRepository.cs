using ECommerceAPI.Data;
using ECommerceAPI.Helpers;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public async Task<IEnumerable<Product>> GetAllProductsAsync(
            int? page = 1,
            int? limit = 10,
            string? search = null,
            string? sortBy = "Id",
            bool ascending = true,
            Dictionary<string, string>? filters = null)
        {
            int currentPage = page > 0 ? page.Value : 1;
            int pageSize = limit > 0 ? limit.Value : 10;

            IQueryable<Product> query = _context.Products.AsQueryable();

            // Search (applies to string fields — customize if needed)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.ProductName.Contains(search)); // Adjust "Name" to match your schema
            }

            // Apply filters
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    var propertyInfo = typeof(Product).GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null) continue;

                    var parameter = Expression.Parameter(typeof(Product), "x");
                    var property = Expression.Property(parameter, propertyInfo);

                    object? typedValue;
                    try
                    {
                        typedValue = Convert.ChangeType(filter.Value, propertyInfo.PropertyType);
                    }
                    catch
                    {
                        continue; // Skip this filter if conversion fails
                    }

                    var constant = Expression.Constant(typedValue);
                    Expression condition;

                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        condition = Expression.Call(property, containsMethod!, constant);
                    }
                    else
                    {
                        condition = Expression.Equal(property, constant);
                    }

                    var lambda = Expression.Lambda<Func<Product, bool>>(condition, parameter);
                    query = query.Where(lambda);
                }
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var propertyInfo = typeof(Product).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    var parameter = Expression.Parameter(typeof(Product), "x");
                    var property = Expression.Property(parameter, propertyInfo);
                    var sortLambda = Expression.Lambda(property, parameter);

                    string methodName = ascending ? "OrderBy" : "OrderByDescending";
                    var method = typeof(Queryable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(Product), propertyInfo.PropertyType);

                    query = (IQueryable<Product>)method.Invoke(null, new object[] { query, sortLambda })!;
                }
            }

            // Pagination
            query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
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

        public async Task<IEnumerable<Product>> GetProductsBySearchTermAsync(
            string searchTerm,
            int? page = 1,
            int? limit = 10,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            page = page.HasValue && page.Value > 0 ? page.Value : 1;
            limit = limit.HasValue && limit.Value > 0 ? limit.Value : 10;

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearchTerm = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.ProductName.ToLower().Contains(normalizedSearchTerm) ||
                    p.Description.ToLower().Contains(normalizedSearchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            return await query
                .OrderBy(p => p.ProductName)
                .Skip((page.Value - 1) * limit.Value)
                .Take(limit.Value)
                .ToListAsync();
        }

    }
}