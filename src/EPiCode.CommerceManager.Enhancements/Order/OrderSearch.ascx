<%@ Control Language="C#" AutoEventWireup="true" Inherits="CommerceManagerEnhancements.Order.OrderSearchList"
	CodeBehind="OrderSearch.ascx.cs" %>
<%@ Register Src="../Core/Controls/EcfListViewControl.ascx" TagName="EcfListViewControl"
	TagPrefix="core" %>
<%@ Register Src="../Core/Controls/CalendarDatePicker.ascx" TagName="CalendarDatePicker" TagPrefix="ecf" %>
<%@ Register TagPrefix="IbnWebControls" Namespace="Mediachase.BusinessFoundation" Assembly="Mediachase.BusinessFoundation, Version=8.7.1.466, Culture=neutral, PublicKeyToken=41d2e7a615ba286c" %>


<%@ Register tagPrefix="orders" namespace="Mediachase.Commerce.Orders.DataSources" assembly="Mediachase.Commerce" %>



     <style type="text/css">
         .auto-style1 {
             height: 26px;
         }
         .auto-style2 {
             width: 20px;
             height: 26px;
         }
         tr.padding10 td{
             padding: 10px;
             margin: 10px !important;
             font-weight: bold;
         }
     </style>




<IbnWebControls:McDock ID="DockTop" runat="server" Anchor="Top" EnableSplitter="False" DefaultSize="160">
    <DockItems>
   
     <asp:Panel runat="server" ID="pnlMain" DefaultButton="btnSearch" Height="160px" BackColor="#F8F8F8" BorderColor="Gray" BorderWidth="0">
            <div id="DataForm">
                <table cellpadding="0" style="background-color: #F8F8F8;" cellspacing="0">
                    <tr class="auto-style1 padding10">
                        <td>Order Information</td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td>Customer Information</td>
                    </tr>
                    <tr>
						<td>
							<asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:SharedStrings, Class_Type %>" />:
						</td>
						<td>
							<asp:DropDownList ID="ClassType" Width="140" runat="server" AutoPostBack="true">
								<asp:ListItem Value="PurchaseOrder" Text="<%$ Resources:OrderStrings, Order_Purchase_Order %>" Selected="True" />
								<asp:ListItem Value="ShoppingCart" Text="<%$ Resources:OrderStrings, Order_Shopping_Cart %>" />
								<asp:ListItem Value="PaymentPlan" Text="<%$ Resources:OrderStrings, Order_Payment_Plan %>" />
							</asp:DropDownList>
						</td>
						<td style="width: 20px;">&nbsp;</td>
						<td>
							<asp:Literal ID="lbRmaDescr" runat="server" Text="<%$ Resources:OrderStrings, Return_Number%>" />:
						</td>
						<td>
							<asp:TextBox ID="tbRmaNUmber" Width="140" runat="server"></asp:TextBox>
						</td>
						<td style="width: 20px;">&nbsp;</td>
						<td>
                            <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:SharedStrings, Customer %>" />:
                        </td>
                        <td>
                            <asp:TextBox ID="CustomerKeyword" runat="server" Width="140"></asp:TextBox>
                        </td>
                        <td></td>
					</tr>
					<tr>
						<td>
							<asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:SharedStrings, Status %>" />:
						</td>
						<td>
							<asp:DropDownList ID="OrderStatusList" Width="140" runat="server"></asp:DropDownList>
						</td>
						<td style="width: 20px;">&nbsp;</td>
						<td>
							<asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:SharedStrings, ID %>" />:
						</td>
						<td>
							<asp:TextBox ID="OrderNumber" Width="140" runat="server"></asp:TextBox>
						</td>
						<td style="width: 20px;">&nbsp;</td>
						<td>Mail:</td>
                        <td>
                            <asp:TextBox ID="MailAddress" runat="server" Width="140"></asp:TextBox>
                        </td>

					</tr>
					<tr>
						<td class="auto-style1">
							<asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:SharedStrings, Date_Range %>" />:
						</td>
						<td class="auto-style1">
							<asp:DropDownList ID="DataRange" Width="140" runat="server">
							</asp:DropDownList>						   
						</td>
						<td class="auto-style2"></td>
						<td class="auto-style1">
							Market:</td>
						<td class="auto-style1">
								<asp:DropDownList ID="MarketList" Width="140" runat="server">
							</asp:DropDownList>
						</td>
						<td class="auto-style2"></td>
                        <td>Phone:</td>
						<td class="auto-style1">
					
									
							
						    <asp:TextBox ID="Phone" runat="server" Width="140"></asp:TextBox>
					
									
							
						</td>
					</tr>                 
                    <tr>
                        <td>Start Date:</td>
                        <td>
                            <ecf:CalendarDatePicker runat="server" ID="StartDate" TimeDisplay="false" ValidationEnabled="False" />
                            
                        </td>
                        <td></td>
                        <td>End Date:</td>
                        <td><ecf:CalendarDatePicker runat="server" ID="EndDate" TimeDisplay="false" ValidationEnabled="False"/></td>
                    <td></td>
                        <td><asp:Button ID="btnSearch" runat="server" Width="100" Text="<%$ Resources:SharedStrings, Search %>" /></td>
                    </tr>
				</table>
			</div>
		</asp:Panel>


	</DockItems>
</IbnWebControls:McDock>
<core:EcfListViewControl ID="MyListView" runat="server" AppId="Order" ViewId="OrderSearch-List"
	ShowTopToolbar="false"></core:EcfListViewControl>
<orders:OrderDataSource runat="server" ID="OrderListDataSource"></orders:OrderDataSource>
