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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.DataHandler.Approval;

namespace Mtd.OrderMaker.Web.Controllers.Config.Approval
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
            string approvalId = Request.Form["IdApproval"];
            string fieldName = Request.Form["fieldName"];
            string fieldNote = Request.Form["fieldNote"];
            string fieldUser = Request.Form["fieldUser"];
            int.TryParse(Request.Form["fieldStage"], out int fieldStage);
            string blockParts = string.Empty;

            if (approvalId == null)
            {
                return NotFound();
            }

            MtdApproval mtdApproval = await _context.MtdApproval.FindAsync(approvalId);
            IList<string> partIds = await _context.MtdFormPart.Where(x => x.MtdForm == mtdApproval.MtdForm).Select(x => x.Id).ToListAsync();

            foreach (string id in partIds)
            {
                bool result = bool.TryParse(Request.Form[id], out bool check);
                if (result && check)
                {
                    blockParts += $"{id}&";
                }
            }

            MtdApprovalStage stage = new MtdApprovalStage
            {
                MtdApproval = approvalId,
                BlockParts = blockParts,
                Description = fieldNote,
                Name = fieldName,
                UserId = fieldUser,
                Stage = fieldStage,

            };

            await _context.MtdApprovalStage.AddAsync(stage);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("stage/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostStageEditAsync()
        {
            string stageId = Request.Form["IdStage"];
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

            await ResolutionUpdate(id);
            await RejectionUpdate(id);
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


        private async Task ResolutionUpdate(int stageId)
        {
            List<MtdApprovalResolution> listUpdate = new List<MtdApprovalResolution>();
            List<MtdApprovalResolution> listAdd = new List<MtdApprovalResolution>();
            List<MtdApprovalResolution> listRemove = new List<MtdApprovalResolution>();

            List<StringValues> resolutionIds = Request.Form.Where(x => x.Key.Contains("resolution-id")).Select(x => x.Value).ToList();
            IList<MtdApprovalResolution> oldData = await _context.MtdApprovalResolution.AsNoTracking().Where(x=>x.MtdApprovalStageId == stageId).ToListAsync();
            List<MtdApprovalResolution> newData = new List<MtdApprovalResolution>();
            foreach (string resolutionId in resolutionIds)
            {
                string intString = Request.Form[$"{resolutionId}-resolution-number"];
                bool isOk = int.TryParse(intString, out int number);

                newData.Add(new MtdApprovalResolution
                {
                    Id = resolutionId,
                    Name = Request.Form[$"{resolutionId}-resolution-name"],
                    Note = Request.Form[$"{resolutionId}-resolution-note"],
                    Sequence = isOk ? number : 0,
                    Color = Request.Form[$"{resolutionId}-resolution-color"],
                    MtdApprovalStageId = stageId,
                });
            }


            foreach (var resolution in newData)
            {
                bool exists = oldData.Where(x => x.Id == resolution.Id).Any();
                if (exists) { listUpdate.Add(resolution); } else { listAdd.Add(resolution); }
            }

            listRemove = oldData.Where(x => !newData.Select(s => s.Id).Contains(x.Id)).ToList();
            
            if (listUpdate.Count > 0)
            {
                try
                {
                    _context.MtdApprovalResolution.UpdateRange(listUpdate);
                } catch (Exception e)
                {
                    throw e.InnerException;
                }
                
            }
            
            if (listAdd.Count > 0)
            {
                await _context.MtdApprovalResolution.AddRangeAsync(listAdd);
            }

            if (listRemove.Count > 0)
            {
                _context.MtdApprovalResolution.RemoveRange(listRemove);
            }
            
            
        }

        private async Task RejectionUpdate(int stageId)
        {
            List<MtdApprovalRejection> listUpdate = new List<MtdApprovalRejection>();
            List<MtdApprovalRejection> listAdd = new List<MtdApprovalRejection>();
            List<MtdApprovalRejection> listRemove = new List<MtdApprovalRejection>();

            List<StringValues> rejectionIds = Request.Form.Where(x => x.Key.Contains("rejection-id")).Select(x => x.Value).ToList();
            IList<MtdApprovalRejection> oldData = await _context.MtdApprovalRejection.AsNoTracking().Where(x => x.MtdApprovalStageId == stageId).ToListAsync();
            List<MtdApprovalRejection> newData = new List<MtdApprovalRejection>();
            foreach (string rejectionId in rejectionIds)
            {
                string intString = Request.Form[$"{rejectionId}-rejection-number"];
                bool isOk = int.TryParse(intString, out int number);

                newData.Add(new MtdApprovalRejection
                {
                    Id = rejectionId,
                    Name = Request.Form[$"{rejectionId}-rejection-name"],
                    Note = Request.Form[$"{rejectionId}-rejection-note"],
                    Sequence = isOk ? number : 0,
                    Color = Request.Form[$"{rejectionId}-rejection-color"],
                    MtdApprovalStageId = stageId,
                });
            }


            foreach (var rejection in newData)
            {
                bool exists = oldData.Where(x => x.Id == rejection.Id).Any();
                if (exists) { listUpdate.Add(rejection); } else { listAdd.Add(rejection); }
            }

            listRemove = oldData.Where(x => !newData.Select(s => s.Id).Contains(x.Id)).ToList();

            if (listUpdate.Count > 0)
            {
                try
                {
                    _context.MtdApprovalRejection.UpdateRange(listUpdate);
                }
                catch (Exception e)
                {
                    throw e.InnerException;
                }

            }

            if (listAdd.Count > 0)
            {
                await _context.MtdApprovalRejection.AddRangeAsync(listAdd);
            }

            if (listRemove.Count > 0)
            {
                _context.MtdApprovalRejection.RemoveRange(listRemove);
            }


        }
    }
}