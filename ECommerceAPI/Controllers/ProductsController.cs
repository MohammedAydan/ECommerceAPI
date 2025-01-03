using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts(
            [FromQuery] int? page = 1,
            [FromQuery] int? limit = 10)
        {
            var products = await _productService.GetAllProductsAsync(page, limit);
            return Ok(products);
        }

        [HttpGet("category/{id}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByCategoryId(
            string id,
            [FromQuery] int? page = 1,
            [FromQuery] int? limit = 10)
        {
            var products = await _productService.GetAllProductsByCategoryIdAsync(id, page, limit);
            return Ok(products);
        }

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
        public async Task<ActionResult<ProductDTO>> AddProduct(ProductDTO productDTO)
        {
            await _productService.AddProductAsync(productDTO);
            return CreatedAtAction(nameof(GetProduct), new { id = productDTO.ProductId }, productDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDTO productDTO)
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
    }
}
