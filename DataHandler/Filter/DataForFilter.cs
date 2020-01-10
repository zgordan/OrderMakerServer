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
using Mtd.OrderMaker.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.DataHandler.Filter
{
    public partial class FilterHandler
    {
       
        public async Task<IList<MtdFilterScript>> GetScriptsAsync()
        {
            MtdFilter mtdFilter = await GetFilterAsync();
            return await _context.MtdFilterScript.Where(x => x.MtdFilter == mtdFilter.Id).ToListAsync();

        } 

        public async Task<OutFlow> GetDataForEmptyAsync(Incomer incomer)
        {            

            return new OutFlow
            {                
                Count = await queryMtdStore.Where(x => x.MtdForm == incomer.IdForm).CountAsync(),
                MtdStores = await queryMtdStore.Where(x => x.MtdForm == incomer.IdForm).OrderByDescending(x => x.Sequence)
                .Skip((incomer.Page - 1) * incomer.PageSize)
                .Take(incomer.PageSize)
                .ToListAsync()
            };
        }

        public async Task<OutFlow> GetDataForNumberAsync(Incomer incomer)
        {
            MtdStore mtdStore = await queryMtdStore.Where(x => x.MtdForm == incomer.IdForm & x.Sequence.ToString().Equals(incomer.SearchNumber)).FirstOrDefaultAsync();

            return new OutFlow
            {
                Count = mtdStore != null ? 1 : 0,
                MtdStores = mtdStore != null ? new List<MtdStore> { mtdStore } : new List<MtdStore>()
            };

        }

        public async Task<OutFlow> GetDataForTextAsync(Incomer incomer)
        {
            List<string> fieldsIds = incomer.FieldForColumn.Select(x => x.Id).ToList();
            IList<string> storeIds = await FindStoreIdsForText(fieldsIds, incomer.SearchText, 4);
            return new OutFlow
            {
                Count = storeIds.Count(),
                MtdStores = await queryMtdStore.Where(x => storeIds.Contains(x.Id))
                .OrderByDescending(x => x.Sequence).Skip((incomer.Page - 1) * incomer.PageSize).Take(incomer.PageSize).ToListAsync()
            };

        }

        public async Task<OutFlow> GetDataForFieldAsync(Incomer incomer)
        {

            IList<string> storeIds = null;

            if (incomer.SearchText.Length > 0)
            {
                List<string> fieldsIds = incomer.FieldForColumn.Select(x => x.Id).ToList();
                storeIds = await FindStoreIdsForText(fieldsIds, incomer.SearchText, 4);
            }

            if (incomer.FieldForFilter != null && incomer.FieldForFilter.Count > 0)
            {
                foreach (var item in incomer.FieldForFilter)
                {
                    int fieldType = item.MtdFormPartFieldNavigation.MtdSysType;
                    List<string> field = new List<string> { item.MtdFormPartField };

                    switch (fieldType)
                    {
                        case 2:
                        case 12:
                            {
                                int valueInt = int.Parse(item.Value);
                                storeIds = await FindStoreIdsForInt(field, valueInt, item.MtdTerm, storeIds);
                                break;
                            }
                        case 3:
                            {
                                decimal valueDecimal = decimal.Parse(item.Value);
                                storeIds = await FindStoreIdsForDecimal(field, valueDecimal, item.MtdTerm, storeIds);
                                break;
                            }
                        case 5:
                            {
                                bool ok = DateTime.TryParse(item.Value, out DateTime dateTime);
                                if (ok)
                                {
                                    storeIds = await FindStoreIdsForDate(field, dateTime.Date, item.MtdTerm, storeIds);
                                }
                                break;
                            }
                        case 6:
                            {
                                bool ok = DateTime.TryParse(item.Value, out DateTime dateTime);
                                if (ok)
                                {
                                    storeIds = await FindStoreIdsForDateTime(field, dateTime, item.MtdTerm, storeIds);
                                }
                                break;
                            }
                        case 11:
                            {
                                storeIds = await FindStoreIdsForList(field, item.Value, item.MtdTerm, storeIds);
                                break;
                            }
                        default:
                            {
                                storeIds = await FindStoreIdsForText(field, item.Value, item.MtdTerm, storeIds);
                                break;
                            }
                    }
                };
            }

            OutFlow paramOut = new OutFlow
            {
                Count = storeIds.Count(),
                MtdStores = await queryMtdStore.Where(x => storeIds.Contains(x.Id))
                .OrderByDescending(x => x.Sequence).Skip((incomer.Page - 1) * incomer.PageSize).Take(incomer.PageSize).ToListAsync()
            };

            return paramOut;
        }
    }
}
