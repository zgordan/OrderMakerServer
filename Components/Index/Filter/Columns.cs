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
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Index;
using Mtd.OrderMaker.Server.Services;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Index
{
    [ViewComponent(Name = "IndexFilterColumns")]
    public class Columns : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public Columns(OrderMakerContext orderMakerContext, UserHandler userHandler)
        {
            _context = orderMakerContext;
            _userHandler = userHandler;
        }

        public async Task<IViewComponentResult> InvokeAsync(string formId)
        {
            var user = await _userHandler.GetUserAsync(HttpContext.User);
            List<MtdFormPart> parts = await _userHandler.GetAllowPartsForView(user, formId);
            List<string> partIds = parts.Select(x => x.Id).ToList();
            MtdFilter mtdFilter = await _context.MtdFilter.Where(x => x.MtdForm == formId && x.IdUser == user.Id).FirstOrDefaultAsync();
            bool showNumber = true;
            bool showDate = true;
            if (mtdFilter != null)
            {
                showNumber = mtdFilter.ShowNumber == 1;
                showDate = mtdFilter.ShowDate == 1;
            }
            
            IList<MtdFilterColumn> columns = await _context.MtdFilterColumn
                .Where(x => x.MtdFilter == mtdFilter.Id)
                .OrderBy(x => x.Sequence)
                .ToListAsync() ?? new List<MtdFilterColumn>();

            
            IList<MtdFormPartField> fields = await _context.MtdFormPartField
                .Where(x => partIds.Contains(x.MtdFormPart))
                .OrderBy(o => o.Sequence)
                .ToListAsync() ?? new List<MtdFormPartField>();


            List<ColumnItem> columnItems = new List<ColumnItem>();
            int i = columns.Count();
            foreach (var p in parts)
            {                
                fields.Where(x => x.MtdFormPart == p.Id).ToList().ForEach((fs) =>
                {
                    i++;
                    MtdFilterColumn column = columns.Where(x => x.MtdFormPartField == fs.Id).FirstOrDefault();
                    columnItems.Add(new ColumnItem
                    {
                        PartId = p.Id,
                        PartName = p.Name,
                        FieldId = fs.Id,
                        FieldName = fs.Name,
                        IsChecked = column != null,
                        Sequence = column != null ? column.Sequence : i,                        
                    });
                });
            }

            ColumnsModelView fieldsModelView = new ColumnsModelView
            {
                FormId = formId,
                ColumnItems = columnItems.OrderBy(x=>x.Sequence).ToList(),
                ShowNumber = showNumber,
                ShowDate = showDate
            };

            return View(fieldsModelView);
        }
    }
}
