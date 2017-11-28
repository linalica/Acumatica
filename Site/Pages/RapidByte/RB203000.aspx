<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="RB203000.aspx.cs" Inherits="Page_RB203000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" PrimaryView="Products" TypeName="RB.RapidByte.ProductMaint" SuspendUnloading="False">
    </px:PXDataSource>
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Products" TabIndex="1900">
        <Template>
            <px:PXLayoutRule runat="server" StartRow="True" ControlSize="M" />
            <px:PXSelector ID="edProductCD" runat="server" DataField="ProductCD">
            </px:PXSelector>
            <px:PXTextEdit ID="edProductName" runat="server" AlreadyLocalized="False" DataField="ProductName" DefaultLocale="">
            </px:PXTextEdit>
            <px:PXCheckBox ID="edActive" runat="server" AlreadyLocalized="False" DataField="Active" Text="Active">
            </px:PXCheckBox>
            <px:PXTextEdit ID="edStockUnit" runat="server" AlreadyLocalized="False" DataField="StockUnit" DefaultLocale="">
            </px:PXTextEdit>
            <px:PXNumberEdit ID="edUnitPrice" runat="server" AlreadyLocalized="False" DataField="UnitPrice" DefaultLocale="">
            </px:PXNumberEdit>
            <px:PXNumberEdit ID="edMinAvailQty" runat="server" AlreadyLocalized="False" DataField="MinAvailQty" DefaultLocale="">
            </px:PXNumberEdit>
            <px:PXNumberEdit ID="edAvailQty" runat="server" DataField="AvailQty">
            </px:PXNumberEdit>
        </Template>
        <AutoSize Container="Window" Enabled="True" MinHeight="200" />
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
</asp:Content>
