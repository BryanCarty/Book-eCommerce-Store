using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.Data;
using Book_eCommerce_Store.DTOs.Purchases;
using Microsoft.EntityFrameworkCore;

namespace Book_eCommerce_Store.Services.PurchasesService
{
    public class PurchasesService : IPurchasesService
    {

        
        private readonly IMapper mapper;
        private readonly DataContext context;

        public PurchasesService(IMapper mapper, DataContext context)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<Response> ReturnPurchase(int product_id, int purchase_id)
        {
            var response = new Response();
            try{
                var dbProduct = await this.context.Products.FirstOrDefaultAsync(p => p.Id == product_id);

                if(dbProduct == null){
                    throw new Exception(message: "A product with this id does not exist");
                }
                Product product = new()
                {
                    Id = dbProduct.Id,
                    Name = dbProduct.Name,
                    Description = dbProduct.Description,
                    PriceInCent = dbProduct.PriceInCent,
                    Quantity = dbProduct.Quantity,
                    ProductCategory = dbProduct.ProductCategory
                };
                product.init();
                response = await product.returnProduct(purchase_id, this.context, this.mapper);
                
            }catch(Exception ex){
                response.Success=false;
                response.Message = ex.Message;
            }
           
            return response;
        }

        public async Task<Response> CreatePurchase(CreatePurchaseDTO newPurchase)
        {
            var response = new Response();
            try{
                var dbProduct = await this.context.Products.FirstOrDefaultAsync(p => p.Id == newPurchase.ProductId);
                if(dbProduct == null){
                    throw new Exception(message: "A product with this id does not exist");
                }
                Product product = new()
                {
                    Id = dbProduct.Id,
                    Name = dbProduct.Name,
                    Description = dbProduct.Description,
                    PriceInCent = dbProduct.PriceInCent,
                    Quantity = dbProduct.Quantity,
                    ProductCategory = dbProduct.ProductCategory
                };
                product.init();
                response = await product.purchaseProduct(newPurchase, this.context, this.mapper);
            }catch(Exception ex){
                response.Success=false;
                response.Message = ex.Message;
            }
           
            return response;
        }

    }
}