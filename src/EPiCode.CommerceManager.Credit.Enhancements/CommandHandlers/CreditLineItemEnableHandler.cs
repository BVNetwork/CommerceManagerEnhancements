using CommerceManagerCreditEnhancements.Services;
using EPiServer.ServiceLocation;
using Mediachase.BusinessFoundation;
using Mediachase.Commerce.Manager.Order.CommandHandlers.ReturnFormHandlers.Strategy;
using Mediachase.Commerce.Orders;

namespace CommerceManagerCreditEnhancements.CommandHandlers
{
    public class CreditLineItemEnableHandler : EditableCommandHandler
    {
        private ICreditService _creditHandler;

        public CreditLineItemEnableHandler()
        {
            _creditHandler = ServiceLocator.Current.GetInstance<ICreditService>();
        }

        protected override bool IsCommandEnable(OrderGroup order, CommandParameters cp)
        {
            return _creditHandler.IsOrderEnabledForLineItemCredit(order);
        }
    }
}