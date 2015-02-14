<%@ Control Language="C#" AutoEventWireup="true" Inherits="CommerceManagerEnhancements.Order.OrdersList" Codebehind="OrderList.ascx.cs" %>
<%@ Register Src="../Core/Controls/EcfListViewControl.ascx" TagName="EcfListViewControl" TagPrefix="core" %>
<%@ Register TagPrefix="orders" Namespace="Mediachase.Commerce.Orders.DataSources" Assembly="Mediachase.Commerce, Version=8.7.1.466, Culture=neutral, PublicKeyToken=6e58b501b34abce3" %>
<orders:OrderDataSource runat="server" ID="OrderListDataSource"></orders:OrderDataSource>
<core:EcfListViewControl id="MyListView" runat="server" DataSourceID="OrderListDataSource" AppId="Order" ViewId="Orders-List" ShowTopToolbar="true"></core:EcfListViewControl>

