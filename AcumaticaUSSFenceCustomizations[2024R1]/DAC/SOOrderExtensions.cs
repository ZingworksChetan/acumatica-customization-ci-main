using PX.Data;
using PX.Data.BQL;

namespace PX.Objects.SO
{    
    public class SOOrderExt : PXCacheExtension<PX.Objects.SO.SOOrder>
    {
        public static bool IsActive() => true;
        // Story: FOX-535  |  Engineer: [Satej Ambekar] | Date: [2025-01-22]         
        #region UsrZWAcquisitionCode
        [PXDBString(80)]
        [PXUIField(DisplayName="Acquisition Code")]
        public virtual string UsrZWAcquisitionCode { get; set; }
        public abstract class usrZWAcquisitionCode : PX.Data.BQL.BqlString.Field<usrZWAcquisitionCode> { }
        #endregion

        //Story: FOX-737 | Engineer: [Divya Kurumkar] | Date: [2025-03-24]   
        #region RefOrderNbr
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Reference Order Nbr", Enabled = false)]
        public virtual string RefOrderNbr { get; set; }
        public abstract class refOrderNbr : BqlString.Field<refOrderNbr> { }
        #endregion

        #region RefOrderType
        [PXString(2, IsFixed = true)]
        [PXUIField(DisplayName = "Reference Order Type", Enabled = false)]
        public virtual string RefOrderType { get; set; }
        public abstract class refOrderType : BqlString.Field<refOrderType> { }
        #endregion
    }
}