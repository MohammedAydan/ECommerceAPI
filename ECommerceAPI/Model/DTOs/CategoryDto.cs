﻿using System.Text.Json.Serialization;

namespace ECommerceAPI.Model.DTOs
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public string Description { get; set; }
        //[JsonIgnore]
        public IEnumerable<CategoryDtoProductDto> Products { get; set; }
    }

    public class CategoryDtoProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
    }
}