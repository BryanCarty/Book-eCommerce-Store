using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Book_eCommerce_Store.Data;
using Book_eCommerce_Store.DTOs.Orders;
using Book_eCommerce_Store.Services.ProductsService.Factory;

namespace Book_eCommerce_Store.Models
{
    public interface OrderStateInterface
    {
     
            
        public Task<Response> cancelOrder(DataContext context, IMapper mapper);

        public Task<Response> updateOrder(UpdateOrderDTO updatedOrder, DataContext context, IMapper mapper, IProductFactory productFactory);
        
        public Task<Response> updateOrderStatus(String status, DataContext context, IMapper mapper);
    }
}