﻿using ECommerceAPI.Model.Entities;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Model.DTOs
{
    public class CartItemDTO
    {
        //[Key]
        public int? CartItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public virtual Product? Product { get; set; }
    }
}