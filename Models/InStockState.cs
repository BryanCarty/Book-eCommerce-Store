using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.data;
using Book_eCommerce_Store.DTOs.Purchases;
using Microsoft.EntityFrameworkCore;

namespace Book_eCommerce_Store.Models
{
    public class InStockState : ProductStateInterface
    {

        
        private Product product;

        public InStockState(Product product){
            this.product = product;
        }
        public async Task<Response<GetPurchaseDTO>> purchaseProduct(CreatePurchaseDTO purchase, DataContext context, IMapper mapper)
        {
            var response = new Response<GetPurchaseDTO>();
            if(this.product.Quantity >= purchase.Quantity){
                this.product.Quantity-=purchase.Quantity;
                await context.SaveChangesAsync();
                //insert transaction into purchases table
                var purchaseModel = mapper.Map<Purchase>(purchase);
                purchaseModel.PriceInCentPaid = product.PriceInCent*purchaseModel.Quantity;
                context.Purchases.Add(purchaseModel);
                await context.SaveChangesAsync();
                response.Success = true;
                response.Message = "Product Successfully Purchased";
                response.Data = mapper.Map<GetPurchaseDTO>(purchaseModel);
                if (this.product.Quantity == 0){
                    this.product.setState(this.product.getSoldOutState());
                }
            }else{
                response.Success=false;
                response.Message = "We do not have that many of this product in stock. Please try again with a reduced quantity or wait until a later date.";
            }
            return response;
        }

        public async Task<Response<GetPurchaseDTO>> returnProductAsync(int purchaseId, DataContext context, IMapper mapper)
        {
            var response = new Response<GetPurchaseDTO>();
            if(product.Quantity >100){
                response.Success=true;
                response.Message = "We are not currently accepting returns at this time as our supply is too large.";
                return response;
            }
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