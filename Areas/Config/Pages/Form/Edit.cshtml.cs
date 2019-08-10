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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Data;

namespace Mtd.OrderMaker.Web.Areas.Config.Pages.Form
{
    public class EditModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public EditModel(OrderMakerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MtdForm MtdForm { get; set; }
        [BindProperty]
        public bool VisibleNumber { get; set; }
        [BindProperty]
        public bool VisibleDate { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }


            MtdForm = await _context.MtdForm.Include(x => x.ParentNavigation).Include(m => m.MtdFormHeader).Include(m => m.MtdFormDesk).FirstOrDefaultAsync(m => m.Id == id);
            if (MtdForm == null)
            {
                return NotFound();
            }


            VisibleNumber = MtdForm.VisibleNumber == 1 ? true : false;
            VisibleDate = MtdForm.VisibleDate == 1 ? true : false;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            MtdForm oldForm = await _context.MtdForm.AsNoTracking().Include(x => x.ParentNavigation).FirstOrDefaultAsync(x => x.Id == MtdForm.Id);
            if (oldForm == null)
            {
                return NotFound();
            }

            MtdForm.Parent = oldForm.Parent;
            _context.Attach(MtdForm).State = EntityState.Modified;

            MtdForm.VisibleNumber = VisibleNumber ? (sbyte)1 : (sbyte)0;
            MtdForm.VisibleDate = VisibleDate ? (sbyte)1 : (sbyte)0;

            string idCheckBox = "header-delete";
            if (Request.Form[idCheckBox].FirstOrDefault() == null || Request.Form[idCheckBox].FirstOrDefault() == "false")
            {
                string idInput = "header-file-upload-input";
                IFormFile file = Request.Form.Files.FirstOrDefault(x => x.Name == idInput);
                if (file != null)
                {
                    byte[] streamArray = new byte[file.Length];
                    await file.OpenReadStream().ReadAsync(streamArray, 0, streamArray.Length);
                    MtdFormHeader header = new MtdFormHeader()
                    {
                        Id = MtdForm.Id,
                        Image = streamArray,
                        ImageSize = streamArray.Length,
                        ImageType = file.ContentType
                    };

                    bool exists = await _context.MtdFormHeader.Where(x => x.Id == MtdForm.Id).AnyAsync();
                    if (exists)
                        _context.Attach(header).State = EntityState.Modified;
                    else
                        _context.Attach(header).State = EntityState.Added;
                }
            }
            else
            {
                MtdFormHeader header = new MtdFormHeader() { Id = MtdForm.Id };
                _context.Attach(header).State = EntityState.Deleted;
            }


            string idCheckDeskBox = "desk-delete";

            if (Request.Form[idCheckDeskBox].FirstOrDefault() == null || Request.Form[idCheckDeskBox].FirstOrDefault() == "false")
            {
                string idInput = "desk-file-upload-input";
                IFormFile file = Request.Form.Files.FirstOrDefault(x => x.Name == idInput);
                if (file != null)
                {
                    byte[] streamArray = new byte[file.Length];
                    await file.OpenReadStream().ReadAsync(streamArray, 0, streamArray.Length);
                    MtdFormDesk desk = new MtdFormDesk()
                    {
                        Id = MtdForm.Id,
                        Image = streamArray,
                        ImageSize = streamArray.Length,
                        ImageType = file.ContentType,
                        ColorBack = "gray",
                        ColorFont = "black"

                    };

                    bool exists = await _context.MtdFormDesk.Where(x => x.Id == MtdForm.Id).AnyAsync();
                    if (exists)
                        _context.Attach(desk).State = EntityState.Modified;
                    else
                        _context.Attach(desk).State = EntityState.Added;
                }
            }
            else
            {
                MtdFormDesk desk = new MtdFormDesk() { Id = MtdForm.Id };
                _context.Attach(desk).State = EntityState.Deleted;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MtdFormExists(MtdForm.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool MtdFormExists(string id)
        {
            return _context.MtdForm.Any(e => e.Id == id);
        }
    }
}
