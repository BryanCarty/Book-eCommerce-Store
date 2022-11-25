using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_eCommerce_Store.Models
{
    class summerSpecialDiscount : discountDecorator {
        public summerSpecialDiscount(iDiscount a) : base(a) { }

            public override string GetDiscountName() 
            {
                string name = base.GetDiscountName();
                name = "summer discount";
                return name;
            }

            public override double getReduction() {
                double discount = base.getReduction();
                discount = 0.10;
                return discount;
            }
    
    }

    class winterMadnessDiscount : discountDecorator {
        public winterMadnessDiscount(iDiscount a) : base(a) { }

            public override string GetDiscountName() 
            {
                string name = base.GetDiscountName();
                name = "winter discount";
                return name;
            }
            public override double getReduction() {
                double discount = base.getReduction();
                discount = 0.15;
                return discount;
            }
    }
}