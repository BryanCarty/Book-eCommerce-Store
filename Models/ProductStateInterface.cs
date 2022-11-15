using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.data;
using Book_eCommerce_Store.DTOs.Purchases;

namespace Book_eCommerce_Store.Models
{
    public interface ProductStateInterface
    {
    // Different states expected - sold out, in stock, reStockedSoon
        public Task<Response<GetPurchaseDTO>> purchaseProduct(CreatePurchaseDTO purchase, DataContext context, IMapper mapper); //user wants to purchase the product. i.e. quantity in db is decreased.

        public Task<Response<GetPurchaseDTO>> returnProductAsync(int purchaseId, DataContext context, IMapper mapper); //user wants to return the product they previously bought. i.e. quantity in db increases.
    }
}