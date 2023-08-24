using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Mtd.OrderMaker.Server.Entity;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Controllers.Store
{
    public class StackParams
    {
        public StringValues Data { get; set; }
        public string StroreID { get; set; }
        public MtdFormPartField Field { get; set; }
        public StringValues ActionDelete { get; set; }
        public StringValues DataLink { get; set; }
        public IFormFile File { get; set; }
    }

    public class DataGenerator
    {
        private readonly OrderMakerContext _context;
        private readonly string userName;
        private readonly string userGroup;

        public DataGenerator(OrderMakerContext context, string user_name, string user_group)
        {
            _context = context;
            userName = user_name;
            userGroup = user_group ?? string.Empty;
        }

        public async Task<MtdStoreStack> CreateStoreStack(StackParams stackParams)
        {
            StringValues data = stackParams.Data;

            MtdStoreStack mtdStoreStack = new MtdStoreStack()
            {
                MtdStore = stackParams.StroreID,
                MtdFormPartField = stackParams.Field.Id
            };

            if (!stackParams.Field.MtdSysTrigger.Equals("9C85B07F-9236-4314-A29E-87B20093CF82"))
            {
                switch (stackParams.Field.MtdSysTrigger)
                {
                    case "D3663BC7-FA05-4F64-8EBD-F25414E459B8":
                        {
                            //Date time now
                            data = new StringValues(DateTime.Now.ToString());
                            break;
                        }
                    case "33E8212E-059B-482D-8CBD-DFDB073E3B63":
                        {
                            //User name group
                            data = new StringValues(userGroup);
                            break;
                        }
                    case "08FE6202-45D7-46C2-B343-B79FD4831F27":
                        {
                            //User name name
                            data = new StringValues(userName);
                            break;
                        }
                    default:
                        {
                            data = new StringValues();
                            break;
                        }
                }
            }

            switch (stackParams.Field.MtdSysType)
            {
                case 2:
                    {
                        if (data.FirstOrDefault() != string.Empty)
                        {
                            bool isOkInt = int.TryParse(data.FirstOrDefault(), out int result);
                            if (isOkInt)
                            {
                                mtdStoreStack.MtdStoreStackInt = new MtdStoreStackInt { Register = result };
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        if (data.FirstOrDefault() != string.Empty)
                        {
                            string separ = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                            bool isOkDecimal = decimal.TryParse(data.FirstOrDefault().Replace(".", separ), out decimal result);
                            if (isOkDecimal)
                            {
                                mtdStoreStack.MtdStoreStackDecimal = new MtdStoreStackDecimal { Register = result };
                            }

                        }
                        break;
                    }
                case 5:
                case 6:
                case 10:
                    {
                        if (data.FirstOrDefault() != string.Empty)
                        {
                            bool isOkDate = DateTime.TryParse(data.FirstOrDefault(), out DateTime dateTime);
                            if (isOkDate)
                            {
                                mtdStoreStack.MtdStoreStackDate = new MtdStoreStackDate { Register = dateTime };
                            }

                        }
                        break;
                    }
                case 7:
                case 8:
                    {

                        // var actionDelete = Request.Form[$"{field.Id}-delete"];
                        var actionDelete = stackParams.ActionDelete;
                        if (actionDelete.FirstOrDefault() == null || actionDelete.FirstOrDefault() == "false")
                        {

                            // IFormFile file =  Request.Form.Files.FirstOrDefault(x => x.Name == field.Id);
                            IFormFile file = stackParams.File;
                            if (file != null)
                            {
                                byte[] streamArray = new byte[file.Length];
                                await file.OpenReadStream().ReadAsync(streamArray, 0, streamArray.Length);
                                mtdStoreStack.MtdStoreStackFile = new MtdStoreStackFile()
                                {
                                    Register = streamArray,
                                    FileName = file.FileName,
                                    FileSize = streamArray.Length,
                                    FileType = file.ContentType
                                };
                            }

                            if (file == null)
                            {
                                MtdStoreStack stackOld = await _context.MtdStoreStack
                                    .Include(m => m.MtdStoreStackFile)
                                    .OrderByDescending(x => x.Id)
                                    .FirstOrDefaultAsync(x => x.MtdStore == stackParams.StroreID & x.MtdFormPartField == stackParams.Field.Id);

                                if (stackOld != null && stackOld.MtdStoreStackFile != null)
                                {
                                    mtdStoreStack.MtdStoreStackFile = new MtdStoreStackFile()
                                    {
                                        FileName = stackOld.MtdStoreStackFile.FileName,
                                        FileSize = stackOld.MtdStoreStackFile.FileSize,
                                        Register = stackOld.MtdStoreStackFile.Register,
                                        FileType = stackOld.MtdStoreStackFile.FileType,
                                    };
                                }
                            }
                        }

                        break;
                    }

                case 11:
                    {
                        if (data.FirstOrDefault() != string.Empty)
                        {
                            //string datalink = Request.Form[$"{field.Id}-datalink"];
                            string datalink = stackParams.DataLink;
                            mtdStoreStack.MtdStoreLink = new MtdStoreLink { MtdStore = data.FirstOrDefault(), Register = datalink };
                        }

                        break;
                    }

                case 12:
                    {
                        bool isOkCheck = bool.TryParse(data.FirstOrDefault(), out bool check);
                        if (isOkCheck)
                        {
                            mtdStoreStack.MtdStoreStackInt = new MtdStoreStackInt { Register = check ? 1 : 0 };
                        }
                        break;
                    }

                default:
                    {
                        if (data.FirstOrDefault() != string.Empty)
                        {
                            mtdStoreStack.MtdStoreStackText = new MtdStoreStackText() { Register = data.FirstOrDefault() };
                        }
                        break;
                    }
            }

            return mtdStoreStack;
        }


    }
}
