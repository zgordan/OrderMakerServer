/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.EntityHandler.Filter
{
    public partial class FilterHandler
    {
        private readonly OrderMakerContext _context;
        private MtdFilter register;
        private readonly WebAppUser _user;
        private IQueryable<MtdStore> queryMtdStore;
        private readonly UserHandler _userHandler;
        private IList<string> allowFieldIds;

        public string FormId { get; private set; }


        public FilterHandler(OrderMakerContext orderMakerContext, string formId, WebAppUser user, UserHandler userHandler)
        {
            _context = orderMakerContext;
            _user = user;
            _userHandler = userHandler;
            FormId = formId;
            queryMtdStore = _context.MtdStore;
        }

        private async Task<IList<string>> GetAllowFieldsAsync()
        {
            if (allowFieldIds != null) { return allowFieldIds; }

            IList<MtdFormPart> parts = await _userHandler.GetAllowPartsForView(_user, FormId);
            List<string> partIds = parts.Select(x => x.Id).ToList();
            IList<string> allowFiieldIds = await _context.MtdFormPartField.Where(x => partIds.Contains(x.MtdFormPart)).Select(x => x.Id).ToListAsync();
            allowFieldIds = allowFiieldIds;

            return allowFieldIds;
        }

        private async Task<MtdFilter> GetFilterAsync()
        {

            if (register == null)
            {
                register = await _context.MtdFilter
                    .Include(x => x.MtdFilterOwner)
                    .Include(x => x.MtdFilterDate)
                    .Include(m => m.MtdFilterColumn)
                    .Include(m => m.MtdFilterRelated)
                    .FirstOrDefaultAsync(x => x.IdUser == _user.Id && x.MtdForm == FormId);

                if (register != null && register.MtdFilterDate != null)
                {
                    queryMtdStore = queryMtdStore
                        .Where(x => x.Timecr.Date >= register.MtdFilterDate.DateStart.Date && x.Timecr.Date <= register.MtdFilterDate.DateEnd.Date);
                }

                if (register != null && register.MtdFilterOwner != null)
                {
                    queryMtdStore = queryMtdStore
                        .Where(x => x.MtdStoreOwner.UserId == register.MtdFilterOwner.OwnerId);
                }

                if (register != null && register.MtdFilterRelated != null)
                {
                    string parentId = await _context.MtdStore
                        .Where(x => x.Sequence == register.MtdFilterRelated.DocBasedNumber && x.MtdForm == register.MtdFilterRelated.FormId)
                        .Select(x => x.Id)
                        .FirstOrDefaultAsync() ?? string.Empty;

                    queryMtdStore = queryMtdStore.Where(x => x.Parent == parentId);

                }

            }
            return register;
        }

        public async Task<Incomer> GetIncomerDataAsync()
        {
            MtdFilter mtdFilter = await GetFilterAsync();

            Incomer ps = new Incomer
            {
                FormId = FormId,
                SearchNumber = "",
                SearchText = "",
                Page = 1,
                PageSize = 10,
                FieldForColumn = await GetFieldsFilterAsync(),
                WaitList = 0,
            };

            if (mtdFilter != null)
            {
                ps.FormId = FormId;
                ps.SearchText = mtdFilter.SearchText;
                ps.SearchNumber = mtdFilter.SearchNumber;
                ps.PageSize = mtdFilter.PageSize;
                ps.Page = mtdFilter.Page;
                ps.FieldForFilter = await GetAdvancedAsync();
                ps.WaitList = mtdFilter.WaitList;
            }

            return ps;

        }

        public async Task<TypeQuery> GetTypeQueryAsync(WebAppUser user)
        {

            TypeQuery typeQuery = TypeQuery.empty;
            bool filterField = false;

            MtdFilter mtdFilter = await GetFilterAsync();
            if (mtdFilter != null)
            {
                filterField = await _context.MtdFilterField.Where(x => x.MtdFilter == mtdFilter.Id).AnyAsync();
                if (mtdFilter.SearchNumber != "") { typeQuery = TypeQuery.number; }
                if (mtdFilter.SearchNumber == "" && mtdFilter.SearchText != "" && !filterField) { typeQuery = TypeQuery.text; }
                if (mtdFilter.SearchNumber == "" && mtdFilter.SearchText == "" && filterField) { typeQuery = TypeQuery.field; }
                if (mtdFilter.SearchNumber == "" && mtdFilter.SearchText != "" && filterField) { typeQuery = TypeQuery.textField; }


                var scripts = await _userHandler.GetFilterScriptsAsync(user, FormId, 1);
                bool isScript = scripts.Any();
                if (isScript) { typeQuery = TypeQuery.script; }
            }

            return typeQuery;
        }

        public async Task<IList<MtdFormPartField>> GetFieldsFilterAsync()
        {

            IList<MtdFormPartField> result;

            MtdFilter mtdFilter = await GetFilterAsync();

            if (mtdFilter != null && mtdFilter.MtdFilterColumn != null && mtdFilter.MtdFilterColumn.Count > 0)
            {
                List<string> fIds = mtdFilter.MtdFilterColumn.Select(x => x.MtdFormPartField).ToList();
                IList<MtdFormPartField> tempFields = await _context.MtdFormPartField.Where(x => fIds.Contains(x.Id)).ToListAsync();

                result = (from s in tempFields
                          join sa in mtdFilter.MtdFilterColumn on s.Id equals sa.MtdFormPartField
                          orderby sa.Sequence
                          select s).ToList();
            }
            else
            {
                result = await _context.MtdFormPartField
                    .Include(m => m.MtdFormPartNavigation)
                    .Where(x => x.MtdFormPartNavigation.MtdForm == FormId & x.MtdSysType == 1)
                    .OrderBy(s => s.MtdFormPartNavigation.Sequence).ThenBy(s => s.Sequence)
                    .Take(3)
                    .ToListAsync();
            }

            if (result != null)
            {
                IList<string> fieldIds = await GetAllowFieldsAsync();
                result = result.Where(x => fieldIds.Contains(x.Id)).ToList();
            }


            return result;
        }

        public async Task<IList<MtdFormPartField>> GetFieldsAllAsync()
        {

            IList<MtdFormPartField> result = await _context.MtdFormPartField
                .Include(m => m.MtdFormPartNavigation)
                .Where(x => x.MtdFormPartNavigation.MtdForm == FormId)
                .OrderBy(s => s.MtdFormPartNavigation.Sequence).ThenBy(s => s.Sequence)
                .ToListAsync();


            if (result != null)
            {
                IList<string> fieldIds = await GetAllowFieldsAsync();
                result = result.Where(x => fieldIds.Contains(x.Id)).ToList();
            }


            return result;
        }

        public async Task<IList<MtdFilterField>> GetAdvancedAsync()
        {
            IList<MtdFilterField> result = null;

            MtdFilter mtdFilter = await GetFilterAsync();

            if (mtdFilter != null)
            {
                result = await _context.MtdFilterField.Include(m => m.MtdFormPartFieldNavigation).Where(x => x.MtdFilter == mtdFilter.Id).ToListAsync();
            }

            return result;
        }

        public async Task<bool> IsShowDate()
        {
            MtdFilter mtdFilter = await GetFilterAsync();
            return mtdFilter.ShowDate == 1;
        }

        public async Task<bool> IsShowNumber()
        {
            MtdFilter mtdFilter = await GetFilterAsync();
            return mtdFilter.ShowNumber == 1;
        }

        public async Task<bool> RemoveFilterScriptAppliedAsync()
        {
            MtdFilter mtdFilter = await GetFilterAsync();
            MtdFilterScriptApply filterApplied = await _context.MtdFilterScriptApply.Where(x => x.MtdFilterId == mtdFilter.Id).FirstOrDefaultAsync();
            if (filterApplied != null)
            {
                try
                {
                    _context.MtdFilterScriptApply.Remove(filterApplied);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    return false;
                }

            }
            return true;
        }

        public async Task<bool> FilterScriptApplyAsync(int scriptId)
        {
            MtdFilter mtdFilter = await GetFilterAsync();
            MtdFilterScriptApply filterApplied = new MtdFilterScriptApply { Id = Guid.NewGuid().ToString(), MtdFilterId = mtdFilter.Id, MtdFilterScriptId = scriptId };
            try
            {
                await _context.MtdFilterScriptApply.AddAsync(filterApplied);
                await _context.SaveChangesAsync();

            }
            catch
            {
                return false;
            }

            return true;
        }


    }
}
