using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Web.Migrations
{
    public partial class RenameGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_form_grooup",
                table: "mtd_form");

            migrationBuilder.RenameColumn(
                name: "mtd_group",
                table: "mtd_form",
                newName: "mtd_category");

            migrationBuilder.RenameTable(
                name: "mtd_group_form", 
                newName: "mtd_category_form");

            migrationBuilder.AddForeignKey(
                name: "fk_form_category",
                table: "mtd_form",
                column: "mtd_category",
                principalTable: "mtd_category_form",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "mtd_category_form",
                newName: "mtd_group_form");

            migrationBuilder.RenameColumn(
                name: "mtd_category",
                table: "mtd_form",
                newName: "mtd_group");

            migrationBuilder.DropForeignKey(
                name: "fk_form_category",
                table: "mtd_form");

            migrationBuilder.AddForeignKey(
                name: "fk_form_grooup",
                table: "mtd_form",
                column: "mtd_group",
                principalTable: "mtd_group_form",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);  

        }
    }
}
