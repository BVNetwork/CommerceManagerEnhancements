using System;
using System.Collections.Generic;
using System.Web;
using EPiServer.ServiceLocation;
using Mediachase.BusinessFoundation;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace CommerceManagerEnhancements.Order.MarketTree
{
    public partial class AppsMarketsTreeTreeSource : BasePage
    {
        private const string ModuleName = "Order";

        public enum TreeListType
        {
            None,
            Root,
            Markets
        }

        #region Properties
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
        #endregion

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
            switch (ListType)
            {
                case TreeListType.Markets:
                    BindMarket();
                    break;
            }
        }

        /// <summary>
        /// Makes the node id.
        /// </summary>
        /// <param name="baseId">The base id.</param>
        /// <returns></returns>
        private string MakeNodeId(string baseId)
        {
            return String.Concat(ModuleName, "_", baseId);
        }

        #region Bind Markets
        /// <summary>
        /// Binds the markets.
        /// </summary>
        private void BindMarket()
        {
            List<JsonTreeNode> nodes = new List<JsonTreeNode>();

            IEnumerable<IMarket> allMarkets = ServiceLocator.Current.GetInstance<IMarketService>().GetAllMarkets();

            foreach (IMarket market in allMarkets)
            {
                JsonTreeNode newNode = JsonTreeNode.CreateNode(MakeNodeId(market.MarketId.Value), HttpUtility.HtmlEncode(market.MarketId.Value), ModuleName, "Orders-List", String.Format("marketid={0}", market.MarketId), true);
                string treeLoader = Request.Url.AbsoluteUri;

                if (!market.IsEnabled)
                    newNode.icon = Page.ResolveUrl("~/Apps/Content/images/folder-disabled.gif");

                nodes.Add(newNode);
            }

            WriteArray(nodes);
        }
        #endregion

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
}
