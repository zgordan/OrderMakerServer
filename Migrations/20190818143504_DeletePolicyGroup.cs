using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Web.Migrations
{
    public partial class DeletePolicyGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_policy_group");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_policy_group",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    member = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    mtd_group = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_policy = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_policy_group", x => x.id);
                    table.ForeignKey(
                        name: "fk_policy_groups_group",
                        column: x => x.mtd_group,
                        principalTable: "mtd_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_policy_groups_policy",
                        column: x => x.mtd_policy,
                        principalTable: "mtd_policy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_policy_group",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_policy_groups_group_idx",
                table: "mtd_policy_group",
                column: "mtd_group");

            migrationBuilder.CreateIndex(
                name: "fk_policy_group_policy_idx",
                table: "mtd_policy_group",
                column: "mtd_policy");
        }
    }
}
