using System;
using System.Collections.Generic;
using DAC;
using NV.Rental360;
using PX.Data;

namespace Graphs
{
    public class NVRTRentalTicketEntry_Extension : PXGraphExtension<NVRTRentalTicketEntry>
    {
        //Story: FOX-538  | Engineer: [Abhishek Sonone]| Date: [2025-02-25]   
        public static bool IsActive() => true;
        #region Events
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
        protected virtual void _(Events.RowSelected<NVRTRentalTicket> e)
        {
            if (e.Row == null) return;
            List<string> userRoles = UserRoleHelper.GetCurrentUserRolesList();
            bool isNonAdmin = !userRoles.Contains("Administrator");
            PXUIFieldAttribute.SetVisible<NVRTRentalTicket.branchID>(e.Cache, e.Row, !isNonAdmin);

        }
        #endregion

    }
}
