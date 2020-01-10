using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class FormPolicyDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<sbyte>(
                name: "change_date",
                table: "mtd_policy_forms",
                type: "tinyint(4)",
                nullable: false,
                defaultValueSql: "'0'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "change_date",
                table: "mtd_policy_forms");
        }
    }
}
