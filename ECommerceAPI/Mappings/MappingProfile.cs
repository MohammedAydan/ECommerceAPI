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

            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Ignore null members

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ReverseMap();

            CreateMap<Cart, CartDTO>().ReverseMap();
            CreateMap<CartItem, CartItemDTO>().ReverseMap();

            // If necessary, explicitly map other items, e.g., CartItemDTO -> OrderItemDTO
            CreateMap<CartItemDTO, OrderItemDTO>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product!.Price))
                .ReverseMap();

            CreateMap<UpdateUserDto, UserModel>().ReverseMap();
            CreateMap<UserModel, UserDto>().ReverseMap();
        }
    }
}