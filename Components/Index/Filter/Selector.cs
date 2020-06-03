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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;
using Mtd.OrderMaker.Server.Models.Index;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Index.Filter
{
    public enum ServiceFilter { DateCreated, DocumentOwner }

    [ViewComponent(Name = "IndexFilterSelector")]
    public class Selector : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;
        private List<MTDSelectListItem> ServiceItems => GetServiceItems();

        public Selector(OrderMakerContext orderMakerContext, UserHandler userHandler)
        {
            _context = orderMakerContext;
            _userHandler = userHandler;
        }

        public async Task<IViewComponentResult> InvokeAsync(string formId)
        {
            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);
            
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == formId);

            List<MTDSelectListItem> customItems = await GetCustomFieldsAsync(user, formId, filter);

            IList<MtdSysTerm> mtdSysTerms = await _context.MtdSysTerm.ToListAsync();
            List<MTDSelectListItem> terms = new List<MTDSelectListItem>();
            mtdSysTerms.ToList().ForEach((term =>
            {
                terms.Add(new MTDSelectListItem { Id = term.Id.ToString(), Value = $"{term.Name} ({term.Sign})", Localized = true });
            }));

            List<MTDSelectListItem> userList = await GetUserItems(user, formId);

            IList<MtdFilterScript> scripts = await _userHandler.GetFilterScripsAsync(user,formId,0);            
            List<MTDSelectListItem> scriptItems = new List<MTDSelectListItem>();
            
            foreach (var script in scripts)
            {
                scriptItems.Add(new MTDSelectListItem { Id = script.Id.ToString(), Value = script.Name });
            }
                                     

            SelectorModelView selector = new SelectorModelView()
            {
                FormId = formId,
                ScriptItems = scriptItems,
                UsersItems = userList,
                CustomItems = customItems,
                TermItems = terms,
                ServiceItems = this.ServiceItems
            };

            return View("Default", selector);
        }

        
        private async Task<List<MTDSelectListItem>> GetCustomFieldsAsync(WebAppUser user, string formId, MtdFilter mtdFilter)
        {
            List<MtdFormPart> parts = await _userHandler.GetAllowPartsForView(user, formId);
            List<string> partIds = parts.Select(x => x.Id).ToList();
            
            var query = _context.MtdFormPartField.Include(m => m.MtdFormPartNavigation)
                .Where(x => x.MtdFormPartNavigation.MtdForm == formId && x.Active == 1 && partIds.Contains(x.MtdFormPart))
                .OrderBy(x => x.MtdFormPartNavigation.Sequence).ThenBy(x => x.Sequence);

            IList<MtdFormPartField> mtdFields;
            if (mtdFilter != null)
            {
                List<string> fieldIds = await _context.MtdFilterField.Where(x => x.MtdFilter == mtdFilter.Id)
                    .Select(x => x.MtdFormPartField).ToListAsync();
                mtdFields = await query.Where(x => !fieldIds.Contains(x.Id)).ToListAsync();
            }
            else
            {
                mtdFields = await query.ToListAsync();
            }

            List<MTDSelectListItem> customItems = new List<MTDSelectListItem>();
            int[] exclude = { 7, 8, 13 };
            List<MtdFormPartField> customFields = mtdFields.Where(x => !exclude.Contains(x.MtdSysType)).ToList();

            foreach (var item in customFields)
            {
                customItems.Add(new MTDSelectListItem
                {
                    Id = item.Id,
                    Value = $"{item.MtdFormPartNavigation.Name}: {item.Name}",
                    Attributes = $" data-type={@item.MtdSysType} "
                });
            }

            return customItems;
        }

        private async Task<List<MTDSelectListItem>> GetUserItems(WebAppUser user, string formId)
        {
            List<MTDSelectListItem> userList = new List<MTDSelectListItem>();

            List<WebAppUser> appUsers = new List<WebAppUser>();

            bool isViewAll = await _userHandler.GetFormPolicyAsync(user, formId, RightsType.ViewAll);
            if (isViewAll)
            {
                appUsers = await _userHandler.Users.ToListAsync();
            }
            else
            {
                appUsers = await _userHandler.GetUsersInGroupsAsync(user);
            }

            appUsers = appUsers.OrderBy(x => x.Title).ToList();

            foreach (var appUser in appUsers)
            {
                userList.Add(new MTDSelectListItem { Id = appUser.Id, Value = appUser.Title });
            }

            return userList;

        }

        private List<MTDSelectListItem> GetServiceItems()
        {
            return new List<MTDSelectListItem>
                    {
                        new MTDSelectListItem {
                            Id=ServiceFilter.DateCreated.ToString(),
                            Value="Date Created",
                            Localized = true
                        },
                        new MTDSelectListItem {
                            Id=ServiceFilter.DocumentOwner.ToString(),
                            Value="Document owner",
                            Localized = true
                        }
                    };
        }
    }
}
