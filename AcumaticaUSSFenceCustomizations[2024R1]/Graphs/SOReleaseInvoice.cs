using PX.Data;
using PX.Objects.AR;
namespace PX.Objects.SO
{
    public class SOReleaseInvoice_Extension : PXGraphExtension<PX.Objects.SO.SOReleaseInvoice>
    {
        public static bool IsActive() => true;
        #region Event Handlers
        //Story: FOX-530 | Engineer: [Divya Kurumkar] | Date: [2025-01-09]
        protected void _(Events.RowSelected<ARInvoice> e)
        {
            var invoiceDetails = e.Row;
            if (invoiceDetails == null) return;
            /// Fetch the ARTran record based on the RefNbr
            var arTran = PXSelect<ARTran, Where<ARTran.refNbr, Equal<Required<ARTran.refNbr>>>>.Select(e.Cache.Graph, ((ARInvoice)invoiceDetails).RefNbr).TopFirst;
            if (arTran != null)
            {
                /// Auto-populate HiddenOrderNbr and HiddenOrderType on the ARInvoice row
                e.Cache.SetValueExt<ARInvoice.hiddenOrderNbr>(invoiceDetails, arTran.SOOrderNbr);
                e.Cache.SetValueExt<ARInvoice.hiddenOrderType>(invoiceDetails, arTran.SOOrderType);
            }
        }
        #endregion
    }
}