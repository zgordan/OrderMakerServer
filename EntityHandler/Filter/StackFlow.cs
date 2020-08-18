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
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Approval;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.EntityHandler.Filter
{
    public partial class FilterHandler
    {
        public async Task<OutFlow> GetStackFlowAsync(Incomer incomer, TypeQuery typeQuery)
        {

            OutFlow outFlow = new OutFlow();

            if (incomer.WaitList == 1)
            {
                List<string> storesForUser = await ApprovalHandler.GetWaitStoreIds(_context, _user, incomer.FormId);
                queryMtdStore = queryMtdStore.Where(x => storesForUser.Contains(x.Id));
                outFlow = new OutFlow
                {
                    Count = queryMtdStore.Count(),
                    MtdStores = await queryMtdStore.OrderByDescending(x => x.Sequence).Skip((incomer.Page - 1) * incomer.PageSize).Take(incomer.PageSize).ToListAsync()
                };

                IList<string> allowFields = await GetAllowFieldsAsync(incomer);
                outFlow.AllowFieldIds = allowFields;

                return outFlow;
            }

            IList<MtdFilterScript> scripts = await _userHandler.GetFilterScriptsAsync(_user, incomer.FormId, 1);
            if (scripts != null && scripts.Count > 0 && incomer.WaitList != 1)
            {
                foreach (var fs in scripts)
                {
                    List<string> scriptIds = await _context.MtdStore.FromSqlRaw(fs.Script).Select(x => x.Id).ToListAsync();
                    queryMtdStore = queryMtdStore.Where(x => scriptIds.Contains(x.Id));
                }
            }

            bool ownOnly = await _userHandler.CheckUserPolicyAsync(_user, incomer.FormId, RightsType.ViewOwn);
            if (ownOnly)
            {
                IList<string> storeIds = await _context.MtdStoreOwner.Where(x => x.UserId == _user.Id).Select(x => x.Id).ToListAsync();
                queryMtdStore = queryMtdStore.Where(x => storeIds.Contains(x.Id));
            }

            bool groupView = await _userHandler.CheckUserPolicyAsync(_user, FormId, RightsType.ViewGroup);
            if (groupView)
            {
                IList<WebAppUser> appUsers = await _userHandler.GetUsersInGroupsOutDenyAsync(_user, FormId);
                List<string> userIds = appUsers.Select(x => x.Id).ToList();

                IList<string> storeIds = await _context.MtdStoreOwner.Where(x => userIds.Contains(x.UserId)).Select(x => x.Id).ToListAsync();
                queryMtdStore = queryMtdStore.Where(x => storeIds.Contains(x.Id));
            }

            switch (typeQuery)
            {
                case TypeQuery.number:
                    {
                        /*ID search*/
                        outFlow = await GetDataForNumberAsync(incomer);
                        break;
                    }
                case TypeQuery.text:
                    {
                        /*Search line*/
                        outFlow = await GetDataForTextAsync(incomer);
                        break;
                    }
                case TypeQuery.field:
                case TypeQuery.textField:
                case TypeQuery.script:
                    {
                        /*Basic search - selection from the list*/
                        outFlow = await GetDataForFieldAsync(incomer);
                        break;
                    }
                default:
                    {
                        /*Without filters*/
                        outFlow = await GetDataForEmptyAsync(incomer);
                        break;
                    }
            }

            int pageCount = 0;
            decimal count = (decimal)outFlow.Count / incomer.PageSize;
            pageCount = Convert.ToInt32(Math.Ceiling(count));
            pageCount = pageCount == 0 ? 1 : pageCount;
            outFlow.PageCount = pageCount;



            IList<string> fieldIds = await GetAllowFieldsAsync(incomer);
            outFlow.AllowFieldIds = fieldIds;

            return outFlow;
        }

        private async Task<IList<string>> GetAllowFieldsAsync(Incomer incomer)
        {
            IList<MtdFormPart> parts = await _userHandler.GetAllowPartsForView(_user, incomer.FormId);
            List<string> partIds = parts.Select(x => x.Id).ToList();

            IList<string> fieldIds = incomer.FieldForColumn.Select(x => x.Id).ToList();
            IList<string> allowFiieldIds = await _context.MtdFormPartField.Where(x => partIds.Contains(x.MtdFormPart)).Select(x => x.Id).ToListAsync();
            fieldIds = allowFiieldIds.Where(x => fieldIds.Contains(x)).ToList();

            return  fieldIds;
        }
    }
}
