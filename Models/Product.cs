using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.Data;
using Book_eCommerce_Store.DTOs.Purchases;

namespace Book_eCommerce_Store.Models
{
    public class Product
    {
        private ProductStateInterface? currentState;
        private ProductStateInterface? soldOut;
        private ProductStateInterface? inStock;
        private ProductStateInterface? reStockSoon;

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int PriceInCent { get; set; } //I'm storing product price as cent to avoid rounding errors with floats.
        public int Quantity { get; set; }
        public ProductCategory ProductCategory { get; set; } = ProductCategory.NoCategoryAssigned;


        public void init(){
            this.soldOut = new SoldOutState(this);
            this.inStock = new InStockState(this);
            this.reStockSoon = new ReStockState(this);
            if (this.Quantity == 0){
                this.currentState = new SoldOutState(this);
            }else if(this.Quantity >0){
                this.currentState = new InStockState(this);
            }else if(this.Quantity == -1){
                this.currentState = new ReStockState(this);
            }
            
        }
        public Task<Response> purchaseProduct(CreatePurchaseDTO purchase, DataContext context, IMapper mapper){
            return this.currentState.purchaseProduct(purchase, context, mapper);
        } //user wants to purchase the product. i.e. quantity in db is decreased.

        public Task<Response> returnProduct(int purchaseId, DataContext context, IMapper mapper){
            return this.currentState.returnProductAsync(purchaseId, context, mapper);
        } //user wants to return the product they previously bought. i.e. quantity in db increases.
        
        public void setState(ProductStateInterface newState){
            this.currentState = newState;
        }

        public ProductStateInterface getSoldOutState(){
            return this.soldOut;
        }

        public ProductStateInterface getInStockState(){
            return this.inStock;
        }

        public ProductStateInterface getReStockSoonState(){
            return this.reStockSoon;
        }
    }
}