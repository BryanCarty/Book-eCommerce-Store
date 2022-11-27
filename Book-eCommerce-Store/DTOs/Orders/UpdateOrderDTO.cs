using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.Data.Entities;

namespace Book_eCommerce_Store.DTOs.Orders
{
    public class UpdateOrderDTO
    {
        public List<OrderItem>? products { get; set; }
    }
}