/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
*/

using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.DataHandler.Approval;
using Mtd.OrderMaker.Server.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.DataHandler.Filter
{
    public partial class FilterHandler
    {
        public async Task<OutFlow> GetStackFlowAsync(Incomer incomer, TypeQuery typeQuery)
        {

            OutFlow outFlow = new OutFlow();

            if (incomer.WaitList == 1)
            {
                List<string> storesForUser = await ApprovalHandler.GetWaitStoreIds(_context, _user, incomer.IdForm);
                queryMtdStore = queryMtdStore.Where(x => storesForUser.Contains(x.Id));
                outFlow = new OutFlow
                {
                    Count = queryMtdStore.Count(),
                    MtdStores = await queryMtdStore.OrderByDescending(x => x.Sequence).Skip((incomer.Page - 1) * incomer.PageSize).Take(incomer.PageSize).ToListAsync()
                };

                return outFlow;
            }

            IList<MtdFilterScript> scripts = await GetScriptsAsync();
            if (scripts != null && scripts.Count > 0)
            {
                foreach (var fs in scripts)
                {
                    if (fs.Apply == 1)
                        queryMtdStore = queryMtdStore.FromSql(fs.Script);
                }
            }

            bool ownOnly = await _userHandler.GetFormPolicyAsync(_user, incomer.IdForm, RightsType.ViewOwn);
            if (ownOnly)
            {
                IList<string> storeIds = await _context.MtdStoreOwner.Where(x => x.UserId == _user.Id).Select(x => x.Id).ToListAsync();
                queryMtdStore = queryMtdStore.Where(x => storeIds.Contains(x.Id));
            }

            bool groupView = await _userHandler.GetFormPolicyAsync(_user, IdForm, RightsType.ViewGroup);
            if (groupView)
            {
                IList<WebAppUser> appUsers = await _userHandler.GetUsersInGroupsAsync(_user);
                List<string> userIds = appUsers.Select(x => x.Id).ToList();
                IList<string> storeIds = await _context.MtdStoreOwner.Where(x => userIds.Contains(x.UserId)).Select(x => x.Id).ToListAsync();
                queryMtdStore = queryMtdStore.Where(x => storeIds.Contains(x.Id));
            }

            switch (typeQuery)
            {
                case TypeQuery.number:
                    {
                        outFlow = await GetDataForNumberAsync(incomer);
                        break;
                    }
                case TypeQuery.text:
                    {
                        outFlow = await GetDataForTextAsync(incomer);
                        break;
                    }
                case TypeQuery.field:
                case TypeQuery.textField:
                    {
                        outFlow = await GetDataForFieldAsync(incomer);
                        break;
                    }
                default:
                    {
                        outFlow = await GetDataForEmptyAsync(incomer);
                        break;
                    }
            }

            return outFlow;
        }
    }
}
