/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Approval;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Areas.Workplace.Pages.Store
{
    public class EditModel : PageModel
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public EditModel(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }


        public MtdForm MtdForm { get; set; }
        public MtdStore MtdStore { get; set; }
        public IList<MtdForm> ParentForms { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MtdStore = await _context.MtdStore.FirstOrDefaultAsync(m => m.Id == id);

            if (MtdStore == null)
            {
                return NotFound();
            }

            var user = await _userHandler.GetUserAsync(HttpContext.User);            
            bool isEditor = await _userHandler.IsEditor(user,MtdStore.MtdForm,MtdStore.Id);
            
            if (!isEditor) {
                return Forbid();
            }

            WebAppUser webUser = await _userHandler.GetUserAsync(HttpContext.User);
            ApprovalHandler approvalHandler = new ApprovalHandler(_context, MtdStore.Id);
            ApprovalStatus approvalStatus = await approvalHandler.GetStatusAsync(webUser);

            if (approvalStatus == ApprovalStatus.Rejected)
            {
                return Forbid();
            }

            MtdForm = await _context.MtdForm.FindAsync(MtdStore.MtdForm);
            ParentForms = new List<MtdForm>();

            List<MtdForm> parentForms = await _context.MtdFormRelated.Include(x => x.MtdParentForm)
                 .Where(x => x.ChildFormId == MtdForm.Id).Select(x => x.MtdParentForm)
                 .OrderBy(x => x.Sequence)
                 .ToListAsync();
            
            bool isRelatedEditor = await _userHandler.CheckUserPolicyAsync(user, MtdForm.Id, RightsType.RelatedEdit);

            if (parentForms != null && isRelatedEditor)
            {
                foreach(MtdForm form in parentForms)
                {
                    bool isViewer = await _userHandler.IsViewer(user, form.Id);                    

                    if (isViewer && isRelatedEditor)
                    {
                        ParentForms.Add(form);
                    }
                }
            }

            return Page();
        }

    }
}
