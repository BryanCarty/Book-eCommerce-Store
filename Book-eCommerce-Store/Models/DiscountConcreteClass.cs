using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_eCommerce_Store.Models
{
    public class basicDiscount : iDiscount {
        public string GetDiscountName() {
            return "basic bulk buying discount";
        }

        public double getReduction() {
            return 0.05;
        }
    }
}