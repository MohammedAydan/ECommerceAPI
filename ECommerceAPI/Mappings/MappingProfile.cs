using AutoMapper;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;

namespace ECommerceAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, CategoryDtoProductDto>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<OrderItem, OrderItemDTO>().ReverseMap();
            CreateMap<Cart, CartDTO>().ReverseMap();
            CreateMap<CartItem, CartItemDTO>().ReverseMap();

            CreateMap<CartItemDTO, OrderItemDTO>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product!.Price))
                .ReverseMap();

        }
    }
}