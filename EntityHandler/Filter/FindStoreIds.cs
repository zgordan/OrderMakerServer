/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.EntityHandler.Filter
{
    public partial class FilterHandler
    {
        public async Task<IList<string>> FindStoreIdsForText(IList<string> fieldIds, string value, int term, IList<string> storeIds = null)
        {
            var query = _context.MtdStoreStack.Where(x => fieldIds.Contains(x.MtdFormPartField));
            if (storeIds != null) query = query.Where(x => storeIds.Contains(x.MtdStore));
            IList<long> stackIds = await query.Select(x => x.Id).ToListAsync();
            IList<long> resultIds;

            switch (term)
            {
                case 1:
                    {
                        IQueryable<long> vs = _context.MtdStoreLink
                            .Where(x => stackIds.Contains(x.Id) && x.Register.ToUpper().Equals(value.ToUpper()))
                            .Select(x => x.Id);

                        resultIds = await _context.MtdStoreStackText
                            .Where(x => stackIds.Contains(x.Id) && x.Register.ToUpper().Equals(value.ToUpper()))
                            .Select(x => x.Id).Union(vs).Distinct().ToListAsync();

                        break;
                    }
                case 5:
                    {
                        IQueryable<long> vs = _context.MtdStoreLink
                            .Where(x => stackIds.Contains(x.Id) && x.Register.ToUpper() != value.ToUpper())
                            .Select(x => x.Id);

                        resultIds = await _context.MtdStoreStackText
                            .Where(x => stackIds.Contains(x.Id) && x.Register.ToUpper() != value.ToUpper())
                            .Select(x => x.Id).Union(vs).Distinct().ToListAsync();

                        break;
                    }
                default:
                    {
                        IQueryable<long> vs = _context.MtdStoreLink
                            .Where(x => stackIds.Contains(x.Id) && x.Register.ToUpper().Contains(value.ToUpper()))
                            .Select(x => x.Id);

                        resultIds = await _context.MtdStoreStackText
                            .Where(x => stackIds.Contains(x.Id) && x.Register.ToUpper().Contains(value.ToUpper()))
                            .Select(x => x.Id).Union(vs).Distinct().ToListAsync();

                        break;
                    }

            };

            /*return storeIds*/
            IList<string> result = await _context.MtdStoreStack.Where(x => resultIds.Contains(x.Id)).Select(x => x.MtdStore).ToListAsync();
            return result;
        }

        public async Task<IList<string>> FindStoreIdsForInt(IList<string> fieldIds, int value, int term, IList<string> store = null)
        {
            var query = _context.MtdStoreStack.Where(x => fieldIds.Contains(x.MtdFormPartField));
            if (store != null) query = query.Where(x => store.Contains(x.MtdStore));
            IList<long> stackIds = await query.Select(x => x.Id).ToListAsync();
            IList<long> resultIds;

            switch (term)
            {
                case 1:
                    {
                        resultIds = await _context.MtdStoreStackInt.Where(x => stackIds.Contains(x.Id) && x.Register.Equals(value)).Select(x => x.Id).ToListAsync();
                        break;
                    }
                case 2:
                    {
                        resultIds = await _context.MtdStoreStackInt.Where(x => stackIds.Contains(x.Id) && x.Register < value).Select(x => x.Id).ToListAsync();
                        break;
                    }
                case 3:
                    {
                        resultIds = await _context.MtdStoreStackInt.Where(x => stackIds.Contains(x.Id) && x.Register > value).Select(x => x.Id).ToListAsync();
                        break;
                    }
                case 4:
                    {
                        resultIds = await _context.MtdStoreStackInt.Where(x => stackIds.Contains(x.Id) && x.Register.ToString().Contains(value.ToString())).Select(x => x.Id).ToListAsync();
                        break;
                    }
                default:
                    {
                        resultIds = await _context.MtdStoreStackInt.Where(x => stackIds.Contains(x.Id) && x.Register != value).Select(x => x.Id).ToListAsync();
                        break;
                    }
            };

            IList<string> storeIds = await _context.MtdStoreStack.Where(x => resultIds.Contains(x.Id)).Select(x => x.MtdStore).ToListAsync();
            return storeIds;
        }

        public async Task<IList<string>> FindStoreIdsForDateTime(IList<string> fieldIds, DateTime value, int term, IList<string> store = null)
        {
            var query = _context.MtdStoreStack.Where(x => fieldIds.Contains(x.MtdFormPartField));
            if (store != null) query = query.Where(x => store.Contains(x.MtdStore));
            IList<long> stackIds = await query.Select(x => x.Id).ToListAsync();
            IList<long> resultIds;

            switch (term)
            {
                case 1:
                    {
                        resultIds = await _context.MtdStoreStackDate.Where(x => stackIds.Contains(x.Id) && x.Register.Equals(value)).Select(x => x.Id).ToListAsync();
                        break;
                    }
                case 2:
                    {
                        resultIds = await _context.MtdStoreStackDate.Where(x => stackIds.Contains(x.Id) && x.Register < value).Select(x => x.Id).ToListAsync();
                        break;
                    }
                case 3:
                    {
                        resultIds = await _context.MtdStoreStackDate.Where(x => stackIds.Contains(x.Id) && x.Register > value).Select(x => x.Id).ToListAsync();
                        break;
                    }
                case 4:
                    {
                        resultIds = await _context.MtdStoreStackDate.Where(x => stackIds.Contains(x.Id) && x.Register.ToString().Contains(value.ToString())).Select(x => x.Id).ToListAsync();
                        break;
                    }
                default:
                    {
                        resultIds = await _context.MtdStoreStackDate.Where(x => stackIds.Contains(x.Id) && x.Register != value).Select(x => x.Id).ToListAsync();
                        break;
                    }

            };

            IList<string> storeIds = await _context.MtdStoreStack.Where(x => resultIds.Contains(x.Id)).Select(x => x.MtdStore).ToListAsync();

            return storeIds;
        }

        public async Task<IList<string>> FindStoreIdsForDate(IList<string> fieldIds, DateTime dateStart, DateTime dateFinish, IList<string> store = null)
        {
            var query = _context.MtdStoreStack.Where(x => fieldIds.Contains(x.MtdFormPartField));
            if (store != null) query = query.Where(x => store.Contains(x.MtdStore));
            IList<long> stackIds = await query.Select(x => x.Id).ToListAsync();

            IList<long> resultIds = await _context.MtdStoreStackDate.Where(x => stackIds.Contains(x.Id) && x.Register.Date >= dateStart && x.Register.Date <= dateFinish).Select(x => x.Id).ToListAsync(); ;

            //switch (term)
            //{
            //    case 1:
            //        {
            //            resultIds = await _context.MtdStoreStackDate.Where(x => stackIds.Contains(x.Id) && x.Register.Date.Equals(dateStart.Date)).Select(x => x.Id).ToListAsync();
            //            break;
            //        }
            //    case 2:
            //        {
            //            resultIds = await _context.MtdStoreStackDate.Where(x => stackIds.Contains(x.Id) && x.Register.Date < dateStart.Date).Select(x => x.Id).ToListAsync();
            //            break;
            //        }
            //    case 3:
            //        {
            //            resultIds = await _context.MtdStoreStackDate.Where(x => stackIds.Contains(x.Id) && x.Register.Date > dateStart.Date).Select(x => x.Id).ToListAsync();
            //            break;
            //        }
            //    case 4:
            //        {
            //            resultIds = await _context.MtdStoreStackDate.Where(x => stackIds.Contains(x.Id) && x.Register.Date.ToString().Contains(dateStart.Date.ToString())).Select(x => x.Id).ToListAsync();
            //            break;
            //        }
            //    default:
            //        {
            //            resultIds = await _context.MtdStoreStackDate.Where(x => stackIds.Contains(x.Id) && x.Register.Date != dateStart.Date).Select(x => x.Id).ToListAsync();
            //            break;
            //        }

            //};

            IList<string> storeIds = await _context.MtdStoreStack.Where(x => resultIds.Contains(x.Id)).Select(x => x.MtdStore).ToListAsync();

            return storeIds;
        }

        public async Task<IList<string>> FindStoreIdsForDecimal(IList<string> fieldIds, decimal value, int term, IList<string> store = null)
        {
            var query = _context.MtdStoreStack.Where(x => fieldIds.Contains(x.MtdFormPartField));
            if (store != null) query = query.Where(x => store.Contains(x.MtdStore));
            IList<long> stackIds = await query.Select(x => x.Id).ToListAsync();
            IList<long> resultIds;

            switch (term)
            {
                case 1:
                    {
                        resultIds = await _context.MtdStoreStackDecimal.Where(x => stackIds.Contains(x.Id) && x.Register.Equals(value)).Select(x => x.Id).ToListAsync();
                        break;
                    }
                case 2:
                    {
                        resultIds = await _context.MtdStoreStackDecimal.Where(x => stackIds.Contains(x.Id) && x.Register < value).Select(x => x.Id).ToListAsync();
                        break;
                    }
                case 3:
                    {
                        resultIds = await _context.MtdStoreStackDecimal.Where(x => stackIds.Contains(x.Id) && x.Register > value).Select(x => x.Id).ToListAsync();
                        break;
                    }
                case 4:
                    {
                        resultIds = await _context.MtdStoreStackDecimal.Where(x => stackIds.Contains(x.Id) && x.Register.ToString().Contains(value.ToString())).Select(x => x.Id).ToListAsync();
                        break;
                    }
                default:
                    {
                        resultIds = await _context.MtdStoreStackDecimal.Where(x => stackIds.Contains(x.Id) && x.Register != value).Select(x => x.Id).ToListAsync();
                        break;
                    }

            };

            IList<string> storeIds = await _context.MtdStoreStack.Where(x => resultIds.Contains(x.Id)).Select(x => x.MtdStore).ToListAsync();

            return storeIds;
        }

        public async Task<IList<string>> FindStoreIdsForList(IList<string> fieldIds, string value, int term, IList<string> store = null)
        {
            var query = _context.MtdStoreStack.Where(x => fieldIds.Contains(x.MtdFormPartField));
            if (store != null) query = query.Where(x => store.Contains(x.MtdStore));
            IList<long> stackIds = await query.Select(x => x.Id).ToListAsync();

            List<long> resultIds = await _context.MtdStoreLink.Where(x => stackIds.Contains(x.Id) && x.MtdStore.Equals(value)).Select(x => x.Id).ToListAsync();
            IList<string> storeIds = await _context.MtdStoreStack.Where(x => resultIds.Contains(x.Id)).Select(x => x.MtdStore).ToListAsync();

            return storeIds;
        }
    }
}
