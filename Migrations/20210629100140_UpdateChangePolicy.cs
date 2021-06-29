using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class UpdateChangePolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "responsibility",
                table: "mtd_policy_forms");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<sbyte>(
                name: "responsibility",
                table: "mtd_policy_forms",
                type: "tinyint(4)",
                nullable: false,
                defaultValueSql: "'0'");
        }
    }
}
