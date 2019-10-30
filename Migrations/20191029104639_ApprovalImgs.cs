using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Web.Migrations
{
    public partial class ApprovalImgs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "img_approved",
                table: "mtd_approval",
                type: "mediumblob",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "img_iteraction",
                table: "mtd_approval",
                type: "mediumblob",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "img_rejected",
                table: "mtd_approval",
                type: "mediumblob",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "img_required",
                table: "mtd_approval",
                type: "mediumblob",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "img_start",
                table: "mtd_approval",
                type: "mediumblob",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "img_waiting",
                table: "mtd_approval",
                type: "mediumblob",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "img_approved",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_iteraction",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_rejected",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_required",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_start",
                table: "mtd_approval");

            migrationBuilder.DropColumn(
                name: "img_waiting",
                table: "mtd_approval");
        }
    }
}
