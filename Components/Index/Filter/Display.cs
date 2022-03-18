/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Index;
using Mtd.OrderMaker.Server.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Components.Index.Filter
{
    [ViewComponent(Name = "IndexFilterDisplay")]
    public class Display : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;
        private readonly IStringLocalizer<SharedResource> localizer;

        public Display(OrderMakerContext orderMakerContext, UserHandler userHandler, IStringLocalizer<SharedResource> localizer)
        {
            _context = orderMakerContext;
            _userHandler = userHandler;
            this.localizer = localizer;
        }

        public async Task<IViewComponentResult> InvokeAsync(string formId)
        {
            List<DisplayData> displayDatas = new List<DisplayData>();
            var user = await _userHandler.GetUserAsync(HttpContext.User);

            List<MtdFormPart> parts = await _userHandler.GetAllowPartsForView(user, formId);
            List<string> partIds = parts.Select(x => x.Id).ToList();

            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == formId);
            if (filter != null)
            {

                if (filter.WaitList == 1)
                {
                    DisplayModelView display = new DisplayModelView
                    {
                        FormId = formId,
                        IdFilter = filter == null ? -1 : filter.Id,
                        DisplayDatas = displayDatas
                    };

                    return View("Default", display);
                }

                List<MtdFilterField> mtdFilterFields = await _context.MtdFilterField
                    .Include(x => x.MtdTermNavigation)
                    .Include(m => m.MtdFormPartFieldNavigation)
                    .Where(x => x.MtdFilter == filter.Id)
                    .ToListAsync();

                foreach (var field in mtdFilterFields)
                {
                    DisplayData displayData = new DisplayData
                    {
                        Id = field.Id,
                        Header = $"{field.MtdFormPartFieldNavigation.Name} ({field.MtdTermNavigation.Sign})",
                        Value = "",
                        Type = "-field"
                    };


                    if (field.MtdFormPartFieldNavigation.MtdSysType != 11)
                    {
                        displayData.Value = field.Value;
                        if (field.MtdFormPartFieldNavigation.MtdSysType == 12)
                        {
                            displayData.Value = field.Value.Equals("1") ? localizer["ON"] : localizer["OFF"];
                        }

                        if (field.MtdFormPartFieldNavigation.MtdSysType == 5 || field.MtdFormPartFieldNavigation.MtdSysType == 6)
                        {
                            displayData.Header = field.MtdFormPartFieldNavigation.Name;
                            displayData.Value = field.Value.Replace("***","-");
                        }

                    }
                    else
                    {
                        MtdStore mtdStore = await _context.MtdStore.FirstOrDefaultAsync(x => x.Id == field.Value);
                        if (mtdStore != null)
                        {
                            var fieldForList = await _context.MtdFormPartField.Include(m => m.MtdFormPartNavigation)
                                .Where(x => x.MtdFormPartNavigation.MtdForm == mtdStore.MtdForm & x.MtdSysType == 1)
                                .OrderBy(o => o.MtdFormPartNavigation.Sequence).ThenBy(o => o.Sequence).FirstOrDefaultAsync();
                            if (fieldForList != null)
                            {
                                IList<long> ids = await _context.MtdStoreStack.Where(x => x.MtdStore == mtdStore.Id & x.MtdFormPartField == fieldForList.Id).Select(x => x.Id).ToListAsync();
                                MtdStoreStackText data = await _context.MtdStoreStackText.FirstOrDefaultAsync(x => ids.Contains(x.Id));
                                displayData.Value = data.Register;
                            }

                        }

                    }

                    displayDatas.Add(displayData);
                }

                MtdFilterDate mtdFilterDate = await _context.MtdFilterDate.FindAsync(filter.Id);
                if (mtdFilterDate != null)
                {
                    DisplayData displayDate = new DisplayData()
                    {
                        Id = filter.Id,
                        Header = localizer["Period"],
                        Value = $"{mtdFilterDate.DateStart.ToShortDateString()} {mtdFilterDate.DateEnd.ToShortDateString()}",
                        Type = "-date"
                    };
                    displayDatas.Add(displayDate);
                }

                MtdFilterOwner mtdFilterOwner = await _context.MtdFilterOwner.FindAsync(filter.Id);
                if (mtdFilterOwner != null)
                {
                    WebAppUser userOwner = await _userHandler.FindByIdAsync(mtdFilterOwner.OwnerId);
                    DisplayData displayDate = new DisplayData()
                    {
                        Id = filter.Id,
                        Header = localizer["Owner"],
                        Value = $"{userOwner.Title}",
                        Type = "-owner"
                    };
                    displayDatas.Add(displayDate);
                }

                IList<MtdFilterScript> scripts = await _userHandler.GetFilterScriptsAsync(user, formId, 1);
                if (scripts != null && scripts.Count > 0)
                {
                    foreach (var fs in scripts)
                    {
                        DisplayData displayDate = new DisplayData()
                        {
                            Id = fs.Id,
                            Header = localizer["Advanced filter"],
                            Value = fs.Name,
                            Type = "-script"
                        };
                        displayDatas.Add(displayDate);
                    }

                }


                MtdFilterRelated mtdFilterRelated = await _context.MtdFilterRelated.FindAsync(filter.Id);
                if (mtdFilterRelated != null)
                {
                    string formName = await _context.MtdForm.Where(x => x.Id == mtdFilterRelated.FormId).Select(x => x.Name).FirstOrDefaultAsync() ?? "Not Found";
                    DisplayData displayDate = new DisplayData()
                    {
                        Id = filter.Id,
                        Header = localizer["Document-based"],
                        Value = $"{formName} {localizer["No."]} {mtdFilterRelated.DocBasedNumber}",
                        Type = "-related"
                    };
                    displayDatas.Add(displayDate);
                }
            }

            DisplayModelView displayModelView = new DisplayModelView
            {
                FormId = formId,
                IdFilter = filter == null ? -1 : filter.Id,
                DisplayDatas = displayDatas
            };

            return View("Default", displayModelView);
        }
    }
}
