using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class UpdateScriptApply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "apply",
                table: "mtd_filter_script");

            migrationBuilder.CreateTable(
                name: "mtd_filter_script_apply",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_filter_id = table.Column<int>(type: "int(11)", nullable: false),
                    mtd_filter_script_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter_script_apply", x => x.id);
                    table.ForeignKey(
                        name: "fk_script_filter_apply1",
                        column: x => x.mtd_filter_id,
                        principalTable: "mtd_filter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_script_filter_apply2",
                        column: x => x.mtd_filter_script_id,
                        principalTable: "mtd_filter_script",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_script_apply",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_script_filter_apply1_idx",
                table: "mtd_filter_script_apply",
                column: "mtd_filter_id");

            migrationBuilder.CreateIndex(
                name: "fk_script_filter_apply2_idx",
                table: "mtd_filter_script_apply",
                column: "mtd_filter_script_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_filter_script_apply");

            migrationBuilder.AddColumn<sbyte>(
                name: "apply",
                table: "mtd_filter_script",
                type: "tinyint(4)",
                nullable: false,
                defaultValueSql: "'0'");
        }
    }
}
