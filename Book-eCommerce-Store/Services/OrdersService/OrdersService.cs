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
           var response = await Order.createOrder(newOrder, this.context, this.mapper, this.productFactory);
           return response;
        }

        public async Task<Response> DeleteOrder(int id)
        {
            var response = new Response();
            var dbOrder = await context.Orders.FirstOrDefaultAsync(order => order.orderId == id);
            if(dbOrder == null){
                return response;
            }
            dbOrder.init();
            response = await dbOrder.cancelOrder(this.context, this.mapper);
            return response;
        }

        public async Task<Response> Get()
        {
            var response = await Order.get(this.context, this.mapper);
            return response;
        }

        public async Task<Response> GetById(int id)
        {
            var response = await Order.getById(id, this.context, this.mapper);
            return response;
        }

        public async Task<Response> UpdateOrder(int id, UpdateOrderDTO updatedOrder)
        {         
            var response = new Response();  
            //get order
            var dbOrder = await this.context.Orders.FirstOrDefaultAsync(order => order.orderId == id);
            if(dbOrder == null){
                    response.Success = false;
                    response.Message = "an order with this id does not exist";
                    return response;
            }
            else if(!updatedOrder.products.Any()){
                    response.Success = false;
                    response.Message = "products list is null";
                    return response;
            }
            dbOrder.init();
            response = await dbOrder.updateOrder(updatedOrder, this.context, this.mapper, this.productFactory);
            return response;
        }


        public async Task<Response> UpdateOrderStatus(int id, String updatedStatus)
        {         
            var response = new Response();  
            //get order
            var dbOrder = await this.context.Orders.FirstOrDefaultAsync(order => order.orderId == id);
            if(dbOrder == null){
                    response.Success = false;
                    response.Message = "an order with this id does not exist";
                    return response;
            }
            dbOrder.init();
            response = await dbOrder.updateOrderStatus(updatedStatus, this.context, this.mapper);
            return response;
        }
    }
}