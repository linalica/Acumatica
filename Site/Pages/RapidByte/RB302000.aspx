<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="RB302000.aspx.cs" Inherits="Page_RB302000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" PrimaryView="Shipments" SuspendUnloading="False" TypeName="RB.RapidByte.ShipmentEntry">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="Shipments" TabIndex="5100">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True"/>
		    <px:PXSelector ID="edShipmentNbr" runat="server" DataField="ShipmentNbr">
            </px:PXSelector>
            <px:PXTextEdit ID="edShipmentType" runat="server" AlreadyLocalized="False" DataField="ShipmentType" DefaultLocale="">
            </px:PXTextEdit>
            <px:PXDateTimeEdit ID="edDeliveryDate" runat="server" AlreadyLocalized="False" DataField="DeliveryDate" DefaultLocale="">
            </px:PXDateTimeEdit>
            <px:PXSelector ID="edCustomerID" runat="server" DataField="CustomerID">
            </px:PXSelector>
            <px:PXTextEdit ID="edDescription" runat="server" AlreadyLocalized="False" DataField="Description" DefaultLocale="">
            </px:PXTextEdit>
            <px:PXLayoutRule runat="server" StartColumn="True">
            </px:PXLayoutRule>
            <px:PXDateTimeEdit ID="edShipmentDate" runat="server" AlreadyLocalized="False" DataField="ShipmentDate" DefaultLocale="">
            </px:PXDateTimeEdit>
            <px:PXTextEdit ID="edStatus" runat="server" AlreadyLocalized="False" DataField="Status" DefaultLocale="">
            </px:PXTextEdit>
            <px:PXDateTimeEdit ID="edDeliveryMaxDate" runat="server" AlreadyLocalized="False" DataField="DeliveryMaxDate" DefaultLocale="">
            </px:PXDateTimeEdit>
            <px:PXLayoutRule runat="server" StartColumn="True">
            </px:PXLayoutRule>
            <px:PXNumberEdit ID="edTotalQty" runat="server" AlreadyLocalized="False" DataField="TotalQty" DefaultLocale="" Enabled="False">
            </px:PXNumberEdit>
            <px:PXNumberEdit ID="edShippedQty" runat="server" AlreadyLocalized="False" DataField="ShippedQty" DefaultLocale="" Enabled="False">
            </px:PXNumberEdit>
            <px:PXNumberEdit ID="edPendingQty" runat="server" AlreadyLocalized="False" DataField="PendingQty" DefaultLocale="" Enabled="False">
            </px:PXNumberEdit>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details">
		<Levels>
			<px:PXGridLevel DataKeyNames="ShipmentNbr,ProductID" DataMember="ShipmentLines">
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
