<%@ Control Language="C#" AutoEventWireup="true" Inherits="CommerceManagerEnhancements.Order.OrdersList" Codebehind="OrderList.ascx.cs" %>
<%@ Register Src="../Core/Controls/EcfListViewControl.ascx" TagName="EcfListViewControl" TagPrefix="core" %>
<%@ Register TagPrefix="orders" Namespace="Mediachase.Commerce.Orders.DataSources" Assembly="Mediachase.Commerce, Version=8.7.1.466, Culture=neutral, PublicKeyToken=6e58b501b34abce3" %>
<%@ Register TagPrefix="IbnWebControls" Namespace="Mediachase.BusinessFoundation" Assembly="Mediachase.BusinessFoundation, Version=8.7.1.466, Culture=neutral, PublicKeyToken=41d2e7a615ba286c" %>

<style>
    .padding10 {
        padding: 5px;
    }
    .padding10 strong {
        font-weight: bold;
    }
</style>

<IbnWebControls:McDock ID="DockTop" runat="server" Anchor="Top" EnableSplitter="False" DefaultSize="30">
    <DockItems>
   <div class="padding10">
        <strong>Filter by market: </strong>
                <asp:DropDownList runat="server" ID="Markets" OnSelectedIndexChanged="Markets_OnSelectedIndexChanged" AutoPostBack="true"/>
       </div>


	</DockItems>
</IbnWebControls:McDock>


<orders:OrderDataSource runat="server" ID="OrderListDataSource"></orders:OrderDataSource>
<core:EcfListViewControl id="MyListView" runat="server" DataSourceID="OrderListDataSource" AppId="Order" ViewId="Orders-List" ShowTopToolbar="true"></core:EcfListViewControl>

