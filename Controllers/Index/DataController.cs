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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Areas.Identity.Pages.Users.Accounts;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Filter;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Controllers.Index
{
    [Route("api/index")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class DataController : ControllerBase
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public DataController(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        [HttpPost("search/text")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostSearchTextAsync()
        {
            string form = Request.Form["indexForm"];
            string value = Request.Form["search-text"];
            WebAppUser user = await _userHandler.GetUserAsync(User);
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == form);
            bool old = true;
            if (filter == null)
            {
                old = false;
                filter = new MtdFilter { IdUser = user.Id, MtdForm = form };
            }

            filter.SearchNumber = "";
            filter.SearchText = value;
            filter.Page = 1;

            if (old)
            {
                _context.MtdFilter.Update(filter);
            }
            else
            {
                await _context.MtdFilter.AddAsync(filter);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex.InnerException; }


            return Ok();
        }

        [HttpPost("search/number")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostSerarchIndexAsync()
        {

            string form = Request.Form["formId"];
            string value = Request.Form["searchNumber"];

            WebAppUser user = await _userHandler.GetUserAsync(User);
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id & x.MtdForm == form);
            bool old = true;
            if (filter == null)
            {
                old = false;
                filter = new MtdFilter { IdUser = user.Id, MtdForm = form };
            }

            filter.SearchNumber = value;
            filter.Page = 1;
            filter.SearchText = "";

            if (old)
            {
                _context.MtdFilter.Update(filter);
            }
            else
            {
                await _context.MtdFilter.AddAsync(filter);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex.InnerException; }
            return Ok();
        }

        [HttpPost("{formId}/pagesize/{number}")]
        public async Task<IActionResult> PostPageSize(string formId, int number)
        {
            int temp = number;
            if (temp > 50) temp = 50;
            var user = await _userHandler.GetUserAsync(User);
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == formId);
            if (filter == null)
            {
                filter = new MtdFilter { SearchNumber = "", SearchText = "" };
                await _context.MtdFilter.AddAsync(filter);
                await _context.SaveChangesAsync();
            }

            filter.PageSize = number;
            _context.MtdFilter.Update(filter);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("pagemove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostPageMove()
        {
            /* number
             * 1 -  First Page; 2 - back; 3 - forward; 4 - Last Page 
             */

            string formId = Request.Form["formId"];
            string formValue = Request.Form["formValue"];
            string pageCount = Request.Form["pageCount"];

            int number = int.Parse(formValue);

            var user = await _userHandler.GetUserAsync(User);
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == formId);
            if (filter == null)
            {
                filter = new MtdFilter { SearchNumber = "", SearchText = "" };
                await _context.MtdFilter.AddAsync(filter);
                await _context.SaveChangesAsync();
            }

            int page = filter.Page;
            bool isOk = int.TryParse(pageCount, out int pageLast);
            if (!isOk) { pageLast = page; }

            switch (number)
            {
                case 2: { if (page > 1) { page--; } break; }
                case 3: { page++; break; }
                case 4: { page = pageLast; break; }
                default: { page = 1; break; }
            };

            filter.Page = page < 0 ? page = 1 : page;

            _context.MtdFilter.Update(filter);
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpPost("filter/remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostFilterRemoveAsync()
        {
            var form = await Request.ReadFormAsync();
            string strID = form["idField"];

            if (strID.Contains("-field"))
            {
                strID = strID.Replace("-field", "");
                bool ok = int.TryParse(strID, out int idField);
                if (!ok) return Ok();
                MtdFilterField mtdFilterField = new MtdFilterField { Id = idField };
                try
                {
                    _context.MtdFilterField.Remove(mtdFilterField);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) { throw ex.InnerException; }
            }

            if (strID.Contains("-date"))
            {
                strID = strID.Replace("-date", "");
                bool ok = int.TryParse(strID, out int idFilter);
                if (!ok) return Ok();
                MtdFilterDate filterDate = new MtdFilterDate { Id = idFilter };
                try
                {
                    _context.MtdFilterDate.Remove(filterDate);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) { throw ex.InnerException; }
            }

            if (strID.Contains("-owner"))
            {
                strID = strID.Replace("-owner", "");
                bool ok = int.TryParse(strID, out int filterId);
                if (!ok) return Ok();
                MtdFilterOwner filterOwner = new MtdFilterOwner { Id = filterId };
                try
                {
                    _context.MtdFilterOwner.Remove(filterOwner);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) { throw ex.InnerException; }
            }

            if (strID.Contains("-script"))
            {

                strID = strID.Replace("-script", "");
                bool ok = int.TryParse(strID, out int idFilter);
                if (!ok) { return BadRequest(new JsonResult("Error: Bad request.")); }
                WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);
                MtdFilterScript mtdFilterScript = await _context.MtdFilterScript.FindAsync(idFilter);
                FilterHandler filterHandler = new FilterHandler(_context, mtdFilterScript.MtdFormId, user, _userHandler);
                ok = await filterHandler.RemoveFilterScriptAppliedAsync();
                if (!ok) { return BadRequest(new JsonResult("Error: Bad request.")); }
            }

            if (strID.Contains("-related"))
            {
                strID = strID.Replace("-related", "");
                bool ok = int.TryParse(strID, out int filterId);
                if (!ok) return Ok();
                MtdFilterRelated filterRelated = new MtdFilterRelated { Id = filterId };
                try
                {
                    _context.MtdFilterRelated.Remove(filterRelated);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) { throw ex.InnerException; }
            }

            return Ok();

        }

        [HttpPost("filter/removeAll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostFilterRemoveAllAsync()
        {
            var form = await Request.ReadFormAsync();
            string strID = form["filter-id"];
            string formId = form["form-id"];
            bool isOk = int.TryParse(strID, out int filterId);
            if (!isOk) { return NotFound(); }

            IList<MtdFilterField> mtdFilterFields = await _context.MtdFilterField.Where(x => x.MtdFilter == filterId).ToListAsync();
            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);
            IList<MtdFilterScript> filterScripts = await _userHandler.GetFilterScriptsAsync(user, formId);

            if (filterScripts != null)
            {
                FilterHandler filterHandler = new FilterHandler(_context, formId, user, _userHandler);
                isOk = await filterHandler.RemoveFilterScriptAppliedAsync();
                if (!isOk) { return BadRequest(new JsonResult("Error: Bad request.")); }
            }


            MtdFilterDate mtdFilterDate = await _context.MtdFilterDate.Where(x => x.Id == filterId).FirstOrDefaultAsync();
            if (mtdFilterDate != null)
            {
                _context.MtdFilterDate.Remove(mtdFilterDate);
            }

            MtdFilterOwner mtdFilterOwner = await _context.MtdFilterOwner.FindAsync(filterId);
            if (mtdFilterOwner != null)
            {
                _context.MtdFilterOwner.Remove(mtdFilterOwner);
            }

            MtdFilterRelated filterRelated = await _context.MtdFilterRelated.FindAsync(filterId);
            if (filterRelated != null)
            {
                _context.MtdFilterRelated.Remove(filterRelated);
            }

            try
            {
                _context.MtdFilterField.RemoveRange(mtdFilterFields);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return BadRequest(new JsonResult(ex.Message));
            }


            return Ok();

        }

        [HttpPost("filter/columns/add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostFilterColumnsAsync()
        {

            string formId = Request.Form["form-id"];
            string data = Request.Form["indexDataColumnList"];
            string showNumber = Request.Form["indexDataColumnNumber"];
            string showDate = Request.Form["indexDataColumnDate"];

            List<string> fieldIds = new List<string>();
            if (data != null && data.Length > 0) fieldIds = data.Split(",").ToList();
            WebAppUser user = await _userHandler.GetUserAsync(User);
            MtdFilter filter = await _context.MtdFilter.Include(m => m.MtdFilterColumn).FirstOrDefaultAsync(x => x.IdUser == user.Id & x.MtdForm == formId);

            if (filter == null)
            {
                filter = new MtdFilter
                {
                    IdUser = user.Id,
                    MtdForm = formId,
                    SearchNumber = "",
                    SearchText = "",
                    Page = 1,
                    PageSize = 10
                };
                await _context.MtdFilter.AddAsync(filter);
                await _context.SaveChangesAsync();
            }

            List<MtdFilterColumn> columns = new List<MtdFilterColumn>();
            int seq = 0;
            foreach (string field in fieldIds.Where(x => x != ""))
            {
                seq++;
                columns.Add(new MtdFilterColumn
                {
                    MtdFilter = filter.Id,
                    MtdFormPartField = field,
                    Sequence = seq
                });
            }


            try
            {
                filter.ShowNumber = showNumber == "true" ? (sbyte)1 : (sbyte)0;
                filter.ShowDate = showDate == "true" ? (sbyte)1 : (sbyte)0;

                _context.MtdFilter.Update(filter);

                if (filter.MtdFilterColumn != null)
                {
                    _context.MtdFilterColumn.RemoveRange(filter.MtdFilterColumn);
                    await _context.SaveChangesAsync();
                }

                await _context.MtdFilterColumn.AddRangeAsync(columns);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex.InnerException; }

            return Ok();
        }


        [HttpPost("waitlist/set")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostWaitListSetAsync()
        {
            string formId = Request.Form["id-form-waitlist"];
            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);
            MtdFilter mtdFilter = await _context.MtdFilter.Where(x => x.IdUser == user.Id && x.MtdForm == formId).FirstOrDefaultAsync();
            if (mtdFilter == null)
            {
                mtdFilter = new MtdFilter
                {
                    IdUser = user.Id,
                    MtdForm = formId,
                    PageSize = 10,
                    SearchText = "",
                    SearchNumber = "",
                    Page = 1,
                    WaitList = 1,
                };
                await _context.MtdFilter.AddAsync(mtdFilter);
                await _context.SaveChangesAsync();
                return Ok();
            }

            mtdFilter.WaitList = mtdFilter.WaitList == 0 ? 1 : 0;
            mtdFilter.Page = 1;
            _context.MtdFilter.Update(mtdFilter);
            await _context.SaveChangesAsync();

            return Ok();

        }


    }
}
