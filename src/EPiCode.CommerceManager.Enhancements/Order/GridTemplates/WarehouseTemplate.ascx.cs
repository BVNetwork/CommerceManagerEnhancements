using System;
using System.Linq;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Orders;
using Mediachase.Web.Console.Interfaces;

namespace CommerceManagerEnhancements.Order.GridTemplates
{
    public partial class WarehouseTemplate : System.Web.UI.UserControl, IEcfListViewTemplate
    {
        
        private object _DataItem;

        public override void DataBind()
        {
            base.DataBind();
            OrderGroup dataItem = this.DataItem as OrderGroup;

            var warehouseRepository = ServiceLocator.Current.GetInstance<IWarehouseRepository>();





            if (dataItem != null && dataItem.OrderForms.Any() && dataItem.OrderForms[0].Shipments.Any())
            {
                var code = dataItem.OrderForms[0].Shipments[0].WarehouseCode;
                var warehouse = warehouseRepository.Get(code);


                if (warehouse != null)
                {
                    TextLabel.Text = warehouse.Name;
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