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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Components;

namespace Mtd.OrderMaker.Web.Areas.Config.Pages.Approval
{
    public class EditModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public EditModel(OrderMakerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MtdApproval MtdApproval { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MtdApproval = await _context.MtdApproval.Include(x => x.MtdFormNavigation).FirstOrDefaultAsync(x => x.Id == id);

            if (MtdApproval == null)
            {
                return NotFound();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(MtdApproval).State = EntityState.Modified;
            _context.Entry(MtdApproval).Property(x => x.MtdForm).IsModified = false;

            MTDImgSModify mStart = await MTDImgSelector.ImageModifyAsync("img-start", Request);
            MtdApproval.ImgStart = mStart.Image;
            MtdApproval.ImgStartType = mStart.ImgType;
            _context.Entry(MtdApproval).Property(x => x.ImgStart).IsModified = mStart.Modify;
            _context.Entry(MtdApproval).Property(x => x.ImgStartType).IsModified = mStart.Modify;

            MTDImgSModify mIteraction = await MTDImgSelector.ImageModifyAsync("img-iteraction", Request);
            MtdApproval.ImgIteraction = mIteraction.Image;
            MtdApproval.ImgIteractionType = mIteraction.ImgType;
            _context.Entry(MtdApproval).Property(x => x.ImgIteraction).IsModified = mIteraction.Modify;
            _context.Entry(MtdApproval).Property(x => x.ImgIteractionType).IsModified = mIteraction.Modify;

            MTDImgSModify mRequired = await MTDImgSelector.ImageModifyAsync("img-required", Request);
            MtdApproval.ImgRequired = mRequired.Image;
            MtdApproval.ImgRequiredType = mRequired.ImgType;
            _context.Entry(MtdApproval).Property(x => x.ImgRequired).IsModified = mRequired.Modify;
            _context.Entry(MtdApproval).Property(x => x.ImgRequiredType).IsModified = mRequired.Modify;

            MTDImgSModify mWaiting = await MTDImgSelector.ImageModifyAsync("img-waiting", Request);
            MtdApproval.ImgWaiting = mWaiting.Image;
            MtdApproval.ImgWaitingType = mWaiting.ImgType;
            _context.Entry(MtdApproval).Property(x => x.ImgWaiting).IsModified = mWaiting.Modify;
            _context.Entry(MtdApproval).Property(x => x.ImgWaitingType).IsModified = mWaiting.Modify;

            MTDImgSModify mApproved = await MTDImgSelector.ImageModifyAsync("img-approved", Request);
            MtdApproval.ImgApproved = mApproved.Image;
            MtdApproval.ImgApprovedType = mApproved.ImgType;
            _context.Entry(MtdApproval).Property(x => x.ImgApproved).IsModified = mApproved.Modify;
            _context.Entry(MtdApproval).Property(x => x.ImgApprovedType).IsModified = mApproved.Modify;

            MTDImgSModify mRejected = await MTDImgSelector.ImageModifyAsync("img-rejected", Request);
            MtdApproval.ImgRejected = mRejected.Image;
            MtdApproval.ImgRejectedType = mRejected.ImgType;
            _context.Entry(MtdApproval).Property(x => x.ImgRejected).IsModified = mRejected.Modify;
            _context.Entry(MtdApproval).Property(x => x.ImgRejectedType).IsModified = mRejected.Modify;


            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }


    }
}