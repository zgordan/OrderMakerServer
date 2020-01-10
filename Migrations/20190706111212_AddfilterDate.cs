using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class AddfilterDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_filter_date",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false),
                    date_start = table.Column<DateTime>(type: "datetime", nullable: false),
                    date_end = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter_date", x => x.id);
                    table.ForeignKey(
                        name: "fk_date_filter",
                        column: x => x.id,
                        principalTable: "mtd_filter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_date",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_filter_date");
        }
    }
}
