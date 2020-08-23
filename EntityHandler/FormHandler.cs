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

        public async Task<List<string>> GetFieldIdsAsync ()
        {
            List<string> listIds = new List<string>();
            List<FormFields> items = await GetFormFieldsAsync();
            items.ForEach((form) =>
            {
                form.Fields.ForEach((field) => {listIds.Add(field.Id);});                
            });

            return listIds;
        }


        public async Task<List<FormFields>> GetFormFieldsAsync()
        {
            if (formFields != null) { return formFields; }

            formFields = new List<FormFields>();
            IList<MtdFormPartField> fields = await context.MtdFormPartField.Where(x => x.MtdSysType == 2 || x.MtdSysType == 3).ToListAsync();            
            IList<MtdFormPart> parts  = await context.MtdFormPart.Where(x => fields.Select(f=>f.MtdFormPart).Contains(x.Id)).ToListAsync();
            IList<MtdForm> forms = await context.MtdForm.Where(x => parts.Select(f => f.MtdForm).Contains(x.Id)).OrderBy(x=>x.Sequence).ToListAsync();

            foreach(MtdForm form in forms)
            {
                List<string> partIds = parts.Where(x => x.MtdForm == form.Id).Select(x => x.Id).ToList();
                formFields.Add(new FormFields { MtdForm = form, Fields = fields.Where(x => partIds.Contains(x.MtdFormPart)).OrderBy(x=>x.Name).ToList() });
            }


            return formFields;
        }

    }
}
