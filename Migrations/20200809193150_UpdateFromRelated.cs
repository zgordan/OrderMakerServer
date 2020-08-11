using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class UpdateFromRelated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_form_related",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    parent_form_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    child_form_id = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_form_related", x => x.id);
                    table.ForeignKey(
                        name: "fk_child_form",
                        column: x => x.child_form_id,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_parent_form",
                        column: x => x.parent_form_id,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "fk_child_form_idx",
                table: "mtd_form_related",
                column: "child_form_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_related",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_parent_form_idx",
                table: "mtd_form_related",
                column: "parent_form_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_form_related");
        }
    }
}
