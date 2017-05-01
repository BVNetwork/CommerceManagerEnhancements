using System;
using System.Data;
using System.Linq;
using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;

namespace CommerceManagerCreditEnhancements.Order.GridTemplates
{
    public partial class ArticleNumberTemplate : BaseUserControl, IEcfListViewTemplate
    {
        
        private object _DataItem;


        public override void DataBind()
        {
            base.DataBind();
            LineItem dataItem = this.DataItem as LineItem;

            if (dataItem != null)
            {
                ArticleNumber.Text = dataItem["ArticleNumber"] != null
                  ? dataItem["ArticleNumber"].ToString()
                  : "---";
            }
            else if (this.DataItem is DataRowView)
            {
                DataRowView dataRowView = this.DataItem as DataRowView;
                var items = LineItem.Load(MetaDataContext.Instance, (int) dataRowView["LineItemId"], "LineItemEx");
                ArticleNumber.Text = items["ArticleNumber"] != null ? items["ArticleNumber"].ToString() : "---";
            }
            else
            {
                ArticleNumber.Text = "---";
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