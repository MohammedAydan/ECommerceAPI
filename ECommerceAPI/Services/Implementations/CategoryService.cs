using AutoMapper;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using ECommerceAPI.Services.Interfaces;
using Org.BouncyCastle.Crypto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync(
            int? page = 1,
            int? limit = 10,
            string? search = null,
            string? sortBy = "Id",
            bool ascending = true,
            Dictionary<string, string>? filters = null)
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync(page, limit, search, sortBy, ascending, filters);
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id, bool? getMyProducts = false, int? page = 1, int? limit = 10)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id, getMyProducts, page, limit);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task AddCategoryAsync(CategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Category>(categoryDTO);
            await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task UpdateCategoryAsync(CategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Category>(categoryDTO);
            await _categoryRepository.UpdateCategoryAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _categoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<IEnumerable<CategoryDTO>> GetTopCategoriesAsync(int? page = 1, int? limit = 10)
        {
            var categories = await _categoryRepository.GetTopCategoriesAsync(page, limit);
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<IEnumerable<SimpleCategory>> GetSimpleCategoriesAsync(int? page = 1, int? limit = 10)
        {
            var categories = await _categoryRepository.GetSimpleCategoriesAsync(page, limit);
            return _mapper.Map<IEnumerable<SimpleCategory>>(categories);
        }
    }
}