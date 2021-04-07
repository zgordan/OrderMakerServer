using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class UpdateBigDataFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "register",
                table: "mtd_store_stack_file",
                type: "longblob",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "mediumblob");

            migrationBuilder.AlterColumn<long>(
                name: "file_size",
                table: "mtd_store_stack_file",
                type: "bigint(20)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int(11)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "register",
                table: "mtd_store_stack_file",
                type: "mediumblob",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "longblob");

            migrationBuilder.AlterColumn<int>(
                name: "file_size",
                table: "mtd_store_stack_file",
                type: "int(11)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint(20)");
        }
    }
}
