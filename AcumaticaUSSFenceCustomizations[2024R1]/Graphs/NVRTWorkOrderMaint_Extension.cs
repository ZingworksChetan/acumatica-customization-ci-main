using System;
using NV.Rental360.WorkOrders;
using PX.Common;
using PX.Data;
using PX.Objects.SO;

namespace NV.Rental360
{
    public class NVRTWorkOrderMaint_Extension : PXGraphExtension<NVRTWorkOrderMaint>
    {
        public static bool IsActive() => true;
        //Story: FOX-612 | Engineer: [Satej Ambekar] | Date: [2025-03-07]
        #region Events
        protected virtual void _(Events.FieldDefaulting<NVRTWorkOrder, NVRTWorkOrder.scheduleStartDate> e)
        {
            e.NewValue = SetTimeTo4AM(e.NewValue);
        }
        protected virtual void _(Events.FieldDefaulting<NVRTWorkOrder, NVRTWorkOrder.scheduleEndDate> e)
        {
            e.NewValue = SetTimeTo4AM(e.NewValue);
        }
        protected virtual void _(Events.FieldDefaulting<NVRTWorkOrder, NVRTWorkOrder.actualStartDate> e)
        {
            e.NewValue = SetTimeTo4AM(e.NewValue);
        }
        protected virtual void _(Events.FieldDefaulting<NVRTWorkOrder, NVRTWorkOrder.actualEndDate> e)
        {
            e.NewValue = SetTimeTo4AM(e.NewValue);
        }
        protected virtual void _(Events.FieldDefaulting<NVRTWorkOrder, NVRTWorkOrder.arrivalTimeEst> e)
        {
            e.NewValue = SetTimeTo4AM(e.NewValue);
        }
        protected virtual void _(Events.FieldDefaulting<NVRTWorkOrder, NVRTWorkOrder.arrivalTimeAct> e)
        {
            e.NewValue = SetTimeTo4AM(e.NewValue);
        }
        protected virtual void _(Events.RowPersisting<NVRTWorkOrder> e)
        {
            var row = e.Row;
            if (row != null)
            {
                e.Cache.SetValueExt<NVRTWorkOrder.scheduleStartDate>(row, AdjustTime(row.ScheduleStartDate));
                e.Cache.SetValueExt<NVRTWorkOrder.scheduleEndDate>(row, AdjustTime(row.ScheduleEndDate));
                e.Cache.SetValueExt<NVRTWorkOrder.actualStartDate>(row, AdjustTime(row.ActualStartDate));
                e.Cache.SetValueExt<NVRTWorkOrder.actualEndDate>(row, AdjustTime(row.ActualEndDate));
                e.Cache.SetValueExt<NVRTWorkOrder.arrivalTimeEst>(row, AdjustTime(row.ArrivalTimeEst));
                e.Cache.SetValueExt<NVRTWorkOrder.arrivalTimeAct>(row, AdjustTime(row.ArrivalTimeAct));
            }
        }
        protected virtual void _(Events.RowSelected<NVRTWorkOrder> e)
        {
            var row = e.Row;
            if (row != null)
            {
                row.ScheduleStartDate = AdjustTime(row.ScheduleStartDate);
                row.ScheduleEndDate = AdjustTime(row.ScheduleEndDate);
                row.ActualStartDate = AdjustTime(row.ActualStartDate);
                row.ActualEndDate = AdjustTime(row.ActualEndDate);
                row.ArrivalTimeEst = AdjustTime(row.ScheduleStartDate);
                row.ArrivalTimeAct = AdjustTime(row.ScheduleStartDate);
            }
        }
        #endregion

        #region Helper Methods
        private DateTime? AdjustTime(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                DateTime dt = dateTime.Value;
                DateTime fourAM = dt.Date.AddHours(4);

                // If time is before 4 AM, set it to 4 AM; otherwise, keep the original time
                return dt < fourAM ? fourAM : dt;
            }
            return null;
        }
        private DateTime? SetTimeTo4AM(object date)
        {
            DateTime? inputDate = date as DateTime?;
            return inputDate?.Date.AddHours(4) ?? PXTimeZoneInfo.Now.Date.AddHours(4);
        }
        #endregion
    }
}
