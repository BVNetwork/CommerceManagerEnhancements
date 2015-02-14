using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Mediachase.BusinessFoundation;
using Mediachase.Commerce.Orders.DataSources;
using Mediachase.Commerce.Orders.Search;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Controls;

namespace CommerceManagerEnhancements.Order
{
    public partial class OrdersList : OrderBaseUserControl
    {
        int _StartRowIndex = 0;

        /// <summary>
        /// Gets the type of the filter.
        /// </summary>
        /// <value>The type of the filter.</value>
        public string FilterType
        {
            get
            {
                return Request.QueryString["filter"];
            }
        }

        public string MarketFilter
        {
            get { return Request.QueryString["marketid"]; }
        }

        /// <summary>
        /// Gets the type of the class.
        /// </summary>
        /// <value>The type of the class.</value>
        public string ClassType
        {
            get
            {
                return ManagementHelper.GetStringValue(Request.QueryString["class"], "PurchaseOrder");
            }
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status
        {
            get
            {
                return Request.QueryString["status"];
            }
        }

		protected int GetMaximumRows()
		{
			return EcfListView.GetSavedPageSize(this.Page, MyListView.ViewId, EcfListView.DefaultPageSize);
		}

		private string GetPageTitle()
		{
			string title = String.Empty;

			string classType = ClassType;

			if (String.Compare(classType, "PurchaseOrder", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				if (!String.IsNullOrEmpty(Status))
					title = UtilHelper.GetResFileString("{OrderStrings:Order_By_Status}");
				else
					title = UtilHelper.GetResFileString("{OrderStrings:Order_List}");
			}
			else if (String.Compare(classType, "ShoppingCart", StringComparison.InvariantCultureIgnoreCase) == 0)
				title = UtilHelper.GetResFileString("{OrderStrings:Cart_List}");
			else if (String.Compare(classType, "PaymentPlan", StringComparison.InvariantCultureIgnoreCase) == 0)
				title = UtilHelper.GetResFileString("{OrderStrings:PaymentPlan_List}");

			return title;
		}


        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
				Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "SetTitleScriptKey", String.Format("CSManagementClient.SetPageTitle('{0}');", GetPageTitle()), true);

            if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
            {
                if (!IsPostBack)
                {
                    MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("OrderGroupId", "CustomerId");
                }

				InitDataSource(_StartRowIndex, GetMaximumRows(), true, "");
                DataBind();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            MyListView.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
            MyListView.CurrentListView.PagePropertiesChanging += new EventHandler<PagePropertiesChangingEventArgs>(CurrentListView_PagePropertiesChanging);
            MyListView.CurrentListView.Sorting += new EventHandler<ListViewSortEventArgs>(CurrentListView_Sorting);

            Page.LoadComplete += new EventHandler(Page_LoadComplete);
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Sorting event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ListViewSortEventArgs"/> instance containing the event data.</param>
        void CurrentListView_Sorting(object sender, ListViewSortEventArgs e)
        {
            AdminView view = MyListView.CurrentListView.GetAdminView();
            foreach (ViewColumn column in view.Columns)
            {
                // find the column which is to be sorted
                if (column.AllowSorting && String.Compare(column.GetSortExpression(), e.SortExpression, true) == 0)
                {
                    // reset start index
                    _StartRowIndex = 0;

                    // update DataSource parameters
                    string sortExpression = e.SortExpression + " " + (e.SortDirection == SortDirection.Descending ? "DESC" : "ASC");
					InitDataSource(_StartRowIndex, GetMaximumRows(), true, sortExpression);
                }
            }
        }

        /// <summary>
        /// Handles the LoadComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Page_LoadComplete(object sender, EventArgs e)
        {
			if (IsPostBack && ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID))
			{
				// reset start index
				_StartRowIndex = 0;

				InitDataSource(_StartRowIndex, GetMaximumRows(), true, MyListView.CurrentListView.SortExpression);
				DataBind();
				MyListView.MainUpdatePanel.Update();
			}
        }

        /// <summary>
        /// Handles the PagePropertiesChanging event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
        void CurrentListView_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            _StartRowIndex = e.StartRowIndex;
        }

        /// <summary>
        /// Handles the PagePropertiesChanged event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
        {
			InitDataSource(_StartRowIndex, GetMaximumRows(), true, MyListView.CurrentListView.SortExpression);
        }

        /// <summary>
        /// Inits the data source.
        /// </summary>
        /// <param name="startRowIndex">Start index of the row.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="returnTotalCount">if set to <c>true</c> [return total count].</param>
        /// <param name="orderByClause">The order by clause.</param>
        private void InitDataSource(int startRowIndex, int recordsCount, bool returnTotalCount, string orderByClause)
        {
			DateTime nowDate = DateTime.Now;
			DateTime startDate = nowDate;
			DateTime endDate = nowDate;
            bool applyDateFilter = false;

            if (FilterType == "thisweek")
            {
				startDate = ManagementHelper.GetStartOfWeek(nowDate.Date);
				endDate = nowDate;
				applyDateFilter = true;
            }
            else if (FilterType == "thismonth")
            {
				startDate = new DateTime(nowDate.Year, nowDate.Month, 1);
				endDate = nowDate;
				applyDateFilter = true;
            }
            else if (FilterType == "today")
            {
				startDate = nowDate.Date;
				endDate = nowDate;
				applyDateFilter = true;
            }
            else
            {
                applyDateFilter = false;
            }



			if (applyDateFilter)
			{
				OrderListDataSource.Parameters.SqlMetaWhereClause = String.Format("META.Modified between '{0}' and '{1}'",
																			  startDate.ToUniversalTime().ToString("s"),
																			  endDate.ToUniversalTime().ToString("s"));
			}

            List<string> sqlWhereStatments = new List<string>();

            if (!String.IsNullOrEmpty(Status))
                sqlWhereStatments.Add(String.Format("Status = '{0}'", ManagementHelper.MakeSafeSearchFilter(Status)));

            if (!string.IsNullOrEmpty(MarketFilter))
            {
                sqlWhereStatments.Add(string.Format("MarketId = '{0}'", ManagementHelper.MakeSafeSearchFilter(MarketFilter)));
            }

            if (sqlWhereStatments.Any())
            {
                OrderListDataSource.Parameters.SqlWhereClause = string.Join(" AND ", sqlWhereStatments);
            }


            if (String.IsNullOrEmpty(orderByClause))
                orderByClause = String.Format("OrderGroupId DESC");

            OrderSearchOptions options = new OrderSearchOptions();
            OrderListDataSource.Options.Namespace = "Mediachase.Commerce.Orders";
            OrderListDataSource.Options.Classes.Add(ClassType);

            if (ClassType == "ShoppingCart")
                MyListView.DataMember = OrderDataSource.OrderDataSourceView.CartsViewName;
            else if (ClassType == "PaymentPlan")
                MyListView.DataMember = OrderDataSource.OrderDataSourceView.PaymentPlansViewName;
            else
                MyListView.DataMember = OrderDataSource.OrderDataSourceView.PurchaseOrdersViewName;

            OrderListDataSource.Options.RecordsToRetrieve = recordsCount;
            OrderListDataSource.Options.StartingRecord = startRowIndex;
            OrderListDataSource.Parameters.OrderByClause = orderByClause;
        }
    }
}
