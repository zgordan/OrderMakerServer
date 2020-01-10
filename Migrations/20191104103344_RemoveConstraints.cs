using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class RemoveConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_log_rejection",
                table: "mtd_log_approval");

            migrationBuilder.DropForeignKey(
                name: "fk_log_resolution",
                table: "mtd_log_approval");

            migrationBuilder.DropForeignKey(
                name: "fk_store_stage_rejection",
                table: "mtd_store_approval");

            migrationBuilder.DropForeignKey(
                name: "fk_store_stage_resolution",
                table: "mtd_store_approval");

            migrationBuilder.DropIndex(
                name: "fk_store_stage_rejection_idx",
                table: "mtd_store_approval");

            migrationBuilder.DropIndex(
                name: "fk_store_stage_resolution_idx",
                table: "mtd_store_approval");

            migrationBuilder.DropIndex(
                name: "fk_log_rejection_idx",
                table: "mtd_log_approval");

            migrationBuilder.DropIndex(
                name: "fk_log_resolution_idx",
                table: "mtd_log_approval");

            migrationBuilder.DropColumn(
                name: "rejection",
                table: "mtd_store_approval");

            migrationBuilder.DropColumn(
                name: "resolution",
                table: "mtd_store_approval");

            migrationBuilder.DropColumn(
                name: "rejection",
                table: "mtd_log_approval");

            migrationBuilder.DropColumn(
                name: "resolution",
                table: "mtd_log_approval");

            migrationBuilder.AddColumn<string>(
                name: "color",
                table: "mtd_log_approval",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "img_data",
                table: "mtd_log_approval",
                type: "mediumblob",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_type",
                table: "mtd_log_approval",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "mtd_log_approval",
                type: "varchar(512)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "color",
                table: "mtd_log_approval");

            migrationBuilder.DropColumn(
                name: "img_data",
                table: "mtd_log_approval");

            migrationBuilder.DropColumn(
                name: "img_type",
                table: "mtd_log_approval");

            migrationBuilder.DropColumn(
                name: "note",
                table: "mtd_log_approval");

            migrationBuilder.AddColumn<string>(
                name: "rejection",
                table: "mtd_store_approval",
                type: "varchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "resolution",
                table: "mtd_store_approval",
                type: "varchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rejection",
                table: "mtd_log_approval",
                type: "varchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "resolution",
                table: "mtd_log_approval",
                type: "varchar(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "fk_store_stage_rejection_idx",
                table: "mtd_store_approval",
                column: "rejection");

            migrationBuilder.CreateIndex(
                name: "fk_store_stage_resolution_idx",
                table: "mtd_store_approval",
                column: "resolution");

            migrationBuilder.CreateIndex(
                name: "fk_log_rejection_idx",
                table: "mtd_log_approval",
                column: "rejection");

            migrationBuilder.CreateIndex(
                name: "fk_log_resolution_idx",
                table: "mtd_log_approval",
                column: "resolution");

            migrationBuilder.AddForeignKey(
                name: "fk_log_rejection",
                table: "mtd_log_approval",
                column: "rejection",
                principalTable: "mtd_approval_rejection",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_log_resolution",
                table: "mtd_log_approval",
                column: "resolution",
                principalTable: "mtd_approval_resolution",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_store_stage_rejection",
                table: "mtd_store_approval",
                column: "rejection",
                principalTable: "mtd_approval_rejection",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_store_stage_resolution",
                table: "mtd_store_approval",
                column: "resolution",
                principalTable: "mtd_approval_resolution",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
