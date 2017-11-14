<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="RB201000.aspx.cs" Inherits="Page_RB201000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Countries" TypeName="RB.RapidByte.CountryMaint" >
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
    <px:PXGrid ID="grid" runat="server" Style="z-index: 100; top: 7px; left: 4px; height: 398px; width: 99%;"
		AllowPaging="True" AllowSearch="True" AdjustPageSize="Auto" DataSourceID="ds" SkinID="Primary" TabIndex="1500" TemporaryFilterCaption="Filter Applied">
<EmptyMsg ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here." NamedComboMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." NamedComboAddMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." FilteredMessage="No records found.
Try to change filter to see records here." FilteredAddMessage="No records found.
Try to change filter to see records here." NamedFilteredMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." NamedFilteredAddMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." AnonFilteredMessage="No records found.
Try to change filter to see records here." AnonFilteredAddMessage="No records found.
Try to change filter to see records here."></EmptyMsg>
		<Levels>
			<px:PXGridLevel DataKeyNames="CountryCD" DataMember="Countries">
			    <Columns>
                    <px:PXGridColumn DataField="CountryCD">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="Description" Width="200px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXGrid>
</asp:Content>
