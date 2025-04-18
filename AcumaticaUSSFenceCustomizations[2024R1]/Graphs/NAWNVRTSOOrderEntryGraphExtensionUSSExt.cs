using PX.Data;
using PX.Objects.SO;
using PX.Objects.IN;
using NV.Rental360;
using PX.Objects.CS;
using PX.Objects.SO.DAC.Projections;
using System;

namespace NAWUnitedSiteServices.Prototype.Extensions.GraphExtensions
{
    [Serializable]
    public class NAWNVRTSOOrderEntryGraphExtensionUSSExt : PXGraphExtension<NVRTSOOrderEntryChangeOrderExtension, NVRTSOOrderEntryGraphExtension, SOOrderEntry>
    {        
        //Story: FOX-623 | Engineer: [Satej Ambekar] | Date: [2025-03-10]         
        public static bool IsActive()
        {
            return true;
        }

        #region Events
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [NVRTCustomQuantityAttribute(typeof(SOLine.uOM), typeof(SOLine.baseOrderQty), InventoryUnitType.SalesUnit, typeof(NVRTSOLineDacExtension.usrNVRTLineType), true)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Quantity")]
        [PXUnboundFormula(typeof(Switch<Case<Where<SOLine.operation, Equal<SOLine.defaultOperation>, And<SOLine.lineType, NotEqual<SOLineType.miscCharge>>>, SOLine.orderQty>, decimal0>), typeof(SumCalc<SOOrder.orderQty>), ValidateAggregateCalculation = false)]
        [PXUnboundFormula(typeof(Switch<Case<Where<SOLine.lineType, NotEqual<SOLineType.miscCharge>>, SOLine.orderQty>, decimal0>), typeof(SumCalc<SOBlanketOrderLink.orderedQty>))]
        [PXUnboundFormula(typeof(Switch<Case<Where<SOLine.cancelled, Equal<False>>, SOLine.orderQty>, decimal0>), typeof(SumCalc<BlanketSOLine.qtyOnOrders>))]
        [PXUnboundFormula(typeof(Switch<Case<Where<SOLine.cancelled, Equal<False>>, SOLine.orderQty>, decimal0>), typeof(SumCalc<BlanketSOLineSplit.qtyOnOrders>))]
        protected virtual void _(Events.CacheAttached<SOLine.orderQty> e) { }
        #endregion
    }
}
