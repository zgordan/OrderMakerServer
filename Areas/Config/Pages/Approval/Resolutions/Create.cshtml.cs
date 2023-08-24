using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mtd.OrderMaker.Server.Components;
using Mtd.OrderMaker.Server.Entity;
using System;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Approval.Resolutions
{
    public class CreateModel : PageModel
    {
        private readonly OrderMakerContext _context;

        public CreateModel(OrderMakerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MtdApprovalResolution MtdApprovalResolution { get; set; }

        [BindProperty]
        public MtdApprovalStage MtdApprovalStage { get; set; }
        public async Task<IActionResult> OnGetAsync(int stage)
        {

            MtdApprovalStage = await _context.MtdApprovalStage.FindAsync(stage);
            if (MtdApprovalStage == null) { return NotFound(); }
            MtdApprovalResolution = new MtdApprovalResolution { Id = Guid.NewGuid().ToString() };

            return Page();
        }



        public async Task OnPostAsync()
        {
            MtdApprovalResolution.MtdApprovalStageId = MtdApprovalStage.Id;

            MTDImgSModify img = await MTDImgSelector.ImageModifyAsync("img", Request);
            MtdApprovalResolution.ImgData = img.Image;
            MtdApprovalResolution.ImgType = img.ImgType;

            _context.MtdApprovalResolution.Add(MtdApprovalResolution);
            await _context.SaveChangesAsync();

        }
    }
}