using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class Singnature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sign_chain",
                table: "mtd_store_approval",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<sbyte>(
                name: "is_sign",
                table: "mtd_log_approval",
                type: "tinyint(4)",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                table: "mtd_log_approval",
                type: "varchar(255)",
                nullable: false,
                defaultValueSql: "'No Name'");

            migrationBuilder.AddColumn<string>(
                name: "user_recipient_id",
                table: "mtd_log_approval",
                type: "varchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_recipient_name",
                table: "mtd_log_approval",
                type: "varchar(255)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sign_chain",
                table: "mtd_store_approval");

            migrationBuilder.DropColumn(
                name: "is_sign",
                table: "mtd_log_approval");

            migrationBuilder.DropColumn(
                name: "user_name",
                table: "mtd_log_approval");

            migrationBuilder.DropColumn(
                name: "user_recipient_id",
                table: "mtd_log_approval");

            migrationBuilder.DropColumn(
                name: "user_recipient_name",
                table: "mtd_log_approval");
        }
    }
}
