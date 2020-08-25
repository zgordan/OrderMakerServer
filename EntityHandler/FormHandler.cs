using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.EntityHandler
{
    public class FormHandler
    {
        private readonly OrderMakerContext context;
        private List<FormFields> formFields;

        public FormHandler(OrderMakerContext orderMakerContext)
        {
            context = orderMakerContext;
        }

        public async Task<List<string>> GetFieldIdsAsync()
        {
            List<string> listIds = new List<string>();
            List<FormFields> items = await GetFormFieldsAsync();
            items.ForEach((form) =>
            {
                form.Fields.ForEach((field) => { listIds.Add(field.Id); });
            });

            return listIds;
        }

        public async Task<MtdStore> GetParentStoreAsync(string storeId)
        {
            string currentId = storeId;
            List<string> infinityBlocker = new List<string>();
        findParent:
            MtdStore mtdStore = await context.MtdStore.Where(x => x.Id == currentId).FirstOrDefaultAsync();
            if (mtdStore == null) { return null; }
            if (mtdStore.Parent == null) { return mtdStore; }
            if (infinityBlocker.Where(x => x == mtdStore.Parent).Any()) { return mtdStore; }
            infinityBlocker.Add(currentId);
            currentId = mtdStore.Parent;

            goto findParent;
        }

        public async Task<List<string>> GetStoreScopeIdsAsync(string storeParentId)
        {

            List<string> parentIds = new List<string> { storeParentId };
            List<string> result = new List<string> { storeParentId };

        findChild:
            IList<MtdStore> stores = await context.MtdStore.Where(x => parentIds.Contains(x.Parent)).ToListAsync();
            if (stores.Count == 0) { return result; }
            List<string> storeIds = stores.Select(x => x.Id).ToList();

            if (result.Where(x => storeIds.Contains(x)).Any()) { return null; };

            parentIds = storeIds;
            result.AddRange(stores.Select(x => x.Id));
            goto findChild;

            return result;
        }


        public async Task<MtdRegister> GetRegister(string fieldId)
        {
            MtdRegister result = null;
            string registerId = context.MtdRegisterField.Where(x => x.Id == fieldId).Select(x => x.MtdRegisterId).FirstOrDefault();
            if (registerId != null)
            {
                result = await context.MtdRegister.FindAsync(registerId);
            }

            return result;
        }


        public async Task<decimal> GetRegisterBalanceAsync(MtdRegister mtdRegister, string currentStoreId = null, string storeParentId = null)
        {
            string registerId = mtdRegister.Id;
            List<string> storeIds = new List<string>();
            if (storeParentId != null)
            {
                storeIds = await GetStoreScopeIdsAsync(storeParentId);
                if (currentStoreId != null)
                {
                    storeIds.Remove(currentStoreId);
                }                
            }

            decimal sumIncomes = 0;
            IList<string> fieldIncomeIds = await context.MtdRegisterField.Where(x => x.MtdRegisterId == registerId && x.Income == 1).Select(x => x.Id).ToListAsync();
            if (fieldIncomeIds.Count > 0)
            {
                IList<long> stackIncomeIds;

                if (storeParentId == null)
                {
                    stackIncomeIds = await context.MtdStoreStack.Where(x => fieldIncomeIds.Contains(x.MtdFormPartField)).Select(x => x.Id).ToListAsync();
                }
                else
                {
                    stackIncomeIds = await context.MtdStoreStack.Where(x => fieldIncomeIds.Contains(x.MtdFormPartField) && storeIds.Contains(x.MtdStore)).Select(x => x.Id).ToListAsync();
                }


                if (stackIncomeIds != null)
                {
                    sumIncomes = await context.MtdStoreStackInt.Where(x => stackIncomeIds.Contains(x.Id)).SumAsync(x => x.Register);
                    sumIncomes += await context.MtdStoreStackDecimal.Where(x => stackIncomeIds.Contains(x.Id)).SumAsync(x => x.Register);
                }
            }

            decimal sumExpense = 0;
            IList<string> fieldExpenseIds = await context.MtdRegisterField.Where(x => x.MtdRegisterId == registerId && x.Expense == 1).Select(x => x.Id).ToListAsync();
            if (fieldExpenseIds.Count > 0)
            {

                IList<long> stackExpenseIds;
                if (storeParentId == null)
                {
                    stackExpenseIds = await context.MtdStoreStack.Where(x => fieldExpenseIds.Contains(x.MtdFormPartField)).Select(x => x.Id).ToListAsync();
                }

                else
                {
                    stackExpenseIds = await context.MtdStoreStack.Where(x => fieldExpenseIds.Contains(x.MtdFormPartField) && storeIds.Contains(x.MtdStore)).Select(x => x.Id).ToListAsync();
                }


                if (stackExpenseIds != null)
                {
                    sumExpense = await context.MtdStoreStackInt.Where(x => stackExpenseIds.Contains(x.Id)).SumAsync(x => x.Register);
                    sumExpense += await context.MtdStoreStackDecimal.Where(x => stackExpenseIds.Contains(x.Id)).SumAsync(x => x.Register);
                }
            }

            return sumIncomes - sumExpense;
        }


        public async Task<List<FormFields>> GetFormFieldsAsync()
        {
            if (formFields != null) { return formFields; }

            formFields = new List<FormFields>();
            IList<MtdFormPartField> fields = await context.MtdFormPartField.Where(x => x.MtdSysType == 2 || x.MtdSysType == 3).ToListAsync();
            IList<MtdFormPart> parts = await context.MtdFormPart.Where(x => fields.Select(f => f.MtdFormPart).Contains(x.Id)).ToListAsync();
            IList<MtdForm> forms = await context.MtdForm.Where(x => parts.Select(f => f.MtdForm).Contains(x.Id)).OrderBy(x => x.Sequence).ToListAsync();

            foreach (MtdForm form in forms)
            {
                List<string> partIds = parts.Where(x => x.MtdForm == form.Id).Select(x => x.Id).ToList();
                formFields.Add(new FormFields { MtdForm = form, Fields = fields.Where(x => partIds.Contains(x.MtdFormPart)).OrderBy(x => x.Name).ToList() });
            }

            return formFields;
        }

    }
}
