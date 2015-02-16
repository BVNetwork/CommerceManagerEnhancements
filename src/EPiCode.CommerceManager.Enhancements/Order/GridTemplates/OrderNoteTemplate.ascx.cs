using System;
using System.Linq;
using Mediachase.Commerce.Orders;
using Mediachase.Web.Console.Interfaces;

namespace CommerceManagerEnhancements.Order.GridTemplates
{
    public partial class OrderNoteTemplate : System.Web.UI.UserControl, IEcfListViewTemplate
    {
        
        private object _DataItem;

        public override void DataBind()
        {
            base.DataBind();
            OrderGroup dataItem = this.DataItem as OrderGroup;

            if (dataItem != null && dataItem.OrderNotes.Any())
            {
                var latestNote = dataItem.OrderNotes.Where(x => x.Type != OrderNoteTypes.System.ToString()).OrderByDescending(x => x.Created).FirstOrDefault();

                if (latestNote != null)
                {
                    var noteText = string.Empty;

                    if (!string.IsNullOrEmpty(latestNote.Title))
                    {
                        noteText += string.Format("{0}: ", latestNote.Title);
                    }
                    
                    LatestOrderNote.Text = noteText + latestNote.Detail;
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