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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Services
{
    public enum RightsType
    {
        View, Create, Edit, Delete, ViewOwn, EditOwn, DeleteOwn, ViewGroup, EditGroup, DeleteGroup, SetOwn, Reviewer, SetDate
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

        public static readonly string PolicyKey = "PolicyCache";

        public UserHandler(PolicyCache cache, OrderMakerContext context,
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
        }

        public async Task<IList<MtdPolicy>> CacheRefresh()
        {
            IList<MtdPolicy> mtdPolicies = await _context.MtdPolicy
                 .Include(x => x.MtdPolicyForms)
                 .Include(x => x.MtdPolicyGroup)
                 .Include(x => x.MtdPolicyParts)
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
                .Include(x => x.MtdPolicyGroup)
                .Include(x => x.MtdPolicyParts)
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

        public async Task<MtdPolicy> GetPolicyForUser (WebAppUser user)
        {            
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null);
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            if (mtdPolicy == null) return null;
            return mtdPolicy.Where(x=>x.Id == policyId).FirstOrDefault();            
        } 

        public async Task<bool> IsAdmin(WebAppUser user)
        {
            return await IsInRoleAsync(user, "Admin");
        }

        public async Task<bool> IsOwner(WebAppUser user, string idStore)
        {
            return await _context.MtdStoreOwner.Where(x => x.Id == idStore && x.UserId == user.Id).AnyAsync();
        }

        public async Task<bool> InGroup(WebAppUser user, string idStore)
        {
            IList<MtdPolicy> mtdPolicies = await CacheGetOrCreateAsync();
            string ownerId = await _context.MtdStoreOwner.Where(x => x.Id == idStore).Select(x => x.UserId).FirstOrDefaultAsync();
            WebAppUser userOwner = new WebAppUser { Id = ownerId };
            string policyIdOwner = await GetPolicyIdAsync(userOwner);
            string policyIdUser = await GetPolicyIdAsync(user);

            if (policyIdOwner == null) return false;
            if (policyIdUser == null) return false;

            List<string> groupsOwner = mtdPolicies
                        .SelectMany(x => x.MtdPolicyGroup)
                        .Where(x => x.Member == 1 && x.MtdPolicy == policyIdOwner)
                        .GroupBy(x => x.MtdGroup)
                        .Select(x => x.Key)
                        .ToList();

            List<string> groupsUser = mtdPolicies
            .SelectMany(x => x.MtdPolicyGroup)
            .Where(x => x.Member == 1 && x.MtdPolicy == policyIdUser)
            .GroupBy(x => x.MtdGroup)
            .Select(x => x.Key)
            .ToList();

            return groupsUser.Where(x => groupsOwner.Contains(x)).Any();
        }

        public async Task<bool> IsCreator(WebAppUser user, string idForm)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == idForm && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            return policyForms.Create == 1 ? true : false;
        }

        public async Task<bool> IsViewer(WebAppUser user, string idForm, string idStore = null)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == idForm && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            if (policyForms.ViewAll == 1) { return true; }

            if (idStore != null)
            {
                bool isOwner = await IsOwner(user, idStore);
                if (policyForms.ViewOwn == 1 && isOwner) { return true; }
                bool inGroup = await InGroup(user, idStore);
                if (policyForms.ViewGroup == 1 && inGroup) { return true; }
            }

            return false;

        }

        public async Task<bool> IsEditor(WebAppUser user, string idForm, string idStore = null)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == idForm && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            if (policyForms.EditAll == 1) { return true; }

            if (idStore != null)
            {
                bool isOwner = await IsOwner(user, idStore);
                if (policyForms.EditOwn == 1 && isOwner) { return true; }
                bool inGroup = await InGroup(user, idStore);
                if (policyForms.EditGroup == 1 && inGroup) { return true; }
            }

            return false;
        }

        public async Task<bool> IsEraser(WebAppUser user, string idForm, string idStore = null)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == idForm && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            if (policyForms.DeleteAll == 1) { return true; }

            if (idStore != null)
            {
                bool isOwner = await IsOwner(user, idStore);
                if (policyForms.DeleteOwn == 1 && isOwner) { return true; }
                bool inGroup = await InGroup(user, idStore);
                if (policyForms.DeleteGroup == 1 && inGroup) { return true; }
            }

            return false;
        }

        public async Task<bool> IsInstallerOwner(WebAppUser user, string idForm)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == idForm && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            return policyForms.ChangeOwner == 1 ? true : false;

        }

        public async Task<bool> IsReviewer(WebAppUser user, string idForm)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == idForm && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            return policyForms.Reviewer == 1 ? true : false;
        }

        public async Task<bool> IsCreatorPartAsync(WebAppUser user, string idPart)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyParts policyParts = mtdPolicy.SelectMany(x => x.MtdPolicyParts).Where(x => x.MtdFormPart == idPart && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyParts == null) return false;

            return policyParts.Create == 1 ? true : false;
        }

        public async Task<bool> IsEditorPartAsync(WebAppUser user, string idPart)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyParts policyParts = mtdPolicy.SelectMany(x => x.MtdPolicyParts).Where(x => x.MtdFormPart == idPart && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyParts == null) return false;

            return policyParts.Edit == 1 ? true : false;
        }

        public async Task<bool> IsViewerPartAsync(WebAppUser user, string idPart)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyParts policyParts = mtdPolicy.SelectMany(x => x.MtdPolicyParts).Where(x => x.MtdFormPart == idPart && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyParts == null) return false;

            return policyParts.View == 1 ? true : false;
        }

        public async Task<List<string>> GetAllowPartsForView(WebAppUser user, string idForm)
        {
            List<string> result = new List<string>();
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return result;
            result = mtdPolicy.SelectMany(x => x.MtdPolicyParts).Where(x => x.View == 1).Select(x => x.MtdFormPart).ToList();
            return result;
        }

        public async Task<List<WebAppUser>> GetUsersInGroupsAsync(WebAppUser webAppUser)
        {
            List<WebAppUser> webAppUsers = new List<WebAppUser>();
            IList<MtdPolicy> mtdPolicies = await CacheGetOrCreateAsync();
            IList<Claim> claims = await GetClaimsAsync(webAppUser);
            string policyID = claims.Where(x => x.Type == "policy").Select(x => x.Value).FirstOrDefault();
            if (policyID == null) return webAppUsers;

            List<MtdPolicyGroup> policyGroups = mtdPolicies
                .SelectMany(x => x.MtdPolicyGroup)
                .Where(x => x.Member == 1)
                .ToList();

            List<string> groupIds = policyGroups
                .Where(x => x.MtdPolicy == policyID)
                .GroupBy(g => g.MtdGroup)
                .Select(x => x.Key)
                .ToList();

            List<string> policyIds = policyGroups
                .Where(x => groupIds.Contains(x.MtdGroup) && x.Member == 1)
                .GroupBy(x => x.MtdPolicy)
                .Select(x => x.Key)
                .ToList();

            foreach (var policyId in policyIds)
            {
                Claim claim = new Claim("policy", policyId);
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


        public async Task<bool> GetFormPolicyAsync(WebAppUser user, string idForm, RightsType rightsType)
        {
            IList<MtdPolicy> mtdPolicy = await CacheGetOrCreateAsync();
            string policyId = await GetPolicyIdAsync(user);
            if (policyId == null) return false;
            MtdPolicyForms policyForms = mtdPolicy.SelectMany(x => x.MtdPolicyForms).Where(x => x.MtdForm == idForm && x.MtdPolicy == policyId).FirstOrDefault();
            if (policyForms == null) return false;

            bool result = false;

            switch (rightsType)
            {
                case RightsType.Create:
                    {
                        result = policyForms.Create == 1 ? true : false;
                        break;
                    }

                case RightsType.Delete:
                    {
                        result = policyForms.DeleteAll == 1 ? true : false;
                        break;
                    }

                case RightsType.DeleteGroup:
                    {
                        result = policyForms.DeleteGroup == 1 ? true : false;
                        break;
                    }
                case RightsType.DeleteOwn:
                    {
                        result = policyForms.DeleteOwn == 1 ? true : false;
                        break;
                    }
                case RightsType.Edit:
                    {
                        result = policyForms.EditAll == 1 ? true : false;
                        break;
                    }
                case RightsType.EditGroup:
                    {
                        result = policyForms.EditGroup == 1 ? true : false;
                        break;
                    }
                case RightsType.EditOwn:
                    {
                        result = policyForms.EditOwn == 1 ? true : false;
                        break;
                    }
                case RightsType.Reviewer:
                    {
                        result = policyForms.Reviewer == 1 ? true : false;
                        break;
                    }

                case RightsType.SetOwn:
                    {
                        result = policyForms.ChangeOwner == 1 ? true : false;
                        break;
                    }
                case RightsType.View:
                    {
                        result = policyForms.ViewAll == 1 ? true : false;
                        break;
                    }
                case RightsType.ViewGroup:
                    {
                        result = policyForms.ViewGroup == 1 ? true : false;
                        break;
                    }
                case RightsType.ViewOwn:
                    {
                        result = policyForms.ViewOwn == 1 ? true : false;
                        break;
                    }
                case RightsType.SetDate:
                    {
                        result = policyForms.ChangeDate == 1 ? true : false;
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



    }
}
