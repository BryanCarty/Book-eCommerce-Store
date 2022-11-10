using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_eCommerce_Store.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int PriceInCent { get; set; } //I'm storing product price as cent to avoid rounding errors with floats.
        public int Quantity { get; set; }
        public ProductCategory ProductCategory { get; set; } = ProductCategory.NoCategoryAssigned;
        
    }
}