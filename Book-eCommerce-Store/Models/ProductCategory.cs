using System.Text.Json.Serialization;

namespace Book_eCommerce_Store.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductCategory
    {
        
        NoCategoryAssigned = 1,
        Books = 2,
        Stationery = 3,
        Other = 4
    }
}