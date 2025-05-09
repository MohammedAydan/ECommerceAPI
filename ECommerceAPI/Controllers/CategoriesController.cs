﻿using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [AllowAnonymous]
        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetTopCategories([FromQuery] int? page = 1, [FromQuery] int? limit = 10)
        {
            var categories = await _categoryService.GetTopCategoriesAsync(page, limit);
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories([FromQuery] int? page = 1, [FromQuery] int? limit = 10, string? search = null, string? sortBy = "Id", bool ascending = true, Dictionary<string, string>? filters = null)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(page, limit, search, sortBy, ascending, filters);
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id,
            [FromQuery] bool? getMyProducts = false,
            [FromQuery] int? page = 1,
            [FromQuery] int? limit = 10)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id, getMyProducts, page, limit);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> AddCategory([FromForm] CategoryDTO categoryDTO)
        {
            await _categoryService.AddCategoryAsync(categoryDTO);
            return Ok(new { success = true });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryDTO categoryDTO)
        {
            if (id != categoryDTO.CategoryId)
            {
                return BadRequest();
            }

            await _categoryService.UpdateCategoryAsync(categoryDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("simple")]
        public async Task<ActionResult<IEnumerable<SimpleCategory>>> GetSimpleCategories([FromQuery] int? page = 1, [FromQuery] int? limit = 10)
        {
            var categories = await _categoryService.GetSimpleCategoriesAsync(page, limit);
            return Ok(categories);
        }
    }
}