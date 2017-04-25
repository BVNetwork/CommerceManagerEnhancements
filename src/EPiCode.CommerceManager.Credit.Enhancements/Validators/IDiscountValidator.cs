using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Orders;

namespace CommerceManagerCreditEnhancements.Validators
{
    public interface IDiscountValidator
    {
        bool IsLineItemDiscountValid(LineItem item, decimal discountAmount);

        bool IsShippingDiscountValid(Shipment shipment, decimal discountAmount);
    }

    [ServiceConfiguration(typeof(IDiscountValidator))]
    public class DiscountValidator : IDiscountValidator
    {
        public bool IsLineItemDiscountValid(LineItem item, decimal discountAmount)
        {
            return item.ExtendedPrice - discountAmount >= 0;
        }

        public bool IsShippingDiscountValid(Shipment shipment, decimal discountAmount)
        {
            return (shipment.ShippingDiscountAmount + discountAmount) <= shipment.ShippingSubTotal;
        }
    }
}
