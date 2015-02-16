<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Core.Controls.CalendarDatePicker" Codebehind="CalendarDatePicker.ascx.cs" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=3.0.30930.28736, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>
<asp:TextBox runat="server" Width="150" ID="Date"></asp:TextBox>
<asp:ImageButton runat="Server" ID="Image1" ImageAlign="Middle" CssClass="CalendarImage" ImageUrl="~/Apps/Shell/styles/images/calendar.png" />
<ajaxToolkit:CalendarExtender ID="calendarButtonExtender" runat="server" TargetControlID="Date" PopupButtonID="Image1" />
<asp:RequiredFieldValidator runat="server" ID="rfvDate" ControlToValidate="Date" Display="Dynamic" ErrorMessage="<%$ Resources:SharedStrings, Valid_Date_Required %>" />
<asp:CompareValidator id="cvDate" runat="Server" Operator="DataTypeCheck" Type="Date" ErrorMessage="<%$ Resources:SharedStrings, Valid_Date_Format_Required %>" Display="Dynamic" ControlToValidate="Date" />
<asp:RangeValidator id="rvDate" runat="Server" Type="Date" MinimumValue="1752/1/1" MaximumValue="9999/12/31" ErrorMessage="<%$ Resources:SharedStrings, Valid_Date_Required %>" Display="Dynamic" ControlToValidate="Date" />

<asp:TextBox runat="server" Width="60" ID="tbTime"></asp:TextBox>
<ajaxToolkit:MaskedEditExtender ID="timeMaskedExtender" runat="server" Mask="99:99" MaskType="Time" MessageValidatorTip="true" TargetControlID="tbTime"></ajaxToolkit:MaskedEditExtender>
<ajaxToolkit:MaskedEditValidator ID="timeMaskedValidator" runat="server" ControlExtender="timeMaskedExtender" ControlToValidate="tbTime" Display="Dynamic" EmptyValueMessage="<%$ Resources:SharedStrings, Time_Required %>" InvalidValueMessage="<%$ Resources:SharedStrings, Valid_Time_Required %>" IsValidEmpty="false"></ajaxToolkit:MaskedEditValidator>
