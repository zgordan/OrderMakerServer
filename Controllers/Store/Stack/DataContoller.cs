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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Services;
using System.IO;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Controllers.Store.Stack
{
    [Route("api/store/stack")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class DataContoller : ControllerBase
    {
        private readonly UserHandler _userHandler;
        private readonly OrderMakerContext _context;

        public DataContoller(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        // GET: api/MtdStoreStack/5/file/
        [HttpGet("{id}/file")]
        public async Task<ActionResult> GetStackFile(int id)
        {

            var mtdStoreStack = await _context.MtdStoreStack
                .Include(m => m.MtdStoreStackFile)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (mtdStoreStack == null || mtdStoreStack.MtdStoreStackFile == null)
            {
                return NotFound();
            }

            var user = await _userHandler.GetUserAsync(HttpContext.User);
            var store = await _context.MtdStore.FindAsync(mtdStoreStack.MtdStore);

            bool isViewer = await _userHandler.IsViewer(user, store.MtdForm,store.Id);
            if (!isViewer)
            {
                return Ok(403);
            }

            return new FileStreamResult(new MemoryStream(mtdStoreStack.MtdStoreStackFile.Register), mtdStoreStack.MtdStoreStackFile.FileType);
        }
    }
}
