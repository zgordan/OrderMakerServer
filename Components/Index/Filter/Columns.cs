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
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Models.Index;
using Mtd.OrderMaker.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Components.Index
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


        public async Task<IViewComponentResult> InvokeAsync(string idForm)
        {

            var user = await _userHandler.GetUserAsync(HttpContext.User);
            List<string> partIds = await _userHandler.GetAllowPartsForView(user, idForm);

            IList<MtdFilterColumn> mtdFilterColumns = await _context.MtdFilterColumn
                .Include(m => m.MtdFormPartFieldNavigation)
                .Include(x => x.MtdFilterNavigation)
                .Where(x => x.MtdFilterNavigation.IdUser == user.Id && x.MtdFilterNavigation.MtdForm == idForm)
                .OrderBy(x => x.Sequence)
                .ToListAsync();

            List<MtdFormPartField> mtdFormPartFields = new List<MtdFormPartField>();
            IList<MtdFormPartField> tempFields = await _context.MtdFormPartField
                .Include(x => x.MtdFormPartNavigation)
                .Where(x => x.MtdFormPartNavigation.MtdForm == idForm && partIds.Contains(x.MtdFormPart))
                .OrderBy(o => o.MtdFormPartNavigation.Sequence).ThenBy(o => o.Sequence)
                .ToListAsync();

            int sequence = 0;
            foreach (var column in mtdFilterColumns)
            {

                var field = tempFields.Where(x => x.Id == column.MtdFormPartField).FirstOrDefault();
                if (field != null)
                {
                    sequence++;
                    field.Sequence = sequence;
                    mtdFormPartFields.Add(field);
                }

            }

            foreach (var field in tempFields)
            {
                if (!mtdFilterColumns.Where(x => x.MtdFormPartField == field.Id).Any())
                {
                    sequence++;
                    field.Sequence = sequence;
                    mtdFormPartFields.Add(field);
                }
            }

            IList<MtdFormPart> mtdFormParts = mtdFormPartFields.GroupBy(x => x.MtdFormPartNavigation.Id)
                .Select(g => g.FirstOrDefault(x => x.MtdFormPartNavigation.Id == g.Key).MtdFormPartNavigation)
                .OrderBy(x => x.Sequence)
                .ToList();

            MtdFilter mtdFilter = await _context.MtdFilter.Where(x => x.MtdForm == idForm && x.IdUser == user.Id).FirstOrDefaultAsync();
            bool showNumber = true;
            bool showDate = true;
            if (mtdFilter != null)
            {
                showNumber = mtdFilter.ShowNumber == 1 ? true : false;
                showDate = mtdFilter.ShowDate == 1 ? true : false;
            }

            ColumnsModelView fieldsModelView = new ColumnsModelView
            {
                IdForm = idForm,
                MtdFormParts = mtdFormParts,
                MtdFilterColumns = mtdFilterColumns,
                MtdFormPartFields = mtdFormPartFields,
                ShowNumber = showNumber,
                ShowDate = showDate                
            };

            return View(fieldsModelView);
        }
    }
}
