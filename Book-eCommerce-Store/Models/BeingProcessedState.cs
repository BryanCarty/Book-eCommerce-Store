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
    public class BeingProcessedState : OrderStateInterface
    {
        private Order order;

        public BeingProcessedState(Order order){
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
            response.Success = false;
            response.Message = "Sorry, this order is being processed. It's too late to update.";
            return response;
        }
        
        public async Task<Response> updateOrderStatus(string status, DataContext context, IMapper mapper)
        {
            var response = new Response();
            
            this.order.purchasedProducts = context.Purchases
                .FromSqlRaw("Select * from Purchases WHERE orderId = "+this.order.orderId)
                .ToList<Purchase>();
            if(this.order.status=="being processed" && (status=="submitted" || status=="in transit")){
                this.order.status = status;
                await context.SaveChangesAsync();
            }else{ 
                response.Success = false;
                response.Message = "You do not have permission to update the order status in this circumstance";
                return response;
            }
            if(status == "submitted"){
                this.order.setState(this.order.getSubmittedState());
            }else if(status == "in transit"){
                this.order.setState(this.order.getInTransitState());
            }
            response.Data = mapper.Map<GetOrderDTO>(this.order);
            return response;
        }

    }
}