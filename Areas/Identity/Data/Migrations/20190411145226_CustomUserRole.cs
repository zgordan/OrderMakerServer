using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Entity.Migrations
{
    public partial class CustomUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AspNetRoles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "AspNetRoles");
        }
    }
}
