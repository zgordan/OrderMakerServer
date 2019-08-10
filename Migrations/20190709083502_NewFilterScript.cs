using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Web.Migrations
{
    public partial class NewFilterScript : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_filter_script",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_filter = table.Column<int>(type: "int(11)", nullable: false),
                    name = table.Column<string>(type: "varchar(256)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    script = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter_script", x => x.id);
                    table.ForeignKey(
                        name: "fk_script_filter",
                        column: x => x.mtd_filter,
                        principalTable: "mtd_filter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_script",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_script_filter_idx",
                table: "mtd_filter_script",
                column: "mtd_filter");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_filter_script");
        }
    }
}
