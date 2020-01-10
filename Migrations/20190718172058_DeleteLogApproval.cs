using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class DeleteLogApproval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_log_approval");

            migrationBuilder.RenameColumn(
                name: "approved",
                table: "mtd_store_approval",
                newName: "complete");

            migrationBuilder.AddColumn<int>(
                name: "result",
                table: "mtd_store_approval",
                type: "int",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.Sql("delete from mtd_store_approval where id<>''");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "result",
                table: "mtd_store_approval");

            migrationBuilder.RenameColumn(
                name: "complete",
                table: "mtd_store_approval",
                newName: "approved");

            migrationBuilder.CreateTable(
                name: "mtd_log_approval",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_approval_stage = table.Column<int>(type: "int(11)", nullable: false),
                    result = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    timech = table.Column<DateTime>(type: "datetime", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_log_approval", x => x.id);
                    table.ForeignKey(
                        name: "fk_log_approval_stage",
                        column: x => x.mtd_approval_stage,
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
                name: "fk_log_approval_stage_idx",
                table: "mtd_log_approval",
                column: "mtd_approval_stage");
        }
    }
}
