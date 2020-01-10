using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class MtdLogApproval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_log_approval",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_store = table.Column<string>(type: "varchar(36)", nullable: false),
                    stage = table.Column<int>(type: "int(11)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    result = table.Column<int>(type: "int(11)", nullable: false),
                    timecr = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_log_approval", x => x.id);
                    table.ForeignKey(
                        name: "fk_log_approval_store",
                        column: x => x.mtd_store,
                        principalTable: "mtd_store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_log_approval_stage",
                        column: x => x.stage,
                        principalTable: "mtd_approval_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_log_approval",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_log_approval_store_idx",
                table: "mtd_log_approval",
                column: "mtd_store");

            migrationBuilder.CreateIndex(
                name: "fk_log_approval_stage_idx",
                table: "mtd_log_approval",
                column: "stage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_log_approval");
        }
    }
}
