using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.Data;
using Book_eCommerce_Store.Data.Entities;
using Book_eCommerce_Store.DTOs.Orders;
using Book_eCommerce_Store.Services.ProductsService.Factory;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Book_eCommerce_Store.Services.OrdersService
{ 
    public class OrdersService : IOrdersService
    {

        private readonly IMapper mapper;
        private readonly DataContext context;
        private readonly IProductFactory productFactory;

        public OrdersService(IMapper mapper, DataContext context, IProductFactory productFactory)
        {
            this.context = context;
            this.mapper = mapper;
            this.productFactory = productFactory;
        }
        public async Task<Response> CreateOrder(CreateOrderDTO newOrder)
        {
            var response = new Response();

            var DiscountQuantityCount = 0;
            var DiscountName = "";
            var DiscountCategory = 0;

            try{ 
                if (newOrder.products == null || !newOrder.products.Any()){
                    throw new Exception(message: "products list is empty");
                }
                Order order = new Order();
                order.purchasedProducts = new List<Purchase>();
                foreach (OrderItem item in newOrder.products) 
                { // Loop through List with foreach

                    //get product
                    var product = await this.productFactory.GetProductsService(item.ProductCategory).GetById(item.itemId, false);
                    
                    if (product == null || product.Data == null)
                    {
                        throw new Exception("a product you listed does not exist");
                    }

                    PRODUCT mappedToProduct;
                    if (item.ProductCategory == ProductCategory.Books)
                    {
                        mappedToProduct = this.mapper.Map<PRODUCT>((Book)product.Data);
                    }
                    else
                    {
                        mappedToProduct = this.mapper.Map<PRODUCT>((Stationary)product.Data);
                    }
                    
                    if (mappedToProduct.Quantity<item.itemQuantity)
                    {
                        throw new Exception("Order can't be placed. We only have "+ mappedToProduct.Quantity+" '"+ mappedToProduct.Name+"'");
                    }
                    
                    //Reduce product quantity in db
                    this.context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = "+(mappedToProduct.Quantity-item.itemQuantity)+" WHERE Id = "+mappedToProduct.Id+";");
                    await this.context.SaveChangesAsync();
                    var mappedToPurchase = this.mapper.Map<Purchase>(mappedToProduct);
                    mappedToPurchase.Quantity = item.itemQuantity;
                    mappedToPurchase.PurchaseId=null;
                    mappedToPurchase.ProductId = mappedToProduct.Id;
                    order.subtotalInCent+=mappedToPurchase.PriceInCent*mappedToPurchase.Quantity;
                    order.purchasedProducts.Add(mappedToPurchase);

                    DiscountName = mappedToPurchase.Name;
                    DiscountQuantityCount = mappedToPurchase.Quantity;
                    DiscountCategory = (int)mappedToPurchase.ProductCategory;
                }

                //Discounts
                var discountName = "";
                var discountReduction = 0.0;
                
                //Specials for specific items
                if(DiscountName == "The Da Vinci Code") {
                    iDiscount SummerSpecial = new summerSpecialDiscount(new basicDiscount());
                    discountName += SummerSpecial.GetDiscountName() + ", ";
                    discountReduction += SummerSpecial.getReduction();
                }
                else if(DiscountQuantityCount >= 5 & DiscountName == "Helix Black Nylon Pencil case") {
                    iDiscount SummerSpecial = new summerSpecialDiscount(new basicDiscount());
                    discountName += SummerSpecial.GetDiscountName() + ", ";
                    discountReduction += SummerSpecial.getReduction();
                }

                //Specific category discounts ie 2 for books, 3 for stationary etc
                if(DiscountCategory == 2) {
                    iDiscount WinterSpecial = new winterMadnessDiscount(new basicDiscount());
                    discountName += WinterSpecial.GetDiscountName() + ", ";
                    discountReduction += WinterSpecial.getReduction();
                }

                //Bulk buying discounts
                if(DiscountQuantityCount >= 3) {
                    iDiscount basic = new basicDiscount();
                    discountName += basic.GetDiscountName() + ", ";
                    discountReduction += basic.getReduction();
                }
                if(DiscountQuantityCount >= 9) {
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

                this.context.Add(order);
                await this.context.SaveChangesAsync();
                
                response.Data = this.mapper.Map<GetOrderDTO>(order);
            }catch(Exception ex){
                response.Success=false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> DeleteOrder(int id)
        {
            var response = new Response();
            try{
                var dbOrder = await this.context.Orders.FirstOrDefaultAsync(order => order.orderId == id);
                if(dbOrder == null){
                    return response;
                }
                dbOrder.purchasedProducts = this.context.Purchases
                    .FromSqlRaw("Select * from Purchases WHERE orderId = "+dbOrder.orderId)
                    .ToList<Purchase>();

                //Get count of products so can increase stock accordingly
                var itemList = new List<OrderItem>();
                dbOrder.purchasedProducts.ForEach(purchasedProduct => {
                    var orderItem = new OrderItem();
                    orderItem.itemId = (int)purchasedProduct.ProductId;
                    orderItem.itemQuantity = purchasedProduct.Quantity;
                    itemList.Add(orderItem);
                });

                this.context.Database.ExecuteSqlRaw("DELETE FROM Purchases WHERE orderId = "+dbOrder.orderId);
                await this.context.SaveChangesAsync();

                this.context.Database.ExecuteSqlRaw("DELETE FROM Orders WHERE orderId = "+dbOrder.orderId);
                await this.context.SaveChangesAsync();

                itemList.ForEach(item => {
                    this.context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = Quantity + "+item.itemQuantity+" WHERE Id = "+item.itemId+";");
                });
                await this.context.SaveChangesAsync();

            }catch(Exception ex){
                        response.Success=false;
                        response.Message=ex.Message;
                        return response;
            }
            return response;
        }

        public async Task<Response> Get()
        {
            var response = new Response();
            try{
                var dbOrders = await this.context.Orders.ToListAsync();
                dbOrders.ForEach(order => {
                    //Get purchases 
                    var purchases = this.context.Purchases
                        .FromSqlRaw("Select * from Purchases WHERE orderId = "+order.orderId)
                        .ToList<Purchase>();
                    order.purchasedProducts = purchases;
                }); 
                response.Data = dbOrders.Select(order => this.mapper.Map<GetOrderDTO>(order)).ToList();
            }catch(Exception ex){
                response.Success=false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> GetById(int id)
        {
            var response = new Response();
            try{
                var dbOrder = await this.context.Orders.FirstOrDefaultAsync(order => order.orderId == id);
                if(dbOrder == null){
                    throw new Exception("an order with this id does not exist");
                }
                dbOrder.purchasedProducts = this.context.Purchases
                    .FromSqlRaw("Select * from Purchases WHERE orderId = "+dbOrder.orderId)
                    .ToList<Purchase>();
                response.Data = this.mapper.Map<GetOrderDTO>(dbOrder);
            }catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> UpdateOrder(int id, UpdateOrderDTO updatedOrder)
        {            
            var response = new Response();
            //get order
            var dbOrder = await this.context.Orders.FirstOrDefaultAsync(order => order.orderId == id);
            try{ 
                if(dbOrder == null){
                    throw new Exception("an order with this id does not exist");
                }
                else if(!updatedOrder.products.Any()){
                    throw new Exception("products list is null");
                }
                dbOrder.purchasedProducts = this.context.Purchases
                    .FromSqlRaw("Select * from Purchases WHERE orderId = "+dbOrder.orderId)
                    .ToList<Purchase>();
                
                foreach (OrderItem item in updatedOrder.products) 
                { // Loop through List with foreach
                    //get product
                    var product = await this.productFactory.GetProductsService(item.ProductCategory).GetById(item.itemId, false);
                    
                    if (product == null || product.Data == null)
                    {
                        throw new Exception("a product you listed does not exist");
                    }
                    PRODUCT mappedToProduct;
                    if (item.ProductCategory == ProductCategory.Books)
                    {
                        mappedToProduct = this.mapper.Map<PRODUCT>((Book)product.Data);
                    }
                    else
                    {
                        mappedToProduct = this.mapper.Map<PRODUCT>((Stationary)product.Data);
                    }
                    //check if product already exists in the order
                    var itemFromExistingOrder = dbOrder.purchasedProducts.FirstOrDefault(product => product.ProductId == item.itemId);
                    if (itemFromExistingOrder != null){//Product already exists in order
                        if(item.itemQuantity<=0){//if item Quantity is <= 0, remove the purchase
                            //remove purchase record
                            dbOrder.purchasedProducts.Remove(itemFromExistingOrder);
                            this.context.Database.ExecuteSqlRaw("DELETE FROM Purchases WHERE PurchaseId = "+itemFromExistingOrder.PurchaseId);
                            await this.context.SaveChangesAsync();

                            //increase stock quantity
                            this.context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = Quantity +"+itemFromExistingOrder.Quantity+" WHERE Id = "+mappedToProduct.Id+";");
                            await this.context.SaveChangesAsync(); 

                            //if order now empty remove order
                            if(!dbOrder.purchasedProducts.Any()){ 
                                this.context.Orders.Remove(dbOrder);
                                await this.context.SaveChangesAsync();
                                dbOrder = new Order();
                            }else{
                                dbOrder.subtotalInCent-=itemFromExistingOrder.PriceInCent*itemFromExistingOrder.Quantity;
                                await this.context.SaveChangesAsync();                                                                                  //This will be changed when discounts implemented
                                this.context.Database.ExecuteSqlRaw("UPDATE Orders SET subtotalInCent = "+dbOrder.subtotalInCent+", totalInCent = "+dbOrder.subtotalInCent+" WHERE orderId = "+dbOrder.orderId+";");
                                await this.context.SaveChangesAsync(); 
                            }
                            continue;
                        }
                        if(item.itemQuantity>itemFromExistingOrder.Quantity){//increment item quantity
                            //Get the number of this product that is available
                            var availableQuantity = mappedToProduct.Quantity+itemFromExistingOrder.Quantity;
                            if(availableQuantity>item.itemQuantity){
                                //decrement quantity from stock
                                var newStockQuantity = mappedToProduct.Quantity-(item.itemQuantity-itemFromExistingOrder.Quantity);
                                this.context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = "+newStockQuantity+" WHERE Id = "+mappedToProduct.Id+";");
                                await this.context.SaveChangesAsync();                                    
                                //update purchase in purchase record
                                this.context.Database.ExecuteSqlRaw("UPDATE Purchases SET Quantity = "+item.itemQuantity+" WHERE PurchaseId = "+itemFromExistingOrder.PurchaseId+";");
                                await this.context.SaveChangesAsync();


                                //increase subtotal
                                dbOrder.subtotalInCent += (item.itemQuantity-itemFromExistingOrder.Quantity)*itemFromExistingOrder.PriceInCent;
                                dbOrder.totalInCent += (item.itemQuantity-itemFromExistingOrder.Quantity)*itemFromExistingOrder.PriceInCent;
                                await this.context.SaveChangesAsync();                                                                                  //This will be changed when discounts implemented
                                this.context.Database.ExecuteSqlRaw("UPDATE Orders SET subtotalInCent = "+dbOrder.subtotalInCent+", totalInCent = "+dbOrder.subtotalInCent+" WHERE orderId = "+dbOrder.orderId+";");
                                await this.context.SaveChangesAsync(); 
                                dbOrder.purchasedProducts = dbOrder.purchasedProducts.Select(product => 
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
                            this.context.Database.ExecuteSqlRaw("UPDATE Purchases SET Quantity = "+item.itemQuantity+" WHERE PurchaseId = "+itemFromExistingOrder.PurchaseId+";");
                            await this.context.SaveChangesAsync();


                            var decrAmount = itemFromExistingOrder.Quantity-item.itemQuantity;
                            this.context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = Quantity +"+decrAmount+" WHERE Id = "+mappedToProduct.Id+";");
                            await this.context.SaveChangesAsync();  
                            
                            dbOrder.subtotalInCent-=itemFromExistingOrder.PriceInCent*(decrAmount);
                            dbOrder.totalInCent-=itemFromExistingOrder.PriceInCent*(decrAmount);                   
                            dbOrder.purchasedProducts = dbOrder.purchasedProducts.Select(product => 
                            { 
                                if (product.PurchaseId==itemFromExistingOrder.PurchaseId){
                                    product.Quantity =item.itemQuantity;
                                } 
                                return product; 
                            }).ToList();
                                                                                                                                            //this will change when discounts are implemented
                            this.context.Database.ExecuteSqlRaw("UPDATE Orders SET subtotalInCent = "+dbOrder.subtotalInCent+", totalInCent = "+dbOrder.subtotalInCent+" WHERE orderId = "+dbOrder.orderId+";");
                            await this.context.SaveChangesAsync();
                            continue;
                        }else{
                            continue;
                        }                         
                    }else{//Product doesn't exist in order yet so can add it.
                        var availableQuantity = mappedToProduct.Quantity;
                        if(availableQuantity>=item.itemQuantity){
                            //update products
                            this.context.Database.ExecuteSqlRaw("UPDATE Products SET Quantity = "+(mappedToProduct.Quantity-item.itemQuantity)+" WHERE Id = "+mappedToProduct.Id+";");
                            await this.context.SaveChangesAsync();
                            var mappedToPurchase = this.mapper.Map<Purchase>(mappedToProduct);
                            mappedToPurchase.Quantity = item.itemQuantity;
                            mappedToPurchase.ProductId = mappedToProduct.Id;
                            
                            //INSERT Purchase -> This code could be tidied up
                            this.context.Database.ExecuteSqlRaw("INSERT INTO Purchases (ProductId, Name, Description, PriceInCent, Quantity, ProductCategory, Genre, Author, PageCount, Publisher, PublicationDate, Manufacturer, Brand, orderId) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13});", mappedToPurchase.ProductId, mappedToPurchase.Name, mappedToPurchase.Description, mappedToPurchase.PriceInCent, mappedToPurchase.Quantity, mappedToPurchase.ProductCategory, mappedToPurchase.Genre, mappedToPurchase.Author, mappedToPurchase.PageCount, mappedToPurchase.Publisher, mappedToPurchase.PublicationDate, mappedToPurchase.Manufacturer, mappedToPurchase.Brand, dbOrder.orderId);
                            await this.context.SaveChangesAsync();      
                            var purchases = await this.context.Purchases.FromSqlRaw("SELECT * FROM Purchases WHERE ProductId = "+(mappedToPurchase.ProductId)+" AND orderId = "+(dbOrder.orderId)+";").ToListAsync();
                            var purchaseId = purchases.First().PurchaseId;
                            mappedToPurchase.PurchaseId = purchaseId;
                            dbOrder.purchasedProducts.Add(mappedToPurchase);
                            await this.context.SaveChangesAsync();
                            
                            dbOrder.subtotalInCent+=mappedToPurchase.PriceInCent*item.itemQuantity;
                            dbOrder.totalInCent+=mappedToPurchase.PriceInCent*item.itemQuantity;
                            //Update order
                            this.context.Database.ExecuteSqlRaw("UPDATE Orders SET subtotalInCent = "+dbOrder.subtotalInCent+", totalInCent = "+dbOrder.subtotalInCent+" WHERE orderId = "+dbOrder.orderId+";");
                            await this.context.SaveChangesAsync();
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
            var discountName = "";
            var discountReduction = 0.0;
            var DiscountQuantityCount = 0;

            //Lists to hold the names of the items in dborder and the categories
            List<string> itemNamesList = new List<string>();
            List<ProductCategory> itemProductCategoriesList = new List<ProductCategory>();

            //go through each purchase in dbOrder
            foreach (Purchase item in dbOrder.purchasedProducts) 
            { 
	            DiscountQuantityCount += item.Quantity;
                itemProductCategoriesList.Add(item.ProductCategory);
                itemNamesList.Add(item.Name);
            }
            
            //Convert Lists to Array's
            String[] itemNamesArray = itemNamesList.ToArray();
            ProductCategory[] itemProductCatagoriesArray = itemProductCategoriesList.ToArray();

            //Specials for specific items
            //Summer Special can only be aquired once
            for (int i = 0; i < itemNamesArray.Length; i++) 
            {
                if(itemNamesArray[i] == "The Da Vinci Code") {
                    iDiscount SummerSpecial = new summerSpecialDiscount(new basicDiscount());
                    discountName += SummerSpecial.GetDiscountName() + ", ";
                    discountReduction += SummerSpecial.getReduction();
                    break;
                }
                if(DiscountQuantityCount >= 5 & itemNamesArray[i] == "Helix Black Nylon Pencil case") {
                    iDiscount SummerSpecial = new summerSpecialDiscount(new basicDiscount());
                    discountName += SummerSpecial.GetDiscountName() + ", ";
                    discountReduction += SummerSpecial.getReduction();
                    break;
                }   
            }
            
            //Specific category discounts ie 2 for books, 3 for stationary etc
            for (int i = 0; i < itemProductCatagoriesArray.Length; i++) 
            {
                if((int)itemProductCatagoriesArray[i] == 2) {
                    iDiscount WinterSpecial = new winterMadnessDiscount(new basicDiscount());
                    discountName += WinterSpecial.GetDiscountName() + ", ";
                    discountReduction += WinterSpecial.getReduction();
                }
            }
            
            //Bulk buying discounts
            if(DiscountQuantityCount >= 3) {
                iDiscount basic = new basicDiscount();
                discountName += basic.GetDiscountName() + ", ";
                discountReduction += basic.getReduction();
            }
            if(DiscountQuantityCount >= 9) {
                iDiscount basic = new basicDiscount();
                discountName += basic.GetDiscountName() + ", ";
                discountReduction += basic.getReduction();
            }

            dbOrder.discountName = discountName;
            dbOrder.discountInCent = Convert.ToInt32(dbOrder.subtotalInCent * discountReduction);

            if(dbOrder.discountInCent == 0) {
                dbOrder.discountName = "No Discount Applied";
            }

            dbOrder.totalInCent = dbOrder.subtotalInCent-dbOrder.discountInCent;
            await this.context.SaveChangesAsync();
            response.Data = this.mapper.Map<GetOrderDTO>(dbOrder);
            return response;
        }
    }
}