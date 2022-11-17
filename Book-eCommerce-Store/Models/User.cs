using Microsoft.Extensions.Configuration.UserSecrets;
using System.Collections.Generic;

namespace Book_eCommerce_Store.Models
{
    public class User
    {
        public int Id { get; set; }

        public int userType { get; set; }

        public string? Name { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public List<Address>? Addresses { get; set; }
    }
}