using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceManagerCreditEnhancements.DTO;
using Mediachase.Commerce.Orders;

namespace CommerceManagerCreditEnhancements.Services
{
    public interface ICreditService
    {
        bool IsOrderEnabledForLineItemCredit(OrderGroup order);

        ServiceResult PreCredit(OrderGroup order, IEnumerable<CreditItem> creditItems);

        ServiceResult Credit(OrderGroup order, IEnumerable<CreditItem> creditItems);

        ServiceResult PostCredit(OrderGroup order, IEnumerable<CreditItem> creditItems);
    }
}
