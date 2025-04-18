using NV.Rental360.RentalCycle;
using PX.Data;
using PX.Objects.SO;

namespace Graphs
{
    public class USS_NVRTCycleBillProcessingExt : PXGraphExtension<NVRTCycleBillProcessing>
    {
        public static bool IsActive() => true;
        //Story: FOX-713 | Engineer: [Abhishek Sonone] | Date: [2025-03-20]
        #region Helper Methods
        protected virtual void ViewSalesOrder()
        {
            // Fetch the current record from the primary data view
            var row = Base.Filter.Current;
            if (row == null || string.IsNullOrEmpty(row.OrderNbr)) return;

            SOOrder order = PXSelect<SOOrder,
                            Where<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>,
                            And<SOOrder.orderType, Equal<Required<SOOrder.orderType>>>>>
                            .Select(Base, row.OrderNbr, row.OrderType);

            if (order != null)
            {
                SOOrderEntry graph = PXGraph.CreateInstance<SOOrderEntry>();
                graph.Document.Current = order;
                throw new PXRedirectRequiredException(graph, true, "Sales Orders");
            }
        }
        #endregion
    }
}
