/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
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

        public async Task<IList<MtdStoreStack>> GetStackAsync(IList<string> storeIds, IList<string> fieldIds) {

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
