
using OnSale.Common.Models;
using System.Collections.Generic;

namespace OnSale.Prism.Helpers
{
    public class CombosHelper : ICombosHelper
    {

        public IEnumerable<PaymentMethod> GetPaymentMethods()
        {
            List<PaymentMethod> paymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod { Id = 1, Name = "Cash" },
                new PaymentMethod { Id = 2, Name = "CreditCard" }
            };

            return paymentMethods;
        }


    }
}
