using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class UpdateFilteRelated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_filter_related",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false),
                    form_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    docbasednumber = table.Column<int>(name: "doc-based-number", type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter_related", x => x.id);
                    table.ForeignKey(
                        name: "fk_related_filter",
                        column: x => x.id,
                        principalTable: "mtd_filter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_related",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_filter_related");
        }
    }
}
