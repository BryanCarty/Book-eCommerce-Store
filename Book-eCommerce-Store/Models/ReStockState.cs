using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.Data;
using Book_eCommerce_Store.DTOs.Purchases;
using Microsoft.EntityFrameworkCore;

namespace Book_eCommerce_Store.Models
{
    public class ReStockState : ProductStateInterface
    {

        private Product product;

        public ReStockState(Product product){
            this.product = product;
        }
        public async Task<Response> purchaseProduct(CreatePurchaseDTO purchase, DataContext context, IMapper mapper)
        {
            var response = new Response();
            response.Success=true;
            response.Message = "We are currently restocking this product. It'll be available shortly";
            return response;
        }

        public async Task<Response> returnProductAsync(int purchaseId, DataContext context, IMapper mapper)
        {
            var response = new Response();
            Purchase purchase = await context.Purchases.FirstOrDefaultAsync(p => p.PurchaseId == purchaseId);
            if (purchase == null){
                throw new Exception(message: "We do not have this purchase on record");
            }
            this.product.Quantity+=purchase.Quantity;
            await context.SaveChangesAsync();
            //insert transaction into purchases table
            context.Purchases.Remove(purchase);
            await context.SaveChangesAsync();
            response.Success = true;
            response.Message = "Product Successfully returned";
            response.Data = mapper.Map<GetPurchaseDTO>(purchase);
            this.product.setState(this.product.getInStockState());
            return response;
        }
    }
}