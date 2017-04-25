using Mediachase.Commerce.Engine;
using Mediachase.Commerce.Workflow.Activities;

namespace CommerceManagerCreditEnhancements.Workflows
{
    [ActivityFlowConfiguration(Name = "OrderGoodwillCreditWorkflow")]
    public class OrderGoodwillCreditWorkflow : ActivityFlow
    {
        public override ActivityFlowRunner Configure(ActivityFlowRunner activityFlow)
        {
            return activityFlow.Do<CalculateTotalsActivity>();
        }
    }
}
