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
    public class SubmittedState : OrderStateInterface
    {
        private Order order;

        public SubmittedState(Order order){
            this.order = order;
        }

        public async Task<Response> cancelOrder(DataContext context, IMapper mapper)
        {
            var response = new Response();
            try{
        
                this.order.purchasedProducts = context.Purchases
                    .FromSqlRaw("Select * from Purchases WHERE orderId = "+this.order.orderId)
                    .ToList<Purchase>();

                //Get count of products so can increase stock accordingly
                var itemList = new List<OrderItem>();
                this.order.purchasedProducts.ForEach(purchasedProduct => {
                    var orderItem = new OrderItem();
                    orderItem.itemId = (int)purchasedProduct.ProductId;
                    orderItem.itemQuantity = purchasedProduct.Quantity;
                    itemList.Add(orderItem);
                });

                context.Database.ExecuteSqlRaw("DELETE FROM Purchases WHERE orderId = "+this.order.orderId);
                await context.SaveChangesAsync();

                context.Database.ExecuteSqlRaw("DELETE FROM Orders WHERE orderId = "+this.order.orderId);
                await context.SaveChangesAsync();

                itemList.ForEach(item => {
                    context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = Quantity + "+item.itemQuantity+" WHERE Id = "+item.itemId+";");
                });
                await context.SaveChangesAsync();

            }catch(Exception ex){
                        response.Success=false;
                        response.Message=ex.Message;
                        return response;
            }
            return response;
        }

        public async Task<Response> updateOrder(UpdateOrderDTO updatedOrder, DataContext context, IMapper mapper, IProductFactory productFactory)
        {
            var response = new Response();
            var discountName = "";
            var discountReduction = 0.0;
            var discounttotalQuantityCount = 0;
            
            try{ 
                this.order.purchasedProducts = context.Purchases
                    .FromSqlRaw("Select * from Purchases WHERE orderId = "+this.order.orderId)
                    .ToList<Purchase>();
            	
                
                foreach (OrderItem item in updatedOrder.products) 
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
                    //check if product already exists in the order
                    var itemFromExistingOrder = this.order.purchasedProducts.FirstOrDefault(product => product.ProductId == item.itemId);
                    if (itemFromExistingOrder != null){//Product already exists in order
                        if(item.itemQuantity<=0){//if item Quantity is <= 0, remove the purchase
                            //remove purchase record
                            var productQuantity = itemFromExistingOrder.Quantity;
                            this.order.purchasedProducts.Remove(itemFromExistingOrder);
                            await context.SaveChangesAsync();
                            context.Database.ExecuteSqlRaw("DELETE FROM Purchases WHERE PurchaseId = "+itemFromExistingOrder.PurchaseId);
                            await context.SaveChangesAsync();

                            //increase stock quantity
                            context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = Quantity +"+productQuantity+" WHERE Id = "+mappedToProduct.Id+";");
                            await context.SaveChangesAsync(); 

                            //if order now empty remove order
                            if(!this.order.purchasedProducts.Any()){ 
                                context.Orders.Remove(this.order);
                                await context.SaveChangesAsync();
                                this.order = new Order();
                            }else{
                                this.order.subtotalInCent-=itemFromExistingOrder.PriceInCent*itemFromExistingOrder.Quantity;
                                await context.SaveChangesAsync();                                                                                  //This will be changed when discounts implemented
                                context.Database.ExecuteSqlRaw("UPDATE Orders SET subtotalInCent = "+this.order.subtotalInCent+", totalInCent = "+this.order.subtotalInCent+" WHERE orderId = "+this.order.orderId+";");
                                await context.SaveChangesAsync(); 
                            }
                            continue;
                        }
                        if(item.itemQuantity>itemFromExistingOrder.Quantity){//increment item quantity
                            //Get the number of this product that is available
                            var availableQuantity = mappedToProduct.Quantity+itemFromExistingOrder.Quantity;
                            if(availableQuantity>=item.itemQuantity){
                                //decrement quantity from stock
                                var newStockQuantity = mappedToProduct.Quantity-(item.itemQuantity-itemFromExistingOrder.Quantity);
                                context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = "+newStockQuantity+" WHERE Id = "+mappedToProduct.Id+";");
                                await context.SaveChangesAsync();                                    
                                //update purchase in purchase record
                                context.Database.ExecuteSqlRaw("UPDATE Purchases SET Quantity = "+item.itemQuantity+" WHERE PurchaseId = "+itemFromExistingOrder.PurchaseId+";");
                                await context.SaveChangesAsync();


                                //increase subtotal
                                this.order.subtotalInCent += (item.itemQuantity-itemFromExistingOrder.Quantity)*itemFromExistingOrder.PriceInCent;
                                this.order.totalInCent += (item.itemQuantity-itemFromExistingOrder.Quantity)*itemFromExistingOrder.PriceInCent;
                                await context.SaveChangesAsync();                                                                                  //This will be changed when discounts implemented
                                context.Database.ExecuteSqlRaw("UPDATE Orders SET subtotalInCent = "+this.order.subtotalInCent+", totalInCent = "+this.order.subtotalInCent+" WHERE orderId = "+this.order.orderId+";");
                                await context.SaveChangesAsync(); 
                                this.order.purchasedProducts = this.order.purchasedProducts.Select(product => 
                                { 
                                    if (product.PurchaseId==itemFromExistingOrder.PurchaseId){
                                        product.Quantity =item.itemQuantity;
                                    } 
                                    return product; 
                                }).ToList();
                                continue;
                            }else{
                                throw new Exception("Order can't be updated. We only have "+ mappedToProduct.Quantity+" '"+ mappedToProduct.Name+"'");
                            } 
                        }else if(item.itemQuantity<itemFromExistingOrder.Quantity){//decrement the quantity
                            
                            //update purchases
                            context.Database.ExecuteSqlRaw("UPDATE Purchases SET Quantity = "+item.itemQuantity+" WHERE PurchaseId = "+itemFromExistingOrder.PurchaseId+";");
                            await context.SaveChangesAsync();


                            var decrAmount = itemFromExistingOrder.Quantity-item.itemQuantity;
                            context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = Quantity +"+decrAmount+" WHERE Id = "+mappedToProduct.Id+";");
                            await context.SaveChangesAsync();  
                            
                            this.order.subtotalInCent-=itemFromExistingOrder.PriceInCent*(decrAmount);
                            this.order.totalInCent-=itemFromExistingOrder.PriceInCent*(decrAmount);                   
                            this.order.purchasedProducts = this.order.purchasedProducts.Select(product => 
                            { 
                                if (product.PurchaseId==itemFromExistingOrder.PurchaseId){
                                    product.Quantity =item.itemQuantity;
                                } 
                                return product; 
                            }).ToList();
                                                                                                                                            //this will change when discounts are implemented
                            context.Database.ExecuteSqlRaw("UPDATE Orders SET subtotalInCent = "+this.order.subtotalInCent+", totalInCent = "+this.order.subtotalInCent+" WHERE orderId = "+this.order.orderId+";");
                            await context.SaveChangesAsync();
                            continue;
                        }else{
                            continue;
                        }                         
                    }else{//Product doesn't exist in order yet so can add it.
                        var availableQuantity = mappedToProduct.Quantity;
                        if(availableQuantity>=item.itemQuantity){
                            //update products
                            context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = "+(mappedToProduct.Quantity-item.itemQuantity)+" WHERE Id = "+mappedToProduct.Id+";");
                            await context.SaveChangesAsync();
                            var mappedToPurchase = mapper.Map<Purchase>(mappedToProduct);
                            mappedToPurchase.Quantity = item.itemQuantity;
                            mappedToPurchase.ProductId = mappedToProduct.Id;
                            await context.SaveChangesAsync();
                            this.order.purchasedProducts.Add(mappedToPurchase);
                            await context.SaveChangesAsync();
                            this.order.subtotalInCent+=mappedToPurchase.PriceInCent*item.itemQuantity;
                            this.order.totalInCent+=mappedToPurchase.PriceInCent*item.itemQuantity;
                            await context.SaveChangesAsync();
                            //Update order
                            context.Database.ExecuteSqlRaw("UPDATE Orders SET subtotalInCent = "+this.order.subtotalInCent+", totalInCent = "+this.order.subtotalInCent+" WHERE orderId = "+this.order.orderId+";");
                            await context.SaveChangesAsync();
                            continue;
                        }else{
                            throw new Exception("Order can't be updated. We only have "+ mappedToProduct.Quantity+" '"+ mappedToProduct.Name+"'");
                        }
                    }
                }
            }catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

            //CHECK IF APPLICABLE FOR A DISCOUNT
            //GET DISCOUNT HERE

            //Lists to hold the names of the items in dborder and the categories
            List<string> itemNamesList = new List<string>();
            List<ProductCategory> itemProductCategoriesList = new List<ProductCategory>();
            List<int> individualItemCount = new List<int>();

            //go through each purchase in dbOrder
            foreach (Purchase item in this.order.purchasedProducts) 
            { 
	            discounttotalQuantityCount += item.Quantity;
                itemProductCategoriesList.Add(item.ProductCategory);
                itemNamesList.Add(item.Name);
                individualItemCount.Add(item.Quantity);
            }
            
            //Convert Lists to Array's
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

            this.order.discountName = discountName;
            this.order.discountInCent = Convert.ToInt32(this.order.subtotalInCent * discountReduction);

            if(this.order.discountInCent == 0) {
                this.order.discountName = "No Discount Applied";
            }

            this.order.totalInCent = this.order.subtotalInCent-this.order.discountInCent;
            await context.SaveChangesAsync();
            response.Data = mapper.Map<GetOrderDTO>(this.order);
            return response;

             
        }

        public async Task<Response> updateOrderStatus(string status, DataContext context, IMapper mapper)
        {
            var response = new Response();
            this.order.purchasedProducts = context.Purchases
                .FromSqlRaw("Select * from Purchases WHERE orderId = "+this.order.orderId)
                .ToList<Purchase>();
            if(status=="being processed" && this.order.status=="submitted"){
                this.order.status = status;
                await context.SaveChangesAsync();
            }else{ 
                response.Success = false;
                response.Message = "You do not have permission to update the order status in this circumstance";
                return response;
            }
            this.order.setState(this.order.getBeingProcessedState());
            response.Data = mapper.Map<GetOrderDTO>(this.order);
            return response;
        }
    }
}