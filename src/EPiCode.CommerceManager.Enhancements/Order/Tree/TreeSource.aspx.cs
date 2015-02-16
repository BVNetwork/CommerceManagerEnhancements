using System;
using System.Collections.Generic;
using System.Linq;
using CommerceManagerEnhancements.Configuration;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using Mediachase.BusinessFoundation;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Security;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace CommerceManagerEnhancements.Order.Tree
{
    public partial class AppsOrderTreeTreeSource : BasePage
    {
        private const string ModuleName = "Order";

        public enum TreeListType
        {
            None,
            Root,
            OrderSearch,
            PurchaseOrders,
            PurchaseOrdersByStatus,
            Carts,
            PaymentPlans,

            PaymentMethods,
            ShippingMethods
        }

        /// <summary>
        /// Gets the type of the list.
        /// </summary>
        /// <value>The type of the list.</value>
        public TreeListType ListType
        {
            get
            {
                string nodeType = Request.Form["type"];

                if (String.IsNullOrEmpty(nodeType))
                    return TreeListType.Root;

                TreeListType type = TreeListType.None;

                try
                {
                    type = (TreeListType)Enum.Parse(typeof(TreeListType), nodeType, true);
                }
                catch
                {
                    type = TreeListType.None;
                }

                return type;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            BindList();
        }

        /// <summary>
        /// Binds the list.
        /// </summary>
        private void BindList()
        {
            var markets = ServiceLocator.Current.GetInstance<IMarketService>().GetAllMarkets().Where(x=>x.IsEnabled);

            switch (ListType)
            {
                case TreeListType.PurchaseOrders:
                    BindPurchaseOrders(markets);
                    break;
                case TreeListType.PurchaseOrdersByStatus:
                    BindPurchaseOrdersByStatus(markets);
                    break;
                case TreeListType.PaymentMethods:
                    BindPaymentMethods();
                    break;
                case TreeListType.ShippingMethods:
                    BindShippingMethods();
                    break;
            }
        }

        /// <summary>
        /// Binds the root.
        /// </summary>
        private void BindRoot()
        {
            List<JsonTreeNode> nodes = new List<JsonTreeNode>();
            nodes.Add(JsonTreeNode.CreateNode("OrderSearch", String.Empty, "Order Search", ModuleName,
                "OrderSearch-List", String.Empty, TreeListType.OrderSearch.ToString(), true));

            // PurchaseOrders node
            JsonTreeNode poNode = JsonTreeNode.CreateNode("PurchaseOrders", String.Empty, "Purchase Orders", ModuleName, "Orders-List", String.Empty, TreeListType.PurchaseOrders.ToString());
            poNode.icon = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("~/Apps/Order/images/PurchaseOrders.png");
            poNode.children = new List<JsonTreeNode>();

            var todayNode = JsonTreeNode.CreateNode("PO-TodayOrders", "Today", ModuleName, "Orders-List",
                "filter=today&class=PurchaseOrder", true);



            poNode.children.Add(todayNode);
            poNode.children.Add(JsonTreeNode.CreateNode("PO-WeekOrders", "This Week", ModuleName, "Orders-List", "filter=thisweek&class=PurchaseOrder", true));
            poNode.children.Add(JsonTreeNode.CreateNode("PO-MonthOrders", "This Month", ModuleName, "Orders-List", "filter=thismonth&class=PurchaseOrder", true));
            poNode.children.Add(JsonTreeNode.CreateNode("PO-AllOrders", "All", ModuleName, "Orders-List", "filter=all&class=PurchaseOrder", true));
            nodes.Add(poNode);

            // PurchaseOrdersByStatus node
            JsonTreeNode posNode = JsonTreeNode.CreateNode("PurchaseOrdersByStatus", String.Empty, "Purchase Orders By Status", ModuleName, "Orders-List", String.Empty, TreeListType.PurchaseOrdersByStatus.ToString());
            posNode.children = new List<JsonTreeNode>();

            OrderStatusDto statusDto = OrderStatusManager.GetDefinedOrderStatuses();

            foreach (OrderStatusDto.OrderStatusRow statusRow in statusDto.OrderStatus.Rows)
                posNode.children.Add(JsonTreeNode.CreateNode("PO-Status-" + statusRow.OrderStatusId, statusRow.Name, ModuleName, "Orders-List", String.Format("status={0}", statusRow.Name), true));
            nodes.Add(posNode);

            // Carts node
            JsonTreeNode cartsNode = JsonTreeNode.CreateNode("Carts", "Carts", ModuleName, "Orders-List", "filter=all&class=ShoppingCart", false);
            cartsNode.children = new List<JsonTreeNode>();
            cartsNode.children.Add(JsonTreeNode.CreateNode("CART-TodayOrders", "Today", ModuleName, "Orders-List", "filter=today&class=ShoppingCart", true));
            cartsNode.children.Add(JsonTreeNode.CreateNode("CART-WeekOrders", "This Week", ModuleName, "Orders-List", "filter=thisweek&class=ShoppingCart", true));
            cartsNode.children.Add(JsonTreeNode.CreateNode("CART-MonthOrders", "This Month", ModuleName, "Orders-List", "filter=thismonth&class=ShoppingCart", true));
            cartsNode.children.Add(JsonTreeNode.CreateNode("CART-AllOrders", "All", ModuleName, "Orders-List", "filter=all&class=ShoppingCart", true));
            nodes.Add(cartsNode);

            // PaymentPlans node
            JsonTreeNode ppNode = JsonTreeNode.CreateNode("PaymentPlans", "Payment Plans (recurring)", ModuleName, "Orders-List", "filter=all&class=PaymentPlan", false);
            ppNode.children = new List<JsonTreeNode>();
            ppNode.children.Add(JsonTreeNode.CreateNode("PP-TodayOrders", "Today", ModuleName, "Orders-List", "filter=today&class=PaymentPlan", true));
            ppNode.children.Add(JsonTreeNode.CreateNode("PP-WeekOrders", "This Week", ModuleName, "Orders-List", "filter=thisweek&class=PaymentPlan", true));
            ppNode.children.Add(JsonTreeNode.CreateNode("PP-MonthOrders", "This Month", ModuleName, "Orders-List", "filter=thismonth&class=PaymentPlan", true));
            ppNode.children.Add(JsonTreeNode.CreateNode("PP-AllOrders", "All", ModuleName, "Orders-List", "filter=all&class=PaymentPlan", true));
            nodes.Add(ppNode);

            WriteArray(nodes);
        }

        /// <summary>
        /// Binds the root.
        /// </summary>
        /// <param name="markets"></param>
        private void BindPurchaseOrders(IEnumerable<IMarket> markets)
        {
            var marketList = markets.ToList();

            List<JsonTreeNode> nodes = new List<JsonTreeNode>();

            List<DateFilterSettings> filterSettings = DateFilterSettings.GetDateFilters(ModuleName, "Orders-List", "PurchaseOrder");

            foreach (var dateFilter in filterSettings)
            {
                var filterNode = JsonTreeNode.CreateNode(dateFilter.Id, dateFilter.Title, dateFilter.Module,
                    dateFilter.List, dateFilter.Parameters, true);                

                filterNode = BindMarketsToNode(filterNode, marketList);

                nodes.Add(filterNode);
            }
            
            WriteArray(nodes);
        }


        /// <summary>
        /// Binds the purchase orders by status.
        /// </summary>
        /// <param name="enumerable"></param>
        private void BindPurchaseOrdersByStatus(IEnumerable<IMarket> markets)
        {
            var marketList = markets.ToList();
            List<JsonTreeNode> nodes = new List<JsonTreeNode>();
            List<KeyValuePair<Enum, string>> orderStatusList = ResourceEnumConverter.GetValues(typeof(OrderStatus));
            
            foreach (KeyValuePair<Enum, string> orderStatus in orderStatusList)
            {
                string status = orderStatus.Key.ToString();
                int statusId = (int)(OrderStatus)Enum.Parse(typeof(OrderStatus), status);

                var statusNode = JsonTreeNode.CreateNode("PO-Status-" + statusId, orderStatus.Value, ModuleName,
                    "Orders-List", String.Format("status={0}", status), true);


                statusNode = BindMarketsToNode(statusNode, marketList);
                                
                nodes.Add(statusNode);

            }

            WriteArray(nodes);
        }

        private JsonTreeNode BindMarketsToNode(JsonTreeNode node, IEnumerable<IMarket> markets)
        {
            if (MarketFilterConfiguration.UseDropdownMarketFilter())
            {
                return node;
            }

            List<JsonTreeNode> marketNodes = new List<JsonTreeNode>();
            foreach (IMarket market in markets)
            {
                var id = node.id + "-market-" + market.MarketId.Value;
                var filter = node.parameters + string.Format("&marketid={0}", market.MarketId.Value);
                marketNodes.Add(JsonTreeNode.CreateNode(id, market.MarketName, ModuleName, "Orders-List", filter,
                    true));
            }

            node.children = marketNodes;

            node.leaf = false;

            return node;
        }

        /// <summary>
        /// Binds the payment methods.
        /// </summary>
        private void BindPaymentMethods()
        {
            SecurityContext.Current.CheckPermissionForCurrentUser("order:admin:payments:mng:view");

            // add payment gateway languages
            List<JsonTreeNode> nodes = LoadLanguages("PaymentLanguage", "PaymentMethods-List");

            WriteArray(nodes);
        }

        /// <summary>
        /// Binds the shipping methods.
        /// </summary>
        private void BindShippingMethods()
        {
            SecurityContext.Current.CheckPermissionForCurrentUser("order:admin:shipping:methods:mng:view");

            // add shipping gateway languages
            List<JsonTreeNode> nodes = LoadLanguages("ShippingMethodLanguage", "ShippingMethodLanguage-List");

            WriteArray(nodes);
        }

        /// <summary>
        /// Loads the languages.
        /// </summary>
        /// <param name="baseNodeId">The base node id.</param>
        /// <param name="viewId">The view id.</param>
        /// <returns></returns>
        private List<JsonTreeNode> LoadLanguages(string baseNodeId, string viewId)
        {
            List<JsonTreeNode> nodes = new List<JsonTreeNode>();

            // add nodes with languages
            IList<LanguageBranch> languages = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>().ListEnabled();
            foreach (LanguageBranch language in languages)
            {
                nodes.Add(JsonTreeNode.CreateNode(baseNodeId + "-" + language.Culture.Name,
                    /*culture.DisplayName*/ language.Name, ModuleName, viewId, String.Format("lang={0}", language.Culture.Name.ToLower()), true));
            }

            return nodes;
        }

        /// <summary>
        /// Writes the array.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        private void WriteArray(List<JsonTreeNode> nodes)
        {
            string json = JsonSerializer.Serialize(nodes);
            Response.Write(json);
        }
    }

    public class DateFilterSettings
    {
        public string List { get; set; }

        public string Module { get; set; }

        public string Parameters { get; set; }

        public string Title { get; set; }

        public string Id { get; set; }

        public static List<DateFilterSettings> GetDateFilters(string moduleName, string listName, string className)
        {
            var list = new List<DateFilterSettings>();

            list.Add(new DateFilterSettings()
            {
                Id = "PO-TodayOrders",
                Title = "Today",
                Parameters = string.Format("filter=today&class={0}",className),
                Module = moduleName,
                List = listName
            });

            list.Add(new DateFilterSettings()
            {
                Id = "PO-WeekOrders",
                Title = "This week",
                Parameters = string.Format("filter=thisweek&class={0}", className),
                Module = moduleName,
                List = listName
            });

            list.Add(new DateFilterSettings()
            {
                Id = "PO-Last7Days",
                Title = "Last 7 days",
                Parameters = string.Format("filter=last7days&class={0}", className),
                Module = moduleName,
                List = listName
            });

            list.Add(new DateFilterSettings()
            {
                Id = "PO-MonthOrders",
                Title = "This month",
                Parameters = string.Format("filter=thismonth&class={0}", className),
                Module = moduleName,
                List = listName
            });

            list.Add(new DateFilterSettings()
            {
                Id = "PO-Last30Days",
                Title = "Last 30 days",
                Parameters = string.Format("filter=last30days&class={0}", className),
                Module = moduleName,
                List = listName
            });

            list.Add(new DateFilterSettings()
            {
                Id = "PO-AllOrders",
                Title = "All",
                Parameters = string.Format("filter=all&class={0}", className),
                Module = moduleName,
                List = listName
            });
        

            return list;
        }


    }
}