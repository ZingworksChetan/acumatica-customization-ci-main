using System;
using System.Linq;
using NV.Rental360.ChangeOrders;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.SO;

namespace NV.Rental360
{
    public class NVRTCustomQuantityAttribute : NVRTQuantityAttribute
    {
        //Story: FOX-623  | Engineer: [Satej Ambekar] | Date: [2025-03-10]        
        public NVRTCustomQuantityAttribute(Type keyField, Type resultField, InventoryUnitType decimalVerifyUnits, Type rentalLineType, bool isSOLine)
            : base(keyField, resultField, decimalVerifyUnits, rentalLineType, isSOLine) { }

        #region Helper Method
        protected override void CalcBaseQty(PXCache sender, QtyConversionArgs e)
        {
            if (!(_ResultField != null))
            {
                return;
            }
            decimal? num = CalcResultValue(sender, e);
            decimal? remainder = num % 1;

            if (remainder == 0 && remainder.HasValue)
            {
                if (e.ExternalCall)
                {
                    sender.SetValueExt(e.Row, _ResultField.Name, num);
                }
                else
                {
                    sender.SetValue(e.Row, _ResultField.Name, num);
                }
            }
            else
            {
                if (!(_RentalLineType != null))
                {
                    return;
                }

                int? rentalLineTypeValue = (int?)sender.GetValue(e.Row, _RentalLineType.Name);
                if (rentalLineTypeValue.HasValue && rentalLineTypeValue.Value != 2)
                {
                    ConversionInfo conversionInfo = sender
                        .GetAttributesOfType<INUnitAttribute>(e.Row, _KeyField.Name)
                        .FirstOrDefault()
                        ?.ReadConversionInfo(sender, e.Row, GetFromUnit(sender, e.Row));

                    decimal enteredQty = Convert.ToDecimal(e.NewValue);
                    if (enteredQty > 0)
                    {                        
                        decimal conversionFactor = Convert.ToDecimal(conversionInfo?.Conversion.UnitRate ?? 1m);
                        decimal correctedQty;
                        if (enteredQty < conversionFactor)
                        {                            
                            correctedQty = conversionFactor;
                        }
                        else
                        {                            
                            correctedQty = Math.Ceiling(enteredQty / conversionFactor) * conversionFactor;
                        }
                        correctedQty = Math.Round(correctedQty, 1);

                        if (correctedQty != enteredQty)
                        {
                            // Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [Justification]
                            PXSetPropertyException ex = (!_isSOLine)
                                ? new PXSetPropertyException(
                                    (NVRTChangeOrderLine)e.Row,
                                    "Incorrect Order Qty, the correct qty should be {0}",
                                    PXErrorLevel.Error,
                                    correctedQty)
                                : new PXSetPropertyException(
                                    (SOLine)e.Row,
                                    "Incorrect Order Qty, the correct qty should be {0}",
                                    PXErrorLevel.Error,
                                    correctedQty);
                            throw ex;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
