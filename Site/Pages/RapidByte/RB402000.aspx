<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="RB402000.aspx.cs" Inherits="Page_RB402000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Filter" TypeName="RB.RapidByte.SupplierInq">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="ViewProduct" Visible="False" DependOnGrid="grid">
            </px:PXDSCallbackCommand>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" DataMember="Filter" TabIndex="700">
        <Template>
            <px:PXLayoutRule runat="server" StartRow="True" ControlSize="M" LabelsWidth="S" StartColumn="True" />
            <px:PXSelector runat="server" DataField="CountryCD" CommitChanges="True" ID="edCountryCD">
            </px:PXSelector>
            <px:PXNumberEdit ID="edMinOrderQty" runat="server" CommitChanges="True"
                DataField="MinOrderQty" Size="Empty" Width="70px" AlreadyLocalized="False">
            </px:PXNumberEdit>
            <px:PXLayoutRule runat="server" StartColumn="True" ControlSize="SM" SuppressLabel="True">
            </px:PXLayoutRule>
            <px:PXCheckBox ID="edGroupBySupplier" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="GroupBySupplier" Text="Show Average Price">
            </px:PXCheckBox>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" Height="150px" SkinID="Inquire" SyncPosition="True">
        <Levels>
            <px:PXGridLevel DataKeyNames="SupplierID,ProductID" DataMember="SupplierProducts">
                <Columns>
                    <px:PXGridColumn DataField="ProductID" Width="140px" LinkCommand="ViewProduct">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="SupplierPrice" TextAlign="Right"
                        Width="100px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="LastPurchaseDate" Width="120px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="MinOrderQty" TextAlign="Right"
                        Width="100px">
                    </px:PXGridColumn>
                    <%--<px:PXGridColumn DataField="Supplier__CountryCD" Width="100px">
                     </px:PXGridColumn>--%>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXGrid>
</asp:Content>
