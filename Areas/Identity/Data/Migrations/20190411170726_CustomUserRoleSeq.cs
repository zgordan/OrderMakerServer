using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Web.Data.Migrations
{
    public partial class CustomUserRoleSeq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Seq",
                table: "AspNetRoles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Seq",
                table: "AspNetRoles");
        }
    }
}
