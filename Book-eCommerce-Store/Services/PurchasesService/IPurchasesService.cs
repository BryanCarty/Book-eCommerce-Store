using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.DTOs.Purchases;
namespace Book_eCommerce_Store.Services.PurchasesService
{
    public interface IPurchasesService
    {

        Task<Response> CreatePurchase(CreatePurchaseDTO newPurchase);

        Task<Response> ReturnPurchase(int product_id, int purchase_id);
    }
}