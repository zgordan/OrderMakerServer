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
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Controllers.Config.Form
{
    [Route("api/config/form")]
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
            string formId = Request.Form["IdForm"];
            if (formId == null)
            {
                return NotFound();
            }

            IList<MtdFormPartField> fields = await _context.MtdFormList
                .Include(x => x.IdNavigation)
                .Where(x => x.MtdForm == formId)
                .Select(x => x.IdNavigation).ToListAsync();

            MtdForm mtdForm = new MtdForm { Id = formId };
            
            _context.MtdForm.Remove(mtdForm);
            if (fields != null)
            {
                _context.MtdFormPartField.RemoveRange(fields);
            }
            
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("sequence")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostSequenceAsync()
        {
            string strData = Request.Form["formSeqData"];

            string[] data = strData.Split("&");

            IList<MtdForm> forms = await _context.MtdForm.ToListAsync();

            int counter = 0;
            foreach (string id in data)
            {
                var form = forms.FirstOrDefault(x => x.Id == id);
                if (form != null)
                {
                    form.Sequence = counter;
                    counter++;
                }
            }

            _context.MtdForm.UpdateRange(forms);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("field/sequence")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostFieldSequenceAsync()
        {
            string strData = Request.Form["fieldSeqData"];
            string partId = Request.Form["fieldPart"];
            string fieldId = Request.Form["fieldForm"];

            string[] data = strData.Split("&");

            IList<MtdFormPartField> fields = await _context.MtdFormPartField.Where(x => x.MtdFormPart == partId).ToListAsync();

            int counter = 0;
            foreach (string id in data)
            {
                var field = fields.Where(x => x.Id == id).FirstOrDefault();
                if (field != null)
                {
                    field.Sequence = counter;
                    counter++;
                }
            }

            _context.MtdFormPartField.UpdateRange(fields);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("field/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostFieldCreateAsync()
        {
            string formId = Request.Form["formId"];
            string partId = Request.Form["partId"];
            string fieldId = Request.Form["fieldId"];
            string fieldName = Request.Form["fieldName"];
            string fieldNote = Request.Form["fieldNote"];
            string fieldType = Request.Form["fieldType"];
            string fieldForm = Request.Form["fieldForm"];

            int fieldTypeID = int.Parse(fieldType);

            bool check = Guid.TryParse(fieldId, out Guid result);

            if (!check)
            {
                fieldId = null;
            }

            bool isExists = await _context.MtdFormPartField.Where(x => x.MtdFormPart == partId).AnyAsync();
            int seq = 0;
            if (isExists)
            {
                seq = await _context.MtdFormPartField.Where(x => x.MtdFormPart == partId).MaxAsync(x => x.Sequence);
            }

            MtdFormPartField mtdFormPartField = new MtdFormPartField
            {
                Id = fieldId,
                Name = fieldName,
                Description = fieldNote,
                Active = 1,
                MtdFormPart = partId,
                MtdSysType = fieldTypeID,
                Sequence = seq + 1,
                Required = 0,

            };


            if (fieldTypeID == 11)
            {
                mtdFormPartField.MtdFormList = new MtdFormList { Id = fieldId, MtdForm = fieldForm };
            }

            await _context.MtdFormPartField.AddAsync(mtdFormPartField);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("field/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostFieldEditAsync()
        {          
            string partId = Request.Form["fieldPart"];
            string fieldId = Request.Form["fieldId"];
            string fieldName = Request.Form["fieldName"];
            string fieldNote = Request.Form["fieldNote"];

            MtdFormPartField mtdFormPartField = await _context.MtdFormPartField.FindAsync(fieldId);

            if (mtdFormPartField == null)
            {
                return NotFound();
            }

            mtdFormPartField.MtdFormPart = partId;
            mtdFormPartField.Name = fieldName;
            mtdFormPartField.Description = fieldNote;

            bool required = await Components.MTDCheckbox.GetResultAsync(mtdFormPartField.Id, Request);
            mtdFormPartField.Required = required ? (sbyte)1 : (sbyte)0;

            _context.MtdFormPartField.Update(mtdFormPartField);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("field/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostFieldDeleteAsync()
        {
            string fieldId = Request.Form["fieldId"];

            MtdFormPartField mtdFormPartField = new MtdFormPartField
            {
                Id = fieldId
            };

            _context.MtdFormPartField.Remove(mtdFormPartField);
            await _context.SaveChangesAsync();


            return Ok();
        }

        [HttpPost("part/sequence")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPartSequenceAsync()
        {
            string strData = Request.Form["fieldSeqData"];
            string partId = Request.Form["fieldPart"];
            string formId = Request.Form["fieldForm"];

            string[] data = strData.Split("&");

            IList<MtdFormPart> parts = await _context.MtdFormPart.Where(x => x.MtdForm == formId).ToListAsync();

            int counter = 0;
            foreach (string id in data)
            {
                var field = parts.Where(x => x.Id == id).FirstOrDefault();
                if (field != null)
                {
                    field.Sequence = counter;
                    counter++;
                }
            }

            _context.MtdFormPart.UpdateRange(parts);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("part/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPartCreateAsync()
        {
            string formId = Request.Form["formId"];
            string partId = Request.Form["partId"];
            string partName = Request.Form["partName"];
            string partNote = Request.Form["partNote"];

            bool check = Guid.TryParse(partId, out Guid result);

            if (!check)
            {
                partId = null;
            }
            
            bool isExists = await _context.MtdFormPart.Where(x => x.MtdForm == formId).AnyAsync();
            int seq = 0;
            if (isExists)
            {
                seq = await _context.MtdFormPart.Where(x => x.MtdForm == formId).MaxAsync(x => x.Sequence);
            }

            MtdFormPart mtdFormPart = new MtdFormPart
            {
                Id = partId,
                Name = partName,
                Description = partNote,
                Active = true,
                MtdForm = formId,
                Title = 1,
                MtdSysStyle = 4,
                Sequence = seq + 1,

            };

            await _context.MtdFormPart.AddAsync(mtdFormPart);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("part/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPartEditAsync()
        {
            string formId = Request.Form["formId"];
            string partId = Request.Form["partId"];
            string partName = Request.Form["partName"];
            string partNote = Request.Form["partNote"];
            string partStyle = Request.Form["partStyle"];
            string partTitle = Request.Form["partTitle"];
            string partChild = Request.Form["partChild"];            
            string partSeq = Request.Form["partSeq"];

            int styleId = int.Parse(partStyle);
            int sequence = int.Parse(partSeq);
            bool.TryParse(partTitle, out bool titleCheck);
            bool.TryParse(partChild, out bool childCheck);            

            MtdFormPart mtdFormPart = new MtdFormPart
            {
                Id = partId,
                Name = partName,
                Description = partNote,
                Active = true,
                MtdForm = formId,
                Title = titleCheck ? (sbyte) 1: (sbyte)0,
                Child = childCheck ? (sbyte)1 : (sbyte)0,
                MtdSysStyle = styleId,
                Sequence = sequence             

            };

            _context.MtdFormPart.Attach(mtdFormPart).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            _context.Attach(mtdFormPart).State = EntityState.Modified;

            string idCheckBox = "header-delete";
            if (Request.Form[idCheckBox].FirstOrDefault() == null || Request.Form[idCheckBox].FirstOrDefault() == "false")
            {
                string idInput = "header-file-upload-input";
                IFormFile file = Request.Form.Files.FirstOrDefault(x => x.Name == idInput);
                if (file != null)
                {
                    byte[] streamArray = new byte[file.Length];
                    await file.OpenReadStream().ReadAsync(streamArray, 0, streamArray.Length);
                    MtdFormPartHeader header = new MtdFormPartHeader()
                    {
                        Id = mtdFormPart.Id,
                        Image = streamArray,
                        ImageSize = streamArray.Length,
                        ImageType = file.ContentType
                    };

                    bool exists = await _context.MtdFormPartHeader.Where(x => x.Id == mtdFormPart.Id).AnyAsync();
                    if (exists)
                        _context.Attach(header).State = EntityState.Modified;
                    else
                        _context.Attach(header).State = EntityState.Added;
                }
            }
            else
            {
                MtdFormPartHeader header = new MtdFormPartHeader() { Id = mtdFormPart.Id };
                _context.Attach(header).State = EntityState.Deleted;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("part/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPartDeleteAsync()
        {
            string partId = Request.Form["partId"];

            MtdFormPart mtdFormPart = new MtdFormPart
            {
                Id = partId
            };

            _context.MtdFormPart.Remove(mtdFormPart);
            await _context.SaveChangesAsync();


            return Ok();
        }
    }
}