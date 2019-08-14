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


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Services;

namespace Mtd.OrderMaker.Web.Areas.Workplace.Pages.Store
{
    public class IndexModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public IndexModel(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        public MtdForm MtdForm { get; set; }

        public async Task<IActionResult> OnGetAsync(string indexForm)
        {

            WebAppUser user = await _userHandler.GetUserAsync(HttpContext.User);
            bool isViewer = await _userHandler.IsViewer(user, indexForm);
            bool OwnerRight = await _userHandler.GetFormPolicyAsync(user, indexForm, RightsType.ViewOwn);
            bool GroupRight = await _userHandler.GetFormPolicyAsync(user, indexForm, RightsType.ViewGroup);

            if (!isViewer & !OwnerRight & !GroupRight)
            {
                return Forbid(); 
            }

            MtdForm = await _context.MtdForm.FindAsync(indexForm);

            if (MtdForm == null)
            {
                return NotFound();
            }
            
            ViewData["IdForm"] = indexForm;
            return Page();
        }


        private async void CloneStore(string idStore)
        {

            var q = from n in _context.MtdStoreStack.Where(x => x.MtdStore == idStore)
                    group n by n.MtdFormPartField into g
                    select new { idField = g.Key, idStack = g.FirstOrDefault(x => x.Id == g.Max(m => m.Id)).Id };

            List<long> ids = await q.Select(x => x.idStack).ToListAsync();


            IList<MtdStoreStack> stack = await _context.MtdStoreStack
               .Include(x => x.MtdStoreStackDate)
               .Include(x => x.MtdStoreStackText)
               .Include(x => x.MtdStoreStackInt)
               .Include(x => x.MtdStoreStackDecimal)
               .Include(x => x.MtdStoreStackFile)
               .Include(x => x.MtdStoreLink)
               .Where(x => ids.Contains(x.Id))
               .ToListAsync();

            string idForm = await _context.MtdStore.Where(s => s.Id == idStore).Select(x => x.MtdForm).FirstOrDefaultAsync();
            int maxSeq = await _context.MtdStore.Where(x => x.MtdForm == idForm).MaxAsync(s => s.Sequence);
            maxSeq++;

            MtdStore mtdStore = new MtdStore()
            {
                Sequence = maxSeq,
                MtdForm = idForm
            };


            foreach (MtdStoreStack storeStack in stack)
            {

                MtdStoreStack newStack = new MtdStoreStack()
                {
                    MtdStore = storeStack.MtdStore,
                    MtdFormPartField = storeStack.MtdFormPartField,
                    MtdStoreStackDate = storeStack.MtdStoreStackDate != null ? new MtdStoreStackDate { Register = storeStack.MtdStoreStackDate.Register } : null,
                    MtdStoreStackText = storeStack.MtdStoreStackText != null ? new MtdStoreStackText { Register = storeStack.MtdStoreStackText.Register } : null,
                    MtdStoreStackInt = storeStack.MtdStoreStackInt != null ? new MtdStoreStackInt { Register = storeStack.MtdStoreStackInt.Register } : null,
                    MtdStoreStackDecimal = storeStack.MtdStoreStackDecimal != null ? new MtdStoreStackDecimal { Register = storeStack.MtdStoreStackDecimal.Register } : null,

                    MtdStoreStackFile = storeStack.MtdStoreStackFile != null ? new MtdStoreStackFile
                    {
                        Register = storeStack.MtdStoreStackFile.Register,
                        FileName = storeStack.MtdStoreStackFile.FileName,
                        FileSize = storeStack.MtdStoreStackFile.FileSize,
                        FileType = storeStack.MtdStoreStackFile.FileType

                    } : null,

                    MtdStoreLink = storeStack.MtdStoreLink,

                };

                mtdStore.MtdStoreStack.Add(newStack);

            }

            _context.MtdStore.Add(mtdStore);
            await _context.SaveChangesAsync();


        }
    }
}
