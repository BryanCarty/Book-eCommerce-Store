using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_eCommerce_Store.DTOs.Products
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int PriceInCent { get; set; } //I'm storing product price as cent to avoid rounding errors with floats.
        public int Quantity { get; set; }
        public ProductCategory ProductCategory { get; set; } = ProductCategory.NoCategoryAssigned;
        public string? Genre { get; set; }
        public string? Author { get; set; }
        public int? PageCount { get; set; }
        public string? Publisher { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string? Manufacturer { get; set; }
        public string? Brand { get; set; }

    }
}