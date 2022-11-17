using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_eCommerce_Store.DTOs.Purchases
{
    public class GetPurchaseDTO
    {
        
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }  
        public int Quantity {get; set;}
        public int PriceInCentPaid { get; set; } //I'm storing product price as cent to avoid rounding errors with floats.
    }
}