namespace Book_eCommerce_Store.Models
{
    public class Address
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Country { get; set; }

        public string FullName { get; set; }

        public string? PostCode { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string City { get; set; }

        public string? County { get; set; }
    }
}
