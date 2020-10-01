using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Components.Index;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Filter;
using Mtd.OrderMaker.Server.EntityHandler.Stack;
using Mtd.OrderMaker.Server.Models.Store;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.EntityHandler.Store
{
    public class StoreListHandler
    {
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly OrderMakerContext context;
        private readonly UserHandler userHandler;
        private readonly string resultValue;
        private readonly string formId;
        private readonly WebAppUser user;
        private string searchText;
        private int searchNumber;



        public int PageNumber { get; private set; }
        public int PageCount { get; private set; }
        public List<StoreListFields> StoreFields { get; private set; }
        public string PageLine { get; private set; }
        public MtdForm MtdForm { get; private set; }
        public List<string> Columns { get; private set; }

        public string ResultView { get; private set; }


        public StoreListHandler(OrderMakerContext context, string formId, string resultValue, WebAppUser user, UserHandler userHandler, IStringLocalizer<SharedResource> localizer)
        {
            this.context = context;
            this.userHandler = userHandler;
            this.user = user;
            this.formId = formId;
            this.PageNumber = 1;
            this.localizer = localizer;
            this.resultValue = resultValue;
            this.Columns = new List<string>();
            this.StoreFields = new List<StoreListFields>();
        }

        public void SetPageNumber(int page)
        {
            this.PageNumber = page <= 0 ? 1 : page;
        }

        public void SetFilterText(string searchText)
        {
            this.searchText = searchText;
        }

        public void SetFilterNumber(int searchNumber)
        {
            this.searchNumber = searchNumber;
        }

        public async Task FillDataAsync()
        {
            
            MtdForm = await context.MtdForm.Include(x => x.MtdFormHeader).Where(x => x.Id == formId).FirstOrDefaultAsync();
             
            /*check user*/
            bool isViewer = await userHandler.IsViewer(user, formId);
            if (!isViewer) { return; }


            if (resultValue != null && resultValue != string.Empty)
            {
                MtdStore mtdStore = await context.MtdStore.Where(x => x.Id == resultValue).FirstOrDefaultAsync();
                if (mtdStore != null) {
                    ResultView = $"{localizer["No."]} {mtdStore.Sequence:D9} {localizer["at"]} {mtdStore.Timecr.ToShortDateString()}";
                }
            }

            FilterHandler filterHandler = new FilterHandler(context, formId, user, userHandler);

            TypeQuery typeQuery = TypeQuery.empty;

            if (searchText != null && searchText.Length > 0) { typeQuery = TypeQuery.text; }
            if (searchNumber > 0) { typeQuery = TypeQuery.number; }

            Incomer incomer = new Incomer
            {
                FormId = formId,
                SearchNumber = searchNumber.ToString(),
                SearchText = searchText,
                Page = this.PageNumber,
                PageSize = 5,
                FieldForColumn = await filterHandler.GetFieldsAsync(),
                WaitList = 0,
            };


            OutFlow outFlow = await filterHandler.GetStackFlowAsync(incomer, typeQuery);
            StackHandler handlerStack = new StackHandler(context);
            List<string> storeIds = outFlow.MtdStores.Select(x => x.Id).ToList();
            IList<MtdStoreStack> mtdStoreStack = await handlerStack.GetStackAsync(storeIds, incomer.FieldIds);
           

            List<StoreListFields> storeFields = new List<StoreListFields>();


            foreach (string storeId in storeIds)
            {
                MtdStore mtdStore = outFlow.MtdStores.Where(x => x.Id == storeId).FirstOrDefault();

                string number = mtdStore.Sequence.ToString("D9");
                string date = mtdStore.Timecr.ToShortDateString();
                StoreListFields store = new StoreListFields()
                {
                    StoreId = storeId,
                    Fields = new List<StoreListField>() {

                         new StoreListField{ Id = "store-id", Name = localizer["ID"], Value = storeId},
                         new StoreListField{ Id = "store-name", Name = localizer["Name"], Value = $"{localizer["No."]} {number} {localizer["at"]} {date}" },
                         new StoreListField{ Id = "store-number", Name = localizer["Number"], Value = number},
                         new StoreListField{ Id = "store-date", Name = localizer["Date"], Value = date},
                    }
                };

                foreach(MtdFormPartField field in incomer.FieldForColumn)
                {
                    MtdStoreStack stack = mtdStoreStack.Where(x => x.MtdStore == storeId && x.MtdFormPartField == field.Id).FirstOrDefault();
                    string value = handlerStack.GetValueAsString(stack, field);
                    store.Fields.Add(new StoreListField { Id = field.Id, SysType = field.MtdSysType.ToString(), Name = field.Name, Value = value });
                }

                storeFields.Add(store);
            }

            PageCount = outFlow.PageCount;
            PageLine = $"{localizer["Page"]} {this.PageNumber} {localizer["of"]} {PageCount} ";
            StoreFields = storeFields;
            Columns = new List<string>();

            if (incomer.FieldForColumn != null)
            {
                Columns.Add(localizer["ID"]);
                Columns.Add(localizer["Name"]);
                Columns.Add(localizer["Number"]);
                Columns.Add(localizer["Date"]);
                Columns.AddRange(incomer.FieldForColumn.Select(x => x.Name).ToList());
            }
        }


    }
}
