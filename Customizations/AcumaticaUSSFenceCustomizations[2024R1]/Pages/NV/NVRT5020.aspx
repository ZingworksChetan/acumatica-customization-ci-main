<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NVRT5020.aspx.cs" Inherits="Page_NVRT5020" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Filter" TypeName="NV.Rental360.RentalCycle.NVRTCycleBillProcessing" >
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="form" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="PXFormView1" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Filter" 
        NoteIndicator="False" FilesIndicator="False"  TabIndex="100">
		<Template>
			<px:PXLayoutRule runat="server"  ControlSize="M" LabelsWidth="SM" StartColumn="True"/>
			<px:PXDateTimeEdit ID="edInvoiceDate" runat="server" DataField="InvoiceDate" CommitChanges="true"></px:PXDateTimeEdit>
			<px:PXCheckBox ID="edIncludeReturns" runat="server" DataField="IncludeReturns" AlignLeft="true"></px:PXCheckBox>
			
			<px:PXLayoutRule runat="server"  ControlSize="M" LabelsWidth="SM" StartColumn="True"/>
			<px:PXSegmentMask ID="edBranchID" runat="server"  DataSourceID="ds" DataField="BranchID" CommitChanges="True" DisplayMode="Text" />
			<px:PXSegmentMask ID="edCustomerID" runat="server" DataField="CustomerID"  CommitChanges="True" AutoRefresh="True" DataSourceID="ds"> </px:PXSegmentMask>
			 
			<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="S" StartColumn="True"/>
			<px:PXSegmentMask ID="edProjectID" runat="server" DataField="ProjectID"  CommitChanges="True" AutoRefresh="True" DataSourceID="ds"> </px:PXSegmentMask>
			<px:PXSelector ID="edJobSiteNbr" runat="server" DataField="JobSiteNbr"  CommitChanges="True" AutoRefresh="True" DataSourceID="ds"></px:PXSelector>
			
            <px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="S" StartColumn="True"/>
			<px:PXSelector ID="edOrderType" runat="server" DataField="OrderType" AutoRefresh="True" DataSourceID="ds" CommitChanges="True"></px:PXSelector>
            <px:PXSelector ID="edfOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="True" DataSourceID="ds" CommitChanges="True"></px:PXSelector>	
			
			
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100" 
		AllowPaging="True" AllowSearch="True" AdjustPageSize="Auto" DataSourceID="ds" SkinID="PrimaryInquire" SyncPosition="true" TabIndex="100" 
        TemporaryFilterCaption="Filter Applied" FilesIndicator="False">
		<Levels>
			<px:PXGridLevel DataKeyNames="CycleNbr" DataMember="Records">
			    <RowTemplate></RowTemplate>
                <Columns>
                    <px:PXGridColumn AllowCheckAll="true" DataField="Selected" TextAlign="Center" Type="CheckBox" Width="60px" />
                    <px:PXGridColumn DataField="CycleNbr" />
					<px:PXGridColumn DataField="NVRTSalesOrderAvailableForCycleBilling__Customer" />
					<px:PXGridColumn DataField="NVRTSalesOrderAvailableForCycleBilling__CustomerName" />
					<px:PXGridColumn DataField="SOOrderType" />
					<px:PXGridColumn DataField="SOOrderNbr" LinkCommand="ViewSalesOrder" />
                    <px:PXGridColumn DataField="NVRTSalesOrderAvailableForCycleBilling__SOOrderDescr" />
					<px:PXGridColumn DataField="NextBillDate" Width="90px" />
					<px:PXGridColumn DataField="NextCycleStartDate" Width="90px" />
					<px:PXGridColumn DataField="NextCycleEndDate" Width="90px" />
					<px:PXGridColumn DataField="JobSiteNbr" />
					<px:PXGridColumn DataField="NVRTSalesOrderAvailableForCycleBilling__JobSiteDesc" />
					<px:PXGridColumn DataField="NVRTSalesOrderAvailableForCycleBilling__ProjectCD" />
					<px:PXGridColumn DataField="BranchID" />
                    <px:PXGridColumn DataField="CycleMode" />
					<px:PXGridColumn DataField="Descr" />
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXGrid>
</asp:Content>