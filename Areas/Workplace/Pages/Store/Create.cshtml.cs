/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Areas.Workplace.Pages.Store
{
    public class CreateModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public CreateModel(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        [BindProperty]
        public MtdStore MtdStore { get; set; }
        public MtdForm MtdForm { get; set; }

        public IList<MtdForm> ParentForms { get; set; }

        public async Task<IActionResult> OnGet(string formId)
        {

            if (formId == null)
            {
                return NotFound();
            }

            var user = await _userHandler.GetUserAsync(HttpContext.User);
            bool isCreator = await _userHandler.IsCreator(user, formId);

            if (!isCreator)
            {
                return Forbid();
            }

            MtdForm = await _context.MtdForm.FindAsync(formId);            
            MtdStore = new MtdStore { MtdForm = MtdForm.Id, MtdFormNavigation = MtdForm};
            ParentForms = new List<MtdForm>();
            List<MtdForm> parentForms = await _context.MtdFormRelated.Include(x=>x.MtdParentForm)
                .Where(x => x.ChildFormId == MtdForm.Id).Select(x=>x.MtdParentForm)
                .OrderBy(x=>x.Sequence)
                .ToListAsync();

            bool isRelatedCreator = await _userHandler.CheckUserPolicyAsync(user, formId, RightsType.RelatedCreate);

            if (parentForms != null && isRelatedCreator)
            {
                foreach(MtdForm form in parentForms)
                {
                    bool isViewer = await _userHandler.IsViewer(user, form.Id);
                    
                    if (isViewer)
                    {
                        ParentForms.Add(form);
                    }
                }

            }

            return Page();
        }

    }
}