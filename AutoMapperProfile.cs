using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.DTOs.Products;
using Book_eCommerce_Store.DTOs.Users;
using Book_eCommerce_Store.Data.Entities;
using Book_eCommerce_Store.DTOs.Orders;

namespace Book_eCommerce_Store
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, GetUserDTO>();
            CreateMap<CreateUserDTO, User>();
            CreateMap<PRODUCT, Book>();
            CreateMap<CreateProductDTO, PRODUCT>();
            CreateMap<PRODUCT, Stationary>();
            CreateMap<Book, PRODUCT>();
            CreateMap<Stationary, PRODUCT>();
            CreateMap<Order, GetOrderDTO>();
            CreateMap<CreateOrderDTO, Order>();
            CreateMap<UpdateOrderDTO, Order>();
            CreateMap<PRODUCT, Purchase>();
        }     
    }
}