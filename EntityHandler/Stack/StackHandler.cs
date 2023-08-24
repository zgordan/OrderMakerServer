/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.EntityHandler.Stack
{
    public partial class StackHandler
    {
        private readonly OrderMakerContext _context;

        public StackHandler(OrderMakerContext orderMakerContext)
        {
            _context = orderMakerContext;
        }

        public async Task<IList<MtdStoreStack>> GetStackAsync(IList<string> storeIds, IList<string> fieldIds)
        {

            IList<long> stackStoreIds = await _context.MtdStoreStack
                .Where(x => storeIds.Contains(x.MtdStore) && fieldIds.Contains(x.MtdFormPartField))
                .Select(x => x.Id).ToListAsync();

            IList<MtdStoreStack> mtdStoreStack = await _context.MtdStoreStack
               .Include(x => x.MtdStoreStackDate)
               .Include(x => x.MtdStoreStackText)
               .Include(x => x.MtdStoreStackInt)
               .Include(x => x.MtdStoreStackDecimal)
               .Include(x => x.MtdStoreStackFile)
               .Include(x => x.MtdStoreLink)
               .Include(x => x.MtdStoreNavigation)
               .Where(x => stackStoreIds.Contains(x.Id))
               .ToListAsync();

            return mtdStoreStack;
        }

        public string GetValueAsString(MtdStoreStack stack, MtdFormPartField field)
        {
            string value = string.Empty;

            switch (field.MtdSysType)
            {
                case 2:
                    {
                        int result = 0;
                        if (stack != null && stack.MtdStoreStackInt != null)
                        {
                            result = stack.MtdStoreStackInt.Register;
                        };
                        value = result.ToString();
                        break;
                    }
                case 3:
                    {
                        double result = 0.00;
                        if (stack != null && stack.MtdStoreStackDecimal != null)
                        {
                            result = (double)stack.MtdStoreStackDecimal.Register;
                        }
                        value = result.ToString();
                        break;
                    }
                case 5:
                    {

                        if (stack != null && stack.MtdStoreStackDate != null)
                        {
                            value = stack.MtdStoreStackDate.Register.Date.ToShortDateString();
                        }
                        break;
                    }
                case 6:
                    {
                        if (stack != null && stack.MtdStoreStackDate != null)
                        {

                            value = stack.MtdStoreStackDate.Register.ToShortDateString();
                        }

                        break;
                    }

                case 11:
                    {

                        if (stack != null && stack.MtdStoreLink != null)
                        {
                            value = stack.MtdStoreLink.Register;
                        }
                        break;
                    }
                case 12:
                    {
                        int result = 0;
                        if (stack != null && stack.MtdStoreStackInt != null)
                        {
                            result = stack.MtdStoreStackInt.Register;
                        }
                        value = result.ToString();
                        break;
                    }
                default:
                    {
                        if (stack != null && stack.MtdStoreStackText != null)
                        {
                            value = stack.MtdStoreStackText.Register;
                        }

                        break;
                    }
            }

            return value;

        }

    }
}
