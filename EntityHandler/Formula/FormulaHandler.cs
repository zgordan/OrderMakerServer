using Mtd.OrderMaker.Server.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.EntityHandler.Formula
{
    public class FormulaHandler
    {
        public FormulaHandler()
        {

        }

        public void Create(MtdFormula mtdFormula)
        {

        }

        public void Create(MtdFormulaBar mtdFormulaBar)
        {

        }

        public void Edit(MtdFormula mtdFormula)
        {

        }

        public void Edit(MtdFormulaBar mtdFormulaBar)
        {

        }

        /// <summary>
        /// Formula return with all linked FormulaBars  
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public MtdFormula Get (string Id)
        {
            throw new Exception();
        }

        /// <summary>
        /// Formulas return with all linked FormulaBars  
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public IList<MtdFormula> Get(int page, int pageSize)
        {
            throw new Exception();
        }

        public void ReindexSequence(string formulaId, Dictionary<string,int> index)
        {

        }


    }
}
