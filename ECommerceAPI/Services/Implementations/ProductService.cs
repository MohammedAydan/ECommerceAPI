﻿using AutoMapper;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using ECommerceAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ECommerceAPI.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetTopProductsAsync(
                        int? page = 1,
                        int? limit = 10
            )
        {
            var products = await _productRepository.GetTopProductsAsync(page, limit);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int? page = 1, int? limit = 10, string? search = null, string? sortBy = "Id", bool ascending = true, Dictionary<string, string>? filters = null)
        {
            var products = await _productRepository.GetAllProductsAsync(page, limit, search, sortBy, ascending, filters);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsByCategoryIdAsync(string categoryId, int? page = 1, int? limit = 10)
        {
            var products = await _productRepository.GetAllProductsByCategoryIdAsync(categoryId, page, limit);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }


        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task AddProductAsync(ProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            await _productRepository.AddProductAsync(product);
        }

        public async Task UpdateProductAsync(ProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            await _productRepository.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsBySearchTermAsync(string searchTerm, int? page = 1, int? limit = 10, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return [];
            }
            else
            {
                var products = await _productRepository.GetProductsBySearchTermAsync(searchTerm, page, limit, categoryId, minPrice, maxPrice);
                return _mapper.Map<IEnumerable<ProductDTO>>(products);
            }
        }
    }
}