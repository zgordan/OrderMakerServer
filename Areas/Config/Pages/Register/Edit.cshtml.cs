using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler;
using Mtd.OrderMaker.Server.Models.Controls.MTDSelectList;


namespace Mtd.OrderMaker.Server.Areas.Config.Pages.Register
{

    public class EditModel : PageModel
    {
        private readonly OrderMakerContext context;

        public EditModel(OrderMakerContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public MtdRegister MtdRegister { get; set; }

        [BindProperty]
        public bool ParentLimit { get; set; }

        public decimal Balance { get; set; }

        public List<FormFields> Forms { get; set; }
        public List<MtdRegisterField> RegisterFields { get; set; }
        public List<string> RejectFieldIds { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) { return NotFound(); }


            MtdRegister = await context.MtdRegister.FirstOrDefaultAsync(m => m.Id == id);
            if (MtdRegister == null)
            {
                return NotFound();
            }

            ParentLimit = MtdRegister.ParentLimit == 1;
            FormHandler formHandler = new FormHandler(context);
            Forms = await formHandler.GetFormFieldsAsync();
            RegisterFields = await context.MtdRegisterField.Where(x => x.MtdRegisterId == MtdRegister.Id).ToListAsync();
            RejectFieldIds = await context.MtdRegisterField.Where(x => x.MtdRegisterId != MtdRegister.Id).Select(x=>x.Id).ToListAsync();
            if (RegisterFields == null) { RegisterFields = new List<MtdRegisterField>(); }
            if (RejectFieldIds == null) { RejectFieldIds = new List<string>(); }

            Balance =  await  formHandler.GetRegisterBalanceAsync(MtdRegister);

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {

            var requestForm = await Request.ReadFormAsync();
            var formHandler = new FormHandler(context);
            List<string> fieldIds = await formHandler.GetFieldIdsAsync();

            IList<MtdRegisterField> fieldlRemove = await context.MtdRegisterField.Where(x => x.MtdRegisterId == MtdRegister.Id).ToListAsync();
            context.MtdRegisterField.RemoveRange(fieldlRemove);
            await context.SaveChangesAsync();

            List<MtdRegisterField> newFields = new List<MtdRegisterField>();

            foreach (var fieldId in fieldIds)
            {
                string linked = requestForm[$"{fieldId}-linked"];
                string income = requestForm[$"{fieldId}-income"];
                string expense = requestForm[$"{fieldId}-expense"];

                if (linked == "true")
                {
                    MtdRegisterField mtdRegisterField = new MtdRegisterField
                    {
                        Id = fieldId,
                        MtdRegisterId = MtdRegister.Id,
                        Income = income == "true" ? (sbyte)1 : (sbyte)0,
                        Expense = expense == "true" ? (sbyte)1 : (sbyte)0
                    };
                    newFields.Add(mtdRegisterField);
                }
            }

            if (newFields.Count > 0) { await context.MtdRegisterField.AddRangeAsync(newFields); }

            MtdRegister.ParentLimit = ParentLimit ? (sbyte)1 : (sbyte)0;
            context.MtdRegister.Update(MtdRegister);
            await context.SaveChangesAsync();

            return RedirectToPage("./Edit", new { id = MtdRegister.Id });
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var requestForm = await Request.ReadFormAsync();
            var registerId = requestForm["registerId"];
            var register = new MtdRegister { Id = registerId };
            context.MtdRegister.Remove(register);
            await context.SaveChangesAsync();
            return new OkResult();
        }

    }
}
