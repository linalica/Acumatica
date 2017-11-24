<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="RB501000.aspx.cs" Inherits="Page_RB501000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Orders" TypeName="RB.RapidByte.SalesOrderProcess" >
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100"
		AllowPaging="True" AllowSearch="True" AdjustPageSize="Manual" DataSourceID="ds" SkinID="Inquire" TabIndex="1300" TemporaryFilterCaption="Filter Applied">
		<Levels>
			<px:PXGridLevel DataKeyNames="OrderNbr" DataMember="Orders">
			    <Columns>
                    <px:PXGridColumn AllowCheckAll="True" DataField="Selected" TextAlign="Center" Type="CheckBox" Width="60px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderNbr">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderDate" Width="90px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="CustomerID">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="ShippedDate" Width="90px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderTotal" TextAlign="Right" Width="100px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="Status">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXGrid>
</asp:Content>
