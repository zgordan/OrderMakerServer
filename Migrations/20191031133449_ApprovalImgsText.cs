using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class ApprovalImgsText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "img_approved_text",
                table: "mtd_approval",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_approved_type",
                table: "mtd_approval",
                type: "varchar(48)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_iteraction_text",
                table: "mtd_approval",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_iteraction_type",
                table: "mtd_approval",
                type: "varchar(48)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_rejected_text",
                table: "mtd_approval",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_rejected_type",
                table: "mtd_approval",
                type: "varchar(48)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_required_text",
                table: "mtd_approval",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_required_type",
                table: "mtd_approval",
                type: "varchar(48)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_start_text",
                table: "mtd_approval",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_start_type",
                table: "mtd_approval",
                type: "varchar(48)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_waiting_text",
                table: "mtd_approval",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_waiting_type",
                table: "mtd_approval",
                type: "varchar(48)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "img_approved_text",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_approved_type",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_iteraction_text",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_iteraction_type",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_rejected_text",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_rejected_type",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_required_text",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_required_type",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_start_text",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_start_type",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_waiting_text",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_waiting_type",
                table: "mtd_approval");
        }
    }
}
