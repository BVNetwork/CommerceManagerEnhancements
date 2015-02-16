using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using EPiServer.ServiceLocation;
using Mediachase.BusinessFoundation;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Manager.Apps.Customer.Primitives;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.DataSources;
using Mediachase.Commerce.Orders.Search;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Controls;
using Resources;

namespace CommerceManagerEnhancements.Order
{
    public partial class OrderSearchList : OrderBaseUserControl
    {
        private const string _ShoppingCartClass = "ShoppingCart";
        private const string _PaymentPlanClass = "PaymentPlan";

        int _MaximumRows = 20;
        int _StartRowIndex = 0;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
            {
                if (!IsPostBack)
                    MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("OrderGroupId", "CustomerId");

                LoadDataAndDataBind();
                DataBind();
            }
            
            InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (ClassType.SelectedValue != "PurchaseOrder")
            {
                tbRmaNUmber.Visible = false;
                lbRmaDescr.Visible = false;
            }
            else
            {
                tbRmaNUmber.Visible = true;
                lbRmaDescr.Visible = true;
            }
        }

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        private void LoadDataAndDataBind()
        {
            OrderStatusList.Items.Add(new ListItem("[ Any ]", ""));

            // Get the localized text strings for the OrderStatus enum
            List<KeyValuePair<Enum, string>> orderStatusList = ResourceEnumConverter.GetValues(typeof(OrderStatus));

            foreach (KeyValuePair<Enum, string> orderStatus in orderStatusList)
            {
                string status = orderStatus.Key.ToString();
                int statusId = (int)(OrderStatus)Enum.Parse(typeof(OrderStatus), status);
                OrderStatusList.Items.Add(new ListItem(orderStatus.Value, status));
            }

            DataRange.Items.Clear();
            DataRange.Items.Add(new ListItem(SharedStrings.All, ""));
            DataRange.Items.Add(new ListItem(SharedStrings.Today, "today"));
            DataRange.Items.Add(new ListItem(SharedStrings.This_Week, "thisweek"));
            DataRange.Items.Add(new ListItem("Last 7 days", "last7days"));
            DataRange.Items.Add(new ListItem(SharedStrings.This_Month, "thismonth"));
            DataRange.Items.Add(new ListItem("Last 30 days", "last30days"));
            DataRange.Items.Add(new ListItem("Custom","custom"));
        

            MarketList.Items.Clear();
            var marketService = ServiceLocator.Current.GetInstance<IMarketService>();
            var markets= marketService.GetAllMarkets().ToList();

            markets.Insert(0,new MarketImpl("All"){MarketName = "All"});

            MarketList.DataSource = markets;
            MarketList.DataValueField = "MarketId";
            MarketList.DataTextField = "MarketName";
            MarketList.DataBind();
            MarketList.Items.Insert(0,new ListItem("All","All"));




            StringBuilder script = new StringBuilder("this.disabled = true;\r\n");
            script.AppendFormat((string) "__doPostBack('{0}', '');", (object) btnSearch.UniqueID);
            btnSearch.OnClientClick = script.ToString();
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

            btnSearch.Click += new EventHandler(btnSearch_Click);

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
                    InitDataSource(_StartRowIndex, _MaximumRows, true, sortExpression);
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
            //if (IsPostBack && ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID))
            //{
            //    // reset start index
            //    _StartRowIndex = 0;

            //    InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
            //    DataBind();
            //    MyListView.MainUpdatePanel.Update();
            //}
        }

        /// <summary>
        /// Handles the PagePropertiesChanging event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
        void CurrentListView_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            _MaximumRows = e.MaximumRows;
            _StartRowIndex = e.StartRowIndex;
        }

        /// <summary>
        /// Handles the PagePropertiesChanged event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
        {
            InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
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
           


            MyListView.DataSourceID = OrderListDataSource.ID;

            //Now date in current time zone
            DateTime nowDate = DateTime.Now;
            DateTime startDate = nowDate;
            DateTime endDate = nowDate;

            string rmaNumber = tbRmaNUmber.Text;
            string classType = ClassType.SelectedValue;

            bool applyDateFilter = false;


            String filterType = DataRange.SelectedValue;

            if (String.Compare(filterType, "thisweek", true) == 0)
            {
                startDate = ManagementHelper.GetStartOfWeek(nowDate.Date);
                endDate = nowDate;
                applyDateFilter = true;
            }
            else if (String.Compare(filterType, "thismonth", true) == 0)
            {
                startDate = new DateTime(nowDate.Year, nowDate.Month, 1);
                endDate = nowDate;
                applyDateFilter = true;
            }
            else if (String.Compare(filterType, "today", true) == 0)
            {
                startDate = nowDate.Date;
                endDate = nowDate;
                applyDateFilter = true;
            }
            else if (String.Compare(filterType, "last7days", true) == 0)
            {
                startDate = nowDate.AddDays(-7);
                endDate = nowDate;
                applyDateFilter = true;
            }
            else if (String.Compare(filterType, "last30days", true) == 0)
            {
                startDate = nowDate.AddDays(-30);
                endDate = nowDate;
                applyDateFilter = true;
            }
            else if (String.Compare(filterType, "custom", true) == 0)
            {
                if (StartDate.Value == DateTime.MinValue)
                {
                    StartDate.Value = DateTime.Today;
                }

                if (EndDate.Value == DateTime.MinValue)
                {
                    EndDate.Value = StartDate.Value;
                }

                startDate = StartDate.Value.Date;
                endDate = EndDate.Value.Date;

               

                if (endDate == startDate)
                {
                    endDate = endDate.AddDays(1).AddMinutes(-1);
                }

                applyDateFilter = true;
            }
            else
            {
                applyDateFilter = false;
            }

         

            if (applyDateFilter)
            {
                var metaField = SearchOnCreatedDate.Checked ? "META.Created" : "META.Modified";

                OrderListDataSource.Parameters.SqlMetaWhereClause = String.Format("{0} between '{1}' and '{2}'",
                                                                                  metaField,
                                                                                  startDate.ToUniversalTime().ToString("s"), 
                                                                                  endDate.ToUniversalTime().ToString("s"));
                OrderListDataSource.Options.Classes.Add(classType);
            }

            OrderListDataSource.Options.Classes.Add(classType);
            
            if (!string.IsNullOrEmpty(rmaNumber) && ClassType.SelectedValue == "PurchaseOrder")
            {
                OrderListDataSource.RMANumber = rmaNumber;
            }

            StringBuilder sqlWhereClause = new StringBuilder("(1=1)");
            string sqlMetaWhereClause = string.Empty;

            int orderId = 0;
            if (int.TryParse(OrderNumber.Text, out orderId) && orderId > 0 && ClassType.SelectedValue != "PurchaseOrder")
            {
                sqlWhereClause.AppendFormat(" AND (OrderGroupId = {0})", orderId);
            }
            else if (!String.IsNullOrEmpty(OrderNumber.Text.Trim()) && ClassType.SelectedValue == "PurchaseOrder")
            {
                sqlMetaWhereClause = String.Format("(TrackingNumber like '{0}')", ManagementHelper.MakeSafeSearchFilter(OrderNumber.Text.Trim()).Replace("*","%"));
            }

            string status = OrderStatusList.SelectedValue;

            if (!String.IsNullOrEmpty(status))
                sqlWhereClause.AppendFormat(" AND (Status = '{0}')", status);

            string marketId = MarketList.SelectedValue;
            if (!String.IsNullOrEmpty(marketId) && marketId!="All")
            {
                sqlWhereClause.AppendFormat(" AND (MarketId = '{0}')", marketId);
            }

            if (String.IsNullOrEmpty(orderByClause))
                orderByClause = String.Format("OrderGroupId DESC");

            //search by CustomerName
            if (!String.IsNullOrEmpty(CustomerKeyword.Text))
            {
                StringBuilder customerWhereClause = new StringBuilder();
                foreach (string keyword in CustomerKeyword.Text.Split(' '))
                {
                    if (customerWhereClause.Length > 0)
                        customerWhereClause.Append(" OR ");
                    customerWhereClause.AppendFormat(" CustomerName LIKE '{0}'", ManagementHelper.MakeSafeSearchFilter(keyword).Replace("*", "%"));
                }
                sqlWhereClause.AppendFormat(" AND ({0})", customerWhereClause);
            }


            //OrderGroupAddress filter
            string phone = Phone.Text;
            List<string> orderAddressFilters = new List<string>();
            bool useAddressFilter = false;


            if (!string.IsNullOrEmpty(phone))
            {
                orderAddressFilters.Add(string.Format("((DaytimePhoneNumber like '{0}') OR (EveningPhoneNumber like '{0}'))", phone).Replace("*", "%"));
                useAddressFilter = true;
            }

            string mail = MailAddress.Text;

            if (!string.IsNullOrEmpty(mail))
            {
                orderAddressFilters.Add(string.Format("Email like '{0}'", mail).Replace("*", "%"));
                useAddressFilter = true;
            }


            if (useAddressFilter)
            {
                sqlWhereClause.AppendFormat(" AND (OrderGroupId in (Select OrderGroupId from OrderGroupAddress where {0}))", string.Join(" AND " , orderAddressFilters));
            }     



            OrderListDataSource.Parameters.SqlWhereClause = sqlWhereClause.ToString();

            if (!String.IsNullOrEmpty(sqlMetaWhereClause))
                OrderListDataSource.Parameters.SqlMetaWhereClause = sqlMetaWhereClause;

            OrderSearchOptions options = new OrderSearchOptions();
            OrderListDataSource.Options.Namespace = "Mediachase.Commerce.Orders";

            
            if (String.Compare(classType, _ShoppingCartClass, true) == 0)
                MyListView.DataMember = OrderDataSource.OrderDataSourceView.CartsViewName;
            else if (String.Compare(classType, _PaymentPlanClass, true) == 0)
                MyListView.DataMember = OrderDataSource.OrderDataSourceView.PaymentPlansViewName;
            else
                MyListView.DataMember = OrderDataSource.OrderDataSourceView.PurchaseOrdersViewName;

            OrderListDataSource.Options.RecordsToRetrieve = recordsCount;
            OrderListDataSource.Options.StartingRecord = startRowIndex;
            OrderListDataSource.Parameters.OrderByClause = orderByClause;
        }

      
        /// <summary>
        /// Handles the Click event of the btnSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void btnSearch_Click(object sender, EventArgs e)
        {
            _StartRowIndex = 0;
            InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
            MyListView.ResetPageNumber();
            DataBind();
            MyListView.MainUpdatePanel.Update();

            btnSearch.Enabled = true;
         //   upSearchButton.Update();
        }

    

    }
}
