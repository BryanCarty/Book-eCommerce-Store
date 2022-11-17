using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_eCommerce_Store.DTOs.Purchases
{
    public class CreatePurchaseDTO
    {
        public int ProductId { get; set; }  
        public int Quantity {get; set;}
    }
}