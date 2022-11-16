using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_eCommerce_Store.DTOs.Products
{
    public class UpdateProductDTO
    {
        public string? Name { get; set; } = null;
        public string? Description { get; set; } = null;
        public int? PriceInCent { get; set; } = null;//I'm storing product price as cent to avoid rounding errors with floats.
        public int? Quantity { get; set; } = null;
        public ProductCategory? ProductCategory { get; set; } = null;
        public string? Genre { get; set; } = null;
        public string? Author { get; set; } = null;
        public int? PageCount { get; set; } = null;
        public string? Publisher { get; set; } = null;
        public DateTime? PublicationDate { get; set; } = null;
        public string? Manufacturer { get; set; } = null;
        public string? Brand { get; set; } = null;

    }
}