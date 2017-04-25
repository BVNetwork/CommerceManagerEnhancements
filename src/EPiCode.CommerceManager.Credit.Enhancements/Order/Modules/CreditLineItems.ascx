<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreditLineItems.ascx.cs" Inherits="CommerceManagerCreditEnhancements.Order.Modules.CreditLineItems" %>

<%@ Register TagPrefix="mc2" Assembly="Mediachase.BusinessFoundation" Namespace="Mediachase.BusinessFoundation" %>

<style>
 #credit-line-items .credit-items-container {   
   margin:15px;   
 }

 #credit-line-items .shipping-credit-container {
   margin: 15px;
 }

 #credit-line-items .shipping-credit-row {
   padding: 3px;
 }

 #credit-line-items .shipping-credit-label {
   width: 120px;
   display:inline-block;
 }

   .myGridClass {
  width: 869px;
  /*this will be the color of the odd row*/
  background-color: #fff;
  margin: 15px;
  
  border: solid 1px #c1c1c1;
  border-collapse:collapse;
}

/*data elements*/
.myGridClass td {
  padding: 2px;
  min-width:40px;
  border: solid 1px #c1c1c1;
  color: #717171;  
}


/*header elements*/
.myGridClass th {
  padding: 4px 2px;
  color: #000;
  background: #f0f0f0;
  border-left: solid 1px #c1c1c1;
  font-size: 0.9em;
}

/*his will be the color of even row*/
.myGridClass .myAltRowClass { background: #fcfcfc repeat-x top; }

/*and finally, we style the pager on the bottom*/
.myGridClass .myPagerClass { background: #424242; }

.myGridClass .myPagerClass table { margin: 5px 0; }

.myGridClass .myPagerClass td {
  border-width: 0;
  padding: 0 6px;
  border-left: solid 1px #666;
  font-weight: bold;
  color: #fff;
  line-height: 12px;
}

.myGridClass .myPagerClass a { color: #666; text-decoration: none; }

.myGridClass .myPagerClass a:hover { color: #000; text-decoration: none; } 
</style>

<div id="credit-line-items" class="credit-items-container">

<asp:GridView runat="server" ID="LineItemGridView" AutoGenerateColumns="False"     
        AllowPaging="true"
  CssClass="myGridClass"
        OnRowEditing="LineItemGridView_RowEditing"         
        OnRowCancelingEdit="LineItemGridView_RowCancelingEdit" 
        OnRowUpdating="LineItemGridView_RowUpdating"
        OnPageIndexChanging="LineItemGridView_PageIndexChanging">
  <Columns>
        <asp:CommandField ButtonType="Image" CancelImageUrl="~/Apps/MetaDataBase/images/undo.png" EditImageUrl="~/Apps/MetaDataBase/images/edit.gif"
 ShowEditButton="True" UpdateImageUrl="~/Apps/MetaDataBase/images/saveitem.gif"/>            
   <asp:TemplateField HeaderText="Id" SortExpression="Id">
    <ItemTemplate>
        <asp:Label ID="Label1" runat="server" Text='<%#Bind("Id") %>'></asp:Label>
    </ItemTemplate>
</asp:TemplateField>
    <asp:TemplateField HeaderText="Code" SortExpression="Code">
    <ItemTemplate>
        <asp:Label ID="Label2" runat="server" Text='<%#Bind("Code") %>'></asp:Label>
    </ItemTemplate>
</asp:TemplateField>
    <asp:TemplateField HeaderText="Name" SortExpression="Name">
    <ItemTemplate>
        <asp:Label ID="Label3" runat="server" Text='<%#Bind("Name") %>'></asp:Label>
    </ItemTemplate>
      </asp:TemplateField>
          <asp:TemplateField HeaderText="Price" SortExpression="PlacedPrice">
    <ItemTemplate>
        <asp:Label ID="Label8" runat="server" Text='<%#Bind("PlacedPrice","{0:#,##0.00}") %>'></asp:Label>
    </ItemTemplate>
</asp:TemplateField>
      <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity">
    <ItemTemplate>
        <asp:Label ID="Label9" runat="server" Text='<%#Bind("Quantity","{0:#,##0.00}") %>'></asp:Label>
    </ItemTemplate>
</asp:TemplateField>    
    <asp:TemplateField HeaderText="Discount" SortExpression="Discount">
    <ItemTemplate>
        <asp:Label ID="Label5" runat="server" Text='<%#Bind("Discount","{0:#,##0.00}") %>'></asp:Label>
    </ItemTemplate>
</asp:TemplateField>
      <asp:TemplateField HeaderText="Order level discount" SortExpression="OrderLevelDiscount">
    <ItemTemplate>
        <asp:Label ID="Label10" runat="server" Text='<%#Bind("OrderLevelDiscount","{0:#,##0.00}") %>'></asp:Label>
    </ItemTemplate>
</asp:TemplateField>
      <asp:TemplateField HeaderText="Total" SortExpression="Price">
    <ItemTemplate>
        <asp:Label ID="Label4" runat="server" Text='<%#Bind("Price","{0:#,##0.00}") %>'></asp:Label>
    </ItemTemplate>
</asp:TemplateField>
    <asp:TemplateField HeaderText="Additional Discount" SortExpression="ExtraDiscount">
    <ItemTemplate>
        <asp:Label ID="Label6" runat="server" Text='<%#Bind("ExtraDiscount","{0:#,##0.00}") %>'></asp:Label>
    </ItemTemplate>
      <EditItemTemplate>
          <asp:TextBox ID="ExtraDiscount" runat="server" Text='<%#Bind("ExtraDiscount","{0:#,##0.00}") %>'></asp:TextBox>
      </EditItemTemplate>
</asp:TemplateField>
  </Columns>
</asp:GridView>

            	<div style="position: absolute; left: 15px; right: 25px; bottom: 0px; height: 45px;">
				<table width="100%">
					<tr>
						<td class="popup-buttons" colspan="2">
							<mc2:IMButton ID="btnSave" runat="server" class="btn-save" OnServerClick="btnSave_ServerClick" Text="Save and credit" CustomImage="~/Apps/MetaDataBase/images/saveclose.gif">
							</mc2:IMButton>
							&nbsp;
							<mc2:IMButton ID="btnCancel" runat="server" CausesValidation="false" OnServerClick="btnCancel_ServerClick"  Text="<%$ Resources:Common, btnCancel %>" CustomImage="~/Apps/MetaDataBase/images/close.gif">
							</mc2:IMButton>
						</td>
					</tr>
				</table>
				</div>
  </div>