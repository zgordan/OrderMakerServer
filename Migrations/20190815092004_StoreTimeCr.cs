using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class StoreTimeCr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "timecr",
                table: "mtd_store",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValueSql: "'CURRENT_TIMESTAMP'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "timecr",
                table: "mtd_store",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "'CURRENT_TIMESTAMP'",
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }
    }
}
