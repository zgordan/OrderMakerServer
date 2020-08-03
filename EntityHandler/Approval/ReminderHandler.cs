using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.EntityHandler.Approval
{
    public partial class ApprovalHandler
    {
        public static async Task<Dictionary<string, List<MtdStore>>> GetHoveringApprovalAsync(OrderMakerContext context, UserHandler userHandler)
        {

            Dictionary<string, List<MtdStore>> result = new Dictionary<string, List<MtdStore>>();

            List<string> storeIds = await context.MtdStoreApproval
                .Where(x => x.Complete == 0 &&  x.LastEventTime.AddHours(10) < DateTime.Now).Select(x => x.Id).ToListAsync();

            if (storeIds == null) { return result; }


            foreach (string storeId in storeIds)
            {
                ApprovalHandler approvalHandler = new ApprovalHandler(context, storeId);
                MtdStore mtdStore = await approvalHandler.GetStoreAsync();

                string userId = await approvalHandler.GetCurrentUserIdAsync();
                if (userId == "owner") { userId = mtdStore.MtdStoreOwner.UserId; }

                WebAppUser user = await userHandler.FindByIdAsync(userId);
                bool emailConfirmed = await userHandler.IsEmailConfirmedAsync(user);
                if (!emailConfirmed) { continue; }

                bool userAdded = result.Keys.Where(x => x == userId).Any();

                if (userAdded)
                {
                    result[userId].Add(mtdStore);
                }
                else
                {
                    List<MtdStore> mtdStores = new List<MtdStore> { mtdStore };
                    result.Add(userId, mtdStores);
                }

            }

            return result;
        }

    }
}
