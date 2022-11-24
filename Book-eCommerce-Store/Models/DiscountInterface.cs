using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_eCommerce_Store.Models
{
    public interface iDiscount
    {
        public string GetDiscountName();
        public double getReduction();
    }
}