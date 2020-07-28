using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class UpdateTimeApproval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update mtd_sys_term set sign='<' where id=2");
            migrationBuilder.Sql("update mtd_sys_term set sign='>' where id=3");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_event_time",
                table: "mtd_store_approval",
                type: "datetime",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update mtd_sys_term set sign='>' where id=2");
            migrationBuilder.Sql("update mtd_sys_term set sign='<' where id=3");

            migrationBuilder.DropColumn(
                name: "last_event_time",
                table: "mtd_store_approval");
        }
    }
}
