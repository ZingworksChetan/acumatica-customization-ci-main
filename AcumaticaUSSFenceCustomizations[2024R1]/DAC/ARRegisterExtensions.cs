using PX.Data;

namespace PX.Objects.AR
{
    [PXNonInstantiatedExtension]
    public class AR_ARInvoice_ExistingColumn : PXCacheExtension<PX.Objects.AR.ARInvoice>
    {
        public static bool IsActive() => true;
        //Story: FOX-530 | Engineer: [Divya Kurumkar] | Date: [2025-01-09]
        #region HiddenOrderType  
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXUIField(DisplayName = "Sales Order Type")]
        public string HiddenOrderType { get; set; }
        #endregion

        #region HiddenOrderNbr  
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXUIField(DisplayName = "Sales Order Nbr.")]
        public string HiddenOrderNbr { get; set; }
        #endregion
    }
}