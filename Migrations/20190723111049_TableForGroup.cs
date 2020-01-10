using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class TableForGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_group",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_group",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_group = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_group", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_store",
                        column: x => x.id,
                        principalTable: "mtd_store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_store_group_mtd_group",
                        column: x => x.mtd_group,
                        principalTable: "mtd_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_group",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_group",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_store_group_mtd_group_idx",
                table: "mtd_store_group",
                column: "mtd_group");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_store_group");

            migrationBuilder.DropTable(
                name: "mtd_group");
        }
    }
}
