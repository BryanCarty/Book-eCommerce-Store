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
    public class InTransitState : OrderStateInterface
    {
        private Order order;

        public InTransitState(Order order){
            this.order = order;
        }

        public async Task<Response> cancelOrder(DataContext context, IMapper mapper)
        {
            var response = new Response();
            response.Success = false;
            response.Message = "Sorry, this order is in transit. It's too late to cancel.";
            return response;
        }

        public async Task<Response> updateOrder(UpdateOrderDTO updatedOrder, DataContext context, IMapper mapper, IProductFactory factory)
        {
            var response = new Response();
            response.Success = false;
            response.Message = "Sorry, this order is in transit. It's too late to update.";
            return response;
        }

        public async Task<Response> updateOrderStatus(string status, DataContext context, IMapper mapper)
        {
            var response = new Response();
            
            this.order.purchasedProducts = context.Purchases
                .FromSqlRaw("Select * from Purchases WHERE orderId = "+this.order.orderId)
                .ToList<Purchase>();
            if(this.order.status=="in transit" && status=="being processed"){
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