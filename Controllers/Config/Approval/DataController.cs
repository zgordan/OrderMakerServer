/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Approval;

namespace Mtd.OrderMaker.Server.Controllers.Config.Approval
{
    [Route("api/config/approval")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DataController : ControllerBase
    {

        private readonly OrderMakerContext _context;


        public DataController(OrderMakerContext context)
        {
            _context = context;
        }

        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            string approvalId = Request.Form["IdApproval"];
            if (approvalId == null)
            {
                return NotFound();
            }

            MtdApproval mtdApproval = new MtdApproval { Id = approvalId };
            _context.MtdApproval.Remove(mtdApproval);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("stage/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostStageCreateAsync()
        {
            string approvalId = Request.Form["approvalId"];
            string fieldName = Request.Form["fieldName"];
            string fieldNote = Request.Form["fieldNote"];
            string fieldUser = "owner";
                 
            string blockParts = string.Empty;
            MtdApproval mtdApproval = await _context.MtdApproval.FindAsync(approvalId ?? "");          

            if (mtdApproval == null)
            {
                return BadRequest(new JsonResult("Error. Approval not found."));
            }

  
            MtdApprovalStage stage = new MtdApprovalStage
            {                
                MtdApproval = approvalId,
                BlockParts = blockParts,
                Description = fieldNote,
                Name = fieldName,
                UserId = fieldUser,
                Stage = 0,

            };

            await _context.MtdApprovalStage.AddAsync(stage);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("stage/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostStageEditAsync()
        {
            string stageId = Request.Form["stageId"];
            string fieldName = Request.Form["fieldName"];
            string fieldNote = Request.Form["fieldNote"];
            string fieldUser = Request.Form["fieldUser"];

            int.TryParse(Request.Form["fieldStage"], out int fieldStage);
            string blockParts = string.Empty;

            if (stageId == null || !int.TryParse(stageId, out int id))
            {
                return NotFound();
            }


            MtdApprovalStage mtdApprovalStage = await _context.MtdApprovalStage.Include(x => x.MtdApprovalNavigation).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (mtdApprovalStage == null) { return NotFound(); }
            IList<string> partIds = await _context.MtdFormPart.Where(x => x.MtdForm == mtdApprovalStage.MtdApprovalNavigation.MtdForm).Select(x => x.Id).ToListAsync();

            foreach (string idPart in partIds)
            {
                bool result = bool.TryParse(Request.Form[idPart], out bool check);
                if (result && check)
                {
                    blockParts += $"{idPart}&";
                }
            }

            mtdApprovalStage.BlockParts = blockParts;
            mtdApprovalStage.Description = fieldNote;
            mtdApprovalStage.Name = fieldName;
            mtdApprovalStage.UserId = fieldUser;
            mtdApprovalStage.Stage = fieldStage;

            _context.MtdApprovalStage.Update(mtdApprovalStage);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("stage/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostStageDeleteAsync()
        {
            string stageId = Request.Form["id-stage-delete"];
            bool ok = int.TryParse(stageId, out int id);
            if (stageId == null && !ok)
            {
                return NotFound();
            }

            MtdApprovalStage stage = new MtdApprovalStage { Id = id };
            _context.MtdApprovalStage.Remove(stage);
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpPost("stage/sequence")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostStageSequenceAsync()
        {
            string strData = Request.Form["drgData"];
            string approvalId = Request.Form["IdApproval"];
            string formId = Request.Form["fieldForm"];

            string[] data = strData.Split("&");

            IList<MtdApprovalStage> stages = await _context.MtdApprovalStage.Where(x => x.MtdApproval == approvalId).ToListAsync();

            int counter = 1;
            foreach (string idStr in data)
            {
                bool isOk = int.TryParse(idStr, out int id);
                if (isOk)
                {
                    var field = stages.Where(x => x.Id == id).FirstOrDefault();
                    if (field != null)
                    {
                        field.Stage = counter;
                        counter++;
                    }
                }
            }

            _context.MtdApprovalStage.UpdateRange(stages);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("stage/update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostStageUpdateAsync()
        {
            string approvalId = Request.Form["approval-id"];

            await ApprovalHandler.UpdateStatusForStartAsync(_context, approvalId);

            return Ok();
        }

    }
}