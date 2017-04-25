using Mediachase.BusinessFoundation;
using Mediachase.Commerce.Manager.Order.CommandHandlers.ReturnFormHandlers.Strategy;
using Mediachase.Commerce.Orders;

namespace CommerceManagerCreditEnhancements.CommandHandlers
{
    public class CreditLineItemEnableHandler : EditableCommandHandler
    {
        public CreditLineItemEnableHandler()
        {
        }

        protected override bool IsCommandEnable(OrderGroup order, CommandParameters cp)
        {
            return true;
        }

        public override bool IsEnable(object sender, object element)
        {
            return true;
        }
    }
}