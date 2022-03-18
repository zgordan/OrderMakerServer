/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler;
using Mtd.OrderMaker.Server.EntityHandler.Approval;
using Mtd.OrderMaker.Server.Extensions;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Controllers.Store
{
    [Route("api/store")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public partial class DataController : ControllerBase
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly IEmailSenderBlank emailSender;

        private enum TypeAction { Create, Edit };

        public DataController(OrderMakerContext context, UserHandler userHandler, IStringLocalizer<SharedResource> localizer, IEmailSenderBlank emailSender)
        {
            _context = context;
            _userHandler = userHandler;
            this.localizer = localizer;
            this.emailSender = emailSender;
        }

        // POST: api/store/save
        [HttpPost("save")]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> OnPostSaveAsync()
        {
            string Id = Request.Form["idStore"];
            string dateCreate = Request.Form["date-create"];
            string storeParentId = Request.Form["store-parent"];

            if (storeParentId == string.Empty || storeParentId.Length > 36) { storeParentId = null; }

            MtdStore mtdStore = await _context.MtdStore.FirstOrDefaultAsync(x => x.Id == Id);
            if (mtdStore == null)
            {
                return NotFound();
            }

            WebAppUser webAppUser = await _userHandler.GetUserAsync(HttpContext.User);
            bool isEditor = await _userHandler.IsEditor(webAppUser, mtdStore.MtdForm, mtdStore.Id);
            ApprovalHandler approvalHandler = new ApprovalHandler(_context, mtdStore.Id);
            ApprovalStatus approvalStatus = await approvalHandler.GetStatusAsync(webAppUser);

            if (!isEditor || approvalStatus == ApprovalStatus.Rejected || approvalStatus == ApprovalStatus.Waiting)
            {
                return Ok(403);
            }

            bool setDate = await _userHandler.CheckUserPolicyAsync(webAppUser, mtdStore.MtdForm, RightsType.SetDate);
            if (setDate)
            {
                bool isOk = DateTime.TryParse(dateCreate, out DateTime dateTime);
                if (isOk)
                {
                    mtdStore.Timecr = dateTime.Add(DateTime.Now.TimeOfDay);
                    _context.MtdStore.Update(mtdStore);
                }
            }

            /*Circular link check*/
            if (storeParentId != null)
            {
                MtdStore storeParent = await _context.MtdStore.FindAsync(storeParentId);
                if (storeParent.Parent == mtdStore.Id)
                {
                    return BadRequest(localizer["Cyclic link! This document cannot be selected."]);
                }

                if (storeParentId == mtdStore.Id)
                {
                    return BadRequest(localizer["Cyclic link! A document cannot be a basis for itself."]);
                }
            }

            bool isRelatedEditor = await _userHandler.CheckUserPolicyAsync(webAppUser, mtdStore.MtdForm, RightsType.RelatedEdit);

            if (isRelatedEditor)
            {
                mtdStore.Parent = storeParentId;
            }

            MtdLogDocument mtdLog = new MtdLogDocument
            {
                MtdStore = mtdStore.Id,
                TimeCh = DateTime.Now,
                UserId = webAppUser.Id,
                UserName = webAppUser.GetFullName()
            };


            OutData outData = await CreateDataAsync(Id, webAppUser, TypeAction.Edit);

            if (!outData.CheckRegister)
            {

                return BadRequest(new JsonResult($"{localizer["Limit for Register:"]} {outData.CheckRegisterInfo}"));
            }

            List<MtdStoreStack> stackNew = outData.MtdStoreStacks;

            IList<MtdStoreStack> stackOld = await _context.MtdStoreStack
                .Include(m => m.MtdStoreStackText)
                .Include(m => m.MtdStoreStackDecimal)
                .Include(m => m.MtdStoreStackFile)
                .Include(m => m.MtdStoreStackDate)
                .Include(m => m.MtdStoreStackInt)
                .Include(m => m.MtdStoreLink)
                .Where(x => x.MtdStore == Id).ToListAsync();

            foreach (MtdStoreStack stack in stackOld)
            {
                MtdStoreStack stackForField = stackNew.SingleOrDefault(x => x.MtdFormPartField == stack.MtdFormPartField);
                if (stackForField != null)
                {
                    stack.MtdStoreStackText = stackForField.MtdStoreStackText;
                    stack.MtdStoreLink = stackForField.MtdStoreLink;
                    stack.MtdStoreStackDate = stackForField.MtdStoreStackDate;
                    stack.MtdStoreStackDecimal = stackForField.MtdStoreStackDecimal;
                    stack.MtdStoreStackFile = stackForField.MtdStoreStackFile;
                    stack.MtdStoreStackInt = stackForField.MtdStoreStackInt;
                }
            }

            try
            {
                if (stackNew.Count > 0)
                {
                    int count = await _context.MtdStoreLink.Where(x => x.MtdStore == Id).CountAsync();
                    if (count > 0)
                    {
                        string titleText = outData.MtdStoreStacks.FirstOrDefault(x => x.MtdFormPartField == outData.MtdFormPartField.Id).MtdStoreStackText.Register;
                        IList<MtdStoreLink> linkIds = await _context.MtdStoreLink.Where(x => x.MtdStore == Id).Select(x => new MtdStoreLink { Id = x.Id, MtdStore = x.MtdStore, Register = titleText }).ToListAsync();
                        _context.MtdStoreLink.UpdateRange(linkIds);
                    }

                    _context.MtdStoreStack.UpdateRange(stackOld);

                    List<MtdStoreStack> stackNewOnly = stackNew.Where(x => !stackOld.Select(f => f.MtdFormPartField).Contains(x.MtdFormPartField)).ToList();

                    if (stackNewOnly.Any())
                    {
                        await _context.MtdStoreStack.AddRangeAsync(stackNewOnly);
                    }

                    _context.MtdLogDocument.Add(mtdLog);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                if (!MtdStoreExists(Id))
                {
                    return NotFound();
                }
            }

            return Ok();

        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]        
        public async Task<IActionResult> OnPostCreateAsync()
        {
            var form = await Request.ReadFormAsync();
            string formId = Request.Form["form-id"];
            string dateCreate = Request.Form["date-create"];
            string idStore = Request.Form["store-id"];
            string storeParentId = Request.Form["store-parent"];

            if (storeParentId == string.Empty || storeParentId.Length > 36) { storeParentId = null; }

            WebAppUser webAppUser = await _userHandler.GetUserAsync(HttpContext.User);
            bool isCreator = await _userHandler.IsCreator(webAppUser, formId);
            if (!isCreator)
            {
                return Ok(403);
            }

            await _context.Database.BeginTransactionAsync();
            //int sequence = 0;
            int? sequence = await _context.MtdStore.Where(x => x.MtdForm == formId).MaxAsync(x => (int?)x.Sequence) ?? 0;
            sequence++;


            /*Circular link check*/
            if (storeParentId != null)
            {
                MtdStore storeParent = await _context.MtdStore.FindAsync(storeParentId);
                if (storeParent.Parent == idStore)
                {
                    return BadRequest(localizer["Cyclic link! This document cannot be selected."]);
                }
            }

            bool isRelatedCreator = await _userHandler.CheckUserPolicyAsync(webAppUser, formId, RightsType.RelatedCreate);
            if (!isRelatedCreator) { storeParentId = null; }

            MtdStore mtdStore = new MtdStore { Id = idStore, MtdForm = formId, Sequence = sequence ?? 1, Parent = storeParentId };


            bool setData = await _userHandler.CheckUserPolicyAsync(webAppUser, mtdStore.MtdForm, RightsType.SetDate);
            if (setData)
            {
                bool isOk = DateTime.TryParse(dateCreate, out DateTime dateTime);
                if (isOk)
                {
                    mtdStore.Timecr = dateTime.Add(DateTime.Now.TimeOfDay);
                }
                else
                {
                    mtdStore.Timecr = DateTime.Now;
                }
            }
            else
            {
                mtdStore.Timecr = DateTime.Now;
            }

            await _context.MtdStore.AddAsync(mtdStore);
            await _context.SaveChangesAsync();

            MtdLogDocument mtdLog = new MtdLogDocument
            {
                MtdStore = mtdStore.Id,
                TimeCh = DateTime.Now,
                UserId = webAppUser.Id,
                UserName = webAppUser.GetFullName()
            };

            mtdStore.MtdStoreOwner = new MtdStoreOwner
            {
                UserId = webAppUser.Id,
                UserName = webAppUser.GetFullName(),
            };


            await _context.MtdLogDocument.AddAsync(mtdLog);

            OutData outParam = await CreateDataAsync(mtdStore.Id, webAppUser, TypeAction.Create);

            if (!outParam.CheckRegister)
            {
                return BadRequest(new JsonResult($"{localizer["Limit for Register:"]} {outParam.CheckRegisterInfo}"));
            }

            List<MtdStoreStack> stackNew = outParam.MtdStoreStacks;
            await _context.MtdStoreStack.AddRangeAsync(stackNew);         

            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();

            return Ok();
        }

        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            string idStore = Request.Form["store-delete-id"];
            MtdStore mtdStore = await _context.MtdStore.FindAsync(idStore);

            if (mtdStore == null)
            {
                return NotFound();
            }

            WebAppUser webAppUser = await _userHandler.GetUserAsync(HttpContext.User);
            bool isEraser = await _userHandler.IsEraser(webAppUser, mtdStore.MtdForm, mtdStore.Id);

            if (!isEraser)
            {
                return Ok(403);
            }

            _context.MtdStore.Remove(mtdStore);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("number/id")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostGetIDAsync()
        {
            string result = "";
            string partId = Request.Form["idFromParent"];
            string parentNumber = Request.Form["store-parent-number"];

            if (parentNumber.Length == 0) { return Ok(result); }

            parentNumber = parentNumber.TrimStart(new char[] { '0' });
            bool isOk = int.TryParse(parentNumber, out int num);

            if (!isOk) { return Ok(result); }

            MtdStore mtdStore = await _context.MtdStore.FirstOrDefaultAsync(x => x.MtdForm == partId & x.Sequence == num);

            if (mtdStore != null)
            {
                result = mtdStore.Id;
            }

            return Ok(result);
        }

        [HttpPost("setowner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostSetOwnerAsync()
        {
            string idStore = Request.Form["setowner-id-store"];
            string idUser = Request.Form["setowner-id-user"];

            MtdStore mtdStore = await _context.MtdStore.FindAsync(idStore);
            if (mtdStore == null) { return Ok(403); }

            WebAppUser webAppUser = await _userHandler.FindByIdAsync(idUser);
            if (webAppUser == null) { return Ok(403); }

            WebAppUser currentUser = await _userHandler.GetUserAsync(HttpContext.User);
            bool isInstallerOwner = await _userHandler.IsInstallerOwner(currentUser, mtdStore.MtdForm);

            if (!isInstallerOwner) { return Ok(403); }

            List<WebAppUser> webAppUsers = new List<WebAppUser>();
            bool isViewAll = await _userHandler.CheckUserPolicyAsync(currentUser, mtdStore.MtdForm, RightsType.ViewAll);
            if (isViewAll)
            {
                webAppUsers = await _userHandler.Users.ToListAsync();
            }
            else
            {
                webAppUsers = await _userHandler.GetUsersInGroupsAsync(currentUser);
            }

            if (!webAppUsers.Where(x => x.Id == idUser).Any()) { return Ok(403); }

            MtdStoreOwner mtdStoreOwner = await _context.MtdStoreOwner.Include(x => x.IdNavigation).FirstOrDefaultAsync(x => x.Id == idStore);

            if (mtdStoreOwner == null)
            {

                string formId = mtdStoreOwner.IdNavigation.MtdForm;
                mtdStoreOwner = new MtdStoreOwner
                {
                    Id = idStore,
                    UserId = webAppUser.Id,
                    UserName = webAppUser.Title
                };

                /**Update all fields for SysTrigger UserGroup 33E8212E-059B-482D-8CBD-DFDB073E3B63**/
                await UpdateTriggerUserGroup(idStore, webAppUser, formId);

                await _context.MtdStoreOwner.AddAsync(mtdStoreOwner);
                await _context.SaveChangesAsync();

                return Ok();

            }

            /**Update all fields for SysTrigger UserGroup 33E8212E-059B-482D-8CBD-DFDB073E3B63**/
            await UpdateTriggerUserGroup(idStore, webAppUser, mtdStore.MtdForm);

            mtdStoreOwner.UserId = webAppUser.Id;
            mtdStoreOwner.UserName = webAppUser.Title;
            _context.Entry(mtdStoreOwner).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task UpdateTriggerUserGroup(string idStore, WebAppUser webAppUser, string formId)
        {
            IList<string> partsIds = await _context.MtdFormPart.Where(x => x.MtdForm == formId).Select(x => x.Id).ToListAsync();
            IList<string> fieldIds = await _context.MtdFormPartField
                .Where(x => x.MtdSysTrigger == "33E8212E-059B-482D-8CBD-DFDB073E3B63" && partsIds.Contains(x.MtdFormPart))
                .Select(x => x.Id)
                .ToListAsync();

            if (fieldIds != null)
            {
                IList<long> stackIds = await _context.MtdStoreStack.Where(x => fieldIds.Contains(x.MtdFormPartField) && x.MtdStore == idStore)
                    .Select(x => x.Id).ToListAsync();

                IList<MtdStoreStackText> newTriggerResult = await _context.MtdStoreStackText.Where(x => stackIds.Contains(x.Id))
                    .Select(x => new MtdStoreStackText { Id = x.Id, Register = webAppUser.TitleGroup }).ToListAsync();

                _context.MtdStoreStackText.UpdateRange(newTriggerResult);
            }
        }

        private async Task<OutData> CreateDataAsync(string store_id, WebAppUser user, TypeAction typeAction)
        {

            var store = await _context.MtdStore
               .Include(m => m.MtdFormNavigation)
               .ThenInclude(p => p.MtdFormPart)
               .FirstOrDefaultAsync(m => m.Id == store_id);

            List<string> partsIds = new List<string>();
            bool isReviewer = await _userHandler.IsReviewer(user, store.MtdForm);
            ApprovalHandler approvalHandler = new ApprovalHandler(_context, store.Id);

            bool isApproval = await approvalHandler.IsApprovalFormAsync();
            if (isApproval)
            {
                bool isExists = await _context.MtdStoreApproval.Where(x => x.Id == store.Id).AnyAsync();
                if (!isExists)
                {
                    var firstStage = await approvalHandler.GetFirstStageAsync();
                    await _context.MtdStoreApproval.AddAsync(new MtdStoreApproval { Id = store.Id, MtdApproveStage = firstStage.Id, PartsApproved = "&", Complete = 0, Result = 0 });
                }
            }

            List<string> blockedParts = new List<string>();
            if (!isReviewer)
            {
                blockedParts = await approvalHandler.GetBlockedPartsIds();
            }

            string userTitleGroup = user.TitleGroup;
            if (typeAction == TypeAction.Edit)
            {
                WebAppUser userOwner = await _userHandler.GetOwnerAsync(store.Id);
                userTitleGroup = userOwner.TitleGroup;
            }

            DataGenerator dataGenerator = new DataGenerator(_context, user.Title, userTitleGroup);

            foreach (var part in store.MtdFormNavigation.MtdFormPart)
            {
                switch (typeAction)
                {
                    case TypeAction.Create:
                        {
                            if (await _userHandler.IsCreatorPartAsync(user, part.Id))
                            {
                                partsIds.Add(part.Id);
                            }
                            break;
                        }
                    default:
                        {
                            if (await _userHandler.IsEditorPartAsync(user, part.Id) && !blockedParts.Contains(part.Id))
                            {
                                partsIds.Add(part.Id);
                            }
                            break;
                        }
                }
            }

            var fields = await _context.MtdFormPartField.Include(m => m.MtdFormPartNavigation)
                .Where(x => partsIds.Contains(x.MtdFormPartNavigation.Id))
                .OrderBy(x => x.MtdFormPartNavigation.Sequence)
                .ThenBy(x => x.Sequence)
                .ToListAsync();

            var titleField = fields.FirstOrDefault(x => x.MtdSysType == 1);

            List<MtdStoreStack> stackNew = new List<MtdStoreStack>();

            foreach (MtdFormPartField field in fields)
            {

                if (field.ReadOnly == 1 && field.MtdSysTrigger.Equals("9C85B07F-9236-4314-A29E-87B20093CF82")) continue;

                StackParams stackParams = new StackParams
                {
                    StroreID = store_id,
                    Data = Request.Form[field.Id],
                    Field = field,
                    ActionDelete = Request.Form[$"{field.Id}-delete"],
                    DataLink = Request.Form[$"{field.Id}-datalink"],
                    File = Request.Form.Files.FirstOrDefault(x => x.Name == field.Id)
                };

                MtdStoreStack mtdStoreStack = await dataGenerator.CreateStoreStack(stackParams);

                stackNew.Add(mtdStoreStack);
            }

            OutData outParam = new OutData()
            {
                MtdFormPartField = titleField,
                MtdStoreStacks = stackNew,
                CheckRegister = true
            };


            /*Check registers*/
            List<string> fieldIds = outParam.MtdStoreStacks.Select(x => x.MtdFormPartField).ToList();
            IList<string> registerIds = await _context.MtdRegister.Where(x => x.ParentLimit == 1).Select(x => x.Id).ToListAsync();
            
            if (registerIds != null && store.Parent != null)
            {

                List<MtdRegisterField> registerFields = await _context.MtdRegisterField.Include(c=>c.MtdRegister)
                    .Where(x => fieldIds.Contains(x.Id) && registerIds.Contains(x.MtdRegisterId) && x.Expense == 1 && x.Income == 0)
                    .ToListAsync();

                if (registerFields != null)
                {
                    FormHandler registerHandler = new FormHandler(_context);
                    foreach(MtdRegister register in registerFields.Select(x => x.MtdRegister))
                    {
                        MtdStore parentStore = await registerHandler.GetParentStoreAsync(store.Id);
                        decimal balance =  await registerHandler.GetRegisterBalanceAsync(register, store.Id, parentStore.Id);
                        List<string> fieldIdsForRegister = registerFields.Where(x => x.MtdRegisterId == register.Id).Select(x => x.Id).ToList();
                        decimal sumInt = outParam.MtdStoreStacks.Where(x => fieldIdsForRegister.Contains(x.MtdFormPartField) && x.MtdStoreStackInt != null).Sum(x => x.MtdStoreStackInt.Register);
                        decimal sumDecimal = outParam.MtdStoreStacks.Where(x => fieldIdsForRegister.Contains(x.MtdFormPartField) && x.MtdStoreStackDecimal != null).Sum(x => x.MtdStoreStackDecimal.Register);
                        decimal allSum = sumInt + sumDecimal;
                        bool checkRegister = (balance - allSum) >= 0;
                        outParam.CheckRegister = checkRegister;
                        if (!checkRegister)
                        {
                            outParam.CheckRegisterInfo = register.Name;
                        }

                    }
                }                                                
            }

            return outParam;
        }

        private bool MtdStoreExists(string id)
        {
            return _context.MtdStore.Any(e => e.Id == id);
        }
    }

    public class OutData
    {
        public List<MtdStoreStack> MtdStoreStacks { get; set; }
        public MtdFormPartField MtdFormPartField { get; set; }
        public bool CheckRegister { get; set; }
        public string CheckRegisterInfo { get; set; }
    }
}
