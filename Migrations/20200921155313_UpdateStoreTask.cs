using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class UpdateStoreTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_store_task",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_store_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(250)", nullable: false),
                    deadline = table.Column<DateTime>(type: "datetime", nullable: false),
                    iniciator = table.Column<string>(type: "varchar(36)", nullable: false),
                    init_note = table.Column<string>(type: "varchar(250)", nullable: false),
                    init_timecr = table.Column<DateTime>(type: "datetime", nullable: false),
                    executor = table.Column<string>(type: "varchar(36)", nullable: false),
                    exec_note = table.Column<string>(type: "varchar(250)", nullable: false),
                    exec_timecr = table.Column<DateTime>(type: "datetime", nullable: false),
                    complete = table.Column<int>(type: "int(11)", nullable: false),
                    private_task = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    last_evant_time = table.Column<DateTime>(type: "datetime", nullable: false)
                },

                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_task", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_store_task",
                        column: x => x.mtd_store_id,
                        principalTable: "mtd_store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_task",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_store_task_idx",
                table: "mtd_store_task",
                column: "mtd_store_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_store_task");
        }
    }
}
