using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_eCommerce_Store.Models
{
    public abstract class discountDecorator : iDiscount {

        protected iDiscount _discount;

        public discountDecorator(iDiscount discount) {
            _discount = discount;
        }

        public virtual string GetDiscountName() {
            return _discount.GetDiscountName();
        }

        public virtual double getReduction() {
            return _discount.getReduction();
        }
    }
}