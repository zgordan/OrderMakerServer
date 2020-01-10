using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class Resolution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "resolution",
                table: "mtd_store_approval",
                type: "varchar(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "resolution",
                table: "mtd_log_approval",
                type: "varchar(36)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "mtd_approval_resolution",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false),
                    note = table.Column<string>(type: "varchar(512)", nullable: false),
                    sequence = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    color = table.Column<string>(type: "varchar(45)", nullable: false, defaultValueSql: "'green'"),
                    mtd_approval_stage_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_approval_resolution", x => x.id);
                    table.ForeignKey(
                        name: "fk_resolution_stage",
                        column: x => x.mtd_approval_stage_id,
                        principalTable: "mtd_approval_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "fk_store_stage_resolution_idx",
                table: "mtd_store_approval",
                column: "resolution");

            migrationBuilder.CreateIndex(
                name: "fk_log_resolution_idx",
                table: "mtd_log_approval",
                column: "resolution");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_approval_resolution",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_resolution_stage_idx",
                table: "mtd_approval_resolution",
                column: "mtd_approval_stage_id");

            migrationBuilder.CreateIndex(
                name: "ix_sequence",
                table: "mtd_approval_resolution",
                column: "sequence");

            migrationBuilder.AddForeignKey(
                name: "fk_log_resolution",
                table: "mtd_log_approval",
                column: "resolution",
                principalTable: "mtd_approval_resolution",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "fk_log_resolution",
                table: "mtd_log_approval");

            migrationBuilder.DropForeignKey(
                name: "fk_store_stage_resolution",
                table: "mtd_store_approval");

            migrationBuilder.DropTable(
                name: "mtd_approval_resolution");

            migrationBuilder.DropIndex(
                name: "fk_store_stage_resolution_idx",
                table: "mtd_store_approval");

            migrationBuilder.DropIndex(
                name: "fk_log_resolution_idx",
                table: "mtd_log_approval");

            migrationBuilder.DropColumn(
                name: "resolution",
                table: "mtd_store_approval");

            migrationBuilder.DropColumn(
                name: "resolution",
                table: "mtd_log_approval");

        }
    }
}
