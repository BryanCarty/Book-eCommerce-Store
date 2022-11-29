using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.Data;
using Book_eCommerce_Store.Data.Entities;
using Book_eCommerce_Store.DTOs.Orders;
using Book_eCommerce_Store.Services.ProductsService.Factory;
using Microsoft.EntityFrameworkCore;

namespace Book_eCommerce_Store.Models
{
    public class Order
    {
        private OrderStateInterface? currentState;
        private OrderStateInterface? submitted; // Can update or cancel
        private OrderStateInterface? beingProcessed; // Can cancel
        private OrderStateInterface? inTransit; // Can NOT update or cancel


        public int orderId { get; set;}
        public List<Purchase>? purchasedProducts{ get; set; }
        public int subtotalInCent {get; set;} //subtotal stored as cent to avoid floating point errors.
        public string discountName {get; set;}
        public int discountInCent {get; set;} //discounts in cent to avoid floating point errors.
        public int totalInCent {get; set;} //total in cent to avoid floating point errrors.
        public string status {get; set;} 


        public void init()
        {
            switch(this.status){
                case "":
                    this.currentState = new SubmittedState(this);
                break;
                case "submitted":
                    this.currentState = new SubmittedState(this);
                break;
                case "being processed":
                    this.currentState = new BeingProcessedState(this);
                break;
                case "in transit":
                    this.currentState = new InTransitState(this);
                break;
            }
        }


        public static async Task<Response> createOrder(CreateOrderDTO newOrder, DataContext context, IMapper mapper, IProductFactory productFactory){
            var response = new Response();

            var discounttotalQuantityCount = 0;
            var discountName = "";
            var discountReduction = 0.0;

            //Lists to hold the names of the items in dborder and the categories
            List<string> itemNamesList = new List<string>();
            List<ProductCategory> itemProductCategoriesList = new List<ProductCategory>();
            List<int> individualItemCount = new List<int>();

            try{ 
                if (newOrder.products == null || !newOrder.products.Any()){
                    throw new Exception(message: "products list is empty");
                }
                Order order = new Order();
                order.purchasedProducts = new List<Purchase>();
                foreach (OrderItem item in newOrder.products) 
                { // Loop through List with foreach

                    //get product
                    var product = await productFactory.GetProductsService(item.ProductCategory).GetById(item.itemId);
                    
                    if (product == null || product.Data == null)
                    {
                        throw new Exception("a product you listed does not exist");
                    }

                    PRODUCT mappedToProduct;
                    if (item.ProductCategory == ProductCategory.Books)
                    {
                        mappedToProduct = mapper.Map<PRODUCT>((Book)product.Data);
                    }
                    else
                    {
                        mappedToProduct = mapper.Map<PRODUCT>((Stationary)product.Data);
                    }
                    
                    if (mappedToProduct.Quantity<item.itemQuantity)
                    {
                        throw new Exception("Order can't be placed. We only have "+ mappedToProduct.Quantity+" '"+ mappedToProduct.Name+"'");
                    }
                    
                    //Reduce product quantity in db
                    context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = "+(mappedToProduct.Quantity-item.itemQuantity)+" WHERE Id = "+mappedToProduct.Id+";");
                    await context.SaveChangesAsync();
                    var mappedToPurchase = mapper.Map<Purchase>(mappedToProduct);
                    mappedToPurchase.Quantity = item.itemQuantity;
                    mappedToPurchase.PurchaseId=null;
                    mappedToPurchase.ProductId = mappedToProduct.Id;
                    order.subtotalInCent+=mappedToPurchase.PriceInCent*mappedToPurchase.Quantity;
                    order.purchasedProducts.Add(mappedToPurchase);

                    individualItemCount.Add(mappedToPurchase.Quantity);  //keeps track of the quantity of individual item count, eg count of the da vinci code's bought
                    discounttotalQuantityCount += mappedToPurchase.Quantity;  //keeps track of the total quantity for use in bulk buying
                    itemProductCategoriesList.Add(mappedToPurchase.ProductCategory); 
                    itemNamesList.Add(mappedToPurchase.Name);
                }

                //Discounts
                String[] itemNamesArray = itemNamesList.ToArray();
                ProductCategory[] itemProductCatagoriesArray = itemProductCategoriesList.ToArray();
                int[] itemCountArray = individualItemCount.ToArray();

                //Specials for specific items
                //Summer Special can only be aquired once
                for (int i = 0; i < itemNamesArray.Length; i++) 
                {
                    if(discounttotalQuantityCount >= 5 & itemCountArray[i] >= 1 & itemNamesArray[i] == "The Da Vinci Code") {
                        iDiscount SummerSpecial = new summerSpecialDiscount(new basicDiscount());
                        discountName += SummerSpecial.GetDiscountName() + ", ";
                        discountReduction += SummerSpecial.getReduction();
                        break;
                    }
                    if(itemCountArray[i] >= 4 & itemNamesArray[i] == "Helix Black Nylon Pencil case") {
                        iDiscount SummerSpecial = new summerSpecialDiscount(new basicDiscount());
                        discountName += SummerSpecial.GetDiscountName() + ", ";
                        discountReduction += SummerSpecial.getReduction();
                        break;
                    }   
                }

                //Specific category discounts ie 2 for books, 3 for stationary etc
                for (int i = 0; i < itemProductCatagoriesArray.Length; i++) 
                {
                    if(itemCountArray[i] >= 1 & (int)itemProductCatagoriesArray[i] == 2) {
                        iDiscount WinterSpecial = new winterMadnessDiscount(new basicDiscount());
                        discountName += WinterSpecial.GetDiscountName() + ", ";
                        discountReduction += WinterSpecial.getReduction();
                        break;
                    }
                }

                //Bulk buying discounts
                if(discounttotalQuantityCount >= 3) {
                    iDiscount basic = new basicDiscount();
                    discountName += basic.GetDiscountName() + ", ";
                    discountReduction += basic.getReduction();
                }
                if(discounttotalQuantityCount >= 9) {
                    iDiscount basic = new basicDiscount();
                    discountName += basic.GetDiscountName() + ", ";
                    discountReduction += basic.getReduction();
                }

                order.discountName = discountName;
                order.discountInCent = Convert.ToInt32(order.subtotalInCent * discountReduction);

                if(order.discountInCent == 0) {
                    order.discountName = "No Discount Applied";
                }
                
                order.totalInCent = order.subtotalInCent - order.discountInCent;  
                order.status="submitted";
                order.currentState = new SubmittedState(order);
                context.Add(order);
                await context.SaveChangesAsync();
                
                response.Data = mapper.Map<GetOrderDTO>(order);
            }catch(Exception ex){
                response.Success=false;
                response.Message = ex.Message;
            }
            return response;
        } 

        public Task<Response> cancelOrder(DataContext context, IMapper mapper){
            return this.currentState.cancelOrder(context, mapper);
        } 

        public static async Task<Response> get(DataContext context, IMapper mapper){
            var response = new Response();
            try{
                var dbOrders = await context.Orders.ToListAsync();
                dbOrders.ForEach(order => {
                    //Get purchases 
                    var purchases = context.Purchases
                        .FromSqlRaw("Select * from Purchases WHERE orderId = "+order.orderId)
                        .ToList<Purchase>();
                    order.purchasedProducts = purchases;
                }); 
                response.Data = dbOrders.Select(order => mapper.Map<GetOrderDTO>(order)).ToList();
            }catch(Exception ex){
                response.Success=false;
                response.Message = ex.Message;
            }
            return response;
        }

        public static async Task<Response> getById(int orderId, DataContext context, IMapper mapper){
            var response = new Response();
            try{
                var dbOrder = await context.Orders.FirstOrDefaultAsync(order => order.orderId == orderId);
                if(dbOrder == null){
                    throw new Exception("an order with this id does not exist");
                }
                dbOrder.purchasedProducts = context.Purchases
                    .FromSqlRaw("Select * from Purchases WHERE orderId = "+dbOrder.orderId)
                    .ToList<Purchase>();
                response.Data = mapper.Map<GetOrderDTO>(dbOrder);
            }catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        } 

        public Task<Response> updateOrder(UpdateOrderDTO updatedOrder, DataContext context, IMapper mapper, IProductFactory productFactory){
            return this.currentState.updateOrder(updatedOrder, context, mapper, productFactory);
        } 

        public Task<Response> updateOrderStatus(String status, DataContext context, IMapper mapper){
            return this.currentState.updateOrderStatus(status, context, mapper);
        } 

        public OrderStateInterface getSubmittedState(){
            return this.submitted;
        }

        public OrderStateInterface getBeingProcessedState(){
            return this.beingProcessed;
        }

        public OrderStateInterface getInTransitState(){
            return this.inTransit;
        }

        public void setState(OrderStateInterface newState){
            this.currentState = newState;
        }
    }
}