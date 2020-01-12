﻿/*
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


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Data;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Mtd.OrderMaker.Server.Controllers.Users
{
    [Route("api/policy")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PolicyController : ControllerBase
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public PolicyController(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPolicyAddAsync()
        {
            string policyName = Request.Form["policy-name"];
            string policyNote = Request.Form["policy-note"];
            string policyId = Request.Form["policy-id"];

            MtdPolicy mtdPolicy = new MtdPolicy
            {
                Id = policyId,
                Name = policyName,
                Description = policyNote
            };

            await _context.MtdPolicy.AddAsync(mtdPolicy);
            await _context.SaveChangesAsync();

            await _userHandler.CacheRefresh();
            return Ok();
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPolicyEditAsync()
        {
            string policyName = Request.Form["policy-name"];
            string policyNote = Request.Form["policy-note"];
            string policyId = Request.Form["policy-id"];

            MtdPolicy mtdPolicy = await _context.MtdPolicy.FindAsync(policyId);
            if (mtdPolicy == null) { return NotFound(); }

            IList<MtdForm> forms = await _context.MtdForm.Include(x => x.MtdFormPart).ToListAsync();
            IList<MtdGroup> groups = await _context.MtdGroup.ToListAsync();          

            IList<MtdPolicyForms> mtdPolicyForms = await _context.MtdPolicyForms.Where(x => x.MtdPolicy == mtdPolicy.Id).ToListAsync();
            if (mtdPolicyForms == null) { mtdPolicyForms = new List<MtdPolicyForms>(); }

            foreach (var form in forms)
            {
                string formCreate = Request.Form[$"{form.Id}-create"];

                string formView = Request.Form[$"{form.Id}-view"];
                string formViewGroup = Request.Form[$"{form.Id}-view-group"];
                string formViewOwn = Request.Form[$"{form.Id}-view-own"];

                string formEdit = Request.Form[$"{form.Id}-edit"];
                string formEditGroup = Request.Form[$"{form.Id}-edit-group"];
                string formEditOwn = Request.Form[$"{form.Id}-edit-own"];

                string formDelete = Request.Form[$"{form.Id}-delete"];
                string formDeleteGroup = Request.Form[$"{form.Id}-delete-group"];
                string formDeleteOwn = Request.Form[$"{form.Id}-delete-own"];

                string formSetOwner = Request.Form[$"{form.Id}-set-own"];
                string formReviewer = Request.Form[$"{form.Id}-reviewer"];
                string formSetDate = Request.Form[$"{form.Id}-set-date"];

                MtdPolicyForms pf = mtdPolicyForms.Where(x => x.MtdForm == form.Id).FirstOrDefault();
                bool newPf = false;
                if (pf == null)
                {
                    pf = new MtdPolicyForms { MtdPolicy = mtdPolicy.Id, MtdForm = form.Id };
                    newPf = true;
                }

                pf.Create = GetSbyte(formCreate);
                pf.ChangeOwner = GetSbyte(formSetOwner);
                pf.Reviewer = GetSbyte(formReviewer);
                pf.ChangeDate = GetSbyte(formSetDate);

                pf.ViewAll = GetSbyte(formView);
                pf.ViewGroup = GetSbyte(formViewGroup);
                pf.ViewOwn = GetSbyte(formViewOwn);

                pf.EditAll = GetSbyte(formEdit);
                pf.EditGroup = GetSbyte(formEditGroup);
                pf.EditOwn = GetSbyte(formEditOwn);

                pf.DeleteAll = GetSbyte(formDelete);
                pf.DeleteGroup = GetSbyte(formDeleteGroup);
                pf.DeleteOwn = GetSbyte(formDeleteOwn);              

                if (newPf)
                {
                    await _context.MtdPolicyForms.AddAsync(pf);
                }
                else
                {
                    _context.MtdPolicyForms.Update(pf);
                }


                IList<MtdPolicyParts> mtdPolicyParts = await _context.MtdPolicyParts.Where(x => x.MtdPolicy == mtdPolicy.Id).ToListAsync();
                if (mtdPolicyParts == null) { mtdPolicyParts = new List<MtdPolicyParts>(); }

                foreach (var part in form.MtdFormPart)
                {
                    string partCreate = Request.Form[$"{part.Id}-part-create"];
                    string partView = Request.Form[$"{part.Id}-part-view"];
                    string partEdit = Request.Form[$"{part.Id}-part-edit"];

                    MtdPolicyParts pp = mtdPolicyParts.Where(x => x.MtdFormPart == part.Id).FirstOrDefault();
                    bool newPP = false;
                    if (pp == null)
                    {
                        pp = new MtdPolicyParts { MtdPolicy = mtdPolicy.Id, MtdFormPart = part.Id };
                        newPP = true;
                    }

                    pp.Create = GetSbyte(partCreate);
                    pp.View = GetSbyte(partView);
                    pp.Edit = GetSbyte(partEdit);

                    if (newPP) { await _context.MtdPolicyParts.AddAsync(pp); }
                    else { _context.MtdPolicyParts.Update(pp); }

                }

            }


            mtdPolicy.Name = policyName;
            mtdPolicy.Description = policyNote;

            _context.MtdPolicy.Update(mtdPolicy);
            await _context.SaveChangesAsync();

            await _userHandler.CacheRefresh();
            return Ok();
        }

        [HttpPost("all")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPolicyAllAsync()
        {
            string policyId = Request.Form["policy-id"];
            MtdPolicy mtdPolicy = await _context.MtdPolicy.FindAsync(policyId);
            if (mtdPolicy == null) { return NotFound(); }

            IList<MtdForm> forms = await _context.MtdForm.Include(x => x.MtdFormPart).ToListAsync();
            IList<MtdGroup> groups = await _context.MtdGroup.ToListAsync();
           
            IList<MtdPolicyForms> mtdPolicyForms = await _context.MtdPolicyForms.Where(x => x.MtdPolicy == mtdPolicy.Id).ToListAsync();
            if (mtdPolicyForms == null) { mtdPolicyForms = new List<MtdPolicyForms>(); }

            foreach (var form in forms)
            {
                MtdPolicyForms pf = mtdPolicyForms.Where(x => x.MtdForm == form.Id).FirstOrDefault();
                bool newPf = false;
                if (pf == null)
                {
                    pf = new MtdPolicyForms { MtdPolicy = mtdPolicy.Id, MtdForm = form.Id };
                    newPf = true;
                }

                pf.Create = 1;
                pf.ChangeOwner = 1;
                pf.Reviewer = 1;
                pf.ChangeDate = 1;

                pf.ViewAll = 1;
                pf.ViewGroup = 0;
                pf.ViewOwn = 0;

                pf.EditAll = 1;
                pf.EditGroup = 0;
                pf.EditOwn = 0;

                pf.DeleteAll = 1;
                pf.DeleteGroup = 0;
                pf.DeleteOwn = 0;

                if (newPf)
                {
                    await _context.MtdPolicyForms.AddAsync(pf);
                }
                else
                {
                    _context.MtdPolicyForms.Update(pf);
                }


                IList<MtdPolicyParts> mtdPolicyParts = await _context.MtdPolicyParts.Where(x => x.MtdPolicy == mtdPolicy.Id).ToListAsync();
                if (mtdPolicyParts == null) { mtdPolicyParts = new List<MtdPolicyParts>(); }

                foreach (var part in form.MtdFormPart)
                {
                    string partCreate = Request.Form[$"{part.Id}-part-create"];
                    string partView = Request.Form[$"{part.Id}-part-view"];
                    string partEdit = Request.Form[$"{part.Id}-part-edit"];

                    MtdPolicyParts pp = mtdPolicyParts.Where(x => x.MtdFormPart == part.Id).FirstOrDefault();
                    bool newPP = false;
                    if (pp == null)
                    {
                        pp = new MtdPolicyParts { MtdPolicy = mtdPolicy.Id, MtdFormPart = part.Id };
                        newPP = true;
                    }

                    pp.Create = 1;
                    pp.View = 1;
                    pp.Edit = 1;

                    if (newPP) { await _context.MtdPolicyParts.AddAsync(pp); }
                    else { _context.MtdPolicyParts.Update(pp); }

                }

            }

            await _context.SaveChangesAsync();
            await _userHandler.CacheRefresh();
            return Ok();
        }

        [HttpPost("clear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPolicyClearAsync()
        {
            string policyId = Request.Form["policy-id"];
            MtdPolicy mtdPolicy = await _context.MtdPolicy.FindAsync(policyId);
            if (mtdPolicy == null) { return NotFound(); }

            IList<MtdForm> forms = await _context.MtdForm.Include(x => x.MtdFormPart).ToListAsync();
            IList<MtdGroup> groups = await _context.MtdGroup.ToListAsync();
            
            IList<MtdPolicyForms> mtdPolicyForms = await _context.MtdPolicyForms.Where(x => x.MtdPolicy == mtdPolicy.Id).ToListAsync();
            if (mtdPolicyForms == null) { mtdPolicyForms = new List<MtdPolicyForms>(); }

            foreach (var form in forms)
            {
                MtdPolicyForms pf = mtdPolicyForms.Where(x => x.MtdForm == form.Id).FirstOrDefault();
                bool newPf = false;
                if (pf == null)
                {
                    pf = new MtdPolicyForms { MtdPolicy = mtdPolicy.Id, MtdForm = form.Id };
                    newPf = true;
                }

                pf.Create = 0;
                pf.ChangeOwner = 0;
                pf.Reviewer = 0;
                pf.ChangeDate = 0;

                pf.ViewAll = 0;
                pf.ViewGroup = 0;
                pf.ViewOwn = 0;

                pf.EditAll = 0;
                pf.EditGroup = 0;
                pf.EditOwn = 0;

                pf.DeleteAll = 0;
                pf.DeleteGroup = 0;
                pf.DeleteOwn = 0;

                if (newPf)
                {
                    await _context.MtdPolicyForms.AddAsync(pf);
                }
                else
                {
                    _context.MtdPolicyForms.Update(pf);
                }


                IList<MtdPolicyParts> mtdPolicyParts = await _context.MtdPolicyParts.Where(x => x.MtdPolicy == mtdPolicy.Id).ToListAsync();
                if (mtdPolicyParts == null) { mtdPolicyParts = new List<MtdPolicyParts>(); }

                foreach (var part in form.MtdFormPart)
                {
                    string partCreate = Request.Form[$"{part.Id}-part-create"];
                    string partView = Request.Form[$"{part.Id}-part-view"];
                    string partEdit = Request.Form[$"{part.Id}-part-edit"];

                    MtdPolicyParts pp = mtdPolicyParts.Where(x => x.MtdFormPart == part.Id).FirstOrDefault();
                    bool newPP = false;
                    if (pp == null)
                    {
                        pp = new MtdPolicyParts { MtdPolicy = mtdPolicy.Id, MtdFormPart = part.Id };
                        newPP = true;
                    }

                    pp.Create = 0;
                    pp.View = 0;
                    pp.Edit = 0;

                    if (newPP) { await _context.MtdPolicyParts.AddAsync(pp); }
                    else { _context.MtdPolicyParts.Update(pp); }

                }

            }

            await _context.SaveChangesAsync();
            await _userHandler.CacheRefresh();
            return Ok();
        }

        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPolicyDelete() {

            string policyId = Request.Form["policy-delete-id"];
            MtdPolicy mtdPolicy = new MtdPolicy { Id = policyId };
            _context.MtdPolicy.Remove(mtdPolicy);            
            await _context.SaveChangesAsync();

            await _userHandler.CacheRefresh();

            return Ok();
        }

        private sbyte GetSbyte(string value)
        {
            if (value == null) return 0;
            return value == "true" ? (sbyte)1 : (sbyte)0;
        }
    }
}
