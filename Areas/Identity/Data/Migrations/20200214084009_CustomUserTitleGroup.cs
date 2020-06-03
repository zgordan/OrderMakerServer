using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Entity.Migrations
{
    public partial class CustomUserTitleGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TitleGroup",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleGroup",
                table: "AspNetUsers");
        }
    }
}
