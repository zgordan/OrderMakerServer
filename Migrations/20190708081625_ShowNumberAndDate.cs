using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class ShowNumberAndDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<sbyte>(
                name: "show_date",
                table: "mtd_filter",
                type: "tinyint(4)",
                nullable: false,
                defaultValueSql: "'1'");

            migrationBuilder.AddColumn<sbyte>(
                name: "show_number",
                table: "mtd_filter",
                type: "tinyint(4)",
                nullable: false,
                defaultValueSql: "'1'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "show_date",
                table: "mtd_filter");

            migrationBuilder.DropColumn(
                name: "show_number",
                table: "mtd_filter");
        }
    }
}
