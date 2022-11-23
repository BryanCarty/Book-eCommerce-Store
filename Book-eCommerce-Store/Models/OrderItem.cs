using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_eCommerce_Store.Models
{
    public class OrderItem
    {
        public int itemId {get; set;}
        public int itemQuantity {get; set;}
        public ProductCategory ProductCategory { get; set; } = ProductCategory.NoCategoryAssigned;
    }
}