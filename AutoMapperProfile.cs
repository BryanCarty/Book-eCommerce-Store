using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.DTOs.Products;
using Book_eCommerce_Store.DTOs.Purchases;

namespace Book_eCommerce_Store
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, GetProductDTO>();
            CreateMap<CreateProductDTO, Product>();
            CreateMap<Purchase, GetPurchaseDTO>();
            CreateMap<CreatePurchaseDTO, Purchase>();
        }     
    }
}