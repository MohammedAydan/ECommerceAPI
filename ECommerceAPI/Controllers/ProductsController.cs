using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts(
            [FromQuery] int? page = 1,
            [FromQuery] int? limit = 10,
            string? search = null, string? sortBy = "Id", bool ascending = true, Dictionary<string, string>? filters = null)
        {
            var products = await _productService.GetAllProductsAsync(page, limit, search, sortBy, ascending, filters);
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetTopProducts(
            [FromQuery] int? page = 1,
            [FromQuery] int? limit = 10)
        {
            var products = await _productService.GetTopProductsAsync(page, limit);
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("category/{id}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByCategoryId(
            string id,
            [FromQuery] int? page = 1,
            [FromQuery] int? limit = 10)
        {
            var products = await _productService.GetAllProductsByCategoryIdAsync(id, page, limit);
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> AddProduct([FromForm] ProductDTO productDTO)
        {
            await _productService.AddProductAsync(productDTO);
            return Ok(new { success = true });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id,[FromForm] ProductDTO productDTO)
        {
            if (id != productDTO.ProductId)
            {
                return BadRequest();
            }

            await _productService.UpdateProductAsync(productDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> SearchProducts(
                [FromQuery] string searchTerm,
                [FromQuery] int? page = 1,
                [FromQuery] int? limit = 10,
                [FromQuery] int? categoryId = null,
                [FromQuery] decimal? minPrice = null,
                [FromQuery] decimal? maxPrice = null
            )
        {
            var products = await _productService.GetProductsBySearchTermAsync(searchTerm, page, limit, categoryId, minPrice, maxPrice);
            return Ok(products);
        }
    }
}