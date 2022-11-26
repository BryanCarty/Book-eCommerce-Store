using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_eCommerce_Store.Data.Entities;

namespace Book_eCommerce_Store.Models
{
    public class Order
    {
        public int orderId { get; set;}
        public List<Purchase>? purchasedProducts{ get; set; }
        public int subtotalInCent {get; set;} //subtotal stored as cent to avoid floating point errors.
        public string discountName {get; set;}
        public int discountInCent {get; set;} //discounts in cent to avoid floating point errors.
        public int totalInCent {get; set;} //total in cent to avoid floating point errrors.
    }
}