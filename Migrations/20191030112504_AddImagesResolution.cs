using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Web.Migrations
{
    public partial class AddImagesResolution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "img_data",
                table: "mtd_approval_resolution",
                type: "mediumblob",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_type",
                table: "mtd_approval_resolution",
                type: "varchar(45)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "img_data",
                table: "mtd_approval_rejection",
                type: "mediumblob",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_type",
                table: "mtd_approval_rejection",
                type: "varchar(45)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "img_data",
                table: "mtd_approval_resolution");

            migrationBuilder.DropColumn(
                name: "img_type",
                table: "mtd_approval_resolution");

            migrationBuilder.DropColumn(
                name: "img_data",
                table: "mtd_approval_rejection");

            migrationBuilder.DropColumn(
                name: "img_type",
                table: "mtd_approval_rejection");
        }
    }
}
