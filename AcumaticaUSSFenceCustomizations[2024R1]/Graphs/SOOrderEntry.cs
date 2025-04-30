using PX.Data;
using PX.Objects.SO;
using System.Collections.Generic;
using DAC;
using static NV.Rental360.NVRTSOOrderDacExtension;
using static NAWUnitedSiteServices.Prototype.Extensions.DACExtensions.NAWSOOrderUSSExt;
using static NAWUnitedSiteServices.Extensions.DAC.NAWSOOrderUSSExt;
using NV.Rental360;
using System;
using PX.Common;
using PX.Concurrency;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.SM;
using System.Collections;
using System.Threading;
using System.Linq;
using NAWUnitedSiteServices.Prototype.Extensions.DACExtensions;

public class SOOrderEntry_Extension : PXGraphExtension<SOOrderEntry>
{
    public static bool IsActive() => true;
    public override void Initialize()
    {
        base.Initialize();
        Base.Actions["Delete"].SetVisible(false);
    }

    #region View
    public PXSelectJoin<SOLine,
       InnerJoin<SOOrder,
           On<SOLine.orderType, Equal<SOOrder.orderType>,
           And<SOLine.orderNbr, Equal<SOOrder.orderNbr>>>>,
       Where<SOLine.origOrderNbr, Equal<SOOrder.orderNbr.FromCurrent>,
           And<SOLine.origOrderType, Equal<SOOrder.orderType.FromCurrent>>>> RelatedROLines;
    #endregion

    #region Events
    protected void _(Events.RowSelected<SOOrder> e)
    {
        PXCache cache = e.Cache;
        var currentSalesOrder = (SOOrder)e.Row;
        if (currentSalesOrder == null) return;
        //Story: FOX-577 | Engineer: [Divya Kurumkar] | Date: [2025-02-04]         
        bool isROorRQ = currentSalesOrder.OrderType == "RO" || currentSalesOrder.OrderType == "RQ";
        List<string> userRolesList = UserRoleHelper.GetCurrentUserRolesList();
        bool isSystemAdmin = userRolesList.Contains("Administrator");
        Base.Delete.SetVisible(isSystemAdmin || !isROorRQ);
        //Story: FOX-446 | Engineer: [Divya Kurumkar] | Date: [2025-02-25]
        //Story: FOX-655 | Engineer: [Divya Kurumkar] | Date: [2025-03-19] 
        int? isInvoiceType = cache.GetValue<usrNVInvoiceType>(currentSalesOrder) as int?;
        //bool isEnableCreateworkOrder = (isInvoiceType == 1);
        int? rentalStatus = cache.GetValue<usrNVRTRentalStatus>(currentSalesOrder) as int?;
        int? invoiceStage = cache.GetValue<usrNAWInvoiceStage>(currentSalesOrder) as int?;
        bool isInitialTermPending = invoiceStage == 0 || invoiceStage == 10;
        bool disableInvoiceType = ((currentSalesOrder.OrderType == "RO" || currentSalesOrder.OrderType == "EX") && !isInitialTermPending) && rentalStatus != 0;
        PXUIFieldAttribute.SetEnabled<usrNVInvoiceType>(cache, currentSalesOrder, !disableInvoiceType);
        //Story: FOX-599  | Engineer: [Abhishek Sonone] | Date: [2025-02-28]
        bool isCanceled = currentSalesOrder.Status == "L";
         bool isEnableCreateworkOrder = (isInvoiceType == 1 || invoiceStage == 20);       
        var nvrtExtension = Base.GetExtension<NVRTSOOrderEntryGraphExtension>();
        if (nvrtExtension != null)
        {
            nvrtExtension.NVRTxCreateWorkOrder.SetEnabled(!isCanceled);
            //nvrtExtension.NVRTxReturnProcessing.SetEnabled(isEnableCreateworkOrder);
        }        

    }
    //Story: FOX-181 | Engineer: [Abhishek Sonone] | Date: [2025-02-04]
    protected void _(Events.RowSelected<SOLine> e)
    {
        SOLine sOLineDetails = (SOLine)e.Row;
        if (sOLineDetails == null) return;
        List<string> userRolesList = UserRoleHelper.GetCurrentUserRolesList();
        bool isSystemAdmin = userRolesList.Contains("Administrator");
        PXUIFieldAttribute.SetEnabled<SOLine.tranDesc>(e.Cache, sOLineDetails, isSystemAdmin);
        //Story: FOX-595 | Engineer: [Satej Ambekar] | Date: [2025-04-02]  
        bool hasCycleBillingAccess = userRolesList.Contains("Administrator") ||
                 userRolesList.Contains("AR Admin") ||
                 userRolesList.Contains("CR Sales Representative");
        PXUIFieldAttribute.SetEnabled<SOLine.curyUnitPrice>(e.Cache, sOLineDetails, hasCycleBillingAccess);
        //Story: FOX-737 | Engineer: [Divya Kurumkar] | Date: [2025-03-24]          
        SOLine relatedROLine = RelatedROLines.SelectWindowed(0, 1);
        if (relatedROLine != null)
        {
            e.Cache.SetValueExt<SOLine.origOrderNbr>(e.Row, relatedROLine.OrderNbr);
            e.Cache.SetValueExt<SOLine.origOrderType>(e.Row, relatedROLine.OrderType);
        }
        PXUIFieldAttribute.SetEnabled<SOLine.origOrderNbr>(e.Cache, e.Row, true);
        PXUIFieldAttribute.SetEnabled<SOLine.origOrderType>(e.Cache, e.Row, true);
    }          
    // Overriding Acumatica default MinValue of -100 with a very high number to allow entry of higher pricing
    //Story: FOX-740 | Engineer: [Satej Ambekar] | Date: [2025-03-20] 
    [PXMergeAttributes(Method = MergeMethod.Replace)]
    [PXDBDecimal(MinValue = -1000.00, MaxValue = 100.00)] // Set your desired min/max values    
    [PXDefault(TypeCode.Decimal, "0.0")]
    [PXUIField(DisplayName = "Discount Percent")]
    protected virtual void _(Events.CacheAttached<SOLine.discPct> e) { }

    //Story: FOX-638 | Engineer: [Divya Kurumkar] | Date: [2025-03-31] 
    protected virtual void _(Events.RowPersisting<SOLine> e)
    {
        if (e.Row == null) return;
        var sOLineDetails = (SOLine)e.Row;
        if (sOLineDetails.OrderQty == 0)
        {
            // Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [Justification]
            throw new PXRowPersistingException(nameof(SOLine.orderQty), sOLineDetails.OrderQty, "Quantity must be greater than 0");

        }
    }

    #endregion

    #region Helper Methods
    //Story: FOX-733  | Engineer: [Satej Ambekar] | Date: [2025-03-24] 
    [PXButton(CommitChanges = true)]
    [PXUIField(DisplayName = "Print Quote", MapEnableRights = PXCacheRights.Select)]
    public virtual IEnumerable PrintQuote(PXAdapter adapter, string reportID = null)
    {
        return Report(adapter.Apply(delegate (PXAdapter it)
        {
            it.Menu = "Print Quote";
        }), reportID ?? "SO641000");
    }

    [PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
    [PXButton(SpecialType = PXSpecialButtonType.ReportsFolder, MenuAutoOpen = true)]
    public virtual IEnumerable Report(PXAdapter adapter, [PXString(8, InputMask = "CC.CC.CC.CC")] string reportID)
    {
        List<SOOrder> list = adapter.Get<SOOrder>().ToList();
        if (!string.IsNullOrEmpty(reportID))
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string text = null;
            PXReportRequiredException ex = null;
            Dictionary<PrintSettings, PXReportRequiredException> reportsToPrint = new Dictionary<PrintSettings, PXReportRequiredException>();
            foreach (SOOrder item in list)
            {
                dictionary = new Dictionary<string, string>();
                dictionary["SOOrder.OrderType"] = item.OrderType;
                dictionary["SOOrder.OrderNbr"] = item.OrderNbr;
                string quoteType = item.GetExtension<NAWSOOrderUSSExt>()?.UsrNAWDefaultFenceQuoteType;
                if (!string.IsNullOrEmpty(quoteType))
                {
                    string format = quoteType == "S" ? "S" : "D";
                    dictionary["Format"] = format;
                }
                text = new NotificationUtility(Base).SearchCustomerReport(reportID, item.CustomerID, item.BranchID);
                ex = PXReportRequiredException.CombineReport(ex, text, dictionary, OrganizationLocalizationHelper.GetCurrentLocalization(Base));
                ex.Mode = PXBaseRedirectException.WindowMode.New;
                reportsToPrint = SMPrintJobMaint.AssignPrintJobToPrinter(reportsToPrint, dictionary, adapter, new NotificationUtility(Base).SearchPrinter, "Customer", reportID, text, item.BranchID, OrganizationLocalizationHelper.GetCurrentLocalization(Base));
            }
            if (ex != null)
            {
                Base.LongOperationManager.StartAsyncOperation(async delegate (CancellationToken ct)
                {
                    await SMPrintJobMaint.CreatePrintJobGroups(reportsToPrint, ct);
                    throw ex;
                });
            }
        }
        return list;
    }
    #endregion
}
