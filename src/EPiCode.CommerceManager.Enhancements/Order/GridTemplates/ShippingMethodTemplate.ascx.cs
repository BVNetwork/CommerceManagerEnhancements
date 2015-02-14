using System;
using System.Linq;
using Mediachase.Commerce.Orders;
using Mediachase.Web.Console.Interfaces;

namespace CommerceManagerEnhancements.Order.GridTemplates
{
    public partial class ShippingMethodTemplate : System.Web.UI.UserControl, IEcfListViewTemplate
    {
        
        private object _DataItem;

        public override void DataBind()
        {
            base.DataBind();
            OrderGroup dataItem = this.DataItem as OrderGroup;

            if (dataItem != null && dataItem.OrderForms.Any() && dataItem.OrderForms[0].Shipments.Any())
            {
                var shipping = dataItem.OrderForms[0].Shipments[0].ShippingMethodName;

                if (shipping != null)
                {
                    TextLabel.Text = shipping;
                }
            }
        }    

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public object DataItem
        {
            get
            {
                return this._DataItem;
            }
            set
            {
                this._DataItem = value;
            }
        }
    }
}