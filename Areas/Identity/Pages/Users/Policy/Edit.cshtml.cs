/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.AppConfig;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;

namespace Mtd.OrderMaker.Server.Areas.Identity.Pages.Users.Policy
{
    public class EditModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly LimitSettings limit;

        public EditModel(OrderMakerContext context, IOptions<LimitSettings> limit)
        {
            _context = context;
            this.limit = limit.Value;
        }

        public MtdPolicy MtdPolicy { get; set; }
        public IList<MtdForm> MtdForms { get; set; }
        public IList<MtdGroup> MtdGroups { get; set; }
        public bool ExportToExcel { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) { return NotFound(); }

            MtdPolicy = await _context.MtdPolicy
                .Include(i => i.MtdPolicyForms)                
                .Include(i => i.MtdPolicyParts)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (MtdPolicy == null) { return NotFound(); }

            MtdGroups = await _context.MtdGroup.OrderBy(x=>x.Name).ToListAsync();
            MtdForms = await _context.MtdForm.Include(x => x.MtdFormPart).OrderBy(x=>x.Sequence).ToListAsync();
            ExportToExcel = limit.ExportExcel;
            return Page();
        }

    }
}