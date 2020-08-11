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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Form
{

    public class FormRelated
    {
        public MtdForm MtdForm { get; set; }
        public bool Checked { get; set; }
    }

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

        public List<FormRelated> ListForms { get; set; }

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


            VisibleNumber = MtdForm.VisibleNumber == 1;
            VisibleDate = MtdForm.VisibleDate == 1;

            IList<MtdForm> forms = await _context.MtdForm.OrderBy(x => x.Sequence).ToListAsync();
            IList<MtdFormRelated> relateds = await _context.MtdFormRelated.Where(x => x.ParentFormId == MtdForm.Id).ToListAsync();
            ListForms = new List<FormRelated>();
            foreach (var form in forms)
            {
                ListForms.Add(new FormRelated
                {
                    MtdForm = form,
                    Checked = relateds.Where(x => x.ChildFormId == form.Id).Any()
                });
            }

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

            var form = await Request.ReadFormAsync();

            IList<MtdForm> mtdForms = await _context.MtdForm.AsNoTracking().OrderBy(x => x.Sequence).ToListAsync();
            List<MtdFormRelated> relateds = new List<MtdFormRelated>();
            IList<MtdFormRelated> listForDelete = await _context.MtdFormRelated.AsNoTracking().Where(x => x.ParentFormId == oldForm.Id).ToListAsync();
            if (listForDelete.Count>0)
            {
                _context.MtdFormRelated.RemoveRange(listForDelete);
                await _context.SaveChangesAsync();
            }
            

            foreach (var mtdForm in mtdForms)
            {
                if (form[$"{mtdForm.Id}-related"].FirstOrDefault() == "true")
                {
                    relateds.Add(new MtdFormRelated { Id = Guid.NewGuid().ToString(), ParentFormId = MtdForm.Id, ChildFormId = mtdForm.Id });
                }
            }

            if (relateds.Count > 0)
            {
                _context.MtdFormRelated.AddRange(relateds);
            }

            MtdForm.Parent = oldForm.Parent;
            _context.Attach(MtdForm).State = EntityState.Modified;

            MtdForm.VisibleNumber = VisibleNumber ? (sbyte)1 : (sbyte)0;
            MtdForm.VisibleDate = VisibleDate ? (sbyte)1 : (sbyte)0;

            string idCheckBox = "header-delete";
            if (form[idCheckBox].FirstOrDefault() == null || form[idCheckBox].FirstOrDefault() == "false")
            {
                string idInput = "header-file-upload-input";
                IFormFile file = form.Files.FirstOrDefault(x => x.Name == idInput);
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

            if (form[idCheckDeskBox].FirstOrDefault() == null || form[idCheckDeskBox].FirstOrDefault() == "false")
            {
                string idInput = "desk-file-upload-input";
                IFormFile file = form.Files.FirstOrDefault(x => x.Name == idInput);
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
