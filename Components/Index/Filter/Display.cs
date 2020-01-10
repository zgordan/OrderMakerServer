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
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.Models.Index;
using Mtd.OrderMaker.Server.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Index.Filter
{
    [ViewComponent(Name = "IndexFilterDisplay")]
    public class Display : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public Display(OrderMakerContext orderMakerContext, UserHandler userHandler)
        {
            _context = orderMakerContext;
            _userHandler = userHandler;
        }

        public async Task<IViewComponentResult> InvokeAsync(string idForm)
        {
            List<DisplayData> displayDatas = new List<DisplayData>();
            var user = await _userHandler.GetUserAsync(HttpContext.User);

            List<string> partIds = await _userHandler.GetAllowPartsForView(user, idForm);

            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == idForm);
            if (filter != null)
            {

                if (filter.WaitList == 1)
                {
                    DisplayModelView display= new DisplayModelView
                    {
                        IdForm = idForm,
                        IdFilter = filter == null ? -1 : filter.Id,
                        DisplayDatas = displayDatas
                    };

                    return View("Default", display);

                }

                List<MtdFilterField> mtdFilterFields = await _context.MtdFilterField
                    .Include(x => x.MtdTermNavigation)
                    .Include(m => m.MtdFormPartFieldNavigation)
                    .Where(x => x.MtdFilter == filter.Id)
                    .ToListAsync();
                foreach (var field in mtdFilterFields)
                {

                    DisplayData displayData = new DisplayData
                    {
                        Id = field.Id,
                        Header = $"{field.MtdFormPartFieldNavigation.Name} ({field.MtdTermNavigation.Sign})",
                        Value = "",
                        Type = "-field"
                    };

                    if (field.MtdFormPartFieldNavigation.MtdSysType != 11)
                    {
                        displayData.Value = field.Value;
                    }
                    else
                    {
                        MtdStore mtdStore = await _context.MtdStore.FirstOrDefaultAsync(x => x.Id == field.Value);
                        if (mtdStore != null)
                        {
                            var fieldForList = await _context.MtdFormPartField.Include(m => m.MtdFormPartNavigation)
                                .Where(x => x.MtdFormPartNavigation.MtdForm == mtdStore.MtdForm & x.MtdSysType == 1)
                                .OrderBy(o => o.MtdFormPartNavigation.Sequence).ThenBy(o => o.Sequence).FirstOrDefaultAsync();
                            if (fieldForList != null)
                            {
                                IList<long> ids = await _context.MtdStoreStack.Where(x => x.MtdStore == mtdStore.Id & x.MtdFormPartField == fieldForList.Id).Select(x => x.Id).ToListAsync();
                                MtdStoreStackText data = await _context.MtdStoreStackText.FirstOrDefaultAsync(x => ids.Contains(x.Id));
                                displayData.Value = data.Register;
                            }

                        }

                    }

                    displayDatas.Add(displayData);
                }

                MtdFilterDate mtdFilterDate = await _context.MtdFilterDate.FindAsync(filter.Id);
                if (mtdFilterDate != null)
                {
                    DisplayData displayDate = new DisplayData()
                    {
                        Id = filter.Id,
                        Header = "Period",
                        Value = $"{mtdFilterDate.DateStart.ToShortDateString()} {mtdFilterDate.DateEnd.ToShortDateString()}",
                        Type = "-date"
                    };
                    displayDatas.Add(displayDate);
                }

                IList<MtdFilterScript> scripts = await _context.MtdFilterScript.Where(x => x.MtdFilter == filter.Id & x.Apply == 1).ToListAsync();
                if (scripts != null && scripts.Count > 0)
                {
                    foreach (var fs in scripts)
                    {
                        DisplayData displayDate = new DisplayData()
                        {
                            Id = fs.Id,
                            Header = "Custom filter",
                            Value = fs.Name,
                            Type = "-script"
                        };
                        displayDatas.Add(displayDate);
                    }

                }
            }

            DisplayModelView displayModelView = new DisplayModelView
            {
                IdForm = idForm,
                IdFilter = filter == null ? -1 : filter.Id,
                DisplayDatas = displayDatas
            };

            return View("Default", displayModelView);
        }
    }
}
