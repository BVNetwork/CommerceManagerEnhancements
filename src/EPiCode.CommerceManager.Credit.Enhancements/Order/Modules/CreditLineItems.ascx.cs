using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommerceManagerCreditEnhancements.Services;
using CommerceManagerCreditEnhancements.Validators;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Mediachase.BusinessFoundation;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Manager.Apps_Code.Order;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console.Common;
using CreditItem = CommerceManagerCreditEnhancements.DTO.CreditItem;

namespace CommerceManagerCreditEnhancements.Order.Modules
{
    public partial class CreditLineItems : System.Web.UI.UserControl
    {
        private const string LINE_ITEM_ID = "Id";
        private const string CODE = "Code";
        private const string NAME = "Name";
        private const string PLACED_PRICE = "PlacedPrice";
        private const string QUANTITY = "Quantity";
        private const string PRICE = "Price";
        private const string DISCOUNT = "Discount";
        private const string ORDER_LEVEL_DISCOUNT = "OrderLevelDiscount";
        private const string EXTRA_DISCOUNT = "ExtraDiscount";

        private static Injected<ICreditService> _creditService;
        private static Injected<IDiscountValidator> _validator;
        private static Injected<ILogger> _logger;



        public int OrderGroupId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("OrderGroupId");
            }
        }

        protected string ReturnCommand
        {
            get
            {
                string str = string.Empty;
                if (this.Request.QueryString["ReturnCommand"] != null)
                    str = this.Request.QueryString["ReturnCommand"];
                return str;
            }
        }

        protected bool IsEditMode
        {
            get
            {
                bool flag = true;
                if (this.ViewState["IsEditMode"] != null)
                    flag = (bool)this.ViewState["IsEditMode"];
                return flag;
            }
            set
            {
                this.ViewState["IsEditMode"] = (object)(bool)(value ? true : false);
            }
        }

        public int ShipmentId
        {
            get
            {
                return (int)HttpContext.Current.Items[(object)"ShipmentId"];
            }
            set
            {
                HttpContext.Current.Items[(object)"ShipmentId"] = (object)value;
            }
        }

        protected PurchaseOrder CurrentOrder
        {
            get
            {
                return OrderHelper.GetPurchaseOrderById(this.OrderGroupId);
            }
        }

    
        protected void Page_Load(object sender, EventArgs e)
        {           
            if (!IsPostBack)
            {
                BindLineItems();
            }
        }

        private void BindLineItems()
        {
            // Create a new table.
            DataTable lineItemTable = new DataTable("LineItemList");

            var items =
                CurrentOrder.OrderForms[0].LineItems
                    .Select(x => new DTO.CreditItem() { LineItemId = x.LineItemId,
                        Code = x.Code,
                        Name = x.DisplayName,
                        Price = x.ExtendedPrice,
                        Discount = x.LineItemDiscountAmount,
                        OrderLevelDiscount = x.OrderLevelDiscountAmount,
                        Quantity = x.Quantity,
                        PlacedPrice = x.PlacedPrice,
                        ExtraDiscount = 0.00M,
                    });

            // Create the columns.  
            lineItemTable.Columns.Add(LINE_ITEM_ID, typeof(long));
            lineItemTable.Columns.Add(CODE, typeof(string));
            lineItemTable.Columns.Add(NAME, typeof(string));
            lineItemTable.Columns.Add(PLACED_PRICE, typeof(decimal));
            lineItemTable.Columns.Add(QUANTITY, typeof(decimal));
            lineItemTable.Columns.Add(DISCOUNT, typeof(decimal));
            lineItemTable.Columns.Add(ORDER_LEVEL_DISCOUNT, typeof(decimal));            
            lineItemTable.Columns.Add(PRICE, typeof(decimal));            
            lineItemTable.Columns.Add(EXTRA_DISCOUNT, typeof(decimal));

            //Add data to the new table.
            foreach (DTO.CreditItem lineItem in items)
            {
                DataRow tableRow = lineItemTable.NewRow();
                tableRow[LINE_ITEM_ID] = lineItem.LineItemId;
                tableRow[CODE] = lineItem.Code;
                tableRow[NAME] = lineItem.Name;
                tableRow[PRICE] = lineItem.Price;
                tableRow[PLACED_PRICE] = lineItem.PlacedPrice;
                tableRow[DISCOUNT] = lineItem.Discount;
                tableRow[ORDER_LEVEL_DISCOUNT] = lineItem.OrderLevelDiscount;
                tableRow[QUANTITY] = lineItem.Quantity;
                tableRow[EXTRA_DISCOUNT] = lineItem.ExtraDiscount;
                lineItemTable.Rows.Add(tableRow);
            }

            //Add shipping
            var shipments = CurrentOrder.OrderForms[0].Shipments;

            foreach (Shipment shipment in shipments)
            {
                DataRow tableRow = lineItemTable.NewRow();
                tableRow[LINE_ITEM_ID] = shipment.ShipmentId;
                tableRow[CODE] = "shipment";
                tableRow[NAME] = shipment.ShippingMethodName;
                tableRow[PRICE] = shipment.ShippingTotal;
                tableRow[PLACED_PRICE] = shipment.ShippingSubTotal;
                tableRow[DISCOUNT] = shipment.ShippingDiscountAmount;
                tableRow[ORDER_LEVEL_DISCOUNT] = 0;
                tableRow[QUANTITY] = 1;
                tableRow[EXTRA_DISCOUNT] = 0;
                lineItemTable.Rows.Add(tableRow);
            }
            
            //Persist the table in the Session object.
            Session["LineItemTable"] = lineItemTable;

            //Bind data to the GridView control.
            BindData();

          
            //LineItems.DataSource = items;

            //LineItems.DataBind();
        }


        protected void LineItemGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            LineItemGridView.PageIndex = e.NewPageIndex;
            //Bind data to the GridView control.
            BindData();
        }

        protected void LineItemGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //Set the edit index.
            LineItemGridView.EditIndex = e.NewEditIndex;
            //Bind data to the GridView control.
            btnSave.Disabled = true;
            BindData();
        }

        protected void LineItemGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            //Reset the edit index.
            LineItemGridView.EditIndex = -1;
            btnSave.Disabled = false;
            //Bind data to the GridView control.
            BindData();
        }

        protected void LineItemGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //Retrieve the table from the session object.
            DataTable dt = (DataTable)Session["LineItemTable"];

            //Update the values.
            GridViewRow row = LineItemGridView.Rows[e.RowIndex];
            bool validDiscount = false;

            decimal discount = 0;

            if (Decimal.TryParse(((TextBox) row.FindControl("ExtraDiscount")).Text, out discount))
            {

                string code = (string) dt.Rows[row.DataItemIndex][CODE];
                long id = (long) dt.Rows[row.DataItemIndex][LINE_ITEM_ID];

                if (code == "shipment")
                {
                    var shipment = CurrentOrder.OrderForms[0].Shipments.FirstOrDefault(x => x.ShipmentId == id);
                    if (_validator.Service.IsShippingDiscountValid(shipment, discount))
                    {
                        validDiscount = true;
                    }
                }
                else
                {
                    var lineItem = CurrentOrder.OrderForms[0].LineItems.FirstOrDefault(x => x.LineItemId == id);
                    if (_validator.Service.IsLineItemDiscountValid(lineItem, discount))
                    {
                        validDiscount = true;
                    }
                }
            }

            if (validDiscount)
            {
                dt.Rows[row.DataItemIndex][EXTRA_DISCOUNT] = discount;
            }
            else
            {
                DisplayMessage("Not a valid discount amount.");
            }


            var items = GetCreditItems();

            btnSave.Text = "Save and credit " +
                           items.Sum(x => x.ExtraDiscount).ToString("#,##0.00 ") +
                           CurrentOrder.BillingCurrency;

            //Reset the edit index.
            LineItemGridView.EditIndex = -1;

            

            btnSave.Disabled = false;

            //Bind data to the GridView control.
            BindData();
        }

        private void BindData()
        {
            LineItemGridView.DataSource = Session["LineItemTable"];
            LineItemGridView.DataBind();
        }

        private List<DTO.CreditItem> GetCreditItems()
        {
            List<DTO.CreditItem> items = new List<CreditItem>();
            var table = Session["LineItemTable"] as DataTable;
            if (table != null)
            {
                foreach (DataRow tableRow in table.Rows)
                {
                    if (tableRow[EXTRA_DISCOUNT] != null && (decimal)tableRow[EXTRA_DISCOUNT] > 0)
                    {
                        items.Add(new CreditItem()
                        {
                            Code = (string)tableRow[CODE],
                            LineItemId = (long)tableRow[LINE_ITEM_ID],
                            ExtraDiscount = (decimal)tableRow[EXTRA_DISCOUNT],
                            Quantity = (decimal)tableRow[QUANTITY]
                        });                      
                    }
                }
            }
            return items;
        }

        protected void btnSave_ServerClick(object sender, EventArgs e)
        {
            try
            {
                var items = GetCreditItems();

                var preResult = _creditService.Service.PreCredit(CurrentOrder, items);

                if (preResult.IsSuccess)
                {
                    var result = _creditService.Service.Credit(CurrentOrder, items);
                    if (result.IsSuccess)
                    {
                        var postResult = _creditService.Service.PostCredit(CurrentOrder, items);
                        if (postResult.IsSuccess)
                        {
                            this.CloseDialog();
                        }
                        else
                        {
                            DisplayMessage(postResult.Messages);
                        }
                    }
                    else
                    {
                        DisplayMessage(result.Messages);
                    }
                }
                else
                {
                    DisplayMessage(preResult.Messages);
                }
            }
            catch (Exception ex)
            {
                _logger.Service.Error(ex.Message,ex);
                DisplayMessage(ex.Message);
            }
        }

        private void DisplayMessage(IEnumerable<string> resultMessages)
        {
            var messsage = string.Join(", ", resultMessages);
            DisplayMessage(messsage);
        }

        private void DisplayMessage(string message)
        {
            ShowStatusMessage(message);
        }

        public void ShowStatusMessage(string message)
        {
            CommandManager manager = CommandManager.GetCurrent(Page);
            manager.InfoMessage = message;            
        }

     
        private void CloseDialog()
        {
            string sParams = string.Empty;
            if (!string.IsNullOrEmpty(this.ReturnCommand))
                sParams = new CommandParameters(this.ReturnCommand).ToString();
            Mediachase.BusinessFoundation.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, sParams, true);
        }

        protected void btnCancel_ServerClick(object sender, EventArgs e)
        {
            this.CloseDialog();
        }
    }
}