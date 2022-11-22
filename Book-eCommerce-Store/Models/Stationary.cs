using System.Runtime.CompilerServices;
using Book_eCommerce_Store.Data.Entities;

namespace Book_eCommerce_Store.Models
{
    public class Stationary : PRODUCT
    {
        public Stationary(int Id, string Name, string Description, int PriceInCent, int Quantity,
            string Manufacturer, string Brand) 
        { 
            this.Id = Id;
            this.Name = Name;
            this.Description = Description;
            this.PriceInCent = PriceInCent;
            this.Quantity = Quantity;

            this.Manufacturer = Manufacturer;
            this.Brand = Brand;
        }

        public string? Manufacturer { get; set; }
        public string? Brand { get; set; }
    }
}
