using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.DTOs.Orders;

namespace Book_eCommerce_Store.Services.OrdersService
{
    public interface IOrdersService
    {
        Task<Response> Get();

        Task<Response> GetById(int id);

        Task<Response> CreateOrder(CreateOrderDTO newOrder);

        Task<Response> UpdateOrder(int id, UpdateOrderDTO updatedOrder);

        Task<Response> DeleteOrder(int id);
    }
}