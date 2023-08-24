namespace Mtd.OrderMaker.Server.Entity
{
    public class MtdRegisterField
    {
        public string Id { get; set; }
        public string MtdRegisterId { get; set; }
        public sbyte Income { get; set; }
        public sbyte Expense { get; set; }

        public virtual MtdRegister MtdRegister { get; set; }
        public virtual MtdFormPartField IdNavigation { get; set; }
    }
}
