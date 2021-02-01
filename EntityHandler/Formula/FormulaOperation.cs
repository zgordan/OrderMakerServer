using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.EntityHandler.Formula
{
    public class FormulaOperation
    {
        public static int None => 0;
        public static int Addition => 1;
        public static int Substraction => 2;
        public static int Multiplication => 3;
        public static int Division => 4;
    }
}
