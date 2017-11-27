<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="RB302000.aspx.cs" Inherits="Page_RB302000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" PrimaryView="Shipments" SuspendUnloading="False" TypeName="RB.RapidByte.ShipmentEntry">
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" DataMember="Shipments" TabIndex="5100">
        <Template>
            <px:PXLayoutRule runat="server" StartRow="True" ControlSize="SM" LabelsWidth="S" StartColumn="True" />
            <px:PXSelector ID="edShipmentNbr" runat="server" DataField="ShipmentNbr">
            </px:PXSelector>
            <px:PXDropDown ID="edShipmentType" runat="server" DataField="ShipmentType" CommitChanges="True">
            </px:PXDropDown>
            <px:PXDateTimeEdit ID="edShipmentDate" runat="server" AlreadyLocalized="False" DataField="ShipmentDate">
            </px:PXDateTimeEdit>
            <px:PXSelector ID="edCustomerID" runat="server" DataField="CustomerID">
            </px:PXSelector>
            <px:PXLayoutRule runat="server" ColumnSpan="3">
            </px:PXLayoutRule>
            <px:PXTextEdit ID="edDescription" runat="server" AlreadyLocalized="False" DataField="Description">
            </px:PXTextEdit>
            <px:PXLayoutRule runat="server" StartColumn="True" ControlSize="S" LabelsWidth="S">
            </px:PXLayoutRule>
            <px:PXDateTimeEdit ID="edDeliveryDate" runat="server" AlreadyLocalized="False" DataField="DeliveryDate">
            </px:PXDateTimeEdit>
            <px:PXDropDown ID="edStatus" runat="server" DataField="Status" CommitChanges="True">
            </px:PXDropDown>
            <px:PXDateTimeEdit ID="edDeliveryMaxDate" runat="server" AlreadyLocalized="False" DataField="DeliveryMaxDate">
            </px:PXDateTimeEdit>
            <px:PXLayoutRule runat="server" StartColumn="True" ColumnWidth="S" LabelsWidth="S">
            </px:PXLayoutRule>
            <px:PXNumberEdit ID="edTotalQty" runat="server" AlreadyLocalized="False" DataField="TotalQty" Enabled="False">
            </px:PXNumberEdit>
            <px:PXNumberEdit ID="edShippedQty" runat="server" AlreadyLocalized="False" DataField="ShippedQty" Enabled="False">
            </px:PXNumberEdit>
            <px:PXNumberEdit ID="edPendingQty" runat="server" AlreadyLocalized="False" DataField="PendingQty" Enabled="False">
            </px:PXNumberEdit>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="150px" SkinID="Details" TabIndex="100"
        TemporaryFilterCaption="Filter Applied" SyncPosition="True">
        <EmptyMsg ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here."
            NamedComboMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here."
            NamedComboAddMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here."
            FilteredMessage="No records found.
Try to change filter to see records here."
            FilteredAddMessage="No records found.
Try to change filter to see records here."
            NamedFilteredMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here."
            NamedFilteredAddMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here."
            AnonFilteredMessage="No records found.
Try to change filter to see records here."
            AnonFilteredAddMessage="No records found.
Try to change filter to see records here."></EmptyMsg>
        <Levels>
            <px:PXGridLevel DataKeyNames="ShipmentNbr,ProductID"
                DataMember="ShipmentLines">
                <RowTemplate>
                    <px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="S" StartRow="True" GroupCaption="Item Details" StartColumn="True">
                    </px:PXLayoutRule>
                    <px:PXSelector ID="edProductID" runat="server" DataField="ProductID">
                    </px:PXSelector>
                    <px:PXTextEdit ID="edDescription" runat="server" AlreadyLocalized="False" DataField="Description" DefaultLocale="">
                    </px:PXTextEdit>
                    <px:PXNumberEdit ID="edLineQty" runat="server" AlreadyLocalized="False" DataField="LineQty" DefaultLocale="">
                    </px:PXNumberEdit>
                    <px:PXCheckBox ID="edCancelled" runat="server" AlreadyLocalized="False" DataField="Cancelled" Text="Cancelled">
                    </px:PXCheckBox>
                    <px:PXLayoutRule runat="server" EndGroup="True">
                    </px:PXLayoutRule>
                    <px:PXLayoutRule runat="server" ControlSize="S" GroupCaption="Delivery" LabelsWidth="S" StartColumn="True">
                    </px:PXLayoutRule>
                    <px:PXDateTimeEdit ID="edShipmentDate" runat="server" DataField="ShipmentDate" AlreadyLocalized="False" DefaultLocale="">
                    </px:PXDateTimeEdit>
                    <px:PXDateTimeEdit ID="edShipmentTime" runat="server" DataField="ShipmentTime" TimeMode="True" AlreadyLocalized="False" DefaultLocale="">
                    </px:PXDateTimeEdit>
                    <px:PXDateTimeEdit ID="edShipmentMinTime" runat="server" DataField="ShipmentMinTime" TimeMode="True" AlreadyLocalized="False" DefaultLocale="">
                    </px:PXDateTimeEdit>
                    <px:PXDateTimeEdit ID="edShipmentMaxTime" runat="server" DataField="ShipmentMaxTime" TimeMode="True" AlreadyLocalized="False" DefaultLocale="">
                    </px:PXDateTimeEdit>
                    <px:PXLayoutRule runat="server" EndGroup="True">
                    </px:PXLayoutRule>
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="ProductID" Width="120px" CommitChanges="True">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="Description" Width="200px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="LineQty" TextAlign="Right" Width="100px" CommitChanges="True">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="Cancelled" TextAlign="Center" Type="CheckBox" Width="60px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="ShipmentDate" Width="90px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="ShipmentTime" TextAlign="Right">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="ShipmentMinTime" TextAlign="Right">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="ShipmentMaxTime" Width="90px">
                    </px:PXGridColumn>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <Mode AllowFormEdit="True" />
    </px:PXGrid>
</asp:Content>
