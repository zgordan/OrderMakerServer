/*
    OrderMaker - http://ordermaker.org
    Copyright(c) 2019 Oleg Bruev. All rights reserved.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see https://www.gnu.org/licenses/.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Services
{
    public enum RightsType
    {
        ViewAll, Create, Edit, Delete, ViewOwn, EditOwn, DeleteOwn, ViewGroup, EditGroup, DeleteGroup, SetOwn, Reviewer, SetDate, OwnDenyGroup, ExportToExcel
    };

    public class PolicyCache
    {
        public MemoryCache Cache { get; set; }

        public PolicyCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions { SizeLimit = null });
        }
    }

    public class UserHandler : UserManager<WebAppUser>
    {
        private readonly PolicyCache _cache;
        private readonly OrderMakerContext _context;
        private readonly SignInManager<WebAppUser> _signInManager;
        private readonly IdentityDbContext identity;
        public static readonly string PolicyKey = "PolicyCache";

        public UserHandler(IdentityDbContext identity,PolicyCache cache, OrderMakerContext context,
            IUserStore<WebAppUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<WebAppUser> passwordHasher,
            IEnumerable<IUserValidator<WebAppUser>> userValidators,
            IEnumerable<IPasswordValidator<WebAppUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<WebAppUser>> logger, SignInManager<WebAppUser> signInManager) :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
                {
                    _context = context;
                    _signInManager = signInManager;
                    _cache = cache;
                    this.identity = identity;
                }

        public async Task<IList<MtdPolicy>> CacheRefresh()
        {
            IList<MtdPolicy> mtdPolicies = await _context.MtdPolicy
                 .Include(x => x.MtdPolicyForms)
                 .Include(x => x.MtdPolicyParts)
                 .Include(x => x.MtdPolicyScripts)
                 .ToListAsync();

            _cache.Cache.Set(PolicyKey, mtdPolicies);

            return mtdPolicies;
        }

        public async Task<IList<MtdPolicy>> CacheGetOrCreateAsync()
        {
            if (!_cache.Cache.TryGetValue(PolicyKey, out IList<MtdPolicy> mtdPolicies))
            {
                mtdPolicies = await _context.MtdPolicy
                .Include(x => x.MtdPolicyForms)
                .Include(x => x.MtdPolicyParts)
                .Include(x => x.MtdPolicyScripts)
                .ToListAsync();

                _cache.Cache.Set(PolicyKey, mtdPolicies);
            }

            return mtdPolicies;
        }

        public async Task<string> GetPolicyIdAsync(WebAppUser user)
        {
            IList<Claim> claims = await GetClaimsAsync(user);
            string policyId = claims.Where(x => x.Type == "policy").Select(x => x.Value).FirstOrDefault();
            return policyId ?? "";
        }

        public async Task<List<string>> GetViewFormIdsAsync(WebAppUser user)
        {
            List<string> formIds = new List<string>();
            if (user == null) return formIds;

            string policyId = await GetPolicyIdAsync(user);
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();

            IList<MtdPolicyForms> policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms)
                .Where(x => x.MtdPolicy == policyId && (x.ViewAll == 1 || x.ViewGroup == 1 || x.ViewOwn == 1))
                .ToList();

            formIds = policyForms.GroupBy(x => x.MtdForm).Select(x => x.Key).ToList();
            return formIds;
        }

        public async Task<MtdPolicy> GetPolicyForUserAsync(WebAppUser user)
        {
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return null;
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            if (mtdPolicy == null) return null;
            return mtdPolicy.Where(x => x.Id == policyId).FirstOrDefault();
        }


        public async Task<WebAppUser> GetOwnerAsync(string idStore)
        {
            WebAppUser webAppUser = null;
            MtdStoreOwner owner = await _context.MtdStoreOwner.Where(x => x.Id == idStore).FirstOrDefaultAsync();
            if (owner != null)
            {
                webAppUser = await FindByIdAsync(owner.UserId);
            }

            return webAppUser;
        }

        public async Task<bool> IsAdmin(WebAppUser user)
        {
            return await IsInRoleAsync(user, "Admin");
        }

        public async Task<bool> IsOwner(WebAppUser user, string storeId)
        {
            return await _context.MtdStoreOwner.Where(x => x.Id == storeId && x.UserId == user.Id).AnyAsync();
        }

        public async Task<bool> InGroup(WebAppUser user, string formId, string storeId)
        {
            string ownerId = await _context.MtdStoreOwner.Where(x => x.Id == storeId).Select(x => x.UserId).FirstOrDefaultAsync();
            WebAppUser userOwner = new WebAppUser { Id = ownerId };
            if (user.Id == userOwner.Id) { return true; }

            bool denyGroup = await CheckUserPolicyAsync(userOwner, formId, RightsType.OwnDenyGroup);
            if (denyGroup) { return false; }

            IList<Claim> ownerClaims = await GetClaimsAsync(userOwner);
            List<string> ownerGroupdIds = ownerClaims.Where(x => x.Type == "group").Select(x => x.Value).ToList();
            IList<Claim> userClaims = await GetClaimsAsync(user);
            List<string> userGroupdIds = userClaims.Where(x => x.Type == "group").Select(x => x.Value).ToList();

            bool result = false;

            foreach (var idgroup in ownerGroupdIds)
            {
                if (userGroupdIds.Contains(idgroup))
                {
                    result = true;
                }
            }

            return result;
        }

        public async Task<bool> IsCreator(WebAppUser user, string formId)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == formId && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            return policyForms.Create == 1;
        }

        public async Task<bool> IsViewer(WebAppUser user, string formId, string storeId)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) { return false; }
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == formId && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) { return false; }
            if (policyForms.ViewAll == 1) { return true; }

            if (storeId != null)
            {
                bool isOwner = await IsOwner(user, storeId);
                if (policyForms.ViewOwn == 1 && isOwner) { return true; }

                bool inGroup = await InGroup(user, formId, storeId);
                if (policyForms.ViewGroup == 1 && inGroup) { return true; }
            }

            return false;

        }

        public async Task<bool> IsEditor(WebAppUser user, string formId, string storeId)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) { return false; }
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == formId && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) { return false; }

            if (policyForms.EditAll == 1) { return true; }

            if (storeId != null)
            {
                bool isOwner = await IsOwner(user, storeId);
                if (policyForms.EditOwn == 1 && isOwner) { return true; }
                bool inGroup = await InGroup(user, formId, storeId);
                if (policyForms.EditGroup == 1 && inGroup) { return true; }
            }

            return false;
        }

        public async Task<bool> IsEraser(WebAppUser user, string formId, string storeId)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == formId && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            if (policyForms.DeleteAll == 1) { return true; }

            if (storeId != null)
            {
                bool isOwner = await IsOwner(user, storeId);
                if (policyForms.DeleteOwn == 1 && isOwner) { return true; }
                bool inGroup = await InGroup(user, formId, storeId);
                if (policyForms.DeleteGroup == 1 && inGroup) { return true; }
            }

            return false;
        }

        public async Task<bool> IsInstallerOwner(WebAppUser user, string formId)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == formId && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            return policyForms.ChangeOwner == 1;

        }

        public async Task<bool> IsReviewer(WebAppUser user, string formId)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == formId && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            return policyForms.Reviewer == 1;
        }

        public async Task<bool> IsCreatorPartAsync(WebAppUser user, string partId)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyParts policyParts = mtdPolicy.SelectMany(x => x.MtdPolicyParts).Where(x => x.MtdFormPart == partId && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyParts == null) return false;

            return policyParts.Create == 1;
        }

        public async Task<bool> IsEditorPartAsync(WebAppUser user, string idPart)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyParts policyParts = mtdPolicy.SelectMany(x => x.MtdPolicyParts).Where(x => x.MtdFormPart == idPart && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyParts == null) return false;

            return policyParts.Edit == 1;
        }

        public async Task<bool> IsViewerPartAsync(WebAppUser user, string idPart)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyParts policyParts = mtdPolicy.SelectMany(x => x.MtdPolicyParts).Where(x => x.MtdFormPart == idPart && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyParts == null) return false;

            return policyParts.View == 1;
        }

        public async Task<List<MtdFormPart>> GetAllowPartsForView(WebAppUser user, string formId)
        {
            List<MtdFormPart> result = new List<MtdFormPart>();
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            MtdPolicy userPolicy = await GetPolicyForUserAsync(user);
            if (userPolicy == null) return result;

            IList<MtdFormPart> parts = await _context.MtdFormPart.Where(x => x.MtdForm == formId).ToListAsync();
            List<string> partIds = parts.Select(x => x.Id).ToList();
            List<string> allowPartsIds = userPolicy.MtdPolicyParts
                .Where(x => partIds.Contains(x.MtdFormPart) && x.View == 1)
                .Select(x => x.MtdFormPart)
                .ToList();

            return parts.Where(x => allowPartsIds.Contains(x.Id)).OrderBy(x => x.Sequence).ToList();
        }

        public async Task<List<WebAppUser>> GetUsersInGroupsOutDenyAsync(WebAppUser webAppUser, string formId)
        {
            List<WebAppUser> result = new List<WebAppUser>();
            List<WebAppUser> users = await GetUsersInGroupsAsync(webAppUser);

            foreach(WebAppUser user in users)
            {
                bool checkDeny = await CheckUserPolicyAsync(user, formId, RightsType.OwnDenyGroup);
                if (!checkDeny || webAppUser.Id == user.Id)
                {
                    result.Add(user);
                }
            }

            return result;
        }

        public async Task<IList<WebAppUser>> GetUsersInGroupAsync(string groupId = null) 
        {
            if (groupId != null)
            {
                Claim claim = new Claim("group", groupId);
                return await GetUsersForClaimAsync(claim);
            }

            IList<string> userIds = await identity.UserClaims.Where(x => x.ClaimType == "group").Select(x => x.UserId).ToListAsync();            
            return  await Users.Where(x=> !userIds.Contains(x.Id)).ToListAsync();
            
        }

        public async Task<List<WebAppUser>> GetUsersInGroupsAsync(WebAppUser webAppUser)
        {

            List<WebAppUser> webAppUsers = new List<WebAppUser>();
            IList<Claim> claims = await GetClaimsAsync(webAppUser);
            IList<Claim> groups = claims.Where(c => c.Type == "group").ToList();

            foreach (var claim in groups)
            {
                IList<WebAppUser> users = await GetUsersForClaimAsync(claim);
                if (users != null)
                {
                    var temp = users.Where(x => !webAppUsers.Select(w => w.Id).Contains(x.Id)).ToList();
                    if (temp != null)
                    {
                        webAppUsers.AddRange(temp);
                    }
                }
            }

            return webAppUsers;

        }

        public async Task<List<WebAppUser>> GetUsersForViewingForm(string formId, string storeId = null)
        {
            List<WebAppUser> usersAll = await this.Users.ToListAsync();
            List<WebAppUser> usersAccess = new List<WebAppUser>();

            foreach (WebAppUser user in usersAll)
            {
                bool viewer = await IsViewer(user, formId, storeId);
                if (viewer)
                {
                    usersAccess.Add(user);
                }
            }

            return usersAccess;
        }

        public async Task<MtdFilter> GetFilterAsync(WebAppUser user, string formId)
        {
            MtdFilter filter = await _context.MtdFilter.AsNoTracking().FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == formId);

            if (filter == null)
            {
                filter = new MtdFilter
                {
                    IdUser = user.Id,
                    MtdForm = formId,
                    SearchNumber = "",
                    SearchText = "",
                    Page = 1,
                    PageSize = 10,
                    WaitList = 0,
                    ShowDate = 1,
                    ShowNumber = 1
                };
                await _context.MtdFilter.AddAsync(filter);
                await _context.SaveChangesAsync();
            }

            return filter;
        }

        public async Task<MtdFilter> GetFilterAsync(ClaimsPrincipal principal, string formId)
        {
            WebAppUser user = await GetUserAsync(principal);
            return await GetFilterAsync(user,formId);
        }

        public async Task<bool> IsFilterAccessingAsync(ClaimsPrincipal user, int scriptId)
        {
            WebAppUser userApp = await GetUserAsync(user);
            string policyId = await GetPolicyIdAsync(userApp);
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            MtdPolicy policy = mtdPolicy.Where(x => x.Id == policyId).FirstOrDefault();
            return policy.MtdPolicyScripts.Where(x => x.MtdFilterScriptId == scriptId).Any();
        }

        public async Task<List<MtdFilterScript>> GetFilterScriptsAsync(WebAppUser user, string formId, sbyte apply = -1)
        {
            string policyId = await GetPolicyIdAsync(user);
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();

            MtdPolicy policy = mtdPolicy.Where(x => x.Id == policyId).FirstOrDefault();

            List<int> filterIds = policy.MtdPolicyScripts.Select(x => x.MtdFilterScriptId).ToList();

            var query = _context.MtdFilterScript.AsNoTracking().Where(x => filterIds.Contains(x.Id) && x.MtdFormId == formId);
            
            if (apply > 0) {
                var filter = await GetFilterAsync(user, formId);
                IList<int> applyIds = await _context.MtdFilterScriptApply.Where(x => x.MtdFilterId == filter.Id).Select(x=>x.MtdFilterScriptId).ToListAsync(); 
                query = query.Where(q => applyIds.Contains(q.Id)); 
            }

            return await query.ToListAsync();
        }

        public async Task<bool> CheckUserPolicyAsync(WebAppUser user, string formId, RightsType rightsType)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == formId && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            bool result = false;

            switch (rightsType)
            {
                case RightsType.Create:
                    {
                        result = policyForms.Create == 1;
                        break;
                    }

                case RightsType.Delete:
                    {
                        result = policyForms.DeleteAll == 1;
                        break;
                    }

                case RightsType.DeleteGroup:
                    {
                        result = policyForms.DeleteGroup == 1;
                        break;
                    }
                case RightsType.DeleteOwn:
                    {
                        result = policyForms.DeleteOwn == 1;
                        break;
                    }
                case RightsType.Edit:
                    {
                        result = policyForms.EditAll == 1;
                        break;
                    }
                case RightsType.EditGroup:
                    {
                        result = policyForms.EditGroup == 1;
                        break;
                    }
                case RightsType.EditOwn:
                    {
                        result = policyForms.EditOwn == 1;
                        break;
                    }
                case RightsType.Reviewer:
                    {
                        result = policyForms.Reviewer == 1;
                        break;
                    }

                case RightsType.SetOwn:
                    {
                        result = policyForms.ChangeOwner == 1;
                        break;
                    }
                case RightsType.ViewAll:
                    {
                        result = policyForms.ViewAll == 1;
                        break;
                    }
                case RightsType.ViewGroup:
                    {
                        result = policyForms.ViewGroup == 1;
                        break;
                    }
                case RightsType.ViewOwn:
                    {
                        result = policyForms.ViewOwn == 1;
                        break;
                    }
                case RightsType.SetDate:
                    {
                        result = policyForms.ChangeDate == 1;
                        break;
                    }
                case RightsType.OwnDenyGroup:
                    {
                        result = policyForms.OwnDenyGroup == 1;
                        break;
                    }
                case RightsType.ExportToExcel:
                    {
                        result = policyForms.ExportToExcel == 1;
                        break;
                    }
                default:
                    {
                        result = false;
                        break;
                    }

            }

            return result;
        }

        public override async Task<WebAppUser> GetUserAsync(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            var id = GetUserId(principal);
            if (id == null) { return null; }
            WebAppUser user = await FindByIdAsync(id);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return null;
            }

            return user;
        }


        public string GeneratePassword()
        {
            var options = Options.Password;

            int length = 8;

            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();
            int[] exceptions = new int[] { 92, 47, 40, 41, 39, 34, 44, 46, 60, 62, 96, 32, 123, 124, 125, 38, 43 };

            while (password.Length < length)
            {
            getCode:
                int code = random.Next(33, 126);
                if (exceptions.Where(x => exceptions.Contains(code)).Any()) { goto getCode; }
                char c = (char)code;

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 46));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));


            return password.ToString();
        }

    }
}
