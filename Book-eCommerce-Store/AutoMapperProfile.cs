using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.DTOs.Products;
using Book_eCommerce_Store.DTOs.Users;
using Book_eCommerce_Store.DTOs.Purchases;
using Book_eCommerce_Store.Data.Entities;

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
            CreateMap<PRODUCT, Product>();
            CreateMap<Purchase, GetPurchaseDTO>();
            CreateMap<CreatePurchaseDTO, Purchase>();
        }     
    }
}