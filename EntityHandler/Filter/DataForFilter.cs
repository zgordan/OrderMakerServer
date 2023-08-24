/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.EntityHandler.Filter
{
    public partial class FilterHandler
    {

        public async Task<OutFlow> GetDataForEmptyAsync(Incomer incomer)
        {

            return new OutFlow
            {
                Count = await queryMtdStore.Where(x => x.MtdForm == incomer.FormId).CountAsync(),
                MtdStores = await queryMtdStore.Where(x => x.MtdForm == incomer.FormId).OrderByDescending(x => x.Sequence)
                .Skip((incomer.Page - 1) * incomer.PageSize)
                .Take(incomer.PageSize)
                .ToListAsync()
            };
        }

        public async Task<OutFlow> GetDataForNumberAsync(Incomer incomer)
        {
            MtdStore mtdStore = await queryMtdStore.Where(x => x.MtdForm == incomer.FormId && x.Sequence.ToString().Equals(incomer.SearchNumber)).FirstOrDefaultAsync();

            return new OutFlow
            {
                Count = mtdStore != null ? 1 : 0,
                MtdStores = mtdStore != null ? new List<MtdStore> { mtdStore } : new List<MtdStore>()
            };

        }

        public async Task<OutFlow> GetDataForTextAsync(Incomer incomer)
        {
            /*to search in all fields*/
            IList<string> fieldsIds = await GetAllowFieldsAsync();

            /*To search only selected columns*/
            //List<string> fieldsIds = incomer.FieldIds;


            IList<string> storeIds = new List<string>();
            bool plus = incomer.SearchText.Contains("+");

            if (plus)
            {
                List<string> words = incomer.SearchText.Split("+").Where(x => x != "").ToList();
                for (int i = 0; i < words.Count(); i++)
                {
                    storeIds = i == 0 ? await FindStoreIdsForText(fieldsIds, words[i], 4) : await FindStoreIdsForText(fieldsIds, words[i], 4, storeIds);
                }
            }
            else
            {
                storeIds = await FindStoreIdsForText(fieldsIds, incomer.SearchText, 4);
            }


            return new OutFlow
            {
                Count = storeIds.Count(),
                MtdStores = await queryMtdStore.Where(x => storeIds.Contains(x.Id))
                .OrderByDescending(x => x.Sequence).Skip((incomer.Page - 1) * incomer.PageSize).Take(incomer.PageSize).ToListAsync()
            };

        }

        public async Task<OutFlow> GetDataForFieldAsync(Incomer incomer)
        {

            IList<string> storeIds = await queryMtdStore.Select(x => x.Id).ToListAsync();

            if (incomer.SearchText.Length > 0)
            {
                List<string> fieldsIds = incomer.FieldForColumn.Select(x => x.Id).ToList();
                storeIds = await FindStoreIdsForText(fieldsIds, incomer.SearchText, 4, storeIds);
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
                                bool isOk = int.TryParse(item.Value, out int valueInt);
                                if (!isOk) { valueInt = 0; }
                                storeIds = await FindStoreIdsForInt(field, valueInt, item.MtdTerm, storeIds);
                                break;
                            }
                        case 3:
                            {
                                bool isOk = decimal.TryParse(item.Value, out decimal valueDecimal);
                                if (!isOk) { valueDecimal = 0; }
                                storeIds = await FindStoreIdsForDecimal(field, valueDecimal, item.MtdTerm, storeIds);
                                break;
                            }
                        case 5:
                        case 6:
                            {
                                //bool ok = DateTime.TryParse(item.Value, out DateTime dateTime);
                                string[] dates = item.Value.Split("***").ToArray();
                                bool isOk = DateTime.TryParseExact(dates[0], item.ValueExtra, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateStart);
                                bool isOk2 = DateTime.TryParseExact(dates[1], item.ValueExtra, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFinish);

                                if (isOk && isOk2)
                                {
                                    storeIds = await FindStoreIdsForDate(field, dateStart.Date, dateFinish.Date, storeIds);
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
